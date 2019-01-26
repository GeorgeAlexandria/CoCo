using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace CoCo.UI.ViewModels
{
    public class VsToolTipViewModel : BaseViewModel
    {
        public VsToolTipViewModel(IEnumerable<UIElement> contents, Color background, Color borderColor)
        {
            if (contents is null) throw new ArgumentNullException(nameof(contents));

            Contents = new ObservableCollection<UIElement>();
            foreach (var content in contents)
            {
                Contents.Add(content);
            }

            Background = background;
            BorderColor = borderColor;
        }

        public ObservableCollection<UIElement> Contents { get; set; }

        public Color Background { get; }

        public Color BorderColor { get; }
    }
}