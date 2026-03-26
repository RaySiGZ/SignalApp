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

      /// <summary>
      /// Модель элемента списка для отображения типа сигнала в ComboBox
      /// </summary>
      private class EnumItem
      {
         public SignalType Value { get; set; }
         public string Display { get; set; }
      }

      /// <summary>
      /// Формирует полный путь к CSV-файлу для сохранения графика выбранного типа
      /// Имя файла включает тип сигнала, амплитуду, частоту и текущую дату
      /// </summary>
      /// <param name="signalType">Тип сигнала</param>
      /// <returns>Полный путь к файлу сохранения</returns>
      /// <exception cref="ArgumentException">
      /// Выбрасывается, если передан некорректный тип сигнала
      /// </exception>
      private string GetFullPath(SignalType signalType)
      {
         if (!Enum.IsDefined(typeof(SignalType), signalType))
            throw new ArgumentException($"Unsupported signal type: {signalType}", nameof(signalType));

         return Path.Combine(
            SavePath.Text,
            $"{EnumHelper.GetShortName(signalType)}_A{ViewModel.Amplitude}_F{ViewModel.Frequency}_{DateTime.UtcNow:ddMMyyyy}.csv");
      }

      /// <summary>
      /// Проверяет введённые данные, генерирует график выбранного типа и сохраняет его в CSV-файл
      /// При некорректных данных выводит сообщение об ошибке
      /// </summary>
      /// <param name="type">Тип сигнала для генерации и сохранения</param>
      /// <returns>true, если график успешно сохранён; иначе false</returns>
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
            out int countMax,
            out int? countPeriod))
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
            var grafValue = new GrafValue(type, amplitude, frequency, countMax, countPeriod);

            var grafResult = MathFunc.GenerateGraf(grafValue);

            SaveGraf.SaveGrafCSV(GetFullPath(type), grafValue, grafResult);

            return true;
         }
      }

      /// <summary>
      /// Сохраняет графики для всех доступных типов сигналов
      /// После успешного сохранения предлагает открыть папку с файлами
      /// </summary>
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
      /// Обрабатывает нажатие кнопки выбора папки сохранения
      /// Открывает диалог выбора и устанавливает выбранный путь
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

      /// <summary>
      /// Обрабатывает нажатие кнопки сохранения выбранного графика
      /// При успешном сохранении предлагает открыть папку с результатом
      /// </summary>
      private void SaveButton_Click(object sender, RoutedEventArgs e)
      {
         this.IsEnabled = false;

         if (SaveSelect((SignalType)GrafTypeComboBox.SelectedValue))
         {
            var message = MessageBox.Show($"Графики сохранены в {SavePath.Text}\n\nОткрыть папку?",
               "Сохранение",
               MessageBoxButton.YesNo);

            if (message == MessageBoxResult.Yes)
               Process.Start("explorer.exe", SavePath.Text);
         }

         this.IsEnabled = true;
      }

      /// <summary>
      /// Обрабатывает нажатие кнопки сохранения всех графиков
      /// </summary>
      private void SaveAllButton_Click(object sender, RoutedEventArgs e)
      {
         this.IsEnabled = false;

         SaveAll();

         this.IsEnabled = true;
      }

      /// <summary>
      /// Обрабатывает нажатие кнопки отображения графика
      /// Проверяет введённые данные и открывает страницу построения графика
      /// </summary>
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
            out int countMax,
            out int? countPeriod))
            MessageBox.Show(
               "Введите все значения",
               "Ошибка ввода",
               MessageBoxButton.OK,
               MessageBoxImage.Error);
         else
         {
            var grafValue = new GrafValue((SignalType)GrafTypeComboBox.SelectedValue, amplitude, frequency, countMax, countPeriod);

            NavigationService.Navigate(new GrafPage(GetFullPath((SignalType)GrafTypeComboBox.SelectedValue), grafValue));
         }
      }
   }
}
