using SignalApp.Pages;
using System.Windows;

namespace SignalApp.Windows
{
   /// <summary>
   /// Логика взаимодействия для MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      /// <summary>
      /// Логика взаимодействия для MainWindows.xaml
      /// </summary>
      public MainWindow()
      {
         InitializeComponent();
         MainFrame.Navigate(new MainPage());
      }
   }
}
