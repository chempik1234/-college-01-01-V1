using AppTkani.DataModel;
using System.Windows.Controls;

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

		public static bool AsGuest = false;

		public static User? User { get; internal set; }

		public static string PhotoPathDirectory = "../../../Media/Product/";
	}
}
