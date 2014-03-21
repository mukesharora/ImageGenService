using System;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using RFIDServiceConfig.ViewModels;
using RFIDServiceConfig.Web.Services;
using RFIDServiceConfig.Views;

namespace RFIDServiceConfig
{
    public class RFIDServiceConfigInit: IModule
    {
        private readonly IRegionManager _regionManager;
        private readonly IUnityContainer _container;

        public RFIDServiceConfigInit(IUnityContainer container, IRegionManager regionManager)
        {
            this._container = container;
            this._regionManager = regionManager;
        }

        public void Initialize()
        {
            _container.RegisterType<RFIDServiceConfigDomainContext>(new InjectionConstructor());
            _container.RegisterType<Object, RFIDReaderView>("RFIDReaderView");
            _container.RegisterType<Object, MainPage>("MainPage");
            _container.RegisterType<Object, RFIDReaderViewModel>("RFIDReaderViewModel");
            _regionManager.RegisterViewWithRegion("MainContentRegion", typeof(MainPage));          
        }
    }
}
