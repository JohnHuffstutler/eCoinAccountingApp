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
                        user.firstName = Convert.ToString(dt.Rows[i]["First Name"]);
                        user.lastName = Convert.ToString(dt.Rows[i]["Last Name"]);
                        user.userName = Convert.ToString(dt.Rows[i]["Username"]);
                        user.password = Convert.ToString(dt.Rows[i]["Password"]);                   
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

        public int AddUser(Users user, IConfiguration config)
        {
            int i = 0;
            using (SqlConnection connection = new SqlConnection(config.GetConnectionString("DefaultConnection").ToString()))
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO [Table] ([First Name],[Last Name],Username,Password,Role,Email,Address,[Date Of Birth]) VALUES('" + user.firstName + "' , '" + user.lastName + "' , '" + user.userName + "' , '" + user.password + "' , '" + user.role + "' , '" + user.email + "' , '" + user.address + "' , '" + user.dateOfBirth + "')", connection);
                connection.Open();
                i = cmd.ExecuteNonQuery();
                connection.Close();
            }
            return i;
        }

        public Users GetUser(string id, IConfiguration config)
        {
            Users user = new Users();
            using (SqlConnection connection = new SqlConnection(config.GetConnectionString("DefaultConnection").ToString()))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM [Table] WHERE Id = '" + id + "' ", connection);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                        user.id = Convert.ToString(dt.Rows[0]["Id"]);
                        user.firstName = Convert.ToString(dt.Rows[0]["First Name"]);
                        user.lastName = Convert.ToString(dt.Rows[0]["Last Name"]);
                        user.userName = Convert.ToString(dt.Rows[0]["Username"]);
                        user.password = Convert.ToString(dt.Rows[0]["Password"]);
                        user.role = Convert.ToString(dt.Rows[0]["Role"]);
                        user.email = Convert.ToString(dt.Rows[0]["Email"]);
                        user.address = Convert.ToString(dt.Rows[0]["Address"]);
                        user.dateOfBirth = Convert.ToString(dt.Rows[0]["Date Of Birth"]);
                }
            }

            return user;
        }

        public int UpdateUser(Users user, IConfiguration config)
        {
            int i = 0;
            using (SqlConnection connection = new SqlConnection(config.GetConnectionString("DefaultConnection").ToString()))
            {
                SqlCommand cmd = new SqlCommand("UPDATE [Table] SET [First Name] = '" + user.firstName + "' ,[Last Name] = '" + user.lastName + "' ,Username = '" + user.userName + "' ,Password = '" + user.password + "' ,Role = '" + user.role + "' ,Email = '" + user.email + "' ,Address = '" + user.address + "' ,[Date Of Birth] = '" + user.dateOfBirth + "' WHERE Id = '" + user.id + "' ", connection);
                connection.Open();
                i = cmd.ExecuteNonQuery();
                connection.Close();
            }
            return i;
        }

        public int DeleteUser(string id, IConfiguration config)
        {
            int i = 0;
            using (SqlConnection connection = new SqlConnection(config.GetConnectionString("DefaultConnection").ToString()))
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM [Table] WHERE Id = '" + id + "' ", connection);
                connection.Open();
                i = cmd.ExecuteNonQuery();
                connection.Close();
            }
            return i;
        }
    }
}
