using Microsoft.Win32;
using SignalApp.Function;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using SignalApp.Data;

namespace SignalApp.Pages
{
   /// <summary>
   /// Логика взаимодействия для MainPage.xaml
   /// </summary>
   public partial class MainPage : Page
   {
      private MainPageViewModel ViewModel = new MainPageViewModel();

      private class EnumItem
      {
         public SignalType Value { get; set; }
         public string Display { get; set; }
      }

      private string GetFullPath(SignalType signalType)
      {
         if (!Enum.IsDefined(typeof(SignalType), signalType))
            throw new ArgumentException("Incorrect signal type");

         return Path.Combine(
            SavePath.Text,
            $"{EnumHelper.GetShortName(signalType)}_A{ViewModel.Amplitude}_F{ViewModel.Frequency}_{DateTime.UtcNow:ddMMyyyy}.csv");
      }

      private bool SaveSelect(SignalType type)
      {
         if (ViewModel.HasErrors())
         {
            MessageBox.Show(
               "Пожалуйста, исправьте ошибки в полях ввода перед продолжением.",
               "Ошибка ввода",
               MessageBoxButton.OK,
               MessageBoxImage.Error);
            return false;
         }

         else if (!ViewModel.TryGetParsedValues(
            out double amplitude,
            out double frequency,
            out int maxCount,
            out int? periodCount))
         {
            MessageBox.Show(
               "Введите все значения",
               "Ошибка ввода",
               MessageBoxButton.OK,
               MessageBoxImage.Error);
            return false;
         }
         else
         {
            var grafValue = new GrafValue(type, amplitude, frequency, maxCount, periodCount);

            var grafResult = MathFunc.GenerateGraf(grafValue);

            SaveGraf.SaveGrafCSV(GetFullPath(type), grafResult);

            return true;
         }
      }

      private void SaveAll()
      {
         var listType = Enum.GetValues(typeof(SignalType));

         foreach (SignalType type in listType)
            if (!SaveSelect(type)) return;

         var message = MessageBox.Show($"Графики сохранены в {SavePath.Text}\n\nОткрыть папку?",
            "Сохранение",
            MessageBoxButton.YesNo);

         if (message == MessageBoxResult.Yes)
            Process.Start("explorer.exe", SavePath.Text);
      }

      /// <summary>
      /// Инициализация основной страницы
      /// </summary>
      public MainPage()
      {
         InitializeComponent();

         GrafTypeComboBox.ItemsSource = Enum
            .GetValues(typeof(SignalType))
            .Cast<SignalType>()
            .Select(x => new EnumItem
            {
               Value = x,
               Display = EnumHelper.GetDisplayName(x)
            })
            .ToList();

         GrafTypeComboBox.SelectedIndex = 0;

         SavePath.Text = Path.GetTempPath();

         DataContext = ViewModel;
      }

      /// <summary>
      /// Обработка нажатия кнопки Папка сохранения
      /// </summary>
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

      private void SaveButton_Click(object sender, RoutedEventArgs e)
      {
         this.IsEnabled = false;

         SaveSelect((SignalType)GrafTypeComboBox.SelectedValue);

         var message = MessageBox.Show($"Графики сохранены в {SavePath.Text}\n\nОткрыть папку?",
            "Сохранение",
            MessageBoxButton.YesNo);

         if (message == MessageBoxResult.Yes)
            Process.Start("explorer.exe", SavePath.Text);

         this.IsEnabled = true;
      }

      /// <summary>
      /// Обработка нажатия кнопки Сохранить все
      /// </summary>
      private void SaveAllButton_Click(object sender, RoutedEventArgs e)
      {
         this.IsEnabled = false;

         SaveAll();

         this.IsEnabled = true;
      }

      private void ShowGrafButton_Click(object sender, RoutedEventArgs e)
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
            var grafValue = new GrafValue((SignalType)GrafTypeComboBox.SelectedValue, amplitude, frequency, maxCount, periodCount);

            NavigationService.Navigate(new GrafPage(GetFullPath((SignalType)GrafTypeComboBox.SelectedValue), grafValue));
         }
      }

      /// <summary>
      /// Функция сохранения всех графиков
      /// </summary>
      //private void SaveAllGraf()
      //{
      //   if (ViewModel.HasErrors())
      //      MessageBox.Show(
      //         "Пожалуйста, исправьте ошибки в полях ввода перед продолжением.",
      //         "Ошибка ввода",
      //         MessageBoxButton.OK,
      //         MessageBoxImage.Error);

      //   else if (!ViewModel.TryGetParsedValues(
      //      out double amplitude,
      //      out double frequency,
      //      out int maxCount,
      //      out int? periodCount))
      //      MessageBox.Show(
      //         "Введите все значения",
      //         "Ошибка ввода",
      //         MessageBoxButton.OK,
      //         MessageBoxImage.Error);

      //   else
      //   {
      //      var grafSin = MathFunc.GenerateGraf(SignalType.Sine, amplitude, frequency, maxCount, periodCount);
      //      var grafSquare = MathFunc.GenerateGraf(SignalType.Square, amplitude, frequency, maxCount, periodCount);

      //      string resultMessage = string.Empty;

      //      bool SaveSin = false,
      //         SaveSquare = false;

      //      try
      //      {
      //         string path = GetFullPath(SignalType.Sine);
      //         SaveSin = SaveGraf.SaveGrafCSV(path, grafSin);
      //         resultMessage += $"График Синусоиды сохранен в {path}";
      //      }
      //      catch (Exception ex)
      //      {
      //         MessageBox.Show(ex.Message, "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
      //      }
      //      try
      //      {
      //         string path = GetFullPath(SignalType.Square);

      //         SaveSquare = SaveGraf.SaveGrafCSV(path, grafSquare);

      //         resultMessage = resultMessage.Length != 0 ?
      //            resultMessage + $"\n\nГрафик Меандра сохранен в {path}" :
      //            $"График Меандра сохранен в {path}";
      //      }
      //      catch (Exception ex)
      //      {
      //         MessageBox.Show(ex.Message, "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
      //      }

      //      if (SaveSin || SaveSquare)
      //      {
      //         var result = MessageBox.Show(
      //            resultMessage + "\n\nОткрыть папку?",
      //            "Сохранение графиков",
      //            MessageBoxButton.YesNo);

      //         if (result == MessageBoxResult.Yes)
      //            Process.Start("explorer.exe", SavePath.Text);
      //      }
      //      else
      //      {
      //         MessageBox.Show(
      //            "Сохранение не успешно",
      //            "Сохранение графиков",
      //            MessageBoxButton.OK);
      //      }
      //   }
      //}
   }
}
