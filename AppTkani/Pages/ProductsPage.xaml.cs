using AppTkani.DataModel;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;

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

			if (!SingletonManager.UserIsAdmin())
				AdminToolbar.Visibility = Visibility.Collapsed;
			else
				AdminToolbar.Visibility = Visibility.Visible;

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
					UpdateAmountText(db);
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

		private void DeleteButton_Click(object sender, RoutedEventArgs e)
		{
			Product? selectedProduct = SelectedProduct();
			if (selectedProduct == null)
			{
				MessageBox.Show("Вы не выбрали товар для удаления", "Удаление товара");
				return;
			}

			try
			{
				using (var db = new DanisContext())
				{
					if (db.OrderProduct.FirstOrDefault(p => p.ProductArticleNumber == selectedProduct.ProductArticleNumber) == null)
					{
						db.Product.Remove(selectedProduct);
						db.SaveChanges();
						products.Remove(selectedProduct);
						RefreshLV();
						UpdateAmountText(db);
						MessageBox.Show($"Товар {selectedProduct.ProductArticleNumber} удалён!", "Удаление товара");
					} else
					{
						MessageBox.Show("Товар присутствует в заказе - удалить нельзя.", "Удаление товара");
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Ошибка удаления товара!\n\nПодробнее: " + ex.Message, "Ошибка БД");
			}
		}

		private void EditButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private void AddButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private void UpdateAmountText(DanisContext db)
		{
			AmountText.Text = products.Count + " / " + db.Product.Count();
		}

		private Product? SelectedProduct()
		{
			return ProductsLV.SelectedItem as Product;
		}
	}
}
