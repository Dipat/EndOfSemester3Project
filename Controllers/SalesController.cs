﻿using Dapper;
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
        ProductsController productsController = new ProductsController();
        ProductTypesController productTypesController = new ProductTypesController();
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
            string sql = "SELECT * FROM Sales WHERE isActive = 1";

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
            string sql = "SELECT * FROM Sales WHERE id = @salesID;";
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
        public void Create(int users_id, int products_id, bool isBid, string description, int currentPrice, int bidHours)
        {
            TimeSpan timeRemaining = new TimeSpan(0, 0, 0);
            string sql = "INSERT INTO Sales (users_id, products_id, isBid, description, currentPrice, timeRemaining, isActive)" +
                " VALUES (@users_id, @products_id, @isBid, @description, @currentPrice, @timeRemaining, @isActive)";
            if (description == null || description == "")
            {
                description = "No description written.";
            }
            if (bidHours != 0)
            {
                timeRemaining = new TimeSpan(bidHours, 0, 0);
            }
            string connStr = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;
            using (var connection = new SqlConnection(connStr))
            {
                var sales = connection.QuerySingleOrDefault<Sales>(sql, new
                {
                    users_id = users_id,
                    products_id = products_id,
                    isBid = isBid,
                    description = description,
                    currentPrice = productsController.Get(products_id).startingPrice,
                    timeRemaining = timeRemaining,
                    isActive = true
                });
            }
        }

        //Bidding function(updates current price by bid Amount, and also sets user as highest bidder)
        public void Bid(int sales_id, int users_id, int bidValue)
        {//Maybe: failed to place bid return bool
            if (Get(sales_id).highestBidder_id != users_id)
            {
                string sql = "UPDATE Sales(currentPrice, highestBidder_id)" +
               "VALUES(@price, @highestBidder_id)" +
               "WHERE id ='" + sales_id + "'";
                string connStr = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;
                using (var connection = new SqlConnection(connStr))
                {
                    connection.Query(sql, new
                    {
                        price = (Get(sales_id).currentPrice + bidValue),
                        highestBidder_id = users_id
                    });
                }
            }
        }

        //Search for Sales that have a specific string snippet in their Product's name
        public IEnumerable<Sales> FindSaleByProductName(string name)
        {
            var sales = GetActive();
            for (int i = 0; i < sales.Count(); i++)
            {
                if (!productsController.Get(sales.ElementAt(i).products_id).name.Contains(name))
                {//if this search doesnt work properly, then the remove is the problem
                    sales.ToList().RemoveAt(i);
                }
            }
            return sales;
        }

        //Sort sales based on price(descending/ascending)
        public IEnumerable<Sales> SortSales(string sortBy)
        {
            var sales = GetActive();
            if (sortBy == "ascending")
            {
                sales.OrderBy(sale => sale.currentPrice);
            }
            else if (sortBy == "descending")
            {
                sales.OrderByDescending(sale => sale.currentPrice);
            }   
            else
            {//if this sorting doesnt work properly, then the remove is the problem
                var productTypes = productTypesController.Get();
                for (int i = 0; i < sales.Count(); i++)
                {
                    for (int j = 0; j < productTypes.Count(); j++)
                    {
                        if (productTypesController.Get(productsController.Get
                            (sales.ElementAt(i).products_id).productTypes_id).type != sortBy)
                        {
                            sales.ToList().RemoveAt(i);
                        }
                    }
                }
            }
            return sales;
        }

        // DELETE: api/Sales/5
        public void Delete(int id)
        {
            string sql = "DELETE * FROM Sales WHERE id = @salesID;";
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
