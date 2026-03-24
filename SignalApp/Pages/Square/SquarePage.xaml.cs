using SignalApp.Funcrion;
using SignalApp.Function;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace SignalApp.Pages.Square
{
   /// <summary>
   /// Логика взаимодействия для MeanPage.xaml
   /// </summary>
   public partial class SquarePage : Page
   {
      private (double[] xs, double xMax, double[] ys, double max, double min, double avg, int zeroCrossing) graf;

      private void DrawGraf(double amplitude, double frequency, int maxCount, int? periodCount)
      {
         if (amplitude <= 0)
            throw new ArgumentOutOfRangeException("Amplitude must be a positive number");

         if (frequency <= 0)
            throw new ArgumentOutOfRangeException("Frequency must be a positive number");

         if (maxCount > 10000 || maxCount < 100)
            throw new ArgumentOutOfRangeException("MaxCount must be a number between 100 and 10000");

         if (!(periodCount is null) && periodCount > maxCount)
            throw new ArgumentOutOfRangeException("PeriodCount must be a number less MaxCount");

         graf = MathFunc.GenerateGraf(SignalType.Square, amplitude, frequency, maxCount, periodCount);

         SignalPlot.Plot.Clear();

         SignalPlot.Plot.Add.Scatter(graf.xs, graf.ys);

         SignalPlot.Plot.Axes.SetLimitsX(0, graf.xMax);

         SignalPlot.Plot.Axes.Bottom.Label.Text = $"Время (сек.)";

         SignalPlot.Plot.Axes.Left.Label.Text = "Амплитуда";

         SignalPlot.Plot.Grid.IsVisible = true;

         SignalPlot.Refresh();
      }

      public SquarePage(string fullPath, bool saveFile, double amplitude, double frequency, int maxCount, int? periodCount)
      {
         InitializeComponent();
         DrawGraf(amplitude, frequency, maxCount, periodCount);

         if (saveFile)
            if (SaveGraf.SaveGrafCSV(fullPath, graf))
            {
               var result = MessageBox.Show(
                        "График Меандра сохранен\n\nОткрыть папку?",
                        "Сохранение графиков",
                        MessageBoxButton.YesNo);

               if (result == MessageBoxResult.Yes)
                  Process.Start("explorer.exe", Path.GetDirectoryName(fullPath));
            }
            else
            {
               MessageBox.Show(
                   "Ошибка сохранения графика",
                   "Сохранение графиков",
                   MessageBoxButton.OK,
                   MessageBoxImage.Error);
            }

         MathParamText.Text = $"Max / Min / Avg / Zero Cross. : {graf.max:0.#####} / {graf.min:0.#####} / {graf.avg:0.#####} / {graf.zeroCrossing}";
      }

      private void BackButton_Click(object sender, RoutedEventArgs e)
      {
         NavigationService.GoBack();
      }
   }
}
