using SignalApp.Data;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace SignalApp.Function
{
   /// <summary>
   /// Содержит методы для сохранения графиков в файл
   /// </summary>
   internal static class SaveGraf
   {
      /// <summary>
      /// Сохраняет параметры сигнала и точки графика в CSV-файл
      /// </summary>
      /// <param name="fullPath">Полный путь к сохраняемому файлу</param>
      /// <param name="grafValue">Параметры сигнала</param>
      /// <param name="graf">Рассчитанные точки и параметры графика</param>
      /// <returns>true, если файл успешно сохранён; иначе false</returns>
      /// <exception cref="ArgumentException">
      /// Выбрасывается, если путь к файлу некорректен, тип сигнала недопустим
      /// или массивы точек имеют разную длину
      /// </exception>
      /// <exception cref="ArgumentNullException">
      /// Выбрасывается, если массив точек X или Y равен null
      /// </exception>
      public static bool SaveGrafCSV(string fullPath, GrafValue grafValue,(double[] xs, double xMax, double[] ys, double max, double min, double avg, int zeroCrossing) graf)
      {
         if (!Enum.IsDefined(typeof(SignalType), grafValue.Type)) 
            throw new ArgumentException($"Unsupported signal type: {grafValue.Type}", nameof(grafValue));

         grafValue.Validate();

         if (string.IsNullOrWhiteSpace(fullPath))
            throw new ArgumentException("File path must not be null or whitespace.", nameof(fullPath));

         if (graf.xs == null)
            throw new ArgumentNullException(nameof(graf.xs));

         if (graf.ys == null)
            throw new ArgumentNullException(nameof(graf.ys));

         if (graf.xs.Length != graf.ys.Length)
            throw new ArgumentException($"X and Y arrays must have the same length. X: {graf.xs.Length}, Y: {graf.ys.Length}.", nameof(graf));

         try
         {
            string directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
               Directory.CreateDirectory(directory);

            StreamWriter writer = new StreamWriter(fullPath, false, Encoding.UTF8);

            var periodText = grafValue.PeriodCount.HasValue ? grafValue.PeriodCount.Value.ToString() : "N/A";
            writer.WriteLine($"Graf Param. Type - {grafValue.Type}, A - {grafValue.Amplitude}, F - {grafValue.Frequency}, Max - {grafValue.MaxCount}, Period - {periodText}");

            writer.WriteLine("X;Y");

            for (int i = 0; i < graf.xs.Length; i++)
            {
               writer.WriteLine(
                   $"{graf.xs[i].ToString("0.#####", CultureInfo.InvariantCulture)};{graf.ys[i].ToString("0.#####", CultureInfo.InvariantCulture)}");
            }

            writer.Close();

            return true;
         }
         catch
         {
            return false;
         }
      }
   }
}
