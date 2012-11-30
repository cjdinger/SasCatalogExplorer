using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Controls;

namespace SAS.Tasks.Examples.CatalogExplorer
{
    /// <summary>
    /// This little class translates the catalog entry object type into the
    /// name/location of the appropriate image to show in the catalog
    /// explorer.
    /// </summary>
    public class EntryImageConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// The value object will be the text name of the object entry type.
        /// The purpose of this Convert routine is to map that entry type into
        /// a resource location, pointing to one of the resource images that
        /// are supplied within this project.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string objType = value.ToString().ToLower();
            switch (objType)
            {
                case "class":
                case "frame":
                case "scl":
                case "slist":
                case "wsave":
                case "menu":
                    return string.Format("resources\\{0}.ico", objType);
                case "source":
                    return "resources\\sascode.ico";
                case "pmenu":
                    return "resources\\menu.ico";
                default:
                    return "resources\\entry.ico";
            }
        }

        /// <summary>
        /// No action taken by the ConvertBack method
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        #endregion
    }
}
