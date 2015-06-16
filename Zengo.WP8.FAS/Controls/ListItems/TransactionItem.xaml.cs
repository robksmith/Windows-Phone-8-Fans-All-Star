
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
using Zengo.WP8.FAS.Models;

#endregion

namespace Zengo.WP8.FAS.Controls.ListItems
{
    public class TransactionHistoryRecord
    {
        public PackageRecord PackageRecord { get; set; }
        public TransactionRecord TransactionRecord { get; set; }
        public DateTime TransactionTime { get; set; }
    }


    public partial class TransactionItem : UserControl
    {
        public TransactionItem()
        {
            InitializeComponent();
        }
    }
}
