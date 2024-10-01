using System.Data;
using System.Data.SqlClient;

namespace eCoinAccountingApp.Models
{
    public class DAL
    {
        public List<Users> GetUsers(IConfiguration config)
        {
            List<Users> listUsers = new List<Users>();
            using (SqlConnection connection = new SqlConnection(config.GetConnectionString("DefaultConnection").ToString()))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM [Table]", connection);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Users user = new Users();
                        user.id = Convert.ToString(dt.Rows[i]["Id"]);
                        user.userName = Convert.ToString(dt.Rows[i]["Username"]);
                        user.firstName = Convert.ToString(dt.Rows[i]["First Name"]);
                        user.lastName = Convert.ToString(dt.Rows[i]["Last Name"]);
                        user.role = Convert.ToString(dt.Rows[i]["Role"]);
                        user.email = Convert.ToString(dt.Rows[i]["Email"]);
                        user.address = Convert.ToString(dt.Rows[i]["Address"]);
                        user.dateOfBirth = Convert.ToString(dt.Rows[i]["Date Of Birth"]);
                        listUsers.Add(user);
                    }
                }
            }

            return listUsers;
        }
    }
}
