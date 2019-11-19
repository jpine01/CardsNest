using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using UofLConnect.Models.Admin;
using UofLConnect.Utilities;

namespace UofLConnect.Controllers
{
    public class AdminController : Controller
    {
        public static string cnnString = System.Configuration.ConfigurationManager.ConnectionStrings["UofLConnectDb"].ConnectionString;


        public ActionResult AccountApproval()
        {
            UtilClass.IsDashboard = false;

            if(!UserInfo.IsAdmin || UserInfo.UserID == 0)
                return RedirectToAction("Login", "Home");

            return View();
        }

        public ActionResult _AcctApprovalModal(string id)
        {
            SqlConnection cnn = new SqlConnection(cnnString);
            DataTable dt = new DataTable();
            List<AccountApprovalModel> acct = new List<AccountApprovalModel>();

            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "GET_USER_TO_APPROVE_INFO"
            };

            try
            {
                cmd.Parameters.Add("@USERID", SqlDbType.Int).Value = id;

                cnn.Open();
                dt.Load(cmd.ExecuteReader());

                foreach (DataRow row in dt.Rows)
                {
                    acct.Add(new AccountApprovalModel
                    {
                        UserID = row["USERID"].ToString(),
                        FName = row["FNAME"].ToString(),
                        LName = row["LNAME"].ToString(),
                        Email = row["EMAIL"].ToString(),
                        StudentID = row["STUDENTID"].ToString(),
                        UserName = row["USERNAME"].ToString(),
                        Role = row["ROLEID"].ToString(),
                    });
                }

                return PartialView(acct[0]);
            }
            catch (Exception ex)
            {
                return Json(new { data = new AccountApprovalModel(), Message = "Error in _AcctApprovalModal. Message: " + ex.Message }, JsonRequestBehavior.DenyGet);
            }
            finally
            {
                cnn?.Close();
                cmd?.Dispose();
                dt?.Clear();
            }
        }

        public ActionResult AcctApprovalSave(AccountApprovalModel model)
        {
            SqlConnection cnn = new SqlConnection(cnnString);

            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "ACCT_APPROVAL_SAVE"
            };

