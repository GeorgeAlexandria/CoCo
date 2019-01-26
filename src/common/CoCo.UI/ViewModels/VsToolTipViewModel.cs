using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace CoCo.UI.ViewModels
{
    public class VsToolTipViewModel : BaseViewModel
    {
        public VsToolTipViewModel(IEnumerable<UIElement> contents)
        {
            if (contents is null) throw new ArgumentNullException(nameof(contents));

            Contents = new ObservableCollection<UIElement>();
            foreach (var content in contents)
            {
                Contents.Add(content);
            }
        }

        public ObservableCollection<UIElement> Contents { get; set; }
    }
}