using System.Globalization;

namespace MauiAppMinhasCompras.Helpers
{
    public class DecimalBrasilConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal dec)
                return dec.ToString("N2", new CultureInfo("pt-BR"));

            return value?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                str = str.Replace(".", ",");

                if (decimal.TryParse(str, NumberStyles.Any, new CultureInfo("pt-BR"), out var result))
                    return result;
            }

            return 0m;
        }
    }
}