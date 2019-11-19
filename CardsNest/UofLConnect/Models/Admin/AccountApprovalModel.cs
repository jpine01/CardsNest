using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UofLConnect.Models.Admin
{
    public class AccountApprovalModel
    {
        public static string cnnString = System.Configuration.ConfigurationManager.ConnectionStrings["UofLConnectDb"].ConnectionString;

        public string UserID { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string Email { get; set; }
        public string StudentID { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }
        public string Type { get; set; }
        public string OldRole { get; set; }
        public string Role { get; set; }
        public IEnumerable<SelectListItem> Role_List
        {
            get
            {
                List<SelectListItem> items = new List<SelectListItem>();
                SqlConnection cnn = new SqlConnection(cnnString);
                DataTable dt = new DataTable();

                SqlCommand cmd = new SqlCommand
                {
                    Connection = cnn,
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "GET_ROLE_LIST"
                };

                try
                {
                    cnn.Open();
                    dt.Load(cmd.ExecuteReader());

                    foreach (DataRow row in dt.Rows)
                    {
                        items.Add(new SelectListItem { Text = string.IsNullOrEmpty(row["Type"].ToString()) ? row["Role_Name"].ToString() : row["Type"].ToString() + $" - {row["Role_Name"].ToString()}", Value = row["RoleID"].ToString() });
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
        }
    }
}