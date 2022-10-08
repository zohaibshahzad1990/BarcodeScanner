using BarcodeScanner.Mobile.Core;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BarcodeScanner
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            BarcodeScanner.Mobile.Core.Methods.SetSupportBarcodeFormat(BarcodeFormats.All);
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
