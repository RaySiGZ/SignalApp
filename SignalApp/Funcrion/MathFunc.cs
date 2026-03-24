using System;

namespace SignalApp.Function
{
   public enum SignalType
   {
      Sine,
      Square
   }

   internal static class MathFunc
   {
      /// <summary>
      /// Generates signal data and calculates its basic characteristics.
      /// </summary>
      /// <param name="signalType">Type of the signal (Sine or Square).</param>
      /// <param name="amplitude">Signal amplitude (must be greater than 0).</param>
      /// <param name="frequency">Signal frequency in Hz (must be greater than 0).</param>
      /// <param name="maxCount">Total number of points (from 100 to 10000).</param>
      /// <param name="periodCount">Optional number of points per period.</param>
      /// <returns>
      /// A tuple (double[] xs, double xMax, double[] ys, double max, double min, double avg, int zeroCrossings)
      /// </returns>
      public static (double[] xs, double xMax, double[] ys, double max, double min, double avg, int zeroCrossing) GenerateGraf(SignalType signalType,
         double amplitude, double frequency, int maxCount, int? periodCount)
      {
         if (!Enum.IsDefined(typeof(SignalType), signalType))
            throw new ArgumentException("Incorrect signal type");

         if (amplitude <= 0)
            throw new ArgumentOutOfRangeException("Amplitude must be a positive number");

         if (frequency <= 0)
            throw new ArgumentOutOfRangeException("Frequency must be a positive number");

         if (maxCount > 10000 || maxCount < 100)
            throw new ArgumentOutOfRangeException("MaxCount must be a number between 100 and 10000");

         if (!(periodCount is null))
         {
            if (periodCount < 20)
               throw new ArgumentOutOfRangeException("PeriodCount must be a number greater than 20");

            if (periodCount > maxCount)
               throw new ArgumentOutOfRangeException("PeriodCount must be a number less MaxCount");
         }

         double[] xs = new double[maxCount],
            ys = new double[maxCount];

         double dx = periodCount.HasValue ?
            1.0 / (frequency * periodCount.Value) :
            1.0 / maxCount;

         double staticParam = 2 * Math.PI * frequency * dx;

         double angle = 0;
         xs[0] = 0;

         double sin = Math.Sin(angle);
         double y = signalType == SignalType.Sine
             ? sin * amplitude
             : sin >= 0 ? amplitude : -amplitude;

         ys[0] = y;

         double max = y,
            min = y,
            avgSum = y,
            prev = y;
         int zeroCrossings = 0;

         for (int step = 1; step < maxCount; step++)
         {
            angle = step * staticParam;

            xs[step] = dx * step;

            sin = Math.Sin(angle);

            y = signalType == SignalType.Sine ?
               sin * amplitude :
               sin >= 0 ?
                  amplitude :
                  -amplitude;

            ys[step] = y;

            if (max < y) max = y;
            if (min > y) min = y;
            avgSum += y;

            if ((prev <= 0 && y >= 0) || (prev >= 0 && y <= 0))
               zeroCrossings++;

            prev = y;
         }

         return (
            xs,
            dx * maxCount,
            ys,
            max,
            min,
            avgSum / maxCount,
            zeroCrossings);
      }
   }
}