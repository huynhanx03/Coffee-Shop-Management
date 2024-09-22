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
    public class TableCoverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string valueConvert = value as string;

            if (valueConvert == Constants.StatusTable.FREE)
                return Application.Current.Resources["GreenCF"];
            else
                return Application.Current.Resources["RedCF"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TableBookingCoverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TableDTO valueConvert = value as TableDTO;

            if (valueConvert == null)
                return true;
            else if (valueConvert.TrangThai == Constants.StatusTable.BOOKED)
                return false;
            else
                return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
