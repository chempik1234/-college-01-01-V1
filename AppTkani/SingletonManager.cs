using AppTkani.DataModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AppTkani
{
	public static class SingletonManager
	{
		public static Frame? MainFrame;

		public static void Navigate(Page newPage)
		{
            if (MainFrame != null)
            {
				MainFrame.Navigate(newPage);
            }
        }

		public static bool asGuest;
		public static bool AsGuest { get => asGuest; set
			{
				asGuest = value;
				UpdateFullName();
			}
		}

		private static User? user;
		public static User? User { get => user; set
			{
				user = value;
				UpdateFullName();
			}
		}

		private static void UpdateFullName()
		{
			MainWindow!.UpdateProperty(nameof(MainWindow.FullName));
		}

		public static string FullName { get
			{
                if (AsGuest)
					return "<гость>";
                if (User == null)
					return "Входа не было";
				return $"{User.UserSurname} {User.UserName} {User.UserPatronymic}";
			} }

		public static string PhotoPathDirectory = "../../../Media/Product/";

		private static ImageSource? altImage;

		public static ImageSource? AltImage { get
			{
				if (altImage == null)
				{
					var img = new BitmapImage(new Uri("/Resources/picture.png", UriKind.Relative));
					altImage = img;
				}
				return altImage;
			} }

		public static MainWindow? MainWindow { get; set; }

		public static bool UserIsAdmin()
		{
			if (SingletonManager.User == null || SingletonManager.AsGuest)
			{
				return false;
			}
			try
			{
				using (var db = new DanisContext())
				{
					var role = db.Role.FirstOrDefault(p => p.RoleId == SingletonManager.User.UserRole);
					if (role == null)
						return false;
					return role.RoleName.ToLower() == "администратор";
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Ошибка проверки роли пользователя!\n\nПодробнее: " + ex.Message, "Ошибка БД");
			}
			return false;
		}
	}
}
