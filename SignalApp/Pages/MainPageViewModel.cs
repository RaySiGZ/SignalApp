using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SignalApp.Pages
{
   /// <summary>
   /// VievModel для MainPage
   /// Также содержит валидацию данных и метод для получения распарсенных значений
   /// </summary>
   public class MainPageViewModel : INotifyPropertyChanged, IDataErrorInfo
   {
      private string _frequency;
      private string _amplitude;
      private string _maxCount;
      private string _periodCount;

      /// <summary>
      /// Частота
      /// </summary>
      public string Frequency
      {
         get => _frequency;
         set
         {
            _frequency = value;
            OnPropertyChanged();
         }
      }

      /// <summary>
      /// Амплитуда
      /// </summary>
      public string Amplitude
      {
         get => _amplitude;
         set
         {
            _amplitude = value;
            OnPropertyChanged();
         }
      }

      /// <summary>
      /// Общее количество точек
      /// </summary>
      public string MaxCount
      {
         get => _maxCount;
         set
         {
            _maxCount = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(PeriodCount));
         }
      }

      /// <summary>
      /// Количество точек в одном периоде (необязательное поле)
      /// </summary>
      public string PeriodCount
      {
         get => _periodCount;
         set
         {
            _periodCount = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(MaxCount));
         }
      }

      public string Error => null;

      /// <summary>
      /// Возвращает сообщение об ошибке валидации для указанного свойства.
      /// </summary>
      /// <param name="columnName">Имя свойства, для которого выполняется валидация.</param>
      /// <returns>Текст ошибки или null, если ошибок нет.</returns>
      public string this[string columnName]
      {
         get
         {
            switch (columnName)
            {
               case nameof(Frequency):
                  if (!string.IsNullOrWhiteSpace(Frequency) && (!double.TryParse(Frequency, out double frequency) || frequency <= 0))
                     return "Частота должна быть числом, больше 0";
                  break;

               case nameof(Amplitude):
                  if (!string.IsNullOrWhiteSpace(Amplitude) && (!double.TryParse(Amplitude, out double amplitude) || amplitude <= 0))
                     return "Амплитуда должна быть числом, больше 0";
                  break;

               case nameof(MaxCount):
                  if (!string.IsNullOrWhiteSpace(MaxCount) && (!int.TryParse(MaxCount, out int maxCount) || maxCount < 100 || maxCount > 10000))
                     return "Количество точек должно быть в диапазоне от 100 до 10000";
                  break;

               case nameof(PeriodCount):
                  if (string.IsNullOrWhiteSpace(PeriodCount))
                     return null;
                  else if (!string.IsNullOrWhiteSpace(PeriodCount) && (!int.TryParse(PeriodCount, out int periodCount) || periodCount < 20))
                     return "Количество точек в периоде должно быть не меньше 20";
                  break;
            }

            if (columnName == nameof(MaxCount) || columnName == nameof(PeriodCount))
            {
               bool maxOk = int.TryParse(MaxCount, out int maxCount);
               bool periodOk = int.TryParse(PeriodCount, out int periodCount);

               if (maxOk && periodOk && maxCount < periodCount)
                  return "Общее количество точек должно быть не меньше количества точек в периоде";
            }

            return null;
         }
      }

      public bool HasErrors()
      {
         return
             this[nameof(Frequency)] != null ||
             this[nameof(Amplitude)] != null ||
             this[nameof(MaxCount)] != null ||
             this[nameof(PeriodCount)] != null;
      }

      /// <summary>
      /// Пытается преобразовать введённые значения в типизированные параметры.
      /// </summary>
      /// <param name="amplitude">Распарсенная амплитуда.</param>
      /// <param name="frequency">Распарсенная частота.</param>
      /// <param name="maxCount">Распарсенное общее количество точек.</param>
      /// <param name="periodCount">Распарсенное количество точек на период или null.</param>
      /// <returns>true, если все обязательные значения успешно распарсены; иначе false.</returns>
      public bool TryGetParsedValues(
          out double amplitude,
          out double frequency,
          out int maxCount,
          out int? periodCount)
      {
         amplitude = 0;
         frequency = 0;
         maxCount = 0;
         periodCount = null;

         if (HasErrors())
            return false;

         bool periodOk = true;

         if (!string.IsNullOrWhiteSpace(PeriodCount))
            if (int.TryParse(PeriodCount, out int parsedPeriod))
               periodCount = parsedPeriod;
            else
               periodOk = false;

         return
             double.TryParse(Amplitude, out amplitude) &&
             double.TryParse(Frequency, out frequency) &&
             int.TryParse(MaxCount, out maxCount) &&
             periodOk;
      }

      /// <summary>
      /// Событие, возникающее при изменении значения свойства.
      /// </summary>
      public event PropertyChangedEventHandler PropertyChanged;

      /// <summary>
      /// Вызывает событие PropertyChanged для указанного свойства.
      /// </summary>
      /// <param name="propertyName">Имя изменившегося свойства.</param>
      private void OnPropertyChanged([CallerMemberName] string propertyName = null)
      {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }
   }
}