using System;
using System.Windows;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Unity;

namespace Shell
{
    public class Bootstrapper : UnityBootstrapper
    {
        private const string ModuleCatalogUri = "/Shell;component/ModuleCatalog.xaml";
        
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<Shell>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            Application.Current.RootVisual = (UIElement)this.Shell;
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            return Microsoft.Practices.Prism.Modularity.ModuleCatalog.CreateFromXaml(
                new Uri(ModuleCatalogUri, UriKind.Relative));
        }
    }
}
