using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BTL_LapTrinhWeb.Models;

namespace BTL_LapTrinhWeb.DAL
{
    public class UserRepository
    {
        public User Login(string username, string password)
        {
            using (var conn = DBHelper.GetConnection())
            using (var cmd = new SqlCommand("SELECT * FROM users WHERE LOWER(username) = LOWER(@Username)", conn))
            {
                DBHelper.AddText(cmd, "Username", username);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read() && PasswordHasher.Verify(password, reader["password"].ToString()))
                        return MapUser(reader);
                }
            }
            return null;
        }

        public void Register(User user)
        {
            using (var conn = DBHelper.GetConnection())
            using (var cmd = new SqlCommand(@"
                INSERT INTO users (username, password, surname, name, email, phone, quequan,
                    address1, address2, postal, city, country)
                OUTPUT INSERTED.userid
                VALUES (@Username, @Password, @Surname, @Name, @Email, @Phone, @QueQuan,
                    @Address1, @Address2, @Postal, @City, @Country)", conn))
            {
                DBHelper.AddText(cmd, "Username", user.Username);
                DBHelper.AddText(cmd, "Password", PasswordHasher.Hash(user.Password));
                AddProfileParameters(cmd, user);
                conn.Open();
                user.Id = Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public List<User> GetAllUsers()
        {
            var users = new List<User>();
            using (var conn = DBHelper.GetConnection())
            using (var cmd = new SqlCommand("SELECT * FROM users ORDER BY userid", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read()) users.Add(MapUser(reader));
            }
            return users;
        }

        public User GetUserByID(int userId)
        {
            using (var conn = DBHelper.GetConnection())
            using (var cmd = new SqlCommand("SELECT * FROM users WHERE userid = @UserID", conn))
            {
                cmd.Parameters.AddWithValue("UserID", userId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    return reader.Read() ? MapUser(reader) : null;
            }
        }

        public void UpdateUser(User user)
        {
            using (var conn = DBHelper.GetConnection())
            using (var cmd = new SqlCommand(@"
                UPDATE users SET surname = @Surname, name = @Name, email = @Email,
                    phone = @Phone, quequan = @QueQuan, address1 = @Address1,
                    address2 = @Address2, postal = @Postal, city = @City, country = @Country
                WHERE userid = @UserID", conn))
            {
                cmd.Parameters.AddWithValue("UserID", user.Id);
                AddProfileParameters(cmd, user);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void ChangePassword(int userId, string newPassword)
        {
            using (var conn = DBHelper.GetConnection())
            using (var cmd = new SqlCommand("UPDATE users SET password = @Password WHERE userid = @UserID", conn))
            {
                cmd.Parameters.AddWithValue("UserID", userId);
                DBHelper.AddText(cmd, "Password", PasswordHasher.Hash(newPassword));
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteUser(int userId)
        {
            using (var conn = DBHelper.GetConnection())
            using (var cmd = new SqlCommand("DELETE FROM users WHERE userid = @UserID", conn))
            {
                cmd.Parameters.AddWithValue("UserID", userId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public bool IsUsernameExists(string username)
        {
            using (var conn = DBHelper.GetConnection())
            using (var cmd = new SqlCommand("SELECT CASE WHEN EXISTS (SELECT 1 FROM users WHERE LOWER(username) = LOWER(@Username)) THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END", conn))
            {
                DBHelper.AddText(cmd, "Username", username);
                conn.Open();
                return (bool)cmd.ExecuteScalar();
            }
        }

        private static void AddProfileParameters(SqlCommand cmd, User user)
        {
            DBHelper.AddText(cmd, "Surname", user.Surname);
            DBHelper.AddText(cmd, "Name", user.Name);
            DBHelper.AddText(cmd, "Email", user.Email);
            DBHelper.AddText(cmd, "Phone", user.Phone);
            DBHelper.AddText(cmd, "QueQuan", user.QueQuan);
            DBHelper.AddText(cmd, "Address1", user.Address1);
            DBHelper.AddText(cmd, "Address2", user.Address2);
            DBHelper.AddText(cmd, "Postal", user.Postal);
            DBHelper.AddText(cmd, "City", user.City);
            DBHelper.AddText(cmd, "Country", user.Country);
        }

        private static User MapUser(SqlDataReader reader)
        {
            return new User
            {
                Id = (int)reader["userid"], Username = reader["username"].ToString(),
                Name = reader["name"].ToString(), Surname = reader["surname"].ToString(),
                Email = reader["email"].ToString(), Phone = reader["phone"].ToString(),
                QueQuan = reader["quequan"].ToString(), Address1 = reader["address1"].ToString(),
                Address2 = reader["address2"].ToString(), Postal = reader["postal"].ToString(),
                City = reader["city"].ToString(), Country = reader["country"].ToString(),
                CreatedAt = (DateTime)reader["createdat"]
            };
        }
    }
}
