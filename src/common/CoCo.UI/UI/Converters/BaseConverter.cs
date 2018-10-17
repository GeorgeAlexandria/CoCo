using System;
using System.Windows.Markup;

namespace CoCo.UI.Converters
{
    public abstract class BaseConverter : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}