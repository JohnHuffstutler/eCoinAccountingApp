using eCoinAccountingApp.Data;
using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Data.SqlClient;

namespace eCoinAccountingApp.Models
{
    public class DAL
    {
        // Get All Users
        public List<Users> GetUsers(IConfiguration config)
        {
            List<Users> listUsers = new List<Users>();

            using (SqlConnection connection = new SqlConnection(config.GetConnectionString("DefaultConnection")))
            {
                SqlCommand cmd = new SqlCommand("SELECT Id, FirstName, LastName, UserName, Email, Role, Address, DateOfBirth FROM [AspNetUsers]", connection);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    Users user = new Users
                    {
                        id = Convert.ToString(row["Id"]),
                        firstName = Convert.ToString(row["FirstName"]),
                        lastName = Convert.ToString(row["LastName"]),
                        userName = Convert.ToString(row["UserName"]),
                        email = Convert.ToString(row["Email"]),
                        role = Convert.ToString(row["Role"]),
                        address = Convert.ToString(row["Address"]),
                        dateOfBirth = Convert.ToString(row["DateOfBirth"])
                    };

                    listUsers.Add(user);
                }
            }

            return listUsers;
        }

        // Add User
        public int AddUser(Users user, IConfiguration config)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(config.GetConnectionString("DefaultConnection")))
            {
                string query = "INSERT INTO [AspNetUsers] (FirstName, LastName, UserName, Email, Address, DateOfBirth) " +
                               "VALUES (@FirstName, @LastName, @UserName, @Email, @Address, @DateOfBirth)";
                SqlCommand cmd = new SqlCommand(query, connection);

                // Use parameterized queries to prevent SQL injection
                cmd.Parameters.AddWithValue("@FirstName", user.firstName);
                cmd.Parameters.AddWithValue("@LastName", user.lastName);
                cmd.Parameters.AddWithValue("@UserName", user.userName);
                cmd.Parameters.AddWithValue("@Email", user.email);
                cmd.Parameters.AddWithValue("@Address", user.address);
                cmd.Parameters.AddWithValue("@DateOfBirth", user.dateOfBirth);

                connection.Open();
                result = cmd.ExecuteNonQuery();
                connection.Close();
            }

            return result;
        }

        // Get Single User by Id
        public Users GetUser(string id, IConfiguration config)
        {
            Users user = null;

            using (SqlConnection connection = new SqlConnection(config.GetConnectionString("DefaultConnection")))
            {
                string query = "SELECT Id, FirstName, LastName, UserName, Email, Role, Address, DateOfBirth FROM [AspNetUsers] WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Id", id);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    user = new Users
                    {
                        id = Convert.ToString(row["Id"]),
                        firstName = Convert.ToString(row["FirstName"]),
                        lastName = Convert.ToString(row["LastName"]),
                        userName = Convert.ToString(row["UserName"]),
                        email = Convert.ToString(row["Email"]),
                        role = Convert.ToString(row["Role"]),
                        address = Convert.ToString(row["Address"]),
                        dateOfBirth = Convert.ToString(row["DateOfBirth"])
                    };
                }
            }

            return user;
        }

        // Update User
        public int UpdateUser(Users user, IConfiguration config)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(config.GetConnectionString("DefaultConnection")))
            {
                string query = "UPDATE [AspNetUsers] SET FirstName = @FirstName, LastName = @LastName, " +
                               "UserName = @UserName, Email = @Email, Address = @Address, DateOfBirth = @DateOfBirth " +
                               "WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, connection);

                cmd.Parameters.AddWithValue("@FirstName", user.firstName);
                cmd.Parameters.AddWithValue("@LastName", user.lastName);
                cmd.Parameters.AddWithValue("@UserName", user.userName);
                cmd.Parameters.AddWithValue("@Email", user.email);
                cmd.Parameters.AddWithValue("@Address", user.address);
                cmd.Parameters.AddWithValue("@DateOfBirth", user.dateOfBirth);
                cmd.Parameters.AddWithValue("@Id", user.id);

                connection.Open();
                result = cmd.ExecuteNonQuery();
                connection.Close();
            }

            return result;
        }

        // Delete User
        public int DeleteUser(string id, IConfiguration config)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(config.GetConnectionString("DefaultConnection")))
            {
                string query = "DELETE FROM [AspNetUsers] WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Id", id);

                connection.Open();
                result = cmd.ExecuteNonQuery();
                connection.Close();
            }

            return result;
        }
    }
}
