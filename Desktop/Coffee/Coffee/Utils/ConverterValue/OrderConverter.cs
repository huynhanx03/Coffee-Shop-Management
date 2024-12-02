using Coffee.DTOs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Coffee.Utils.ConverterValue
{
    public class OrderConverter
    {
    }

    public class OrderStatusColorCoverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string valueConvert = value as string;

            if (valueConvert == Constants.StatusOrder.CANCEL)
                return Application.Current.Resources["RedCF"];

            if (valueConvert == Constants.StatusOrder.WAITTING)
                return Application.Current.Resources["GrayCF"];

            return Application.Current.Resources["GreenCF"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class OrderStatusBoolCoverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string valueConvert = value as string;

            return valueConvert == Constants.StatusOrder.WAITTING;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class OrderPayCoverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool valueConvert = (bool) value;

            return valueConvert == true ? "Đã thanh toán" : "Chưa thanh toán";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
