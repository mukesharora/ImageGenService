using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BridgestoneSimulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        

        public MainWindow()
        {
            this.DataContext = this;
            MiddlewareHelper = new MiddlewareHelper();
            InitializeComponent();
        }

        #region Public Properties

        public MiddlewareHelper MiddlewareHelper { get; set; }

        public string WorkStationName
        {
            get
            {
                return "Workstation:\t" + Properties.Settings.Default.WorkstationName;
            }
        }

        #endregion


        #region EventHandlers

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {         
            if (!MiddlewareHelper.Initialize())
            {
                App.LogError("Window_Loaded", "Could not initialize Middleware");
                if (MiddlewareHelper != null)
                {
                    MiddlewareHelper.Close();
                }
                MiddlewareHelper = null;
                MessageBox.Show("Could not initialize Middleware, shutting down.");
                this.Close();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MiddlewareHelper != null)
            {
                MiddlewareHelper.Close();
            }
        }

        private void ResetStatus_Click(object sender, RoutedEventArgs e)
        {
            // In lieu of a MVVM framework, pass click on down
            //
            MiddlewareHelper.ResetWorkstation();
        }

        /// <summary>
        /// Event that gets fired when the EventLogTextBox contents change.
        /// </summary>
        private void EventLogTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                tb.ScrollToEnd();
            }
        }

        #endregion

    }
}
