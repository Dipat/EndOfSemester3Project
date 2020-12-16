using Dapper;
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
    public class ProductsController : ApiController
    {
        // GET: api/Products
        public IEnumerable<Products> Get()
        {
            string sql = "SELECT * FROM Products";

            string connStr = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;

            using (var connection = new SqlConnection(connStr))
            {
                var product = connection.Query<Products>(sql).ToList();
                return product;
            }
        }

        // GET: api/Products/5
        public Products Get(int id)
        {
            string sql = "SELECT * FROM Products WHERE Id = @productsID;";
            string connStr = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;

            using (var connection = new SqlConnection(connStr))
            {
                return connection.QuerySingleOrDefault<Products>(sql, new 
                { 
                    productsID = id
                });
            }
        }

        // CREATE: api/Products
        public int Create(string name, int startingPrice, string location, int productTypes_id)
        {
            //Error msg, please beware (null return at product)
            string sql = "INSERT INTO Products (Name, StartingPrice, Location, ProductTypes_id)" +
                "OUTPUT INSERTED.id" +
                " VALUES (@name, @startingPrice, @location, @productTypes_id)";
            if ((name != null && name != "") && (startingPrice >= 0) &&
                (location != null && location != "") && (productTypes_id != 0))
            {
                string connStr = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;
                using (var connection = new SqlConnection(connStr))
                {
                    var product = connection.QuerySingleOrDefault<Products>(sql, new
                    {
                        name,
                        startingPrice,
                        location,
                        productTypes_id
                    });
                    return product.Id;
                }
                
            }
            return 0;
        }

        // DELETE: api/Products/5
        public void Delete(int id)
        {
            string sql = "DELETE FROM Products WHERE Id = @productsID;";
            string connStr = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;

            using (var connection = new SqlConnection(connStr))
            {
                connection.Query(sql, new 
                { 
                    productsID = id 
                });
            }
        }
    }
}
