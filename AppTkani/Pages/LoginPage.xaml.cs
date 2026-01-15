using AppTkani.DataModel;
using EasyCaptcha.Wpf;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace AppTkani.Pages
{
	/// <summary>
	/// Логика взаимодействия для LoginPage.xaml
	/// </summary>
	public partial class LoginPage : Page
	{
		System.Timers.Timer lockInputTimer;
		public LoginPage()
		{
			InitializeComponent();
			lockInputTimer = new System.Timers.Timer(TimeSpan.FromSeconds(10));
			lockInputTimer.AutoReset = false;
			lockInputTimer.Elapsed += UnlockInput;
		}

		private void LoginBtn_Click(object sender, RoutedEventArgs e)
		{
			if (CaptchaPanel.Visibility == Visibility.Visible)
			{
				string answer = CaptchaInput.Text;
				if (answer != CaptchaWidget.CaptchaText)
				{
					MessageBox.Show("Введите капчу правильно!", "Капча");
					OnFailedLogin();
					return;
				}
			}

			bool asGuest = AsGuestCheckBox.IsChecked!.Value;
			if (asGuest)
			{
				SingletonManager.AsGuest = true;
				// SingletonManager.User = ??
			} else
			{
				string login = LoginInput.Text;
				string password = PasswordBox.Password;

				StringBuilder sb = new StringBuilder();

				if (string.IsNullOrWhiteSpace(login))
				{
					sb.AppendLine("* Логин не должен быть пустым (\"пробел\" тоже считаются пустым!)");
				}
				if (string.IsNullOrWhiteSpace(password))
				{
					sb.AppendLine("* Пароль не должен быть пустым (\"пробел\" тоже считаются пустым!)");
				}

				if (sb.Length > 0)
				{
					MessageBox.Show(sb.ToString(), "Ошибка входа");

					OnFailedLogin();

					return;
				}

				try
				{
					using (var context = new DanisContext())
					{
						var user = context.User.FirstOrDefault(p => p.UserLogin == login);
						if (user == null)
						{
							MessageBox.Show("Логин не существует", "Ошибка входа");
							OnFailedLogin();
							return;
						}

						if (user.UserPassword != password)
						{
							MessageBox.Show("Неверный пароль", "Ошибка входа");
							OnFailedLogin();
							return;
						}

						SingletonManager.AsGuest = false;
						SingletonManager.User = user;
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show("Ошибка БД: " + ex, "Ошибка БД");
					return;
				}
				
			}

			MessageBox.Show("Вы успешно вошли!", "Вход");
			SingletonManager.MainWindow?.ShowLoginButton();
			SingletonManager.MainFrame?.Navigate(new ProductsPage());
		}

		private void OnFailedLogin()
		{
			bool captchaAlreadyVisible = CaptchaPanel.Visibility == Visibility.Visible;

			CaptchaPanel.Visibility = Visibility.Visible;
			CaptchaWidget.CreateCaptcha(Captcha.LetterOption.Alphanumeric, 4);


			if (captchaAlreadyVisible)
			{
				LockInput();
			}
		}

		private void LockInput()
		{
			lockInputTimer.Start();
			LoginInput.IsEnabled = false;
			PasswordBox.IsEnabled = false;
			AsGuestCheckBox.IsEnabled = false;
			CaptchaInput.IsEnabled = false;
		}

		private void UnlockInput(object? sender, ElapsedEventArgs a)
		{
			Application.Current.Dispatcher.Invoke(() => {
				LoginInput.IsEnabled = true;
				PasswordBox.IsEnabled = true;
				AsGuestCheckBox.IsEnabled = true;
				CaptchaInput.IsEnabled = true;
			});
		}
	}
}
