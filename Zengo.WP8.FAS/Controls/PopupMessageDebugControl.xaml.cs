﻿
#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

#endregion

namespace Zengo.WP8.FAS.Controls
{
    public partial class PopupMessageDebugControl : UserControl
    {
        #region Properties

        public string Message { get { return TextBlockMessage.Text; } set { TextBlockMessage.Text = "DBG: " + value; } }

        #endregion


        #region Constructors

        public PopupMessageDebugControl()
        {
            InitializeComponent();
        }

        #endregion
    }
}
