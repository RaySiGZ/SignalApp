using SignalApp.Data;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace SignalApp.Function
{
   internal static class SaveGraf
   {
      public static bool SaveGrafCSV(string fullPath, GrafValue grafValue,(double[] xs, double xMax, double[] ys, double max, double min, double avg, int zeroCrossing) graf)
      {
         if (!Enum.IsDefined(typeof(SignalType), grafValue.Type) || !grafValue.Validate())
            throw new ArgumentException("Incorrect signal type");

         if (string.IsNullOrWhiteSpace(fullPath))
            throw new ArgumentException("File path cannot be empty.", nameof(fullPath));

         if (graf.xs == null)
            throw new ArgumentNullException(nameof(graf.xs));

         if (graf.ys == null)
            throw new ArgumentNullException(nameof(graf.ys));

         if (graf.xs.Length != graf.ys.Length)
            throw new ArgumentException("X and Y arrays must have the same length.");

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
