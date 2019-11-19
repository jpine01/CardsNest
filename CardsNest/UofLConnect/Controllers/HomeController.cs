using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UofLConnect.Models.Home;
using System.Data.SqlClient;
using System.Data;
using System.Net;
using UofLConnect.Utilities;
using UofLConnect.Models.Mentor;

namespace UofLConnect.Controllers
{
    public class HomeController : Controller
    {
        public static string cnnString = System.Configuration.ConfigurationManager.ConnectionStrings["UofLConnectDb"].ConnectionString;
        private string infoMsg = "Your account must be approved by an Admin before you can access the system. Please wait until an Admin can review your account";

        public ActionResult Index()
        {
            UtilClass.IsDashboard = false;

            return View();
        }

        public ActionResult Calendar()
        {
            if (UserInfo.UserID == 0)
                return RedirectToAction("Login", "Home");

            if (UserInfo.IsApproved == "False")
                return RedirectToAction("Information", "Home", new { msg = infoMsg });

            return View();
        }

        public ActionResult Login(string msg = "")
        {
            UtilClass.IsDashboard = false;


            if (UserInfo.UserID != 0 && UserInfo.IsApproved == "True" && UserInfo.IsProfileCompleted == "True")
                return RedirectToAction("Network", "Home");

            ViewBag.Msg = msg;

            return View();
        }

        public ActionResult LoginSubmit(LoginModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return RedirectToAction("NotAllowed", "Errors");

                int userID = LoginUser(model);
                if (userID == 0)
                    return RedirectToAction("Login", "Home", new { msg = "Username and Password do not match" });

                UserInfo.UserID = userID;

                if (UserInfo.UserID != 0)
                    UtilClass.GetUserInfo(UserInfo.UserID.ToString());

                if (UserInfo.IsApproved == "False")
                    return RedirectToAction("Information", "Home", new { msg = infoMsg });

                if (UserInfo.IsProfileCompleted == "True")
                return RedirectToAction("Network", "Home");

                
                    return RedirectToAction("UserProfile", "Home");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Register", "Home", new { message = "Error Occured" + ex.Message });
            }

        }

        public ActionResult Dashboard()
        {
            UtilClass.IsDashboard = true;
            if (UserInfo.UserID == 0)
                return RedirectToAction("Login", "Home");

            if (UserInfo.IsApproved == "False")
                return RedirectToAction("Information", "Home", new { msg = infoMsg });

            return RedirectToAction("Network", "Home");
        }

        public ActionResult Chat()
        {
            return View();
        }

        public ActionResult Network()
        {
            UtilClass.IsDashboard = true;

            if (UserInfo.UserID == 0)
                return RedirectToAction("Login", "Home");

            if (UserInfo.IsApproved == "False")
                return RedirectToAction("Information", "Home", new { msg = infoMsg });

            if(UserInfo.RoleName[0] == "Mentor")
            {
                return RedirectToAction("MentorNetwork", "Home");
            }

            if (UserInfo.RoleName[0] == "Mentee")
            {
                if(getMatchedMentor()[0] > 0)
                {

                    int[] matchedUser = getMatchedMentor();

                    int matchedUserID = matchedUser[0];

                    TempData["mentorUserID"] = matchedUserID;

                    return RedirectToAction("MatchedMenteeNetwork", "Home");

                }
            }

            ViewBag.myID = UserInfo.UserID;

            List<MentorInfo> mentors = UtilClass.GetAllMentors();
            mentors = mentors.OrderByDescending(o => o.MatchPercentage).ToList();

            return View(mentors);
        }

        public ActionResult MentorNetwork()
        {
          

            try
            {
                if (getMatchedUser()[0] > 0)
                {



                    int[] matchedUser = getMatchedUser();



                    int matchedUserID = matchedUser[0];

                    TempData["menteeUserID"] = matchedUserID;

                    return RedirectToAction("MatchedMentorNetwork", "Home");


                }

                int[] mentorRequestUserIDs = getMentorMatchRequests();

                int reqUserID = mentorRequestUserIDs[0];

                ProfileModel pm = GetUsersProfileInfo(reqUserID);

                ViewBag.pm = pm;

                ViewBag.reqUserID = reqUserID;

                ViewBag.myID = UserInfo.UserID;

                

                UtilClass.IsDashboard = true;

                if (UserInfo.UserID == 0)
                    return RedirectToAction("Login", "Home");

                if (UserInfo.IsApproved == "False")
                    return RedirectToAction("Information", "Home", new { msg = infoMsg });


                List<MentorInfo> mentors = UtilClass.GetAllMentors();
                mentors = mentors.OrderByDescending(o => o.MatchPercentage).ToList();

                return View(mentors);
            }
            catch (Exception)
            {

                return RedirectToAction("UnRequestedMentorNetwork", "Home");

            }
            finally
            {
         

            }
        }

        public ActionResult MatchedMenteeNetwork()
        {
            int matchedUserID = Convert.ToInt32(TempData["mentorUserID"]);

            ProfileModel pm = GetUsersProfileInfo(matchedUserID);

            ViewBag.matchedUserID = matchedUserID;

            ViewBag.pm = pm;

            ViewBag.myID = UserInfo.UserID;

            UtilClass.IsDashboard = true;

            if (UserInfo.UserID == 0)
                return RedirectToAction("Login", "Home");

            if (UserInfo.IsApproved == "False")
                return RedirectToAction("Information", "Home", new { msg = infoMsg });

            List<MentorInfo> mentors = UtilClass.GetAllMentors();
            mentors = mentors.OrderByDescending(o => o.MatchPercentage).ToList();

            return View(mentors);

        }

        public ActionResult UnRequestedMentorNetwork()
        {
            UtilClass.IsDashboard = true;

            if (UserInfo.UserID == 0)
                return RedirectToAction("Login", "Home");

            if (UserInfo.IsApproved == "False")
                return RedirectToAction("Information", "Home", new { msg = infoMsg });

            List<MentorInfo> mentors = UtilClass.GetAllMentors();
            mentors = mentors.OrderByDescending(o => o.MatchPercentage).ToList();

            ViewBag.myID = UserInfo.UserID;


            return View(mentors);

        }

       public ActionResult MatchedMentorNetwork()
        {
            int matchedUserID = Convert.ToInt32(TempData["menteeUserID"]);

            ProfileModel pm = GetUsersProfileInfo(matchedUserID);

            ViewBag.matchedUserID = matchedUserID;

            ViewBag.pm = pm;

            ViewBag.myID = UserInfo.UserID;

            UtilClass.IsDashboard = true;

            if (UserInfo.UserID == 0)
                return RedirectToAction("Login", "Home");

            if (UserInfo.IsApproved == "False")
                return RedirectToAction("Information", "Home", new { msg = infoMsg });

            List<MentorInfo> mentors = UtilClass.GetAllMentors();
            mentors = mentors.OrderByDescending(o => o.MatchPercentage).ToList();

            return View(mentors);
         
        }

        [HttpPost]
        public JsonResult viewMatchedProfile(int userID)
        {

            TempData["viewID"] = userID;


            return Json("View Matched Profile Executed Successfully");

        }

        [HttpPost]
        public JsonResult viewMentorProfile(int userID)
        {

            TempData["mentorID"] = userID;

            return Json(" Matched Mentor Executed Successfully");
        }

        [HttpPost]
        public JsonResult viewRequestedProfile(int userID)
        {
            

            TempData["reqUserID"] = userID;


            return Json("View Matched Profile Executed Successfully");

        }

