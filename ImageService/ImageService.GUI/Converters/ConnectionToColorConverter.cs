 using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace ImageService.GUI.Converters
{
    /// <summary>
    /// Match color to the connection status
    /// </summary>
    class ConnectionToColorConverter : IValueConverter
    {
        /// <summary>
        /// Checks the connection vaule - if it is connected, the color is light blue, if it is disconnected the color is gray.
        /// </summary>
        /// <param name="value"> Is the client connected to the server. </param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns> The color according the status. </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Brush))
                throw new InvalidOperationException("Must convert to a brush!");

            bool conneted = (bool)value;
            if (conneted)
                return Brushes.LightBlue;
            else
                return Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
