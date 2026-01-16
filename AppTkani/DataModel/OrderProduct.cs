using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTkani.DataModel
{
	[PrimaryKey("OrderID", "ProductArticleNumber")]
	public class OrderProduct
	{
		public int OrderID { get; set; }
		public string? ProductArticleNumber { get; set; }
	}
}
