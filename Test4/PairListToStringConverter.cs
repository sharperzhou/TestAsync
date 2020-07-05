using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Test4
{
    class PairListToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;

            if (!(value is List<KeyValuePair<string, string>> list) || list.Count <= 0)
                return string.Empty;

            return string.Join(", ", list.Select(pair => $"{{{pair.Key}:{pair.Value}}}"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string str) || str.Length <= 0) return null;

            var ret = new List<KeyValuePair<string, string>>();
            foreach (var pair in str.Split(','))
            {
                var keyVal = pair.Trim('{', '}', ' ')
                    .Split(new char[] { ':', ' ' }, 
                    StringSplitOptions.RemoveEmptyEntries);
                if (keyVal.Length < 2) continue;

                ret.Add(new KeyValuePair<string, string>(keyVal[0], keyVal[1]));
            }

            return ret;
        }
    }
}
