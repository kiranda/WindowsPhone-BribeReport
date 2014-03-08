using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using BribeReport.Resources;
using Windows.Devices.Geolocation;
using System.Threading;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;
using Microsoft.Advertising.Mobile.UI;


namespace BribeReport
{
    public partial class MainPage : PhoneApplicationPage
    {
        public static string d_latitude;
        public static string d_longitude;
        public static string UniqueDeviceID;
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            fPreFetchData(); //get all the necessary data preloaded in async

        }

        
        public async void fPreFetchData()
        {
            try
            {
                //get the unique device id
                Byte[] DeviceArrayID = (Byte[])Microsoft.Phone.Info.DeviceExtendedProperties.GetValue("DeviceUniqueId");

                //get the latitude and longitude
                Geolocator geolocator = new Geolocator();
                geolocator.DesiredAccuracyInMeters = 50;

                Geoposition geoposition = await geolocator.GetGeopositionAsync(
                        maximumAge: TimeSpan.FromMinutes(5),
                        timeout: TimeSpan.FromSeconds(10)
                        );


                UniqueDeviceID = Convert.ToBase64String(DeviceArrayID);
                d_latitude = Convert.ToString(geoposition.Coordinate.Latitude);
                d_longitude = Convert.ToString(geoposition.Coordinate.Longitude);
            }
            catch (Exception ex) 
            {
                if ((uint)ex.HResult == 0x80004004)
                {
                    // the application does not have the right capability or the location master switch is off
                    //MessageBoxResult result =
                    MessageBox.Show("Oops! We have a problem getting your location. Your phone may not have GPS capability? Please check your settings.",
                        "Location",
                        MessageBoxButton.OK);
                    btnSubmit.IsEnabled = true;
                }
                else
                {
                    // there is some other issue with the connectivity
                    //MessageBoxResult result =
                    MessageBox.Show("Oops! We have a problem getting your location. Please check your settings.",
                    "Problem",
                    MessageBoxButton.OK);
                    btnSubmit.IsEnabled = true;
                }
            }

        }
        private async void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if ((txtDescription.Text).Trim() =="" || (txtAmount.Text).Trim() == "")
            {
                MessageBox.Show("Please enter both description and an amount?",
                        "Sorry",
                        MessageBoxButton.OK);
                return;
            }
            //start the progress bar 
            ProgressIndicator progressIndicator = new ProgressIndicator()
            {
                IsVisible = true,
                IsIndeterminate = false,
                Text = "Sending bribe data..."
            };
            SystemTray.SetProgressIndicator(this, progressIndicator);

            
            try
            {

                //diable the button to prevent double submit
                btnSubmit.IsEnabled = false;

                //round the amount
                decimal b_amt = Convert.ToDecimal(txtAmount.Text);
                b_amt = Math.Round(b_amt);
               
                //insert the data into the table

               BribeData bribedata = new BribeData { bribe_description = txtDescription.Text, bribe_amount = Convert.ToInt32(b_amt), device_type = UniqueDeviceID, latitude = d_latitude, longitude = d_longitude };
               await App.MobileService.GetTable<BribeData>().InsertAsync(bribedata);

                //hide the progress bar
                progressIndicator.IsVisible = false;
               



            }
            catch 
            {
                    // there is some other issue with the connectivity
                    //MessageBoxResult result =
                        MessageBox.Show("Oops! We have a problem getting your location and/or connecting to our server. Please try again later?",
                        "Problem",
                        MessageBoxButton.OK);
                        btnSubmit.IsEnabled = true;
                
            }


            MessageBoxResult result = MessageBox.Show("We salute your courage in reporting a bribe payment. Thank you! Press Ok to continue and Cancel to Exit the app.",
                        "Success!",
                        MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                btnSubmit.IsEnabled = true;
                txtDescription.Text = "";
                txtAmount.Text = "";
            }
            if (result == MessageBoxResult.Cancel)
            { Application.Current.Terminate(); }
        }
        

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            CheckLocationConsent();
        }

        private Boolean SetLocationConsent()
        {
            Boolean bLocationAccessisOn;

            try
            {
            MessageBoxResult result =
                        MessageBox.Show("This app needs to accesses your phone's location to report the place the bribe was given. Is that ok?",
                        "Location",
                        MessageBoxButton.OKCancel);

            
                if (result == MessageBoxResult.OK)
                {
                    IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = true;
                    bLocationAccessisOn = true;
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = false;
                    bLocationAccessisOn = false;
                }

                IsolatedStorageSettings.ApplicationSettings.Save();
                return (bLocationAccessisOn);
            }

            catch 
            {
                //MessageBoxResult result =
                        MessageBox.Show("Oops! We have a problem turning the location service ON. Please turn on location service from your phone settings and try again?",
                        "Problem",
                        MessageBoxButton.OK);
                        
                        return false;
                        
            }
        }
        private void CheckLocationConsent()
        {

            
                if (IsolatedStorageSettings.ApplicationSettings.Contains("LocationConsent"))
                {
                    // User has opted in or out of Location
                    string strLocConsent = Convert.ToString(IsolatedStorageSettings.ApplicationSettings["LocationConsent"]);
                    if (strLocConsent == "False")
                    {
                        Boolean b = SetLocationConsent();
                        if (b == false)
                        {
                            MessageBox.Show("Sorry, this app needs to accesses your phone's location to report the place the bribe was given. Please turn your location setting On and come back to the app.",
                        "Location",
                        MessageBoxButton.OK);
                         //close the app
                            Application.Current.Terminate();
                        }
                    }

                    return;
                }
                else
                {
                    Boolean b = SetLocationConsent();

                    return;
                }
            
           
        }

        private void exitBribeReport_Click(object sender, EventArgs e)
        {
            MessageBoxResult result =
            MessageBox.Show("Thank you for using Bribe Report. Press ok to exit, cancel to stay.",
            "Bye!",
            MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            { Application.Current.Terminate(); }
            else { return; }

            
        }

        private void aboutBribeReport_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }

        private void txtAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch("[^0-9]", txtAmount.Text))
            {
                if (txtAmount.Text.Length > 0) { MessageBox.Show("Please enter only numbers, no decimals please."); }
                if (txtAmount.Text.Length == 1 || txtAmount.Text.Length == 0) { txtAmount.Text = ""; }
                else
                {
                    txtAmount.Text.Remove(txtAmount.Text.Length - 1);
                }
            }
        }
     
    }
}