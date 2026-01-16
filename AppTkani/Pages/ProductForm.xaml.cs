using AppTkani.DataModel;
using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace AppTkani.Pages
{
	/// <summary>
	/// Логика взаимодействия для ProductForm.xaml
	/// </summary>
	public partial class ProductForm : Page
	{
		Product product;
		Random rnd = new Random();
		bool creatingNew = false;

		bool deleteProductImage = false;
		string newPhotoPath = "";

		public ProductForm(Product? editedProduct)
		{
			if (editedProduct == null)
			{
				product = new Product();
				creatingNew = true;
			}
			else
			{
				product = editedProduct;
				creatingNew = false;
			}

			InitializeComponent();

			ProductCategoryInput.ItemsSource = SingletonManager.GetCategories();
			ProductCategoryInput.SelectedIndex = 0;

			DataContext = product;

			// execute after InitializeComponent
			if (editedProduct != null)
			{
				IDText.Text = "ID: " + product.Id;
			}
		}

		private void SaveBtn_Click(object sender, RoutedEventArgs e)
		{
			var errorsSb = ValidateProduct();
			if (errorsSb.Length > 0)
			{
				MessageBox.Show(errorsSb.ToString(), "Ошибка ввода");
				return;
			}

			try
			{
				using (var db = new DanisContext())
				{
					if (creatingNew)
					{
						// if new, then generate article
						product.ProductArticleNumber = ValidArticleNumber(db);
						// and maybe set photo later

						// create directly
						db.Product.Add(product);
					}
					else
					{
						// if editing, maybe delete existing photo
						if (deleteProductImage)
						{
							DropImage();
							product.ProductPhoto = null;
						}

						// stage for updating
						db.Product.Attach(product);
						db.Entry(product).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
					}

					// maybe set image to something new
					if (!string.IsNullOrEmpty(newPhotoPath))
					{
						product.ProductPhoto = UploadImage(newPhotoPath);
					}

					db.SaveChanges();
				}

				MessageBox.Show("Товар сохранён!", "Успех");
			}
			catch (Exception ex)
			{
				MessageBox.Show("Ошибка сохранения продукта!\n\nПодробнее: " + ex.Message, "Ошибка БД");
			}
		}

		private string ValidArticleNumber(DanisContext db)
		{
			string articleName = GenerateArticle();
			while (db.Product.Any(p => p.ProductArticleNumber == articleName))
				articleName = GenerateArticle();
			return articleName;
		}

		string GenerateArticle()
		{
			// A000A0
			return generateSymbolBetween('A', 'Z') + generateSymbolBetween('0', '9') + generateSymbolBetween('0', '9') + generateSymbolBetween('0', '9') + generateSymbolBetween('A', 'Z') + generateSymbolBetween('0', '9');
		}

		string generateSymbolBetween(char A, char B)
		{
			return ((char)(rnd.Next(A, B))).ToString();
		}

		private StringBuilder ValidateProduct()
		{
			StringBuilder sb = new StringBuilder();

			product.ProductName = product.ProductName?.Trim();
			product.ProductDescription = product.ProductDescription?.Trim();
			product.ProductManufacturer = product.ProductManufacturer?.Trim();

			if (string.IsNullOrWhiteSpace(product.ProductName))
			{
				sb.AppendLine("* Название должно быть заполнено!");
			}
			if (product.ProductQuantityInStock < 0)
			{
				sb.AppendLine("* Количество должно быть неотрицательным!");
			}
			if (product.ProductCost < 0)
			{
				sb.AppendLine("* Цена должна быть неотрицательной!");
			}
			if (string.IsNullOrWhiteSpace(product.ProductManufacturer))
			{
				sb.AppendLine("* Производитель/Поставщик должен быть заполнен!");
			}
			if (string.IsNullOrWhiteSpace(product.ProductUnit))
			{
				sb.AppendLine("* Ед. измерения должна быть заполнена!");
			}
			if (string.IsNullOrWhiteSpace(product.ProductCategory))
			{
				sb.AppendLine("* Категория не должна быть пустым текстом!");
			}
			if (string.IsNullOrEmpty(product.ProductPhoto))
			{
				product.ProductPhoto = null;
			}

			return sb;
		}

		private void ProductCostInput_TextChanged(object sender, TextChangedEventArgs e)
		{
			ProductCostInput.Text = ProductCostInput.Text.Replace(',', '.');
		}

		private void ProductPhotoBtn_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Images|*.png;*.jpg;*.jpeg;*.bmp";

			if (openFileDialog.ShowDialog() == true)
			{
				BitmapImage img = new BitmapImage();
				img.BeginInit();
				img.UriSource = new Uri(openFileDialog.FileName, UriKind.RelativeOrAbsolute);
				img.CacheOption = BitmapCacheOption.OnLoad;
				img.EndInit();
				img.Freeze();

				if (img != null && (img.PixelHeight > 200 || img.PixelWidth > 300))
				{
					MessageBox.Show("Ошибка ввода: ограничение на размер изображения 300x200!\n\nСброс изображения не произойдёт, просто уменьшите свою картинку", "Ошибка ввода");
					return;
				}

				deleteProductImage = false;
				newPhotoPath = openFileDialog.FileName;
			}
			else
			{
				deleteProductImage = true;
				newPhotoPath = "";
			}
		}

		private void DropImage()
		{
			if (!TryDeleteCurrentProductImage())
			{
				MessageBox.Show("Изображения товара уже нет в хранилище! Удаление изображения не произойдёт", "Инфо");
			}
		}

		bool TryDeleteCurrentProductImage()
		{
			string relativePath = SingletonManager.PhotoPathDirectory + product.ProductPhoto;
			if (File.Exists(relativePath))
			{
				try
				{
					File.Delete(relativePath);
					return true;
				}
				catch (Exception ex)
				{
					MessageBox.Show("Ошибка удаления файла " + relativePath + " - " + ex.Message, "Ошибка системы ввода-вывода");
					return false;
				}
			}
			return false;
		}

		private string UploadImage(string fileName)
		{
			TryDeleteCurrentProductImage();

			if (File.Exists(fileName))
			{
				var imageExt = fileName.Split('.').Last(); // png jpg
				string newShortFilename = product.ProductArticleNumber + '.' + imageExt;
				try
				{
					File.Copy(fileName, SingletonManager.PhotoPathDirectory + newShortFilename, true);
				}
				catch (Exception ex)
				{
					MessageBox.Show("Ошибка копирования изображения в хранилище приложения: " + ex, "Ошибка системы ввода-вывода");
				}

				return newShortFilename;
			}

			MessageBox.Show("Ошибка копирования изображения в хранилище приложения: файл, который вы хотите загрузить, не найден", "Ошибка системы ввода-вывода");
			return "";
		}

		private void BackBtn_Click(object sender, RoutedEventArgs e)
		{
			SingletonManager.Navigate(new ProductsPage());
		}
	}
}
