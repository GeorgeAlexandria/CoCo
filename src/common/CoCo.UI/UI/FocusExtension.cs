using System;
using System.Windows;
using System.Windows.Input;

namespace CoCo.UI
{
    public static class FocusExtension
    {
        public static readonly DependencyProperty LostFocusCommandProperty =
            DependencyProperty.RegisterAttached(
                "LostFocusCommand",
                typeof(ICommand),
                typeof(FocusExtension),
                new PropertyMetadata(LostFocusCommandChanged));

        public static ICommand GetLostFocusCommand(DependencyObject element) => element is null
            ? throw new ArgumentNullException(nameof(element))
            : (ICommand)element.GetValue(LostFocusCommandProperty);

        public static void SetLostFocusCommand(DependencyObject element, ICommand value)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            element.SetValue(LostFocusCommandProperty, value);
        }

        private static void LostFocusCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element && e.OldValue is null)
            {
                element.LostFocus += OnLostFocus;
            }
        }

        private static void OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.GetValue(LostFocusCommandProperty) is ICommand command)
            {
                command.Execute(null);
            }
        }
    }
}