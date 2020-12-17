using Dapper;
using EndOfSemester3.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Web.Http;

namespace EndOfSemester3.Controllers
{
    public class SalesController : ApiController
    {
        ProductsController _productsController = new ProductsController();
        ProductTypesController _productTypesController = new ProductTypesController();
        // GET: api/Sales
        public IEnumerable<Sales> Get()
        {
            string sql = "SELECT * FROM Sales";

            string connStr = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;

            using (var connection = new SqlConnection(connStr))
            {
                var sale = connection.Query<Sales>(sql).ToList();
                return sale;
            }
        }

        // GET: api/Sales
        public IEnumerable<Sales> GetActive()
        {
            string sql = "SELECT * FROM Sales WHERE IsActive = 1";

            string connStr = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;

            using (var connection = new SqlConnection(connStr))
            {
                var sale = connection.Query<Sales>(sql).ToList();
                return sale;
            }
        }

        // GET: api/Sales/5
        public Sales Get(int id)
        {
            string sql = "SELECT * FROM Sales WHERE Id = @salesID;";
            string connStr = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;

            using (var connection = new SqlConnection(connStr))
            {
                return connection.QuerySingleOrDefault<Sales>(sql, new 
                { 
                    salesID = id
                });
            }
        }

        // CREATE: api/Sales (Take a look at this!)
        public bool Create(string usersId, int productsId, string description, int currentPrice, DateTime endTime)
        {
            bool result = false;
            string sql = "INSERT INTO Sales (Users_id, Products_id, Description, CurrentPrice, EndTime, IsActive)" +
                " VALUES (@users_id, @products_id, @description, @currentPrice, @endTime, @isActive)";
            if (description == null || description == "")
            {
                description = "No description written.";
            }
            string connStr = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;
            using (var connection = new SqlConnection(connStr))
            {
                var sales = connection.QuerySingleOrDefault<Sales>(sql, new
                {
                    users_id = usersId,
                    products_id = productsId,
                    description = description,
                    currentPrice = _productsController.Get(productsId).StartingPrice,
                    endTime = endTime,
                    isActive = true
                });
                result = true;
            }
            return result;
        }

        //Bidding function(updates current price by bid Amount, and also sets user as highest bidder)
        public bool Bid(int salesId, string usersId, int bidValue)
        {//Maybe: failed to place bid return bool
            //Continue from here. Random syntax error
            var sale = Get(salesId);
            bool success = false;
            if (sale.HighestBidder_id != usersId && sale.Users_id != usersId && sale.IsActive)
            {
                string sql = "UPDATE Sales" +
                " SET CurrentPrice = @price, HighestBidder_id = @highestBidder_id" +
                " WHERE Id = @salesID;";
                string connStr = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;
                using (var connection = new SqlConnection(connStr))
                {
                    connection.Query(sql, new
                    {
                        price = (Get(salesId).CurrentPrice + bidValue),
                        highestBidder_id = usersId,
                        salesID = salesId
                    });
                }
                success = true;
            }
            return success;
        }

        //
        public void SetInactive(int salesId)
        {
            string sql = "UPDATE Sales" +
            " SET IsActive = @isActive WHERE Id = @salesID;";
            string connStr = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;
            using (var connection = new SqlConnection(connStr))
            {
                connection.Query(sql, new
                {
                    isActive = false,
                    salesID = salesId
                });
            }
        }

        //Search for Sales that have a specific string snippet in their Product's name
        public IEnumerable<Sales> FindSaleByProductName(string name)
        {
            var sales = GetActive().ToList();
            List<Sales> sorted = new List<Sales>();
            for (int i = 0; i < sales.Count(); i++)
            {
                if (_productsController.Get(sales.ElementAt(i).Products_id).Name.Contains(name))
                {//if this search doesnt work properly, then the remove is the problem
                    sorted.Add(sales.ElementAt(i));
                }
            }
            return sorted;
        }

        //Sort sales based on price(descending/ascending)
        public IEnumerable<Sales> SortSales(string sortBy)
        {
            var sales = GetActive();
            List<Sales> sorted = new List<Sales>();
            if (sortBy == "ascending")
            {
                sorted = sales.OrderBy(sale => sale.CurrentPrice).ToList();
            }
            else if (sortBy == "descending")
            {
                sorted = sales.OrderByDescending(sale => sale.CurrentPrice).ToList();
            }   
            else
            {//if this sorting doesnt work properly, then the remove is the problem
                var productTypes = _productTypesController.Get();
                for (int i = 0; i < sales.Count(); i++)
                {
                    if (_productTypesController.Get(_productsController.Get
                        (sales.ElementAt(i).Products_id).ProductTypes_id).Type == sortBy)
                    {
                        sorted.Add(sales.ElementAt(i));
                    }
                }
            }
            return sorted;
        }

        // DELETE: api/Sales/5
        public void Delete(int id)
        {
            string sql = "DELETE FROM Sales WHERE Id = @salesID;";
            string connStr = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;

            using (var connection = new SqlConnection(connStr))
            {
                connection.Query(sql, new
                {
                    salesID = id 
                });
            }
        }
    }
}
