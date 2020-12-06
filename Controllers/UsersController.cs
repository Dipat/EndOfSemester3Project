﻿using Dapper;
using EndOfSemester3.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EndOfSemester3.Controllers
{
    public class UsersController : ApiController
    {
        // GET: api/Users
        public IEnumerable<Users> Get()
        {
            string sql = "SELECT * FROM Users";

            string connStr = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;

            using (var connection = new SqlConnection(connStr))
            {
                var user = connection.Query<Users>(sql).ToList();
                return user;
            }
        }

        // GET: api/Users/username
        public Users Get(string username)
        {
            string sql = "SELECT * FROM Users WHERE username = @username;";
            string connStr = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;

            using (var connection = new SqlConnection(connStr))
            {
                return connection.QuerySingleOrDefault<Users>(sql, new 
                { 
                    username = username 
                });
            }
        }

        // CREATE: api/Users
        public int Create(string userName, string password, string name, string email, string address)
        {
            EncryptionController encryptionController = new EncryptionController();
            string sql = "INSERT INTO Users (username, password, name, email, address, rating, numberOfSales, isAdmin, SALT)" +
                " VALUES (@username, @password, @name, @email, @address, @rating, @numberOfSales, @isAdmin, @SALT)";
            if (Get(userName) != null)
            {
                return 1;//Username taken error code
            }
            if ((userName != null && userName != "") && (password != null && password != "") &&
                (name != null && name != "") && (email != null && email != "") &&
                (address != null && address != ""))
            {
                string connStr = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;
                string salt = Guid.NewGuid().ToString("N").Substring(0, 20);
                string hashedPassword = encryptionController.EncryptPassword(password + salt);
                using (var connection = new SqlConnection(connStr))
                {
                    connection.Query(sql, new
                    {
                        username = userName,
                        password = hashedPassword,
                        name = name,
                        email = email,
                        address = address,
                        rating = 0,
                        numberOfSales = 0,
                        isAdmin = 0,
                        SALT = salt
                    });
                }
                return 0;//No errors found
            }
            return 2;//User couldn't be created
        }

        

        // UPDATE: api/Users
        public void Update(string userName, string password, string name, string email, string address)
        {
            EncryptionController encryptionController = new EncryptionController();
            string sql = "UPDATE Users(username, password, name, email, address, rating, numberOfSales, isAdmin, SALT)" +
                "VALUES(@username, @password, @name, @email, @address, @rating, @numberOfSales, @isAdmin, @SALT)" +
                "WHERE username ='" + userName + "'";
            //Current User Data
            var user = Get(userName);
            string salt = user.SALT;
            string hashedPassword = encryptionController.EncryptPassword(password + salt);
            if (password == null || password == "")
            {
                hashedPassword = user.password;
            }
            if (name == null || name == "")
            {
                name = user.name;
            }
            if (email == null || email == "")
            {
                email = user.email;
            }
            if (address == null || address == "")
            {
                address = user.address;
            }
            string connStr = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;
            
            using (var connection = new SqlConnection(connStr))
            {
                connection.Query(sql, new
                {
                    username = userName,
                    password = hashedPassword,
                    name = name,
                    email = email,
                    address = address,
                    rating = user.rating,
                    numberOfSales = user.numberOfSales,
                    isAdmin = user.isAdmin,
                    SALT = salt
                });

            }
        }

        // DELETE: api/Users/5
        public void Delete(string username)
        {
            string sql = "DELETE * FROM Users WHERE username = @username;";
            string connStr = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;

            using (var connection = new SqlConnection(connStr))
            {
                connection.Query(sql, new 
                { 
                    username = username
                });
            }
        }
    }
}
