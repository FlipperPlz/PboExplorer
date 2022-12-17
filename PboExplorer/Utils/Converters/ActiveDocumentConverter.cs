using PboExplorer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace PboExplorer.Utils.Converters;

class ActiveDocumentConverter :MarkupExtension, IValueConverter 
{
    private static ActiveDocumentConverter? _instance;

    public object Convert(object value, Type targetType, object parameter,
                          System.Globalization.CultureInfo culture)
    {
        //TODO: Replace TextEntry with common base class for entry documents
        if (value is TextEntry)
            return value;

        return Binding.DoNothing;
    }

    public object ConvertBack(object value, Type targetType, object parameter,
                              System.Globalization.CultureInfo culture)
    {
        //TODO: Replace TextEntry with common base class for entry documents
        if (value is TextEntry)
            return value;

        return Binding.DoNothing;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if(_instance is null) {
            _instance = new ActiveDocumentConverter();
        }
        return _instance;
    }
}
