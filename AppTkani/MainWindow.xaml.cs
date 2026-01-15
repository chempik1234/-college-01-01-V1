using System.Windows;

namespace AppTkani
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			DataContext = this;
			SingletonManager.MainFrame = MainFrame;
			SingletonManager.Navigate(new Pages.LoginPage());
		}
	}
}
