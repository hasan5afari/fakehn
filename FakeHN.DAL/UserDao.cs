﻿using FakeHN.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeHN.DAL
{
    public partial class UserDao
    {
        private const string connectionString = "Data Source=DESKTOP-5A7KHGH\\SQLSERVER2022;Initial Catalog=FakeHN;Integrated Security=true;";
        public User getUser(int userid)
        {
            User user = new User();

            // connect to SQL
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            // SQL command
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.CommandText = "Users_getUser";
            SqlParameter useridParameter = sqlCommand.Parameters.Add("@userid", SqlDbType.Int);
            useridParameter.Value = userid.ToString();

            // SQL read data
            SqlDataReader reader = sqlCommand.ExecuteReader();
            reader.Read();
            user.userid = Convert.ToInt32(reader["userid"]);
            user.username = Convert.ToString(reader["username"]);
            user.password = Convert.ToString(reader["password"]);
            user.name = Convert.ToString(reader["name"]);
            user.family = Convert.ToString(reader["family"]);

            sqlConnection.Close();

            return user;
        }

        public User isValidUser(string username, string password)
        {
            User user = null;

            // connect to SQL
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            // SQL command
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.CommandText = "Users_isValidUser";
            SqlParameter usernameParameter = sqlCommand.Parameters.Add("@username", SqlDbType.Char);
            SqlParameter passwordParameter = sqlCommand.Parameters.Add("@password", SqlDbType.Char);
            usernameParameter.Value = username;
            passwordParameter.Value = password;

            // SQL read data
            SqlDataReader reader = sqlCommand.ExecuteReader();
            reader.Read();

            if (reader.HasRows)
            {
                user = new User();
                user.userid = Convert.ToInt32(reader["userid"]);
                user.username = Convert.ToString(reader["username"]);
                user.password = Convert.ToString(reader["password"]);
                user.name = Convert.ToString(reader["name"]);
                user.family = Convert.ToString(reader["family"]);
            }

            return user;
        }
    }
}