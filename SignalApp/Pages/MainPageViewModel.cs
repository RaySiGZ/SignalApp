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
      private string _countMax;
      private string _countPeriod;

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
      public string CountMax
      {
         get => _countMax;
         set
         {
            _countMax = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CountPeriod));
         }
      }

      /// <summary>
      /// Количество точек в одном периоде (необязательное поле)
      /// </summary>
      public string CountPeriod
      {
         get => _countPeriod;
         set
         {
            _countPeriod = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CountMax));
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

               case nameof(CountMax):
                  if (!string.IsNullOrWhiteSpace(CountMax) && (!int.TryParse(CountMax, out int maxCount) || maxCount < 100 || maxCount > 10000))
                     return "Количество точек должно быть в диапазоне от 100 до 10000";
                  break;

               case nameof(CountPeriod):
                  if (string.IsNullOrWhiteSpace(CountPeriod))
                     return null;
                  else if (!string.IsNullOrWhiteSpace(CountPeriod) && (!int.TryParse(CountPeriod, out int periodCount) || periodCount < 20))
                     return "Количество точек в периоде должно быть не меньше 20";
                  break;
            }

            if (columnName == nameof(CountMax) || columnName == nameof(CountPeriod))
            {
               bool maxOk = int.TryParse(CountMax, out int maxCount);
               bool periodOk = int.TryParse(CountPeriod, out int periodCount);

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
             this[nameof(CountMax)] != null ||
             this[nameof(CountPeriod)] != null;
      }

      /// <summary>
      /// Пытается преобразовать введённые значения в типизированные параметры.
      /// </summary>
      /// <param name="amplitude">Распарсенная амплитуда.</param>
      /// <param name="frequency">Распарсенная частота.</param>
      /// <param name="countMax">Распарсенное общее количество точек.</param>
      /// <param name="countPeriod">Распарсенное количество точек на период или null.</param>
      /// <returns>true, если все обязательные значения успешно распарсены; иначе false.</returns>
      public bool TryGetParsedValues(
          out double amplitude,
          out double frequency,
          out int countMax,
          out int? countPeriod)
      {
         amplitude = 0;
         frequency = 0;
         countMax = 0;
         countPeriod = null;

         if (HasErrors())
            return false;

         bool periodOk = true;

         if (!string.IsNullOrWhiteSpace(CountPeriod))
            if (int.TryParse(CountPeriod, out int parsedPeriod))
               countPeriod = parsedPeriod;
            else
               periodOk = false;

         return
             double.TryParse(Amplitude, out amplitude) &&
             double.TryParse(Frequency, out frequency) &&
             int.TryParse(CountMax, out countMax) &&
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