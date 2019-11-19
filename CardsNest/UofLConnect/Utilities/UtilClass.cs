using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UofLConnect.Models.Home;
using UofLConnect.Models.Mentor;

namespace UofLConnect.Utilities
{
    public class UtilClass
    {
        public static string cnnString = System.Configuration.ConfigurationManager.ConnectionStrings["UofLConnectDb"].ConnectionString;


        public static bool IsDashboard { get; set; }


        public static void GetUserInfo(string userID)
        {
            SqlConnection cnn = new SqlConnection(cnnString);
            DataTable dt = new DataTable();

            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "GET_USER_INFO"
            };

            try
            {
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;


                cnn.Open();
                dt.Load(cmd.ExecuteReader());


                foreach(DataRow row in dt.Rows)
                {
                    UserInfo.FName = row["FName"].ToString();
                    UserInfo.LName = row["LName"].ToString();
                    UserInfo.DOB = row["DOB"].ToString();
                    UserInfo.Ethnicity = row["Ethnicity"].ToString();
                    UserInfo.Orientation = row["Orientation"].ToString();
                    UserInfo.Email = row["Email"].ToString();
                    UserInfo.Gender = row["Gender"].ToString();
                    UserInfo.StudentID = row["StudentID"].ToString();
                    UserInfo.UserName = row["UserName"].ToString();
                    UserInfo.IsProfileCompleted = row["IsProfileCompleted"].ToString();
                    UserInfo.IsApproved = row["IsApproved"].ToString();
                    UserInfo.IsDenied = row["IsDenied"].ToString();
                    UserInfo.Grade = row["Grade"].ToString();
                    UserInfo.Job = row["Job"].ToString();
                    UserInfo.Major = row["Major"].ToString();
                    UserInfo.Industry = row["Industry"].ToString();
                    UserInfo.Age = row["Age"].ToString();
                }
                GetUserRoles(UserInfo.UserID.ToString());
                GetUserCourses(UserInfo.UserID.ToString());
            }
            catch (Exception)
            {
                throw new HttpException((int)HttpStatusCode.BadRequest, "Error in GetUserInfo");
            }
            finally
            {
                cnn.Close();
                cmd.Dispose();
            }
        }

        public static List<MentorInfo> GetAllMentors()
        {
            SqlConnection cnn = new SqlConnection(cnnString);
            DataTable dt = new DataTable();
            List<MentorInfo> mentors = new List<MentorInfo>();

            mentors?.Clear();

            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "GET_ALL_MENTORS"
            };

