using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EndOfSemester3.Models
{
	public class Products
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int StartingPrice { get; set; }
		public string Location { get; set; }
		public int ProductTypes_id { get; set; }

		public Products()
		{
		}

		public Products(int id, string name, int startingPrice, string location, int productTypesId)
		{
			this.Id = id;
			this.Name = name;
			this.StartingPrice = startingPrice;
			this.Location = location;
			this.ProductTypes_id = productTypesId;
		}

		
	}
}