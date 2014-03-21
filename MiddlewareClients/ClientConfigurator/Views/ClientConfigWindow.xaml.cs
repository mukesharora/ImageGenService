using ClientConfigurator.Validation;
using ClientConfigurator.ViewModels;
using NLog;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientConfigurator.Views
{
    /// <summary>
    /// Interaction logic for ClientConfigWindow.xaml
    /// </summary>
    public partial class ClientConfigWindow : Window
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public ClientConfigWindow()
        {
            try
            {
                Closing += ClientConfigWindow_Closing;
                Loaded += ClientConfigWindow_Loaded;

                ViewModel = new ClientConfigViewModel();
                DataContext = ViewModel;
                InitializeComponent();
            }
            catch (Exception ex)
            {
                logger.FatalException("Unexpected exception in ClientConfigWindow ctor", ex);
                throw;
            }
        }

        #region Public properties

        public ClientConfigViewModel ViewModel { get; set; }

        public string AppVersion 
        {
            get { return App.AppVersion;  }
        }

        #endregion

        # region Event handlers

        /// <summary>
        /// Window has been loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ClientConfigWindow_Loaded(object sender, RoutedEventArgs e)
        {
            UserError.RegisterHandler(err =>
            {
                var result = MessageBox.Show(err.ErrorMessage, "Client Configurator", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return Observable.Return(RecoveryOptionResult.CancelOperation);                
            });
        }

        /// <summary>
        /// Event handler for the host name ComboBoxes. The MachineNameComboBox_PreviewKeyDown
        /// and MachineNameComboBox_LostFocus event handlers allow items to be added
        /// to the host name ComboBoxes.
        /// </summary>
        private void MachineNameComboBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                ComboBox cb = sender as ComboBox;
                if ((e.Key == Key.Enter) && (cb != null))
                {
                    string newItem = cb.Text;
                    Collection<string> col = cb.ItemsSource as Collection<string>;
                    RegexValidationRule rule = new RegexValidationRule();

                    // TODO get this pattern from binding
                    rule.Pattern = "^[a-zA-Z][a-zA-Z_0-9]*$";
                    ValidationResult valResult = rule.Validate(newItem, null);
                    if (col != null && !col.Contains(newItem))
                    {
                        if (valResult.IsValid)
                        {
                            col.Add(newItem);
                        }

                        // Get rid of text edit cursor. 
                        // TODO: figure out how to make text highlighted so user knows item was added.
                        //
                        cb.IsEditable = false;
                        cb.IsEditable = true;
                        cb.SelectedItem = newItem;
                    }
                    else
                    {
                        cb.IsEditable = false;
                        cb.IsEditable = true;
                    }
                }

                e.Handled = false;
            }
            catch (Exception ex)
            {
                logger.ErrorException("Unhandled in MachineNameComboBox_PreviewKeyDown", ex);
            }
        }

        private void MachineNameComboBox_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                ComboBox cb = sender as ComboBox;
                if ((cb != null))
                {
                    string newItem = cb.Text;
                    Collection<string> col = cb.ItemsSource as Collection<string>;
                    RegexValidationRule rule = new RegexValidationRule();

                    // TODO get this pattern from binding
                    rule.Pattern = "^[a-zA-Z][a-zA-Z_0-9]*$";
                    ValidationResult valResult = rule.Validate(newItem, null);
                    if (col != null && !col.Contains(newItem))
                    {
                        if (valResult.IsValid)
                        {
                            col.Add(newItem);
                        }

                        // Get rid of text edit cursor. 
                        // TODO: figure out how to make text highlighted so user knows item was added.
                        //
                        cb.IsEditable = false;
                        cb.IsEditable = true;
                        cb.SelectedItem = newItem;                        
                        //((ComboBox)sender).GetBindingExpression(ComboBox.SelectedItemProperty).UpdateTarget();
                        //((ComboBox)sender).GetBindingExpression(ComboBox.SelectedItemProperty).UpdateSource();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.ErrorException("Unhandled in MachineNameComboBox_LostFocus", ex);
            }
        }

        private void ccWindow_Loaded(object sender, RoutedEventArgs e)
        {          
            ViewModel.LoadModel();            
        }

        void ClientConfigWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if ((ViewModel != null) && ViewModel.IsDirty)
            {
                MessageBoxResult result = MessageBox.Show("There are unsaved changes, are you you sure you want to exit?", "Exit Application?", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    e.Cancel = false;
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        #endregion
    }
}