            try
            {
                cnn.Open();
                dt.Load(cmd.ExecuteReader());


                foreach (DataRow row in dt.Rows)
                {
                    mentors.Add(new MentorInfo
                    {
                        UserID = row["UserID"].ToString(),
                        FName = row["FNAME"].ToString(),
                        LName = row["LNAME"].ToString(),
                        DOB = row["DOB"].ToString(),
                        Ethnicity = row["Ethnicity"].ToString(),
                        Orientation = row["Orientation"].ToString(),
                        Email = row["Email"].ToString(),
                        Gender = row["Gender"].ToString(),
                        StudentID = row["StudentID"].ToString(),
                        UserName = row["UserName"].ToString(),
                        IsProfileCompleted = row["IsProfileCompleted"].ToString(),
                        Grade = row["Grade"].ToString(),
                        Job = row["Job"].ToString(),
                        Major = row["Major"].ToString(),
                        Industry = row["Industry"].ToString(),
                        Age = row["Age"].ToString(),
                        RoleName = row["Role_Name"].ToString(),
                        CurrentCourses = GetUserCurrentCourses(row["UserID"].ToString()),
                        PrevCourses = GetUserPrevCourses(row["UserID"].ToString())
                    });
                }

                float matchCount;
                foreach(MentorInfo mentor in mentors)
                {
                    matchCount = 0;

                    if (Math.Abs((int.Parse(UserInfo.Age) - int.Parse(mentor.Age))) <= 1)
                        matchCount++;
                    if (UserInfo.Ethnicity == mentor.Ethnicity)
                        matchCount++;
                    if(UserInfo.Orientation == mentor.Orientation)
                        matchCount++;
                    if(UserInfo.Gender == mentor.Gender)
                        matchCount++;
                    if(UserInfo.Grade == mentor.Grade)
                        matchCount++;
                    if(UserInfo.Major == mentor.Major)
                        matchCount++;
                    if (UserInfo.Industry == mentor.Industry)
                        matchCount++;
                    
                    foreach(string course in UserInfo.CurrentCourses)
                    {
                        float p = 5.00f / UserInfo.CurrentCourses.Count;
                        var match = mentor.CurrentCourses.FirstOrDefault(s => s.Contains(course));

                        if(match != null)
                        {
                            matchCount += p;
                        }
                    }

                    foreach(string course in UserInfo.PrevCourses)
                    {
                        float p = 8.00f / UserInfo.PrevCourses.Count;
                        var match = mentor.PrevCourses.FirstOrDefault(s => s.Contains(course));

                        if(match != null)
                        {
                            matchCount += p;
                        }
                    }

                    mentor.MatchPercentage = (matchCount / 20).ToString("P2");
                }

                return mentors;
            }
            catch (Exception ex)
            {
                throw new HttpException((int)HttpStatusCode.BadRequest, "Error in GetUserInfo. Message: " + ex.Message);
            }
            finally
            {
                cnn?.Close();
                cmd?.Dispose();
                dt?.Clear();
            }
        }

        public static void GetUserCourses(string userID)
        {
            SqlConnection cnn = new SqlConnection(cnnString);
            DataTable dt = new DataTable();

            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "GET_USER_COURSES"
            };

            try
            {
                cmd.Parameters.Add("@USERID", SqlDbType.Int).Value = userID;

                cnn.Open();
                dt.Load(cmd.ExecuteReader());

                List<string> currCourses = new List<string>();
                List<string> prevCourses = new List<string>();

                foreach (DataRow row in dt.Rows)
                {
                    if(row["IsCurrent"].ToString() == "True")
                    {
                        currCourses.Add(row["Course_Text"].ToString());
                    }
                    else
                    {
                        prevCourses.Add(row["Course_Text"].ToString());
                    }
                }

                UserInfo.CurrentCourses = currCourses;
                UserInfo.PrevCourses = prevCourses;
            }
            catch (Exception ex)
            {
                throw new HttpException((int)HttpStatusCode.BadRequest, "Error in GetUserInfo. Message: " + ex.Message);
            }
            finally
            {
                cnn.Close();
                cmd.Dispose();
            }
        }

        public static void GetUserRoles(string userID)
        {
            SqlConnection cnn = new SqlConnection(cnnString);
            DataTable dt = new DataTable();

            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "GET_USER_ROLES"
            };

            try
            {
                cmd.Parameters.Add("@USERID", SqlDbType.Int).Value = userID;

                cnn.Open();
                dt.Load(cmd.ExecuteReader());

                List<string> roles = new List<string>();

                foreach (DataRow row in dt.Rows)
                {
                    roles.Add(row["ROLE_NAME"].ToString());
                }

                UserInfo.RoleName = roles;

                // If the user is an admin set the property to true
                var match = UserInfo.RoleName.FirstOrDefault(s => s.Contains("Admin"));
                if (match != null)
                    UserInfo.IsAdmin = true;
            }
            catch (Exception ex)
            {
                throw new HttpException((int)HttpStatusCode.BadRequest, "Error in GetUserRoles. Message: " + ex.Message);
            }
            finally
            {
                cnn.Close();
                cmd.Dispose();
            }
        }

        public static List<string> GetUserCurrentCourses(string userID)
        {
            SqlConnection cnn = new SqlConnection(cnnString);
            DataTable dt = new DataTable();
            List<string> currentCourses = new List<string>();

            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "GET_USER_CURRENT_COURSES"
            };

            try
            {
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;

                cnn.Open();
                dt.Load(cmd.ExecuteReader());

                foreach (DataRow row in dt.Rows)
                {
                    currentCourses.Add(row["Course_Text"].ToString());
                }

                return currentCourses;
            }
            catch (Exception)
            {
                throw new HttpException((int)HttpStatusCode.BadRequest, "Error in GetUserInfo");
            }
            finally
            {
                cnn.Close();
                cmd.Dispose();
            }
        }

        public static List<string> GetUserPrevCourses(string userID)
        {
            SqlConnection cnn = new SqlConnection(cnnString);
            DataTable dt = new DataTable();
            List<string> prevCourses = new List<string>();

            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "GET_USER_PREV_COURSES"
            };

            try
            {
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;

                cnn.Open();
                dt.Load(cmd.ExecuteReader());

                foreach (DataRow row in dt.Rows)
                {
                    prevCourses.Add(row["Course_Text"].ToString());
                }

                return prevCourses;
            }
            catch (Exception)
            {
                throw new HttpException((int)HttpStatusCode.BadRequest, "Error in GetUserInfo");
            }
            finally
            {
                cnn.Close();
                cmd.Dispose();
            }
        }

        public static List<CalendarEventsModel> GetEventById(string id)
        {
            SqlConnection cnn = new SqlConnection(cnnString);
            DataTable dt = new DataTable();
            List<CalendarEventsModel> ev = new List<CalendarEventsModel>();

            ev.Clear();

            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "GET_EVENT_BY_ID"
            };

            try
            {
                cmd.Parameters.Add("@EVENTID", SqlDbType.Int).Value = id;

                cnn.Open();
                dt.Load(cmd.ExecuteReader());

                foreach (DataRow row in dt.Rows)
                {
                    ev.Add( new CalendarEventsModel {
                        id = row["Event_ID"].ToString(),
                        title = row["Event_Name"].ToString(),
                        start = row["Event_Start"].ToString(),
                        end = row["Event_End"].ToString(),
                        Description = row["Event_Description"].ToString(),
                        allDay = row["AllDay"].ToString() == "True" ? true : false,
                        RepeatingEvent = row["RepeatingEvent"].ToString() == "True" ? true : false,
                        Visibility = row["Visibility"].ToString(),
                    });
                }

                return ev;
            }
            catch (Exception ex)
            {
                throw new HttpException((int)HttpStatusCode.BadRequest, "Error in GetUserInfo. Message: " + ex.Message);
            }
            finally
            {
                cnn.Close();
                cmd.Dispose();
            }
        }
    }
}