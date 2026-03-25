using SignalApp.Function;
using SignalApp.Data;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace SignalApp.Pages
{
   /// <summary>
   /// Логика взаимодействия для GrafPage.xaml
   /// </summary>
   public partial class GrafPage : Page
   {
      private (double[] xs, double xMax, double[] ys, double max, double min, double avg, int zeroCrossing) graf;

      private void DrawGraf(GrafValue grafValue)
      {
         if (grafValue.Amplitude <= 0)
            throw new ArgumentOutOfRangeException("Amplitude must be a positive number");

         if (grafValue.Frequency <= 0)
            throw new ArgumentOutOfRangeException("Frequency must be a positive number");

         if (grafValue.MaxCount > 10000 || grafValue.MaxCount < 100)
            throw new ArgumentOutOfRangeException("MaxCount must be a number between 100 and 10000");

         if (!(grafValue.PeriodCount is null) && grafValue.PeriodCount > grafValue.MaxCount)
            throw new ArgumentOutOfRangeException("PeriodCount must be a number less MaxCount");

         graf = MathFunc.GenerateGraf(grafValue);

         SignalPlot.Plot.Clear();

         SignalPlot.Plot.Add.Scatter(graf.xs, graf.ys);

         SignalPlot.Plot.Axes.SetLimitsX(0, graf.xMax);

         SignalPlot.Plot.Axes.Bottom.Label.Text = $"Время (сек.)";

         SignalPlot.Plot.Axes.Left.Label.Text = "Амплитуда";

         SignalPlot.Plot.Grid.IsVisible = true;

         SignalPlot.Refresh();
      }

      public GrafPage(string fullPath, GrafValue grafValue)
      {
         InitializeComponent();
         DrawGraf(grafValue);

         MathParamText.Text = $"Max / Min / Avg / Zero Cross. : {graf.max:0.#####} / {graf.min:0.#####} / {graf.avg:0.#####} / {graf.zeroCrossing}";
      }

      private void BackButton_Click(object sender, RoutedEventArgs e)
      {
         NavigationService.GoBack();
      }
   }
}
