using AppTkani.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AppTkani.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProductsPage.xaml
    /// </summary>
    public partial class ProductsPage : Page
    {
		string searchText = string.Empty;
		string manufacturer = string.Empty;
		SortMode sortMode = SortMode.No;

		enum SortMode
		{
			Up, Down, No
		}

		List<Product> products = new();

        public ProductsPage()
        {
            InitializeComponent();

			var manufacturers = GetManufacturers();
			manufacturers.Insert(0, "Все производители");
			ManufacturerDropDown.ItemsSource = manufacturers;
			ManufacturerDropDown.SelectedIndex = 0;

			RefreshProductsSelectAll();
        }

		void RefreshProductsSelectAll()
		{
			ApplyFilterToProducts(_ => true);
		}

		private void ApplyFilterToProducts(System.Linq.Expressions.Expression<Func<Product, bool>> condition)
        {
			try
			{
				using (var db = new DanisContext())
				{
					products = db.Product.Where(condition).ToList();
					AmountText.Text = products.Count + " / " + db.Product.Count();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Ошибка обновления списка товаров!\n\nПодробнее: " + ex.Message, "Ошибка БД");
			}
			ApplySortToProducts();
			RefreshLV();
		}

		void RefreshLV()
		{
			if (ProductsLV != null)
			{
				ProductsLV.ItemsSource = null;
				ProductsLV.ItemsSource = products;
			}
		}

		private void SortByDropDown_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			switch (SortByDropDown.SelectedIndex)
			{
				case 0: { sortMode = SortMode.No; break; }
				case 1: { sortMode = SortMode.Up; break; }
				case 2: { sortMode = SortMode.Down; break; }
			}
			ApplySortToProducts();
			RefreshLV();
		}

		private void ApplySortToProducts()
		{
			switch (sortMode)
			{
				case SortMode.Up: { products = products.OrderBy(p => p.ProductCostActualNumber).ToList();  break; }
				case SortMode.Down: { products = products.OrderBy(p => -p.ProductCostActualNumber).ToList(); break; }
			}
		}

		// apply sort
		// apply all filters -> apply sort -> refresh list
		// refresh list
		// change search -> apply all filters -> apply sort -> refresh list
		// change sort -> apply sort -> refresh list
		// change manuf -> apply all filters -> refresh list

		private void SearchInput_TextChanged(object sender, TextChangedEventArgs e)
		{
			searchText = SearchInput.Text;
			ApplyAllFilters();
		}

		private void ManufacturerDropDown_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (ManufacturerDropDown.SelectedIndex == 0)
			{
				manufacturer = string.Empty;
			} else
			{
				manufacturer = ManufacturerDropDown.SelectedValue?.ToString() ?? string.Empty;
			}
			ApplyAllFilters();
		}

		void ApplyAllFilters()
		{
			ApplyFilterToProducts(p =>
				(p.ProductArticleNumber + ' ' + p.ProductName + ' ' + p.ProductDescription).Contains(searchText) &&
				(string.IsNullOrEmpty(manufacturer) || p.ProductManufacturer == manufacturer)
			);
		}

		private List<string> GetManufacturers()
		{
			var list = new List<string>();

			try
			{
				using (var db = new DanisContext())
				{
					list = db.Product.Select(p => p.ProductManufacturer ?? string.Empty).Distinct().ToList();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Ошибка обновления списка товаров!\n\nПодробнее: " + ex.Message, "Ошибка БД");
			}

			return list;
		}
	}
}
