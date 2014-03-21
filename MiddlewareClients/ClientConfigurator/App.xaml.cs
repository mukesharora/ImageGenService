using ClientConfigurator.Utility;
using ClientConfigurator.Views;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;

namespace ClientConfigurator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const string LOCAL_HOST = "localhost";

        private static Logger logger = LogManager.GetCurrentClassLogger();

        private Mutex _instanceMutex = null;

        bool _isStartingUp = false;

        private Window _wpfBugWindow = null;

        public App()
        {
            LogAppVersion();
            
            _isStartingUp = true;

            this.Startup += App_Startup;

            _instanceMutex = new Mutex(false, "Global\\" + ClientConfigConstants.AppGUID);
            {
                if (!_instanceMutex.WaitOne(TimeSpan.Zero, true))
                {
                    logger.Info("Terminating. Another instance is running.");
                    Application.Current.Shutdown();
                    return;
                }
            }
        }

        #region Private methods

        private void CreateWpfBugWindow()
        {
            // WPF Bug Workaround: while we have no WPF window open we can't show MessageBox.
            _wpfBugWindow = new Window()
            {
                AllowsTransparency = true,
                Background = System.Windows.Media.Brushes.Transparent,
                WindowStyle = WindowStyle.None,
                Top = 0,
                Left = 0,
                Width = 1,
                Height = 1,
                ShowInTaskbar = false
            };

            _wpfBugWindow.Show();

        }

        private void CloseWpfBugWindow()
        {
            if (_wpfBugWindow != null)
            {
                _wpfBugWindow.Close();
                _wpfBugWindow = null;
            }
        }

        #endregion


        #region base class methods

        protected override void OnExit(ExitEventArgs e)
        {           
            DispatcherUnhandledException -= App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;       
        }

        #endregion

        #region Event handlers

        void App_Startup(object sender, StartupEventArgs e)
        {
            // Catch unhandled exceptions for logging....
            //
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            try
            {
                CreateWpfBugWindow();
                var window = new ClientConfigWindow();
                window.Show();
            }
            catch (Exception ex)
            {
                
                logger.ErrorException("Unhandled App_Startup", ex);               

                MessageBox.Show("Error starting application\n\n" + ex,
                    "Client Configurator", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
                
            }

            CloseWpfBugWindow();
            ShutdownMode = ShutdownMode.OnLastWindowClose;
        }

        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            logger.ErrorException("Unhandled DispatcherUnhandledException", e.Exception);

            MessageBox.Show("Unhandled exception.\nApplication will exit.\n\n" + e.Exception.ToString(),
                "Client Configurator", MessageBoxButton.OK, MessageBoxImage.Error);            

            e.Handled = true;

            Application.Current.Shutdown();            
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                logger.ErrorException("Unhandled CurrentDomain_UnhandledException", ex);            

                MessageBox.Show("Unhandled exception.\nApplication will exit.\n\n" + ex.ToString(),
                    "Client Configurator", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                logger.Error("Unhandled CurrentDomain_UnhandledException");            
            }

            CloseWpfBugWindow();
        }

        #endregion

        /// <summary>
        /// Logs the application version.
        /// </summary>
        private void LogAppVersion()
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);

                AppVersion = fvi.FileVersion;

                logger.Info("ClientConfigurator startup. File version: {0}", fvi.FileVersion);                
            }
            catch (Exception ex)
            {
                logger.Warn(string.Format("Unexpected exception in LogAppVersion: ", ex.ToString()));
            }
        }

        public static string AppVersion { get; private set; }

    }
}
