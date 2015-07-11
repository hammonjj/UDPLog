using System;
using System.Windows.Data;
using System.Globalization;

namespace UPDLog.Utilities
{

    [ValueConversion(typeof(object), typeof(int))]
    public class SeverityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)System.Convert.ChangeType(value, typeof(string));
#if false
            switch(severity)
            {
                case "Error":
                    break;
                case "Warning":
                    break;
                case "Notice":
                    break;
                case "Info":
                    break;
                case "Debug":
                    break;
                default:
                    break;
            }

            return "";
#endif
#if false
            if (number < 0.0)
                return -1;

            if (number == 0.0)
                return 0;

            return +1;
#endif
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("ConvertBack not supported");
        }
    }
}
