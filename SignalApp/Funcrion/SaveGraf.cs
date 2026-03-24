using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace SignalApp.Funcrion
{
   internal static class SaveGraf
   {
      public static bool SaveGrafCSV(string fullPath, (double[] xs, double xMax, double[] ys, double max, double min, double avg, int zeroCrossing) graf)
      {
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

            //writer.WriteLine($"Max;{graf.max.ToString("0.#####", CultureInfo.InvariantCulture)}");
            //writer.WriteLine($"Min;{graf.min.ToString("0.#####", CultureInfo.InvariantCulture)}");
            //writer.WriteLine($"Avg;{graf.avg.ToString("0.#####", CultureInfo.InvariantCulture)}");
            //writer.WriteLine($"ZeroCross;{graf.zeroCrossing}");

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