        public ActionResult CourseRegistration()
        {
            try
            {
                UtilClass.IsDashboard = false;
                if (UserInfo.UserID == 0)
                    return RedirectToAction("Login", "Home");

                List<SelectListItem> allCourses = GetAllCourses();
                List<SelectListItem> allHobbies = GetAllHobbies();
                List<SelectListItem> allProfessions = GetAllProfessions();
                int count = allCourses.Count; // Total amount of courses to know how long our arrays need to be
                ProfileModel pm = GetProfileInfo();
                List<SelectListItem> currentCourses = GetCurrentCourses();
                List<SelectListItem> prevCourses = GetPreviousCourses();
                ProfileModel check = CheckMajor(pm.Major);



                var model = new ProfileModel
                {
                    SelectedCourses = new string[count],
                    CourseList = allCourses,
                    CurrentCourses = new string[count],
                    CurrentCourseList = currentCourses,
                    PreviousCourses = new string[count],
                    PreviousCourseList = prevCourses,
                    FirstName = pm.FirstName,
                    LastName = pm.LastName,
                    Email = pm.Email,
                    DOB = pm.DOB,
                    Ethnicity = pm.Ethnicity,
                    Gender = pm.Gender,
                    SexualOrientation = pm.SexualOrientation,
                    Grade = pm.Grade,
                    Job = pm.Job,
                    No_Path = check.No_Path,
                    Web_Dev = check.Web_Dev,
                    Info_Sec = check.Info_Sec,
                    BPM = check.BPM,
                    Major = 0,
                    Industry = pm.Industry,

                };

                return View(model);
            }
            catch (Exception)
            {
                return RedirectToAction("Register", "Home", new { message = "Error Occured" });
            }

        }


        [HttpPost]
        public ActionResult getInterests(string[] professionalInterestIDs, string[] personalInterestIDs, string[] personalInterestNames, string[] professionalInterestNames, InterestsModel model)
        {
            List<SelectListItem> allHobbies = GetAllHobbies();
            List<SelectListItem> allProfessions = GetAllProfessions();
            int hobbieCount = allHobbies.Count; // Total amount of courses to know how long our arrays need to be
            int professionCount = allProfessions.Count; // Total amount of professions to know how long our array needs to be 
            int[] professionalIDsParsed = new int[professionalInterestIDs.Length];
            int[] personaIDsParsed = new int[personalInterestIDs.Length];

            for (int i = 0; i < professionalInterestIDs.Length; i++)
            {
                professionalIDsParsed[i] = int.Parse(professionalInterestIDs[i]);
            }

            for (int i = 0; i < professionalInterestIDs.Length; i++)
            {
                personaIDsParsed[i] = int.Parse(personalInterestIDs[i]);
            }


            var reqmodel = new InterestsModel
            {
                PersonalInterest_List = allHobbies,
                ProfessionalInterest_List = allProfessions,
                Personal_Interest_ID = 0,
                Personal_Interest_Name = "",
                User_ID = 0,
                Professional_Interest_ID = 0,
                Professional_Interest_Name = "",
                Professional_Industry = 0,
                PersonalInterests = new string[hobbieCount],
                ProfessionalInterests = new string[professionCount],
                SelectedPersonalInterests = new List<SelectListItem>(),
                SelectedProfessionalInterests = new List<SelectListItem>(),
                SelectedPersonalInterestsList = new List<SelectListItem>(),
                SelectedProfessionalInterestsList = new List<SelectListItem>(),
                SelectedPersonalInterestNames = personalInterestNames,
                SelectedProfessionalInterestNames = professionalInterestNames,
                SelectedPersonallInterestIDs = personaIDsParsed,
                SelectedProfessionalInterestIDs = professionalIDsParsed
            };


            SqlConnection cnn = new SqlConnection(cnnString);

            SqlCommand personal_cmd = new SqlCommand
            {
                Connection = cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "REGISTER_PERSONAL_INTERESTS_TABLE"
            };

            SqlCommand professional_cmd = new SqlCommand
            {
                Connection = cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "REGISTER_PROFESSIONAL_INTERESTS_TABLE"
            };


            DataTable dt = new DataTable();

            dt.Columns.AddRange(new DataColumn[3]
            {
                new DataColumn("Interest_ID", typeof(int)),
                new DataColumn("User_ID", typeof(int)),
                new DataColumn("Interest_Name", typeof(string))
            });

            for (int i = 0; i < reqmodel.SelectedPersonallInterestIDs.Length; i++)
            {
                int Interest_ID = reqmodel.SelectedPersonallInterestIDs[i];
                int User_ID = UserInfo.UserID;
                string Interest_Name = reqmodel.SelectedPersonalInterestNames[i];
                dt.Rows.Add(Interest_ID, User_ID, Interest_Name);
            }

            DataTable db = new DataTable();

            db.Columns.AddRange(new DataColumn[4]
            {
                new DataColumn("Interest_ID", typeof(int)),
                new DataColumn("User_ID", typeof(int)),
                new DataColumn("Interest_Name", typeof(string)),
                new DataColumn("Industry", typeof(int))
            });

            for (int i = 0; i < reqmodel.SelectedProfessionalInterestIDs.Length; i++)
            {
                int Interest_ID = reqmodel.SelectedProfessionalInterestIDs[i];
                int User_ID = UserInfo.UserID;
                string Interest_Name = reqmodel.SelectedProfessionalInterestNames[i];
                int Industry = 1;
                db.Rows.Add(Interest_ID, User_ID, Interest_Name, Industry);
            }

            try
            {
                if (ModelState.IsValid && model != null)
                {

                    personal_cmd.Parameters.AddWithValue("@PERSONAL_INTEREST_TABLE", dt);

                    professional_cmd.Parameters.AddWithValue("@Professional_Interest_Table", db);

                    cnn.Open();
                    personal_cmd.ExecuteNonQuery();
                    professional_cmd.ExecuteNonQuery();


                    return RedirectToAction("Network", "Home");
                }

                return RedirectToAction("Interests", "Home", new { message = "Check your data and try again." });
            }
            catch (Exception)
            {

                return RedirectToAction("Interests", "Home", new { message = "Error Occured" });

            }
            finally
            {
                cnn?.Close();
                personal_cmd?.Dispose();
                professional_cmd?.Dispose();
            }
        }



        public ActionResult Interests(string message = "")
        {

            try
            {
                UtilClass.IsDashboard = false;
                if (UserInfo.UserID == 0)
                    return RedirectToAction("Login", "Home");


                List<SelectListItem> allHobbies = GetAllHobbies();
                List<SelectListItem> allProfessions = GetAllProfessions();
                int hobbieCount = allHobbies.Count; // Total amount of courses to know how long our arrays need to be
                int professionCount = allProfessions.Count; // Total amount of professions to know how long our array needs to be 
                List<SelectListItem> PreviousProfessionalInterests = GetProfessionalInterests();
                List<SelectListItem> PreviousPersonalInterests = GetPersonalInterests();

                var model = new InterestsModel
                {

                    PersonalInterest_List = allHobbies,
                    ProfessionalInterest_List = allProfessions,
                    Personal_Interest_ID = 0,
                    Personal_Interest_Name = "",
                    User_ID = 0,
                    Professional_Interest_ID = 0,
                    Professional_Interest_Name = "",
                    Professional_Industry = 0,
                    PersonalInterests = new string[hobbieCount],
                    ProfessionalInterests = new string[professionCount],
                    SelectedPersonalInterests = new List<SelectListItem>(),
                    SelectedProfessionalInterests = new List<SelectListItem>(),
                    SelectedPersonalInterestsList = PreviousPersonalInterests,
                    SelectedProfessionalInterestsList = PreviousProfessionalInterests,

                };

                ViewBag.message = message;
                return View(model);
            }
            catch (Exception)
            {
                return RedirectToAction("Interests", "Home", new { message = "Error Occured" });
            }
        }


