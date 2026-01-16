using Microsoft.EntityFrameworkCore;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AppTkani.DataModel
{
	[PrimaryKey("Id")]
	public class Product
    {
        public int? Id { get; set; }
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

				BitmapImage img = new BitmapImage();
				img.BeginInit();
                img.UriSource = new Uri(SingletonManager.PhotoPathDirectory + ProductPhoto, UriKind.Relative);
				img.CacheOption = BitmapCacheOption.OnLoad;
                img.EndInit();
                img.Freeze();

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
