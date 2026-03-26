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
      /// <summary>
      /// Рассчитанные точки и параметры текущего графика.
      /// </summary>
      private (double[] xs, double xMax, double[] ys, double max, double min, double avg, int zeroCrossing) graf;

      /// <summary>
      /// Проверяет параметры сигнала, генерирует точки графика и отображает график на элементе SignalPlot.
      /// </summary>
      /// <param name="grafValue">Параметры сигнала для построения графика.</param>
      /// <exception cref="ArgumentException">
      /// Выбрасывается, если переданы некорректные параметры сигнала.
      /// </exception>
      private void DrawGraf(GrafValue grafValue)
      {
         if (!Enum.IsDefined(typeof(SignalType), grafValue.Type))
            throw new ArgumentException($"Unsupported signal type: {grafValue.Type}", nameof(grafValue));

         grafValue.Validate();

         graf = MathFunc.GenerateGraf(grafValue);

         SignalPlot.Plot.Clear();

         SignalPlot.Plot.Add.Scatter(graf.xs, graf.ys);

         SignalPlot.Plot.Axes.SetLimitsX(0, graf.xMax);

         SignalPlot.Plot.Axes.Bottom.Label.Text = $"Время (сек.)";

         SignalPlot.Plot.Axes.Left.Label.Text = "Амплитуда";

         SignalPlot.Plot.Grid.IsVisible = true;

         SignalPlot.Refresh();
      }

      /// <summary>
      /// Инициализирует страницу отображения графика и выводит его параметры.
      /// </summary>
      /// <param name="fullPath">Полный путь к файлу графика.</param>
      /// <param name="grafValue">Параметры сигнала для построения графика.</param>
      public GrafPage(string fullPath, GrafValue grafValue)
      {
         InitializeComponent();
         DrawGraf(grafValue);

         MathParamText.Text = $"Max / Min / Avg / Zero Cross. : {graf.max:0.#####} / {graf.min:0.#####} / {graf.avg:0.#####} / {graf.zeroCrossing}";
      }

      /// <summary>
      /// Обрабатывает нажатие кнопки возврата на предыдущую страницу.
      /// </summary>
      private void BackButton_Click(object sender, RoutedEventArgs e)
      {
         NavigationService.GoBack();
      }
   }
}