        public ActionResult InterestSave(InterestsModel model)
        {

            //SqlConnection cnn = new SqlConnection(cnnString);

            //SqlCommand personal_cmd = new SqlCommand
            //{
            //    Connection = cnn,
            //    CommandType = CommandType.StoredProcedure,
            //    CommandText = "REGISTER_PERSONAL_INTERESTS_TABLE"
            //};

            //SqlCommand professional_cmd = new SqlCommand
            //{
            //    Connection = cnn,
            //    CommandType = CommandType.StoredProcedure,
            //    CommandText = "REGISTER_PROFESSIONAL_INTERESTS_TABLE"
            //};

            //int[] ids = { 1, 2, 3, 4, 5 };

            //string[] names = { "john", "mark", "stacy", "lucy", "eric" };

            //DataTable dt = new DataTable();

            //dt.Columns.AddRange(new DataColumn[3]
            //{
            //    new DataColumn("Interest_ID", typeof(int)),
            //    new DataColumn("User_ID", typeof(int)),
            //    new DataColumn("Interest_Name", typeof(string))
            //});

            //for(int i = 0; i < names.Length; i++)
            //{
            //    int Interest_ID = ids[i];
            //    int User_ID = UserInfo.UserID;
            //    string Interest_Name = names[i];
            //    dt.Rows.Add(Interest_ID, User_ID, Interest_Name);
            //}

            //DataTable db = new DataTable();

            //db.Columns.AddRange(new DataColumn[4]
            //{
            //    new DataColumn("Interest_ID", typeof(int)),
            //    new DataColumn("User_ID", typeof(int)),
            //    new DataColumn("Interest_Name", typeof(string)),
            //    new DataColumn("Industry", typeof(int))
            //});

            //for (int i = 0; i < names.Length; i++)
            //{
            //    int Interest_ID = ids[i];
            //    int User_ID = UserInfo.UserID;
            //    string Interest_Name = names[i];
            //    int Industry = 1;
            //    db.Rows.Add(Interest_ID, User_ID, Interest_Name,Industry);
            //}

            //try
            //{
            //    if (ModelState.IsValid && model != null)
            //    {

            //        personal_cmd.Parameters.AddWithValue("@PERSONAL_INTEREST_TABLE", dt);

            //        professional_cmd.Parameters.AddWithValue("@Professional_Interest_Table", db);

            //        cnn.Open();
            //        personal_cmd.ExecuteNonQuery();
            //        professional_cmd.ExecuteNonQuery();


            //    return RedirectToAction("Dashboard", "Home");
            //    }

            //    return RedirectToAction("Interests", "Home", new { message = "Check your data and try again." });
            //}
            //catch (Exception)
            //{

            //    return RedirectToAction("Interests", "Home", new { message = "Error Occured" });

            //}
            //finally
            //{
            //    cnn?.Close();
            //    personal_cmd?.Dispose();
            //    professional_cmd?.Dispose();
            //}
            return RedirectToAction("Index", "Home");

        }





        public ActionResult Information(string msg)
        {
            UtilClass.IsDashboard = false;

            if (UserInfo.IsDenied == "True")
                ViewBag.Msg = "Your account was Denied by an Admin, if you have any questions email: rmbark01@louisville.edu";
            else
                ViewBag.Msg = msg;

            return View();
        }

        public ActionResult _CalendarModal(string info)
        {
            if (info == "Add")
            {
                ViewBag.title = "New Event";
                ViewBag.id = "";

                return View(new CalendarEventsModel());
            }
            else
            {
                var calEvent = UtilClass.GetEventById(info);

                var model = new CalendarEventsModel
                {
                    id = calEvent[0].id,
                    title = calEvent[0].title,
                    start = calEvent[0].start,
                    end = calEvent[0].end,
                    Description = calEvent[0].Description,
                    allDay = calEvent[0].allDay,
                    RepeatingEvent = calEvent[0].RepeatingEvent,
                    Visibility = calEvent[0].Visibility
                };

                ViewBag.allDay = calEvent[0].allDay;
                ViewBag.id = calEvent[0].id;

                return View(model);
            }
        }

        public ActionResult AddEditCalendar(CalendarEventsModel model)
        {
            SqlConnection cnn = new SqlConnection(cnnString);

            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = model.Mode == "Add" ? "ADD_CAL_EVENT" : "EDIT_CAL_EVENT"
            };

