using System;
using System.Windows.Controls;

namespace MetroPlurk.Helpers
{
    public static class TextBoxExtensions
    {
        public static void ForceBinding(this TextBox textBox)
        {
            var binding = textBox.GetBindingExpression(TextBox.TextProperty);
            if (binding != null) { binding.UpdateSource(); }
        }
    }
}
