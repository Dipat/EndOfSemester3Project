using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EndOfSemester3.Models
{
	public class Sales
	{
		public int Id { get; set; }
		public string Users_id { get; set; }
		public int Products_id { get; set; }
		public string Description { get; set; }
		public int CurrentPrice { get; set; }
		public string HighestBidder_id { get; set; }
		public DateTime EndTime { get; set; }
		public bool IsActive { get; set; }

		public Sales()
        {
        }
		public Sales(int id, string usersId, int productsId, string description, 
			int currentPrice, string highestBidderId, DateTime endTime, bool isActive)
		{
			this.Id = id;
			this.Users_id = usersId;
			this.Products_id = productsId;
			this.Description = description;
			this.CurrentPrice = currentPrice;
			this.HighestBidder_id = highestBidderId;
			this.EndTime = endTime;
			this.IsActive = isActive;
		}
	}
}