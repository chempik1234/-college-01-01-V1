using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AppTkani.DataModel
{
	[PrimaryKey("ProductArticleNumber")]
	public class Product
    {
        
        public string? ProductArticleNumber { get; set; }
        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public string? ProductCategory { get; set; }
        public string? ProductPhoto { get; set; }
        public string? ProductManufacturer { get; set; }
        public double ProductCost { get; set; }
        public int ProductDiscountAmount { get; set; }
        public int ProductQuantityInStock { get; set; }
        public string? ProductUnit { get; set; }
        public int ProductDiscountMax { get; set; }

		public ImageSource? PhotoPath
		{
			get
			{
                if (string.IsNullOrEmpty(ProductPhoto))
                {
                    return SingletonManager.AltImage;
                }

				var img = new BitmapImage(new Uri(SingletonManager.PhotoPathDirectory + ProductPhoto, UriKind.Relative));
				if (img != null && img.PixelHeight > 0)
                {
                    return img;
                }
                return SingletonManager.AltImage;
			}
		}

		public string ProductCostActual { get { return $"{ProductCostActualNumber} Р"; } }

        public double ProductCostActualNumber { get { return ProductCost * (100 - ProductDiscountAmount) / 100; } }
	}
}
