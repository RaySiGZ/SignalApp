using SignalApp.Function;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SignalApp.Data
{
   /// <summary>
   /// Вспомогательный класс для получения отображаемых имен и коротких обозначений значений перечислений.
   /// </summary>
   public static class EnumHelper
   {
      /// <summary>
      /// Возвращает отображаемое имя значения перечисления из атрибута Display
      /// Если атрибут отсутствует, возвращает имя значения перечисления
      /// </summary>
      /// <param name="value">Значение перечисления</param>
      /// <returns>Отображаемое имя значения перечисления</returns>
      public static string GetDisplayName(Enum value)
      {
         var attr = GetDisplay(value);
         return attr?.Name ?? value.ToString();
      }

      /// <summary>
      /// Возвращает короткое имя значения перечисления из атрибута Display
      /// Если атрибут отсутствует, возвращает имя значения в нижнем регистре
      /// </summary>
      /// <param name="value">Значение перечисления</param>
      /// <returns>Короткое имя значения перечисления</returns>
      public static string GetShortName(Enum value)
      {
         var attr = GetDisplay(value);
         return attr?.ShortName ?? value.ToString().ToLower();
      }

      /// <summary>
      /// Возвращает атрибут Display для указанного значения перечисления
      /// </summary>
      /// <param name="value">Значение перечисления</param>
      /// <returns>Атрибут Display или null, если он отсутствует</returns>
      private static DisplayAttribute GetDisplay(Enum value)
      {
         var field = value.GetType().GetField(value.ToString());
         return field?.GetCustomAttribute<DisplayAttribute>();
      }
   }

   /// <summary>
   /// Перечисление доступных типов сигналов.
   /// </summary>
   public enum SignalType
   {
      /// <summary>
      /// Синус
      /// </summary>
      [Display(Name = "Синус", ShortName = "sin")]
      Sine,

      /// <summary>
      /// Меандр
      /// </summary>
      [Display(Name = "Меандр", ShortName = "sq")]
      Square,

      /// <summary>
      /// Треугольный сигнал
      /// </summary>
      [Display(Name = "Треугольник", ShortName = "tri")]
      Triangle,

      /// <summary>
      /// Пилообразный сигнал
      /// </summary>
      [Display(Name = "Пилообразный", ShortName = "saw")]
      Sawtooth
   }

   /// <summary>
   /// Содержит параметры сигнала, необходимые для генерации и сохранения графика.
   /// </summary>
   public class GrafValue
   {
      /// <summary>
      /// Тип сигнала
      /// </summary>
      public SignalType Type { get; }

      /// <summary>
      /// Амплитуда сигнала
      /// </summary>
      public double Amplitude { get; }

      /// <summary>
      /// Общее количество точек сигнала
      /// </summary>
      public double Frequency { get; }

      /// <summary>
      /// Количество точек в одном периоде сигнала
      /// </summary>
      public int CountMax { get; }

      /// <summary>
      /// Количество точек в одном периоде
      /// </summary>
      public int? CountPeriod { get; }

      /// <summary>
      /// Проверяет корректность параметров сигнала.
      /// </summary>
      /// <returns>true, если параметры корректны.</returns>
      /// <exception cref="ArgumentOutOfRangeException">
      /// Выбрасывается, если одно из значений выходит за допустимый диапазон.
      /// </exception>
      public bool Validate()
      {
         if (Amplitude <= 0)
            throw new ArgumentOutOfRangeException(nameof(Amplitude), Amplitude, "Amplitude must be greater than 0.");

         if (Frequency <= 0)
            throw new ArgumentOutOfRangeException(nameof(Frequency), Frequency, "Frequency must be greater than 0.");

         if (CountMax < 100 || CountMax > 10000)
            throw new ArgumentOutOfRangeException(nameof(CountMax), CountMax, "MaxCount must be in range [100, 10000].");

         if (CountPeriod != null)
         {
            if (CountPeriod < 20)
               throw new ArgumentOutOfRangeException(nameof(CountPeriod), CountPeriod, "PeriodCount must be greater than or equal to 20.");

            if (CountPeriod > CountMax)
               throw new ArgumentOutOfRangeException(nameof(CountPeriod), CountPeriod, "PeriodCount must not exceed MaxCount.");
         }

         return true;
      }

      /// <summary>
      /// Инициализирует параметры сигнала.
      /// </summary>
      /// <param name="type">Тип сигнала.</param>
      /// <param name="amplitude">Амплитуда сигнала.</param>
      /// <param name="frequency">Частота сигнала.</param>
      /// <param name="countMax">Общее количество точек сигнала.</param>
      /// <param name="countPeriod">Количество точек в одном периоде сигнала.</param>
      public GrafValue(
         SignalType type,
         double amplitude,
         double frequency,
         int countMax,
         int? countPeriod)
      {
         Type = type;
         Amplitude = amplitude;
         Frequency = frequency;
         CountMax = countMax;
         CountPeriod = countPeriod;
      }
   }
}
