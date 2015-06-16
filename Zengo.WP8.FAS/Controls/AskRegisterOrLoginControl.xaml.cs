
#region Usings

using System;
using System.Windows;
using System.Windows.Controls;

#endregion

namespace Zengo.WP8.FAS.Controls
{
    public partial class AskRegisterOrLoginControl : UserControl
    {
        #region Events

        public event EventHandler<EventArgs> RegisterPressed;
        public event EventHandler<EventArgs> LoginPressed;
        public event EventHandler<EventArgs> LaterPressed;
        public event EventHandler<EventArgs> ResetPasswordPressed;
        //public event EventHandler<EventArgs> IHavePinPressed;

        #endregion


        #region Properties

        public AskRegisterOrLoginControl()
        {
            InitializeComponent();
        }

        #endregion


        #region Event Handlers

        private void ButtonRegister_Click(object sender, RoutedEventArgs e)
        {
            if (RegisterPressed != null)
            {
                RegisterPressed(this, EventArgs.Empty);
            }
        }

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            if (LoginPressed != null)
            {
                LoginPressed(this, EventArgs.Empty);
            }
        }

        private void HyperlinkResetPassword_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ResetPasswordPressed != null)
            {
                ResetPasswordPressed(this, EventArgs.Empty);
            }
        }

        //private void HyperlinkIHavePin_Click(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    if (IHavePinPressed != null)
        //    {
        //        IHavePinPressed(this, EventArgs.Empty);
        //    }
        //}

        private void HyperlinkButtonLater_Click(object sender, RoutedEventArgs e)
        {
            if (LaterPressed != null)
            {
                LaterPressed(this, EventArgs.Empty);
            }
        }

        #endregion


        #region Enable Control

        internal void Enable()
        {
            Visibility = System.Windows.Visibility.Visible;
        }

        internal void Disable()
        {
            Visibility = System.Windows.Visibility.Collapsed;
        }

        internal bool IsVisible()
        {
            return Visibility == System.Windows.Visibility.Collapsed ? false : true;
        }

        #endregion
    }
}
