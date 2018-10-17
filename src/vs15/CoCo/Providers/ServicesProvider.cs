using System;
using System.ComponentModel.Composition;
using EnvDTE;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Classification;

namespace CoCo.Providers
{
    public sealed class ServicesProvider
    {
        private ServicesProvider()
        {
        }

#pragma warning disable 649

        [Import]
        public IClassificationTypeRegistryService RegistryService { get; private set; }

        [Import]
        public IClassificationFormatMapService FormatMapService { get; private set; }

#pragma warning restore 649

        private static ServicesProvider _instance;

        public static ServicesProvider Instance
        {
            get
            {
                if (_instance != null) return _instance;

                if (ServiceProvider.GlobalProvider.GetService(typeof(SComponentModel)) is IComponentModel componentModel)
                {
                    _instance = new ServicesProvider();
                    componentModel.DefaultCompositionService.SatisfyImportsOnce(_instance);
                    _instance.DTE = ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE)) as _DTE;
                    return _instance;
                }

                // TODO: change to custom exception type?
                throw new InvalidOperationException("Couldn't retrieve IComponentModel service");
            }
        }

        public _DTE DTE { get; private set; }
    }
}