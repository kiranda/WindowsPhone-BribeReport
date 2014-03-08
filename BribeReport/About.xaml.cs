using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace BribeReport
{
    public partial class Page1 : PhoneApplicationPage
    {
        public Page1()
        {
            InitializeComponent();
        }
        protected void homeBribeReport_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void ExitIconButton_Click(object sender, EventArgs e)
        {
            MessageBoxResult result =
            MessageBox.Show("Thank you for using Bribe Report. Press ok to exit, cancel to stay.",
            "Bye!",
            MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            { Application.Current.Terminate(); }
            else { return; }
        }

        private void HomeIconButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

      
    }
}