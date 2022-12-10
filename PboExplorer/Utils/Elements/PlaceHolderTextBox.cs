using System;
using System.Drawing;
using System.Windows.Controls;

namespace PboExplorer.Utils.Elements; 

public class PlaceHolderTextBox : TextBox
{
    private bool isPlaceHolder = true;
    private string _placeHolderText;
    public string PlaceHolderText
    {
        get => _placeHolderText;
        set
        {
            _placeHolderText = value;
            setPlaceholder();
        }
    }

    public new string Text
    {
        get => isPlaceHolder ? string.Empty : base.Text;
        set => base.Text = value;
    }

    //when the control loses focus, the placeholder is shown
    private void setPlaceholder() {
        if (!string.IsNullOrEmpty(base.Text)) return;
        base.Text = PlaceHolderText;
        isPlaceHolder = true;
    }

    //when the control is focused, the placeholder is removed
    private void removePlaceHolder() {
        if (!isPlaceHolder) return;
        base.Text = "";
        isPlaceHolder = false;
    }
    public PlaceHolderTextBox()
    {
        GotFocus += removePlaceHolder;
        LostFocus += setPlaceholder;
    }

    private void setPlaceholder(object sender, EventArgs e)
    {
        setPlaceholder();
    }

    private void removePlaceHolder(object sender, EventArgs e)
    {
        removePlaceHolder();
    }
}