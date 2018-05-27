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
    /// Match color to the log message type.
    /// </summary>
    class MesTypeToColor : IValueConverter
    {
        /// <summary>
        /// Gets the current type and return it's matching color.
        /// </summary>
        /// <param name="value"> The message type. </param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns> The matching color - green for info, yellow for warning and red for error. </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Brush))
                throw new InvalidOperationException("Must convert to a brush!");
            string type = (string)value;
            if (type == "INFO")
                return Brushes.LightGreen;
            else if (type == "WARNING")
                return Brushes.Yellow;
            else if (type == "ERROR")
                return Brushes.LightCoral;
            else
                return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
