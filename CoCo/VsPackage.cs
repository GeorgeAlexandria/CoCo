using System;
using System.Runtime.InteropServices;
using System.Windows;
using CoCo.UI;
using CoCo.UI.ViewModels;
using Microsoft.VisualStudio.Shell;

namespace CoCo
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading =true)]
    [ProvideOptionPage(typeof(DialogOption), "CoCo", "CoCo", 0, 0, true)]
    [Guid("b933474d-306e-434f-952d-a820c849ed07")]
    public sealed class VsPackage : Package
    {
    }

    public class DialogOption : UIElementDialogPage
    {
        private OptionViewModel _view;

        private OptionControl _child;

        protected override UIElement Child
        {
            get
            {
                if (_child != null) return _child;

                _view = new OptionViewModel(new OptionProvider());
                _child = new OptionControl
                {
                    DataContext = _view
                };
                return _child;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _view.SaveOption();
            base.OnClosed(e);
        }
    }
}