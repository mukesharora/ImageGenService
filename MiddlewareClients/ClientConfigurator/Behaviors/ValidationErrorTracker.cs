using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;
using System.Windows.Shapes;

namespace ClientConfigurator.Behaviors
{
    /// <summary>
    /// Behavior class to enable/disable a control based on validation error of different
    /// controls.
    /// 
    /// Limitations:
    /// Currently supports one target on one form. Could easily be modified to support multiple
    /// targets/forms.
    /// </summary>
    public class ValidationErrorTracker : Behavior<UIElement>, INotifyPropertyChanged
    {
        #region Public properties

        public bool IsTarget { get; set; }

        private bool _hasError = false;
        public bool HasError
        {
            get { return _hasError; }
            private set
            {
                _hasError = value;
                OnPropertyChanged("HasError");
            }
        }

        #endregion

        #region Private properties

        static private ValidationErrorTracker Target { get; set; }

        private static Dictionary<UIElement, Boolean> associatedObjects = new Dictionary<UIElement, bool>();

        #endregion

        #region Constructor

        public ValidationErrorTracker()
        {
            IsTarget = false;
            HasError = false;
        }

        #endregion

        #region Behavior base methods

        protected override void OnAttached()
        {
            base.OnAttached();

            if (IsTarget)
            {
                Target = this;
            }
            else
            {
                associatedObjects.Add(AssociatedObject, false);

                var hasErrorDescriptor = DependencyPropertyDescriptor.FromProperty(System.Windows.Controls.Validation.HasErrorProperty, typeof(UIElement));
                hasErrorDescriptor.AddValueChanged(AssociatedObject, hasErrorDescriptor_Changed);

                FrameworkElement element = AssociatedObject as FrameworkElement;
                if (element != null)
                {
                    element.Unloaded += element_Unloaded;
                }
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
        }

        #endregion

        #region Event handlers

        void element_Unloaded(object sender, RoutedEventArgs e)
        {
            if (associatedObjects.Keys.Contains(sender))
            {
                associatedObjects.Remove(sender as UIElement);
            }
        }

        static private void hasErrorDescriptor_Changed(object sender, EventArgs e)
        {
            UIElement element = sender as UIElement;
            if (element != null)
            {
                bool elementHasError = System.Windows.Controls.Validation.GetHasError(element);
                if (associatedObjects.Keys.Contains(element))
                {
                    associatedObjects[element] = elementHasError;
                }
            }
        }

        private static void UpdateHasError()
        {
            bool hasError = associatedObjects.Values.Any(a => (a == true));                    
            if (Target != null)
            {
                Target.HasError = hasError;
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }
}
