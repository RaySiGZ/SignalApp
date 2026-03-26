using System;
using SignalApp.Data;


namespace SignalApp.Function
{
   /// <summary>
   /// Содержит методы для генерации сигналов и расчёта их характеристик
   /// </summary>
   internal static class MathFunc
   {
      /// <summary>
      /// Вычисляет значение сигнала в заданной точке
      /// </summary>
      /// <param name="type">Тип сигнала</param>
      /// <param name="angle">Значение угла в радианах</param>
      /// <param name="amplitude">Амплитуда сигнала</param>
      /// <returns>Значение сигнала по оси Y</returns>
      /// <exception cref="NotImplementedException">
      /// Выбрасывается, если для указанного типа сигнала вычисление не реализовано
      /// </exception>
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
      /// Генерирует точки сигнала и вычисляет его основные характеристики
      /// </summary>
      /// <param name="grafValue">Параметры сигнала</param>
      /// <returns>
      /// Кортеж, содержащий массив точек X, максимальное значение X,
      /// массив точек Y, максимум, минимум, среднее значение и количество пересечений нуля
      /// </returns>
      /// <exception cref="ArgumentException">
      /// Выбрасывается, если переданы некорректные параметры сигнала
      /// </exception>
      public static (double[] xs, double xMax, double[] ys, double max, double min, double avg, int zeroCrossing)
         GenerateGraf(GrafValue grafValue)
      {
         if (!Enum.IsDefined(typeof(SignalType), grafValue.Type))
            throw new ArgumentException($"Unsupported signal type: {grafValue.Type}", nameof(grafValue));

         grafValue.Validate();

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