            try
            {
                if (ModelState.IsValid && model != null)
                {
                    if (String.IsNullOrEmpty(model.FName) && String.IsNullOrEmpty(model.LName) && String.IsNullOrEmpty(model.Email) && String.IsNullOrEmpty(model.LName) && String.IsNullOrEmpty(model.UserName) && String.IsNullOrEmpty(model.Role))
                        return Json(new { isSuccess = false, message = "" });
                
                    cmd.Parameters.Add("@USERID", SqlDbType.Int).Value = model.UserID;
                    cmd.Parameters.Add("@FNAME", SqlDbType.VarChar, 50).Value = model.FName;
                    cmd.Parameters.Add("@LNAME", SqlDbType.VarChar, 50).Value = model.LName;
                    cmd.Parameters.Add("@EMAIL", SqlDbType.VarChar, 50).Value = model.Email;
                    cmd.Parameters.Add("@STUDENTID", SqlDbType.VarChar).Value = model.StudentID ?? "0";
                    cmd.Parameters.Add("@USERNAME", SqlDbType.VarChar, 50).Value = model.UserName;
                    cmd.Parameters.Add("@ROLEID", SqlDbType.VarChar, 50).Value = model.Role;
                    cmd.Parameters.Add("@OLDROLE", SqlDbType.VarChar, 50).Value = model.OldRole;

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

        public ActionResult GetAccountsApprovalQueue()
        {
            SqlConnection cnn = new SqlConnection(cnnString);
            DataTable dt = new DataTable();
            List<AccountApprovalModel> accts = new List<AccountApprovalModel>();

            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "GET_ACCOUNTS_TO_APPROVE"
            };

            try
            {
                cnn.Open();
                dt.Load(cmd.ExecuteReader());

                foreach (DataRow row in dt.Rows)
                {
                    accts.Add(new AccountApprovalModel
                    {
                        UserID = row["USERID"].ToString(),
                        FName = row["FNAME"].ToString(),
                        LName = row["LNAME"].ToString(),
                        Email = row["EMAIL"].ToString(),
                        StudentID = row["STUDENTID"].ToString() == "0" ? "" : row["STUDENTID"].ToString(),
                        UserName = row["USERNAME"].ToString(),
                        RoleName = row["ROLENAME"].ToString(),
                        Type = row["TYPE"].ToString(),
                    });
                }

                return Json(new { data = accts }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                return Json(new { data = new AccountApprovalModel(), Message = "Error in GetAccountsApprovalQueue. Message: " + ex.Message }, JsonRequestBehavior.DenyGet);
            }
            finally
            {
                cnn?.Close();
                cmd?.Dispose();
                dt?.Clear();
            }

        }

        public ActionResult ApproveOrDenyAcct(string id, string mode)
        {
            SqlConnection cnn = new SqlConnection(cnnString);

            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = mode == "Approve" ? "APPROVE_ACCT" : "DENY_ACCT"
            };

            try
            {
                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(mode))
                    return Json(new { isSuccess = false, message = "" });

                cmd.Parameters.Add("@USERID", SqlDbType.Int).Value = id;

                cnn.Open();
                cmd.ExecuteNonQuery();

                return Json(new { isSuccess = true, message = "" });
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

        public ActionResult AccountDenied()
        {
            UtilClass.IsDashboard = false;

            if (!UserInfo.IsAdmin || UserInfo.UserID == 0)
                return RedirectToAction("Login", "Home");

            return View();
        }

        public ActionResult GetAccountsDeniedQueue()
        {
            SqlConnection cnn = new SqlConnection(cnnString);
            DataTable dt = new DataTable();
            List<AccountDeniedModel> accts = new List<AccountDeniedModel>();

            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "GET_ACCOUNTS_DENIED"
            };

            try
            {
                cnn.Open();
                dt.Load(cmd.ExecuteReader());

                foreach (DataRow row in dt.Rows)
                {
                    accts.Add(new AccountDeniedModel
                    {
                        UserID = row["USERID"].ToString(),
                        FName = row["FNAME"].ToString(),
                        LName = row["LNAME"].ToString(),
                        Email = row["EMAIL"].ToString(),
                        StudentID = row["STUDENTID"].ToString() == "0" ? "" : row["STUDENTID"].ToString(),
                        UserName = row["USERNAME"].ToString(),
                        RoleName = row["ROLENAME"].ToString(),
                        Type = row["TYPE"].ToString(),
                    });
                }

                return Json(new { data = accts }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                return Json(new { data = new AccountDeniedModel(), Message = "Error in GetAccountsDeniedQueue. Message: " + ex.Message }, JsonRequestBehavior.DenyGet);
            }
            finally
            {
                cnn?.Close();
                cmd?.Dispose();
                dt?.Clear();
            }
        }

        public ActionResult DeniedUserDelete(string id)
        {
            SqlConnection cnn = new SqlConnection(cnnString);

            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "DENIED_USER_DELETE"
            };

            try
            {
                if (string.IsNullOrEmpty(id))
                    return Json(new { isSuccess = false, message = "" });

                cmd.Parameters.Add("@USERID", SqlDbType.Int).Value = id;

                cnn.Open();
                cmd.ExecuteNonQuery();

                return Json(new { isSuccess = true, message = "" });
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

        public ActionResult Reports()
        {
            UtilClass.IsDashboard = false;

            if (!UserInfo.IsAdmin || UserInfo.UserID == 0)
                return RedirectToAction("Login", "Home");

            return View();
        }

        public ActionResult GetReportsQueue()
        {
            SqlConnection cnn = new SqlConnection(cnnString);
            DataTable dt = new DataTable();
            List<ReportsQueueModel> accts = new List<ReportsQueueModel>();

            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "GET_ALL_REPORTS"
            };

            try
            {
                cnn.Open();
                dt.Load(cmd.ExecuteReader());

                foreach (DataRow row in dt.Rows)
                {
                    accts.Add(new ReportsQueueModel
                    {
                        Report_Number = row["Report_Number"].ToString(),
                        Report_Time = Convert.ToDateTime(row["Report_Time"].ToString()).ToString("hh:mm:ss tt"),
                        Report_Date = Convert.ToDateTime(row["Report_Date"].ToString()).ToString("MM-dd-yyyy"),
                        Report_Name = row["Report_Name"].ToString(),
                        User_Name = row["User_Name"].ToString(),
                        Report_Message = row["Report_Message"].ToString(),
                    });
                }

                return Json(new { data = accts }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                return Json(new { data = new ReportsQueueModel(), Message = "Error in GetReportsQueue. Message: " + ex.Message }, JsonRequestBehavior.DenyGet);
            }
            finally
            {
                cnn?.Close();
                cmd?.Dispose();
                dt?.Clear();
            }
        }

        public ActionResult ResolveReport(string id)
        {
            SqlConnection cnn = new SqlConnection(cnnString);

            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "RESOLVE_REPORT"
            };

            try
            {
                if (string.IsNullOrEmpty(id))
                    return Json(new { isSuccess = false, message = "" });

                cmd.Parameters.Add("@REPORTID", SqlDbType.Int).Value = id;

                cnn.Open();
                cmd.ExecuteNonQuery();

                return Json(new { isSuccess = true, message = "" });
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

        public ActionResult EmailBlast(string msg = "")
        {
            UtilClass.IsDashboard = false;

            if (!UserInfo.IsAdmin || UserInfo.UserID == 0)
                return RedirectToAction("Login", "Home");

            ViewBag.Msg = msg;

            return View();
        }

        public ActionResult EmailBlastSubmit(EmailBlastModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var senderEmail = new MailAddress("user@gmail.com");
                    var receiverEmail = new MailAddress("user@hotmail.com", "Receiver");
                    var password = "";
                    var sub = model.EmailSubject;
                    var body = model.EmailContent;
                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 465,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(senderEmail.Address, password)
                    };
                    using (var mess = new MailMessage(senderEmail, receiverEmail)
                    {
                        Subject = sub,
                        Body = body
                    })
                    {
                        smtp.Send(mess);
                    }
                    return View();
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("EmailBlast", "Admin", new { msg = "Error Occured! " + ex.Message });
            }

            return RedirectToAction("EmailBlast", "Admin", new { msg = "Sent!" });
        }
    }
}