using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Regions;

namespace RFIDServiceConfig
{
    public partial class MainPage : UserControl
    {
        private readonly IRegionManager _regionManager;

        public MainPage(IRegionManager regionManager)
        {
            this._regionManager = regionManager;
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainPage_Loaded);
            InstallButton.Click += new RoutedEventHandler(InstClick);
            Application.Current.InstallStateChanged +=
                new EventHandler(OnInstallStateChanged);
        }

        private void UpdateUserInterface()              
        {
            if (Application.Current.IsRunningOutOfBrowser)
            {
                _regionManager.RequestNavigate(
                    "MainContentRegion",
                    new Uri("RFIDReaderView", UriKind.Relative));
            }
            else
            {
                if (Application.Current.InstallState == InstallState.Installed)
                {
                    // FOR DEBUGGING PURPOSES ONLY
                    // WHEN NOT DEBUGGING COMMENT THIS OUT
                    _regionManager.RequestNavigate(
                    "MainContentRegion",
                    new Uri("RFIDReaderView", UriKind.Relative));

                    // WHEN NOT DEBUGGING UNCOMMENT THIS
                    //IBInstalledExperience.Visibility = Visibility.Visible;
                    //IBNotInstalledExperience.Visibility = Visibility.Collapsed;                    
                }
                else
                {
                    IBInstalledExperience.Visibility = Visibility.Collapsed;
                    IBNotInstalledExperience.Visibility = Visibility.Visible;                    
                }
            }
        }

        private void OnInstallStateChanged(object sender, EventArgs e) 
        {
            UpdateUserInterface();
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateUserInterface();
            CheckForUpdates();
        }
        
        private void InstClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Install();
        }

        private void CheckForUpdates()
        { 
            Application.Current.CheckAndDownloadUpdateCompleted += (s, e) =>
            {
                if (e.UpdateAvailable)
                {
                    MessageBox.Show("A new version was downloaded. Please restart the application");
                }
            };
            Application.Current.CheckAndDownloadUpdateAsync();
        }
    }
}