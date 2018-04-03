using System;
using System.Windows.Controls;

namespace ConsumerTestRail.Application.Helpers
{
	public class ScrollTextBox : TextBox {

    protected override void OnInitialized (EventArgs e) {
        base.OnInitialized(e);
        VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
        HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
    }

    protected override void OnTextChanged (TextChangedEventArgs e) {
        base.OnTextChanged(e);
        CaretIndex = Text.Length;
        ScrollToEnd();
    }

}
}
