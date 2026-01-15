using System.ComponentModel;
using System.Windows;

namespace AppTkani
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		public MainWindow()
		{
			InitializeComponent();
			DataContext = this;
			SingletonManager.MainFrame = MainFrame;
			SingletonManager.MainWindow = this;
			SingletonManager.Navigate(new Pages.LoginPage());
		}

		public string FullName { get { return SingletonManager.FullName; } }

		public event PropertyChangedEventHandler? PropertyChanged;

		public void UpdateProperty(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		private void GoToLoginBtn_Click(object sender, RoutedEventArgs e)
		{
			SingletonManager.Navigate(new Pages.LoginPage());
		}

		public void ShowLoginButton()
		{
			GoToLoginBtn.Visibility = Visibility.Visible;
		}
	}
}
