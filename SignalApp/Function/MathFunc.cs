using System;
using SignalApp.Data;


namespace SignalApp.Function
{
   /// <summary>
   /// Класс с функциями для обработки математических функций
   /// </summary>
   internal static class MathFunc
   {
      /// <summary>
      /// Получение значения точки 
      /// </summary>
      /// <param name="type">Тип функции</param>
      /// <param name="angle">Угол</param>
      /// <param name="amplitude">Амплитуда</param>
      /// <returns></returns>
      /// <exception cref="NotImplementedException"></exception>
      private static double Graf_YValue(SignalType type, double angle, double amplitude)
      {
         double sin = Math.Sin(angle);

         switch (type)
         {
            case SignalType.Sine:
               return sin * amplitude;
            case SignalType.Square:
               return (sin >= 0 ? amplitude : -amplitude);
            case SignalType.Triangle:
               return (2 * amplitude / Math.PI) * Math.Asin(Math.Sin(angle));
            case SignalType.Sawtooth:
               {
                  double normalized = angle / (2 * Math.PI);
                  return 2 * amplitude * (normalized - Math.Floor(normalized + 0.5));
               }
            default:
               throw new NotImplementedException();
         }
      }

      /// <summary>
      /// Generates signal data and calculates its basic characteristics.
      /// </summary>
      /// <param name="grafValue">Type of the signal (Sine or Square).</param>
      /// <returns>
      /// A tuple (double[] xs, double xMax, double[] ys, double max, double min, double avg, int zeroCrossings)
      /// </returns>
      public static (double[] xs, double xMax, double[] ys, double max, double min, double avg, int zeroCrossing)
         GenerateGraf(GrafValue grafValue)
      {
         if (!Enum.IsDefined(typeof(SignalType), grafValue.Type) || !grafValue.Validate())
            throw new ArgumentException("Incorrect signal type");

         double[] xs = new double[grafValue.MaxCount],
            ys = new double[grafValue.MaxCount];

         double dx = grafValue.PeriodCount.HasValue ?
            1.0 / (grafValue.Frequency * grafValue.PeriodCount.Value) :
            1.0 / grafValue.MaxCount;

         double staticParam = 2 * Math.PI * grafValue.Frequency * dx;

         double angle = 0;
         xs[0] = 0;

         double y = Graf_YValue(grafValue.Type, angle, grafValue.Amplitude);

         ys[0] = y;

         double max = y,
            min = y,
            avgSum = y,
            prev = y;

         // Изменить на 1, если нужно учесть точку (0;0)
         int zeroCrossings = 0;

         for (int step = 1; step < grafValue.MaxCount; step++)
         {
            angle = step * staticParam;

            xs[step] =  dx * step;

            y = Graf_YValue(grafValue.Type, angle, grafValue.Amplitude);

            ys[step] = y;

            if (max < y) max = y;
            if (min > y) min = y;
            avgSum += y;

            // Чтобы учитывать (0; 0) - (prev <= 0 && y >= 0) || (prev > 0 && y <= 0)
            // Чтобы не учитывать (0; 0) - (prev < 0 && y >= 0) || (prev >= 0 && y <= 0)

            if ((prev < 0 && y >= 0) || (prev >= 0 && y <= 0))
               zeroCrossings++;

            prev = y;
         }

         return (
            xs,
            dx * grafValue.MaxCount,
            ys,
            max,
            min,
            avgSum / grafValue.MaxCount,
            zeroCrossings);
      }
   }
}