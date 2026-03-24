using Microsoft.Win32;
using SignalApp.Funcrion;
using SignalApp.Function;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace SignalApp.Pages
{
   /// <summary>
   /// Логика взаимодействия для MainPage.xaml
   /// </summary>
   public partial class MainPage : Page
   {
      private MainPageViewModel ViewModel = new MainPageViewModel();

      private string GetFullPath(SignalType signalType)
      {
         if (!Enum.IsDefined(typeof(SignalType), signalType))
            throw new ArgumentException("Incorrect signal type");

         return Path.Combine(
            SavePath.Text,
            $"{(signalType == SignalType.Sine ? "sin" : "square")}_A{ViewModel.Amplitude}_F{ViewModel.Frequency}_{DateTime.UtcNow:yyyyMMdd}.csv");
      }

      public MainPage()
      {
         InitializeComponent();

         SavePath.Text = Path.GetTempPath();

         DataContext = ViewModel;
      }

      private void MeanButton_Click(object sender, RoutedEventArgs e)
      {
         if (ViewModel.HasErrors())
            MessageBox.Show(
               "Пожалуйста, исправьте ошибки в полях ввода перед продолжением.",
               "Ошибка ввода",
               MessageBoxButton.OK,
               MessageBoxImage.Error);

         else if (!ViewModel.TryGetParsedValues(
            out double amplitude,
            out double frequency,
            out int maxCount,
            out int? periodCount))
            MessageBox.Show(
               "Введите все значения",
               "Ошибка ввода",
               MessageBoxButton.OK,
               MessageBoxImage.Error);

         else
            NavigationService.Navigate(
               new Square.SquarePage(GetFullPath(SignalType.Square), SaveCheckBox.IsChecked ?? false, amplitude, frequency, maxCount, periodCount));
      }

      private void SinButton_Click(object sender, RoutedEventArgs e)
      {
         if (ViewModel.HasErrors())
            MessageBox.Show(
               "Пожалуйста, исправьте ошибки в полях ввода перед продолжением.",
               "Ошибка ввода",
               MessageBoxButton.OK,
               MessageBoxImage.Error);

         else if (!ViewModel.TryGetParsedValues(
            out double amplitude,
            out double frequency,
            out int maxCount,
            out int? periodCount))
            MessageBox.Show(
               "Введите все значения",
               "Ошибка ввода",
               MessageBoxButton.OK,
               MessageBoxImage.Error);

         else
            NavigationService.Navigate(
               new Sin.SinPage(GetFullPath(SignalType.Sine), SaveCheckBox.IsChecked ?? false, amplitude, frequency, maxCount, periodCount));
      }

      private void SaveFolderButton_Click(object sender, RoutedEventArgs e)
      {
         var dialog = new OpenFileDialog
         {
            Title = "Выберите любую папку",
            CheckFileExists = false,
            CheckPathExists = true,
            FileName = "Выбрать папку"
         };

         if (dialog.ShowDialog() == true)
            SavePath.Text = Path.GetDirectoryName(dialog.FileName);
      }

      private void SaveAllButton_Click(object sender, RoutedEventArgs e)
      {
         this.IsEnabled = false;

         SaveAllGraf();

         this.IsEnabled = true;
      }

      public void SaveAllGraf()
      {
         if (ViewModel.HasErrors())
            MessageBox.Show(
               "Пожалуйста, исправьте ошибки в полях ввода перед продолжением.",
               "Ошибка ввода",
               MessageBoxButton.OK,
               MessageBoxImage.Error);

         else if (!ViewModel.TryGetParsedValues(
            out double amplitude,
            out double frequency,
            out int maxCount,
            out int? periodCount))
            MessageBox.Show(
               "Введите все значения",
               "Ошибка ввода",
               MessageBoxButton.OK,
               MessageBoxImage.Error);

         else
         {
            var grafSin = MathFunc.GenerateGraf(SignalType.Sine, amplitude, frequency, maxCount, periodCount);
            var grafSquare = MathFunc.GenerateGraf(SignalType.Square, amplitude, frequency, maxCount, periodCount);

            string resultMessage = string.Empty;

            bool SaveSin = false,
               SaveSquare = false;

            try
            {
               string path = GetFullPath(SignalType.Sine);
               SaveSin = SaveGraf.SaveGrafCSV(path, grafSin);
               resultMessage += $"График Синусоиды сохранен в {path}";
            }
            catch (Exception ex)
            {
               MessageBox.Show(ex.Message, "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            try
            {
               string path = GetFullPath(SignalType.Square);

               SaveSquare = SaveGraf.SaveGrafCSV(path, grafSquare);

               resultMessage = resultMessage.Length != 0 ?
                  resultMessage + $"\n\nГрафик Меандра сохранен в {path}" :
                  $"График Меандра сохранен в {path}";
            }
            catch (Exception ex)
            {
               MessageBox.Show(ex.Message, "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (SaveSin || SaveSquare)
            {
               var result = MessageBox.Show(
                  resultMessage + "\n\nОткрыть папку?",
                  "Сохранение графиков",
                  MessageBoxButton.YesNo);

               if (result == MessageBoxResult.Yes)
                  Process.Start("explorer.exe", SavePath.Text);
            }
            else
            {
               MessageBox.Show(
                  "Сохранение не успешно",
                  "Сохранение графиков",
                  MessageBoxButton.OK);
            }
         }
      }
   }
}
