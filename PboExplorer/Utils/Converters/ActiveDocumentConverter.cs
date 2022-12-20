using System;
using System.Windows.Data;
using System.Windows.Markup;
using PboExplorer.ViewModels;

namespace PboExplorer.Utils.Converters;

class ActiveDocumentConverter :MarkupExtension, IValueConverter 
{
    private static ActiveDocumentConverter? _instance;

    public object Convert(object value, Type targetType, object parameter,
                          System.Globalization.CultureInfo culture)
    {
        if (value is EntryViewModel)
            return value;

        return Binding.DoNothing;
    }

    public object ConvertBack(object value, Type targetType, object parameter,
                              System.Globalization.CultureInfo culture)
    {
        if (value is EntryViewModel)
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