            try
            {
                if (model.Mode == "Add")
                {
                    cmd.Parameters.Add("@USERID", SqlDbType.Int).Value = UserInfo.UserID;
                    cmd.Parameters.Add("@TITLE", SqlDbType.VarChar, 50).Value = model.title;
                    cmd.Parameters.Add("@ALLDAY", SqlDbType.Bit).Value = model.allDay == true ? 1 : 0;
                    cmd.Parameters.Add("@REPEATINGEVENT", SqlDbType.Bit).Value = model.RepeatingEvent == true ? 1 : 0;
                    cmd.Parameters.Add("@VISIBILITY", SqlDbType.VarChar, 25).Value = model.Visibility;
                    cmd.Parameters.Add("@START", SqlDbType.VarChar, 30).Value = model.start;
                    cmd.Parameters.Add("@END", SqlDbType.VarChar, 30).Value = model.end ?? string.Empty;
                    cmd.Parameters.Add("@DESCRIPTION", SqlDbType.VarChar, 250).Value = model.Description ?? string.Empty;
                }
                else
                {
                    if (model.allDay)
                        model.end = string.Empty;

                    cmd.Parameters.Add("@USERID", SqlDbType.Int).Value = UserInfo.UserID;
                    cmd.Parameters.Add("@EVENTID", SqlDbType.Int).Value = model.id;
                    cmd.Parameters.Add("@TITLE", SqlDbType.VarChar, 50).Value = model.title;
                    cmd.Parameters.Add("@ALLDAY", SqlDbType.Bit).Value = model.allDay == true ? 1 : 0;
                    cmd.Parameters.Add("@REPEATINGEVENT", SqlDbType.Bit).Value = model.RepeatingEvent == true ? 1 : 0;
                    cmd.Parameters.Add("@VISIBILITY", SqlDbType.VarChar, 25).Value = model.Visibility;
                    cmd.Parameters.Add("@START", SqlDbType.VarChar, 30).Value = model.start;
                    cmd.Parameters.Add("@END", SqlDbType.VarChar, 30).Value = model.end ?? string.Empty;
                    cmd.Parameters.Add("@DESCRIPTION", SqlDbType.VarChar, 250).Value = model.Description ?? string.Empty;
                }


                cnn.Open();
                cmd.ExecuteNonQuery();

                return Json(new { isSuccess = true, message = "Success!" });


            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = ex.Message });

            }
            finally
            {
                cnn?.Close();
                cmd?.Dispose();
            }
        }

        public ActionResult GetCalEvents()
        {
            SqlConnection cnn = new SqlConnection(cnnString);
            List<CalendarEventsModel> events = new List<CalendarEventsModel>();
            DataTable dt = new DataTable();

            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "GET_CAL_EVENTS"
            };

            try
            {

                cmd.Parameters.Add("@USERID", SqlDbType.Int).Value = UserInfo.UserID;

                cnn.Open();
                dt.Load(cmd.ExecuteReader());

                foreach (DataRow row in dt.Rows)
                {
                    events.Add(new CalendarEventsModel
                    {
                        id = row["Event_ID"].ToString(),
                        title = row["Event_Name"].ToString(),
                        start = row["Event_Start"].ToString(),
                        end = row["Event_End"].ToString(),
                        allDay = row["AllDay"].ToString() == "True" ? true : false,
                        RepeatingEvent = row["RepeatingEvent"].ToString() == "True" ? true : false,
                    });
                }

                return Json(events.ToArray(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new List<CalendarEventsModel>().ToArray(), JsonRequestBehavior.AllowGet);
            }
            finally
            {
                cnn?.Close();
                cmd?.Dispose();
            }
        }

        public ActionResult _Register()
        {
            ViewBag.email = "@louisville.edu";

            return PartialView(new RegisterModel());
        }


        [HttpPost]
        public ActionResult registerCourses(string[] previousCourseNames, string[] previousCourseIDs, string[] currentCourseNames, string[] currentCourseIDs)
        {
            SqlConnection cnn = new SqlConnection(cnnString);

            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "ADD_COURSES_HISTORY_TABLES"
            };


            int[] parsedPrevious = new int[previousCourseIDs.Length];

            int[] parsedCurrent = new int[currentCourseIDs.Length];

            for (int i = 0; i < previousCourseIDs.Length; i++)
            {
                parsedPrevious[i] = int.Parse(previousCourseIDs[i]);
            }

            for (int i = 0; i < currentCourseIDs.Length; i++)
            {
                parsedCurrent[i] = int.Parse(currentCourseIDs[i]);
            }


            DataTable db = new DataTable();

            db.Columns.AddRange(new DataColumn[4]
            {
                new DataColumn("Course_ID", typeof(int)),
                new DataColumn("User_ID", typeof(int)),
                new DataColumn("Course_Text", typeof(string)),
                new DataColumn("IsCurrent", typeof(bool))
            });

            for (int i = 0; i < parsedCurrent.Length; i++)
            {
                int Course_ID = parsedCurrent[i];
                int User_ID = UserInfo.UserID;
                string Course_Text = currentCourseNames[i];
                bool IsCurrent = true;
                db.Rows.Add(Course_ID, User_ID, Course_Text, IsCurrent);
            }


            DataTable dp = new DataTable();

            dp.Columns.AddRange(new DataColumn[4]
            {
                new DataColumn("Course_ID", typeof(int)),
                new DataColumn("User_ID", typeof(int)),
                new DataColumn("Course_Text", typeof(string)),
                new DataColumn("IsCurrent", typeof(bool))
            });

            for (int i = 0; i < parsedPrevious.Length; i++)
            {
                int Course_ID = parsedPrevious[i];
                int User_ID = UserInfo.UserID;
                string Course_Text = previousCourseNames[i];
                bool IsCurrent = false;
                dp.Rows.Add(Course_ID, User_ID, Course_Text, IsCurrent);
            }



            try
            {



                    //  Classes Data table to register current courses
                    cmd.Parameters.AddWithValue("@CURRENT_COURSES_TABLE", db);


                    // Classes Data table to register previous courses
                    cmd.Parameters.AddWithValue("@PREVIOUS_COURSES_TABLE", dp);

                    cnn.Open();

                    cmd.ExecuteNonQuery();

                    return Json("Classes Successfully Registered");

            }
            catch (Exception)
            {
                return Json("Error Registering Classes");
            }
            finally
            {

                cnn?.Close();
                cmd?.Dispose();

            }




        }




        public ActionResult UserProfile(string message = "")
        {
            try
            {
                UtilClass.IsDashboard = false;
                if (UserInfo.UserID == 0)
                    return RedirectToAction("Login", "Home");

                List<SelectListItem> allCourses = GetAllCourses();
                List<SelectListItem> allHobbies = GetAllHobbies();
                List<SelectListItem> allProfessions = GetAllProfessions();
                int count = allCourses.Count; // Total amount of courses to know how long our arrays need to be
                ProfileModel pm = GetProfileInfo();
                List<SelectListItem> currentCourses = GetCurrentCourses();
                List<SelectListItem> prevCourses = GetPreviousCourses();
                ProfileModel check = CheckMajor(pm.Major);




                var model = new ProfileModel
                {
                    SelectedCourses = new string[count],
                    CourseList = allCourses,
                    CurrentCourses = new string[count],
                    CurrentCourseList = currentCourses,
                    PreviousCourses = new string[count],
                    PreviousCourseList = prevCourses,
                    FirstName = pm.FirstName,
                    LastName = pm.LastName,
                    Email = pm.Email,
                    DOB = pm.DOB,
                    Ethnicity = pm.Ethnicity,
                    Gender = pm.Gender,
                    SexualOrientation = pm.SexualOrientation,
                    Grade = pm.Grade,
                    Job = pm.Job,
                    No_Path = check.No_Path,
                    Web_Dev = check.Web_Dev,
                    Info_Sec = check.Info_Sec,
                    BPM = check.BPM,
                    Major = 0,
                    Industry = pm.Industry,

                };

                ViewBag.message = message;
                return View(model);
            }
            catch (Exception)
            {
                return RedirectToAction("Register", "Home", new { message = "Error Occured" });
            }
        }


        public ActionResult ProfileSave(ProfileModel model)
        {
            SqlConnection cnn = new SqlConnection(cnnString);

            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "SAVE_PROFILE"
            };

            int major = GetMajor(model);

            try
            {
                if (ModelState.IsValid && model != null)
                {
                    // user profile paramaters
                    cmd.Parameters.Add("@USERID", SqlDbType.Int).Value = UserInfo.UserID;
                    cmd.Parameters.Add("@EMAIL", SqlDbType.VarChar, 50).Value = model.Email;
                    cmd.Parameters.Add("@FNAME", SqlDbType.VarChar, 50).Value = model.FirstName;
                    cmd.Parameters.Add("@LNAME", SqlDbType.VarChar, 50).Value = model.LastName;
                    cmd.Parameters.Add("@DOB", SqlDbType.Date).Value = model.DOB;
                    cmd.Parameters.Add("@ETHNICITY", SqlDbType.VarChar, 50).Value = model.Ethnicity;
                    cmd.Parameters.Add("@ORIENTATION", SqlDbType.VarChar, 50).Value = model.SexualOrientation;
                    cmd.Parameters.Add("@GENDER", SqlDbType.VarChar, 50).Value = model.Gender;
                    cmd.Parameters.Add("@GRADE", SqlDbType.Int).Value = model.Grade;
                    cmd.Parameters.Add("@JOB", SqlDbType.VarChar, 55).Value = model.Job;
                    cmd.Parameters.Add("@MAJOR", SqlDbType.Int).Value = major;
                    cmd.Parameters.Add("@INDUSTRY", SqlDbType.Int).Value = model.Industry;
                    cmd.Parameters.Add("@ISPROFILECOMPLETED", SqlDbType.Bit).Value = model.IsProfileComplete;




                    cnn.Open();
                    cmd.ExecuteNonQuery();

                    //Adds user to Messaging table after profile completion
                    RegisterUserMessaging();

                    return RedirectToAction("CourseRegistration", "Home");
                }

                return RedirectToAction("Profile", "Home", new { message = "Check your data and try again." });
            }
            catch (Exception)
            {

                return RedirectToAction("Profile", "Home", new { message = "Error Occured" });

            }
            finally
            {
                cnn?.Close();
                cmd?.Dispose();
            }
        }




        public JsonResult RegisterSubmit(RegisterModel model)
        {
            SqlConnection cnn = new SqlConnection(cnnString);

            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "REGISTER_USER"
            };

            try
            {
                if (ModelState.IsValid && model != null)
                {
                    if (String.IsNullOrEmpty(model.StudentEmail) && String.IsNullOrEmpty(model.AlumniEmail))
                        return Json(new { isSuccess = false, message = "" });
                    else if (model.RegisterType == "Alumni")
                    {
                        if (String.IsNullOrEmpty(model.AlumniEmail))
                            return Json(new { isSuccess = false, message = "" });
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(model.StudentEmail) || String.IsNullOrEmpty(model.StudentID))
                            return Json(new { isSuccess = false, message = "" });

                        if (model.StudentID?.Trim()?.Length != 7)
                            return Json(new { isSuccess = false, message = "Student ID must be 7 digits long" });
                    }

                    model.Email = model.RegisterType == "Alumni" ? model.AlumniEmail.Trim() : model.StudentEmail.Trim() + "@louisville.edu";

                    if (DoesEmailExist(model.Email))
                        return Json(new { isSuccess = false, message = "Email already exists" });

                    int roleID = GetRoleID(model.RegisterType, model.SchoolYear, model.AlumniRole);
                    if (roleID == 0)
                        return Json(new { isSuccess = false, message = $"Invalid Combination {model.RegisterType} and {model.SchoolYear}" });

                    cmd.Parameters.Add("@FNAME", SqlDbType.VarChar, 50).Value = model.FirstName;
                    cmd.Parameters.Add("@LNAME", SqlDbType.VarChar, 50).Value = model.LastName;
                    cmd.Parameters.Add("@EMAIL", SqlDbType.VarChar, 50).Value = model.Email;
                    cmd.Parameters.Add("@USERNAME", SqlDbType.VarChar, 50).Value = model.UserName;
                    cmd.Parameters.Add("@PASSWORD", SqlDbType.VarChar, 50).Value = model.Password;
                    cmd.Parameters.Add("@STUDENTID", SqlDbType.VarChar).Value = model.StudentID ?? "0";
                    cmd.Parameters.Add("@ROLEID", SqlDbType.Int).Value = roleID;

                    cnn.Open();
                    cmd.ExecuteNonQuery();

                    return Json(new { isSuccess = true, message = "" });
                }

                return Json(new { isSuccess = false, message = "" });
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = ex.Message });
            }
            finally
            {
                cnn?.Close();
                cmd?.Dispose();
            }
        }

        public ActionResult courseSave()
        {
            return RedirectToAction("Interests", "Home");
        }

        public ActionResult LogOut()
        {
            UserInfo.UserID = 0;
            UserInfo.IsAdmin = false;
            UserInfo.IsApproved = null;
            UserInfo.IsDenied = null;

            return RedirectToAction("Index", "Home");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }


        public ActionResult Donate()
        {
            UtilClass.IsDashboard = false;

            ViewBag.Message = "Donate Page";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact Page";

            return View();
        }

        public ActionResult Report(string message = "")
        {
            UtilClass.IsDashboard = false;
            try
            {
                var model = new ReportModel
                {
                    Report_Time = "",
                    Report_Date = "",
                    Report_Name = "",
                    User_Name = "",
                    Report_Message = "",
                    Seen = false

                };

                ViewBag.message = message;
                return View(model);

            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        public ActionResult ReportSubmit(ReportModel model)
        {
            SqlConnection cnn = new SqlConnection(cnnString);

            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "REGISTER_REPORT"
            };

            try
            {
                if (ModelState.IsValid && model != null)
                {

                    cmd.Parameters.Add("@Report_Time", SqlDbType.Time, 7).Value = model.Report_Time;
                    cmd.Parameters.Add("@Report_Date", SqlDbType.Date).Value = model.Report_Date;
                    cmd.Parameters.Add("@Report_Name", SqlDbType.VarChar, 25).Value = model.Report_Name;
                    cmd.Parameters.Add("@User_Name", SqlDbType.VarChar, 25).Value = model.User_Name;
                    cmd.Parameters.Add("@Report_Message", SqlDbType.VarChar, 225).Value = model.Report_Message;
                    cmd.Parameters.Add("@Seen", SqlDbType.Bit).Value = model.Seen;
                    // Add logic to inserting current/previous courses

                    cnn.Open();
                    cmd.ExecuteNonQuery();

                    return RedirectToAction("Index", "Home");
                }

                return RedirectToAction("Report", "Home", new { message = "Check Your Data and Try Again." });
            }
            catch (Exception)
            {
                return RedirectToAction("Report", "Home", new { message = "Error Occured, Please Try Again" });
            }
            finally
            {
                cnn?.Close();
                cmd?.Dispose();
            }
        }

        public ActionResult Settings()
        {
            UtilClass.IsDashboard = true;

            if (UserInfo.UserID == 0)
                return RedirectToAction("Login", "Home");

            if (UserInfo.IsApproved == "False")
                return RedirectToAction("Information", "Home", new { msg = infoMsg });

            return View();
        }

        public ActionResult ViewProfile()
        {
            UtilClass.IsDashboard = true;

            if (UserInfo.UserID == 0)
                return RedirectToAction("Login", "Home");

            if (UserInfo.IsApproved == "False")
                return RedirectToAction("Information", "Home", new { msg = infoMsg });

            List<MentorInfo> mentors = UtilClass.GetAllMentors();
            mentors = mentors.OrderByDescending(o => o.MatchPercentage).ToList();

            //Gets tempdata variable that holds mentor index 
            int mentorIndex = Convert.ToInt32(TempData["mentorIndex"]);

            // Corrects index due to list starting from pos 0
            int correctedMentorIndex = mentorIndex -1;

            // Gets mentor from index sent by postMentofInfo
            MentorInfo returnMentor = mentors[correctedMentorIndex];

            // Returns selected mentor as viewbag
            ViewBag.returnMentor = returnMentor;

            string grade = "";
            string major = "";

            if (returnMentor.Major == "0")
            {
                major = "No Path";
            }
            else if (returnMentor.Major == "1")
            {
                major = "Web Development";
            }
            else if (returnMentor.Major == "2")
            {
                major = "Information Security";
            }
            else if (returnMentor.Major == "3")
            {
                major = "Business Process Management";
            }


            if (returnMentor.Grade == "1")
            {
                grade = "Freshman";
            }
            else if (returnMentor.Grade == "2")
            {
                grade = "Sophomore";
            }
            else if (returnMentor.Grade == "3")
            {
                grade = "Junior";
            }
            else if (returnMentor.Grade == "4")
            {
                grade = "Senior";
            }


            ViewBag.major = major;

            ViewBag.grade = grade;

            return View();

        }

        public ActionResult ViewProfileNoMatch()
        {
            UtilClass.IsDashboard = true;

            if (UserInfo.UserID == 0)
                return RedirectToAction("Login", "Home");

            if (UserInfo.IsApproved == "False")
                return RedirectToAction("Information", "Home", new { msg = infoMsg });

            List<MentorInfo> mentors = UtilClass.GetAllMentors();
            mentors = mentors.OrderByDescending(o => o.MatchPercentage).ToList();

            //Gets tempdata variable that holds mentor index 
            int mentorIndex = Convert.ToInt32(TempData["mentorIndex"]);

            // Corrects index due to list starting from pos 0
            int correctedMentorIndex = mentorIndex - 1;

            // Gets mentor from index sent by postMentofInfo
            MentorInfo returnMentor = mentors[correctedMentorIndex];

            // Returns selected mentor as viewbag
            ViewBag.returnMentor = returnMentor;

            string grade = "";
            string major = "";

            if (returnMentor.Major == "0")
            {
                major = "No Path";
            }
            else if (returnMentor.Major == "1")
            {
                major = "Web Development";
            }
            else if (returnMentor.Major == "2")
            {
                major = "Information Security";
            }
            else if (returnMentor.Major == "3")
            {
                major = "Business Process Management";
            }


            if (returnMentor.Grade == "1")
            {
                grade = "Freshman";
            }
            else if (returnMentor.Grade == "2")
            {
                grade = "Sophomore";
            }
            else if (returnMentor.Grade == "3")
            {
                grade = "Junior";
            }
            else if (returnMentor.Grade == "4")
            {
                grade = "Senior";
            }


            ViewBag.major = major;

            ViewBag.grade = grade;

            return View();
        }

        [HttpPost]
        public JsonResult postMentorInfo(int mentorIndex)
        {

            TempData["mentorIndex"] = mentorIndex;


            return Json("postMentorInfo Executed Properly");

        }

        [HttpPost]
        public JsonResult matchRequest(int UserID)
        {
            SqlConnection cnn = new SqlConnection(cnnString);

            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "REGISTER_MATCH_REQUEST"
            };

            try
            {
                if (UserID > -1)
                {
                    cmd.Parameters.Add("@REQ_USER_ID", SqlDbType.Int).Value = UserInfo.UserID;
                    cmd.Parameters.Add("@TARGET_USER_ID", SqlDbType.Int).Value = UserID;

                    // Add logic to inserting current/previous courses

                    cnn.Open();
                    cmd.ExecuteNonQuery();

                    return Json("matchRequest Executed Properly");
                }

                return Json("matchRequest Failed To Execute");
            }
            catch (Exception)
            {
                return Json("Exception Thrown"); ;
            }
            finally
            {
                cnn?.Close();
                cmd?.Dispose();
            }


            
        }

        [HttpPost]
        public JsonResult acceptMatch(int userID)
        {
            SqlConnection cnn = new SqlConnection(cnnString);

            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "REGISTER_MATCHED_USERS"
            };

            try
            {
                if (userID > -1)
                {
                    cmd.Parameters.Add("@MENTOR_USER_ID", SqlDbType.Int).Value = UserInfo.UserID;
                    cmd.Parameters.Add("@MENTEE_USER_ID", SqlDbType.Int).Value = userID;

                    // Add logic to inserting current/previous courses

                    cnn.Open();
                    cmd.ExecuteNonQuery();

                    return Json("matchRequest Executed Properly");
                }

                return Json("matchRequest Failed To Execute");
            }
            catch (Exception)
            {
                return Json("Exception Thrown"); ;
            }
            finally
            {
                cnn?.Close();
                cmd?.Dispose();
            }

        }

        [HttpPost]
        public ActionResult denyRequest(int userID)
        {
            SqlConnection cnn = new SqlConnection(cnnString);

            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "DENY_MENTOR_REQUEST"
            };

            try
            {
                if (userID > -1)
                {
                    cmd.Parameters.Add("@USERID", SqlDbType.Int).Value = UserInfo.UserID;
  

                    // Add logic to inserting current/previous courses

                    cnn.Open();
                    cmd.ExecuteNonQuery();

                    return Json("matchRequest Executed Properly");
                }

                return Json("matchRequest Failed To Execute");
            }
            catch (Exception)
            {
                return Json("Exception Thrown"); ;
            }
            finally
            {
                cnn?.Close();
                cmd?.Dispose();
            }




        }

        public ActionResult ViewNetworkProfile()
        {

            int userID = Convert.ToInt32(TempData["viewID"]);

            ProfileModel pm = GetUsersProfileInfo(userID);

            string major = "";
            string grade = "";

            if(pm.Major == 0)
            {
                major = "No Path";
            }
            else if(pm.Major == 1)
            {
                major = "Web Development";
            }
            else if(pm.Major == 2)
            {
                major = "Information Security";
            }
            else if(pm.Major == 3)
            {
                major = "Business Process Management";
            }

            if(pm.Grade == 1)
            {
                grade = "Freshman";
            }
            else if(pm.Grade == 2)
            {
                grade = "Sophomore";
            }
            else if(pm.Grade == 3)
            {
                grade = "Junior";
            }
            else if(pm.Grade == 4)
            {
                grade = "Senior";
            }

            ViewBag.major = major;

            ViewBag.grade = grade;

            ViewBag.pm = pm;

            string userName = GetUserName(userID);

            ViewBag.userName = userName;

            return View();

        }

        public ActionResult ViewMentorMatchedProfile()
        {
            int userID = Convert.ToInt32(TempData["mentorID"]);

            ProfileModel pm = GetUsersProfileInfo(userID);

            ViewBag.pm = pm;

            string userName = GetUserName(userID);

            ViewBag.userName = userName;

            string major = "";
            string grade = "";

            if (pm.Major == 0)
            {
                major = "No Path";
            }
            else if (pm.Major == 1)
            {
                major = "Web Development";
            }
            else if (pm.Major == 2)
            {
                major = "Information Security";
            }
            else if (pm.Major == 3)
            {
                major = "Business Process Management";
            }

            if (pm.Grade == 1)
            {
                grade = "Freshman";
            }
            else if (pm.Grade == 2)
            {
                grade = "Sophomore";
            }
            else if (pm.Grade == 3)
            {
                grade = "Junior";
            }
            else if (pm.Grade == 4)
            {
                grade = "Senior";
            }

            ViewBag.major = major;

            ViewBag.grade = grade;

            return View();
        }

        public ActionResult ViewRequestedProfile()
        {

            int userID = Convert.ToInt32(TempData["reqUserID"]);

            ProfileModel pm = GetUsersProfileInfo(userID);

            ViewBag.pm = pm;

            string userName = GetUserName(userID);

            ViewBag.userName = userName;

            string grade = "";
            string major = "";

            if (pm.Major == 0)
            {
                major = "No Path";
            }
            else if (pm.Major == 1)
            {
                major = "Web Development";
            }
            else if (pm.Major == 2)
            {
                major = "Information Security";
            }
            else if (pm.Major == 3)
            {
                major = "Business Process Management";
            }


            if (pm.Grade == 1)
            {
                grade = "Freshman";
            }
            else if (pm.Grade == 2)
            {
                grade = "Sophomore";
            }
            else if (pm.Grade == 3)
            {
                grade = "Junior";
            }
            else if (pm.Grade == 4)
            {
                grade = "Senior";
            }


            ViewBag.major = major;

            ViewBag.grade = grade;

            return View();

        }

        public ActionResult DelAcct()
        {
            UserInfo.UserID = 0;
            UserInfo.IsAdmin = false;
            UserInfo.IsApproved = null;
            UserInfo.IsDenied = null;

            return RedirectToAction("Index", "Home");
        }

        #region HELPER METHODS

        public List<SelectListItem> GetAllCourses()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            SqlConnection cnn = new SqlConnection(cnnString);
            DataTable dt = new DataTable();

            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "GET_COURSES_LIST"
            };

            try
            {
                cnn.Open();
                dt.Load(cmd.ExecuteReader());

                foreach (DataRow row in dt.Rows)
                {
                    items.Add(new SelectListItem { Text = row["COURSE_TEXT"].ToString(), Value = row["COURSE_VALUE"].ToString() });
                }

                return items;
            }
            catch (Exception)
            {
                return new List<SelectListItem>();
            }
            finally
            {
                cnn?.Close();
                dt?.Dispose();
                cmd?.Dispose();
            }
        }

        public List<SelectListItem> GetAllHobbies()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            SqlConnection cnn = new SqlConnection(cnnString);
            DataTable dt = new DataTable();

            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "GET_HOBBIES_LIST"
            };

            try
            {
                cnn.Open();
                dt.Load(cmd.ExecuteReader());

                foreach (DataRow row in dt.Rows)
                {
                    items.Add(new SelectListItem { Text = row["Hobby_Name"].ToString(), Value = row["Hobby_ID"].ToString() });
                }

                return items;
            }
            catch (Exception)
            {
                return new List<SelectListItem>();
            }
            finally
            {
                cnn?.Close();
                dt?.Dispose();
                cmd?.Dispose();
            }
        }

        public List<SelectListItem> GetAllProfessions()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            SqlConnection cnn = new SqlConnection(cnnString);
            DataTable dt = new DataTable();

            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "GET_PROFESSIONAL_LIST"
            };

            try
            {
                cnn.Open();
                dt.Load(cmd.ExecuteReader());

                foreach (DataRow row in dt.Rows)
                {
                    items.Add(new SelectListItem { Text = row["Interest_Name"].ToString(), Value = row["Interest_ID"].ToString() });
                }

                return items;
            }
            catch (Exception)
            {
                return new List<SelectListItem>();
            }
            finally
            {
                cnn?.Close();
                dt?.Dispose();
                cmd?.Dispose();
            }
        }

        public ProfileModel GetProfileInfo()
        {
            SqlConnection cnn = new SqlConnection(cnnString);
            DataTable dt = new DataTable();
            ProfileModel pm = new ProfileModel();

            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "GET_PROFILE_INFO"
            };

            try
            {
                cmd.Parameters.Add("@USER_ID", SqlDbType.Int).Value = UserInfo.UserID;
                cnn.Open();
                cmd.ExecuteNonQuery();

                dt.Load(cmd.ExecuteReader());

                foreach (DataRow row in dt.Rows)
                {
                    //   items.Add(new SelectListItem { Text = row["Hobby_Name"].ToString(), Value = row["Hobby_ID"].ToString() });

                    pm.FirstName = row["FName"].ToString();
                    pm.LastName = row["LName"].ToString();
                    pm.DOB = row["DOB"].ToString();
                    pm.Ethnicity = row["Ethnicity"].ToString();
                    pm.SexualOrientation = row["Orientation"].ToString();
                    pm.Email = row["Email"].ToString();
                    pm.Gender = row["Gender"].ToString();
                    pm.StudentID = row["StudentID"].ToString();
                    pm.Grade = int.Parse(row["Grade"].ToString());
                    pm.Job = row["Job"].ToString();
                    pm.Major = int.Parse(row["Major"].ToString());
                    pm.Industry = int.Parse(row["Industry"].ToString());


                }

                return pm;
            }
            catch (Exception)
            {
                return new ProfileModel();
            }
            finally
            {
                cnn?.Close();
                dt?.Dispose();
                cmd?.Dispose();
            }
        }

        public bool DoesEmailExist(string email)
        {
            SqlConnection cnn = new SqlConnection(cnnString);

            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "DOES_EMAIL_EXIST"
            };

            try
            {
                cmd.Parameters.Add("@EMAIL", SqlDbType.VarChar, 50).Value = email;
                cmd.Parameters.Add("@IS_FOUND", SqlDbType.Char, 1).Direction = ParameterDirection.Output;

                cnn.Open();
                cmd.ExecuteNonQuery();

                if (cmd.Parameters["@IS_FOUND"].Value.ToString() == "1")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw new HttpException((int)HttpStatusCode.BadRequest, "Error in DoesEmailExist");
            }
            finally
            {
                cnn.Close();
                cmd.Dispose();
            }
        }

        public int LoginUser(LoginModel model)
        {
            SqlConnection cnn = new SqlConnection(cnnString);

            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "LOGIN_USER"
            };

            try
            {
                cmd.Parameters.Add("@EMAIL", SqlDbType.VarChar, 50).Value = model.Email;
                cmd.Parameters.Add("@PASSWORD", SqlDbType.VarChar, 50).Value = model.Password;
                cmd.Parameters.Add("@USER_ID", SqlDbType.Int).Direction = ParameterDirection.Output;

                cnn.Open();
                cmd.ExecuteNonQuery();

                return int.Parse(cmd.Parameters["@USER_ID"].Value.ToString());
            }
            catch (Exception ex)
            {
                throw new HttpException((int)HttpStatusCode.BadRequest, "Error at LoginUser method - " + ex.Message);
            }
            finally
            {
                cnn.Close();
                cmd.Dispose();
            }
        }

        public int GetRoleID(string registerType, string schoolYear, string alumniRole)
        {
            if (registerType == "Mentor")
            {
                if (schoolYear == "Senior")
                    return 1;
                if (schoolYear == "Junior")
                    return 2;
            }

            if (registerType == "Mentee")
            {
                if (schoolYear == "Sophomore")
                    return 4;
                if (schoolYear == "Freshman")
                    return 5;
            }

            if (registerType == "Alumni")
            {
                if (alumniRole == "Mentor")
                    return 3;
                else
                    return 13;
            }

            return 0;
        }

        public DataTable GetDataTable(ProfileModel model)
        {

            DataTable classesTable = new DataTable("classesTable");

            DataColumn column;

            DataRow row;

            // User ID Column
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Int32");
            column.ColumnName = "Course_ID";
            column.ReadOnly = false;
            column.Unique = false;
            // Add the Column to the DataColumnCollection.
            classesTable.Columns.Add(column);

            // User ID Column
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Int32");
            column.ColumnName = "User_ID";
            column.ReadOnly = false;
            column.Unique = false;
            // Add the Column to the DataColumnCollection.
            classesTable.Columns.Add(column);

            //Course_Text Column
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "Course_Text";
            column.AutoIncrement = false;
            column.ReadOnly = false;
            column.Unique = false;
            // Add the column to the table.
            classesTable.Columns.Add(column);

            //Course_Text Column
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Bool");
            column.ColumnName = "IsCurrent";
            column.AutoIncrement = false;
            column.ReadOnly = false;
            column.Unique = false;
            // Add the column to the table.
            classesTable.Columns.Add(column);

            foreach (var i in model.PreviousCourseList)
            {
                row = classesTable.NewRow();
                row["Course_ID"] = 1;
                row["User_ID"] = UserInfo.UserID;
                row["Course_Text"] = "CIS-150";
                row["IsCurrent"] = false;

            }

            foreach (var i in model.CurrentCourseList)
            {
                row = classesTable.NewRow();
                row["Course_ID"] = 2;
                row["User_ID"] = UserInfo.UserID;
                row["Course_Text"] = "CIS-199";
                row["IsCurrent"] = false;

            }

            return classesTable;

        }

        public List<SelectListItem> GetCurrentCourses()
        {
            SqlConnection cnn = new SqlConnection(cnnString);
            DataTable dt = new DataTable();
            List<SelectListItem> items = new List<SelectListItem>();

            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "GET_USER_CURRENT_COURSES"
            };

            try
            {
                cmd.Parameters.Add("@USERID", SqlDbType.Int).Value = UserInfo.UserID;
                cnn.Open();
                cmd.ExecuteNonQuery();

                dt.Load(cmd.ExecuteReader());

                foreach (DataRow row in dt.Rows)
                {
                    items.Add(new SelectListItem { Text = row["Course_Text"].ToString(), Value = row["Course_ID"].ToString() });

                }

                return items;
            }
            catch (Exception)
            {
                return new List<SelectListItem>();
            }
            finally
            {
                cnn?.Close();
                dt?.Dispose();
                cmd?.Dispose();
            }
        }

        public List<SelectListItem> GetPreviousCourses()
        {
            SqlConnection cnn = new SqlConnection(cnnString);
            DataTable dt = new DataTable();
            List<SelectListItem> items = new List<SelectListItem>();

            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "GET_USER_PREV_COURSES"
            };

            try
            {
                cmd.Parameters.Add("@USERID", SqlDbType.Int).Value = UserInfo.UserID;
                cnn.Open();
                cmd.ExecuteNonQuery();

                dt.Load(cmd.ExecuteReader());

                foreach (DataRow row in dt.Rows)
                {
                    items.Add(new SelectListItem { Text = row["Course_Text"].ToString(), Value = row["Course_ID"].ToString() });

                }

                return items;
            }
            catch (Exception)
            {
                return new List<SelectListItem>();
            }
            finally
            {
                cnn?.Close();
                dt?.Dispose();
                cmd?.Dispose();
            }
        }

        public List<SelectListItem> GetProfessionalInterests()
        {
            SqlConnection cnn = new SqlConnection(cnnString);
            DataTable dt = new DataTable();
            List<SelectListItem> items = new List<SelectListItem>();

            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "GET_ PROFESSIONAL_INTERESTS"
            };

            try
            {
                cmd.Parameters.Add("@USER_ID", SqlDbType.Int).Value = UserInfo.UserID;
                cnn.Open();
                cmd.ExecuteNonQuery();

                dt.Load(cmd.ExecuteReader());

                foreach (DataRow row in dt.Rows)
                {
                    items.Add(new SelectListItem { Text = row["Interest_Name"].ToString(), Value = row["Interest_ID"].ToString() });

                }

                return items;
            }
            catch (Exception)
            {
                return new List<SelectListItem>();
            }
            finally
            {
                cnn?.Close();
                dt?.Dispose();
                cmd?.Dispose();
            }
        }

        public List<SelectListItem> GetPersonalInterests()
        {
            SqlConnection cnn = new SqlConnection(cnnString);
            DataTable dt = new DataTable();
            List<SelectListItem> items = new List<SelectListItem>();

            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "GET_ PERSONAL_INTERESTS"
            };

            try
            {
                cmd.Parameters.Add("@USER_ID", SqlDbType.Int).Value = UserInfo.UserID;
                cnn.Open();
                cmd.ExecuteNonQuery();

                dt.Load(cmd.ExecuteReader());

                foreach (DataRow row in dt.Rows)
                {
                    items.Add(new SelectListItem { Text = row["Interest_Name"].ToString(), Value = row["Interest_ID"].ToString() });

                }

                return items;
            }
            catch (Exception)
            {
                return new List<SelectListItem>();
            }
            finally
            {
                cnn?.Close();
                dt?.Dispose();
                cmd?.Dispose();
            }
        }

        public int GetMajor(ProfileModel model)
        {
            if (model.No_Path == true)
            {
                return 0;
            }
            else if (model.Web_Dev == true)
            {
                return 1;

            }
            else if (model.Info_Sec == true)
            {
                return 2;
            }
            else if (model.BPM == true)
            {
                return 3;
            }
            else if (model.Info_Sec == true && model.Web_Dev == true)
            {
                return 12;
            }
            else if (model.BPM == true && model.Info_Sec == true)
            {
                return 23;
            }
            else if (model.BPM == true && model.Web_Dev == true)
            {
                return 13;
            }
            else
            {
                return 123;
            }

        }

        public ProfileModel CheckMajor(int value)
        {
            ProfileModel check = new ProfileModel();

            check.No_Path = false;
            check.Web_Dev = false;
            check.Info_Sec = false;
            check.BPM = false;

            if (value == 0)
            {
                check.No_Path = true;


            }
            else if (value == 1)
            {

                check.Web_Dev = true;
            }
            else if (value == 2)
            {

                check.Info_Sec = true;
            }
            else if (value == 3)
            {

                check.BPM = true;
            }
            else if (value == 12)
            {

                check.Info_Sec = true;
                check.Web_Dev = true;
            }
            else if (value == 23)
            {

                check.BPM = true;
                check.Info_Sec = true;

            }
            else if (value == 13)
            {
                check.BPM = true;
                check.Web_Dev = true;
            }
            else
            {
                check.BPM = true;
                check.Web_Dev = true;
                check.Info_Sec = true;
            }

            return check;

        }

        public int[] getMentorMatchRequests()
        {
            SqlConnection cnn = new SqlConnection(cnnString);
            DataTable dt = new DataTable();
            List<SelectListItem> items = new List<SelectListItem>();
           

            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "GET_MENTOR_MATCH_REQUESTS"
            };

            try
            {
                cmd.Parameters.Add("@USER_ID", SqlDbType.Int).Value = UserInfo.UserID;
                cnn.Open();
                cmd.ExecuteNonQuery();

                dt.Load(cmd.ExecuteReader());

                foreach (DataRow row in dt.Rows)
                {
                    items.Add(new SelectListItem { Text = row["Req_User_ID"].ToString(), Value = row["Req_User_ID"].ToString() });

                }

                int[] req_userIDs = new int[dt.Rows.Count];

                for(var i = 0; i < items.Count; i++)
                {
                    req_userIDs[i] = int.Parse(items[i].Value);
                }

                return req_userIDs;
            }
            catch (Exception)
            {
                return new int[0];
            }
            finally
            {
                cnn?.Close();
                dt?.Dispose();
                cmd?.Dispose();
            }
        }

        public int[] getMatchedUser()
        {
            SqlConnection cnn = new SqlConnection(cnnString);
            DataTable dt = new DataTable();
            List<SelectListItem> items = new List<SelectListItem>();


            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "GET_MATCHED_MENTEE"
            };

            try
            {
                cmd.Parameters.Add("@USER_ID", SqlDbType.Int).Value = UserInfo.UserID;
                cnn.Open();
                cmd.ExecuteNonQuery();

                dt.Load(cmd.ExecuteReader());

                foreach (DataRow row in dt.Rows)
                {
                    items.Add(new SelectListItem { Text = row["Mentee_User_ID"].ToString(), Value = row["Mentee_User_ID"].ToString() });

                }

                int[] req_userIDs = new int[dt.Rows.Count];

                req_userIDs[0] = 0;

                for (var i = 0; i < items.Count; i++)
                {
                    req_userIDs[i] = int.Parse(items[i].Value);
                }

                return req_userIDs;
            }
            catch (Exception)
            {
                int[] newInt = { 0 };
                return newInt;
            }
            finally
            {
                cnn?.Close();
                dt?.Dispose();
                cmd?.Dispose();
            }
        }

        public int[] getMatchedMentor()
        {
            SqlConnection cnn = new SqlConnection(cnnString);
            DataTable dt = new DataTable();
            List<SelectListItem> items = new List<SelectListItem>();


            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "GET_MATCHED_MENTOR"
            };

            try
            {
                cmd.Parameters.Add("@USER_ID", SqlDbType.Int).Value = UserInfo.UserID;
                cnn.Open();
                cmd.ExecuteNonQuery();

                dt.Load(cmd.ExecuteReader());

                foreach (DataRow row in dt.Rows)
                {
                    items.Add(new SelectListItem { Text = row["Mentor_User_ID"].ToString(), Value = row["Mentor_User_ID"].ToString() });

                }

                int[] req_userIDs = new int[dt.Rows.Count];

                req_userIDs[0] = 0;

                for (var i = 0; i < items.Count; i++)
                {
                    req_userIDs[i] = int.Parse(items[i].Value);
                }

                return req_userIDs;
            }
            catch (Exception)
            {
                int[] newInt = { 0 };
                return newInt;
            }
            finally
            {
                cnn?.Close();
                dt?.Dispose();
                cmd?.Dispose();
            }
        }

        public ProfileModel GetUsersProfileInfo(int userID)
        {
            SqlConnection cnn = new SqlConnection(cnnString);
            DataTable dt = new DataTable();
            ProfileModel pm = new ProfileModel();

            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "GET_PROFILE_INFO"
            };

            try
            {
                cmd.Parameters.Add("@USER_ID", SqlDbType.Int).Value = userID;
                cnn.Open();
                cmd.ExecuteNonQuery();

                dt.Load(cmd.ExecuteReader());

                foreach (DataRow row in dt.Rows)
                {
                    //   items.Add(new SelectListItem { Text = row["Hobby_Name"].ToString(), Value = row["Hobby_ID"].ToString() });

                    pm.FirstName = row["FName"].ToString();
                    pm.LastName = row["LName"].ToString();
                    pm.DOB = row["DOB"].ToString();
                    pm.Ethnicity = row["Ethnicity"].ToString();
                    pm.SexualOrientation = row["Orientation"].ToString();
                    pm.Email = row["Email"].ToString();
                    pm.Gender = row["Gender"].ToString();
                    pm.StudentID = row["StudentID"].ToString();
                    pm.Grade = int.Parse(row["Grade"].ToString());
                    pm.Job = row["Job"].ToString();
                    pm.Major = int.Parse(row["Major"].ToString());
                    pm.Industry = int.Parse(row["Industry"].ToString());


                }

                return pm;
            }
            catch (Exception)
            {
                return new ProfileModel();
            }
            finally
            {
                cnn?.Close();
                dt?.Dispose();
                cmd?.Dispose();
            }
        }

        public string GetUserName(int userID)
        {
            SqlConnection cnn = new SqlConnection(cnnString);
            DataTable dt = new DataTable();
            List<SelectListItem> items = new List<SelectListItem>();

            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "GET_USER_NAME"
            };

            try
            {
                cmd.Parameters.Add("@USER_ID", SqlDbType.Int).Value = userID;
                cnn.Open();
                cmd.ExecuteNonQuery();

                dt.Load(cmd.ExecuteReader());

                foreach (DataRow row in dt.Rows)
                {
                    items.Add(new SelectListItem { Text = row["UserName"].ToString(), Value = row["UserName"].ToString() });

                }

                string[] req_userIDs = new string[dt.Rows.Count];

                req_userIDs[0] = "0";

                for (var i = 0; i < items.Count; i++)
                {
                    req_userIDs[i] = (items[i].Value);
                }

                string userName = req_userIDs[0];

                return userName;
            }
            catch (Exception)
            {
               
                return "0";
            }
            finally
            {
                cnn?.Close();
                dt?.Dispose();
                cmd?.Dispose();
            }
        }

        public JsonResult RegisterUserMessaging()
        {
            SqlConnection cnn = new SqlConnection(cnnString);

            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "REGISTER_USER_MODELS"
            };

            try
            {
             
                    // user profile paramaters
                    cmd.Parameters.Add("@NAME", SqlDbType.NVarChar, 50).Value = UserInfo.UserName;
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = UserInfo.UserID;
                   

                    cnn.Open();
                    cmd.ExecuteNonQuery();



                    return Json("User Registered Successfully");
                

            }
            catch (Exception)
            {

                return Json("Error Occured");

            }
            finally
            {
                cnn?.Close();
                cmd?.Dispose();
            }
        }







        #endregion
    }
}