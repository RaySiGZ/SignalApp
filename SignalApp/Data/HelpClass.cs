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
   public static class EnumHelper
   {
      public static string GetDisplayName(Enum value)
      {
         var attr = GetDisplay(value);
         return attr?.Name ?? value.ToString();
      }

      public static string GetShortName(Enum value)
      {
         var attr = GetDisplay(value);
         return attr?.ShortName ?? value.ToString().ToLower();
      }

      private static DisplayAttribute GetDisplay(Enum value)
      {
         var field = value.GetType().GetField(value.ToString());
         return field?.GetCustomAttribute<DisplayAttribute>();
      }
   }

   /// <summary>
   /// Enum для типа сигнала
   /// </summary>
   public enum SignalType
   {
      /// <summary>
      /// Синусоида
      /// </summary>
      [Display(Name = "Синус", ShortName = "sin")]
      Sine,

      /// <summary>
      /// Меандр
      /// </summary>
      [Display(Name = "Меандр", ShortName = "sq")]
      Square,

      /// <summary>
      /// Треугольная волна
      /// </summary>
      [Display(Name = "Треугольник", ShortName = "tri")]
      Triangle,

      /// <summary>
      /// Пилообразный график
      /// </summary>
      [Display(Name = "Пилообразный", ShortName = "saw")]
      Sawtooth
   }

   /// <summary>
   /// Класс для хранения параметров графика
   /// </summary>
   public class GrafValue
   {
      /// <summary>
      /// Тип графика
      /// </summary>
      public SignalType Type { get; }

      /// <summary>
      /// Амплитуда графика
      /// </summary>
      public double Amplitude { get; }

      /// <summary>
      /// Частота графика
      /// </summary>
      public double Frequency { get; }

      /// <summary>
      /// Количество точек всего
      /// </summary>
      public int MaxCount { get; }

      /// <summary>
      /// Количество точек в одном периоде
      /// </summary>
      public int? PeriodCount { get; }

      /// <summary>
      /// Конструктор класса
      /// </summary>
      public GrafValue(
         SignalType type,
         double amplitude,
         double frequency,
         int maxCount,
         int? periodCount)
      {
         Type = type;
         Amplitude = amplitude;
         Frequency = frequency;
         MaxCount = maxCount;
         PeriodCount = periodCount;
      }
   }
}
