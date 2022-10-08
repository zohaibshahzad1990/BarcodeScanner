using BarcodeScanner.Mobile.Core;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BarcodeScanner
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
          
        }
        protected  override void OnAppearing()
        {
            base.OnAppearing();
           // image.Source = ImageSource.FromResource("BarcodeScanner.defautimage.jpg", typeof(MainPage).GetType().Assembly);
            //bool allowed = await BarcodeScanner.Mobile.XamarinForms.Methods.AskForRequiredPermission();
        }
        //private  void CameraView_OnDetected(object sender, OnDetectedEventArg e)
        //{
        //    List<BarcodeResult> obj = e.BarcodeResults;

        //    string result = string.Empty;
        //    for (int i = 0; i < obj.Count; i++)
        //    {
        //        result += $"Type : {obj[i].BarcodeType}, Value : {obj[i].DisplayValue}{Environment.NewLine}";
        //    }
        //    Device.BeginInvokeOnMainThread(async () =>
        //    {
        //        await DisplayAlert("Result", result, "OK");
        //        // If you want to start scanning again
        //        Camera.IsScanning = true;
        //    });

        //}

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
               await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = "test.png"
            });

            if (file == null)
                return;

            await DisplayAlert("File Location", file.Path, "OK");
            //Stream stream = file.GetStream();
            //byte[] bytes = new byte[stream.Length];
            //stream.Read(bytes, 0, bytes.Length);
            //stream.Seek(0, SeekOrigin.Begin);
            //List<BarcodeResult> barCodeResultList= await BarcodeScanner.Mobile.Core.Methods.ScanFromImage(bytes);
            //if (barCodeResultList != null && barCodeResultList.Count > 0)
            //{

            //    using (var client = new WebClient())
            //    {
            //        client.Credentials = new NetworkCredential("product_ftp@thesat.co.uk", "123456");
            //        client.UploadFile($"ftp://thesat.co.uk/{barCodeResultList.FirstOrDefault().DisplayValue}.png", WebRequestMethods.Ftp.UploadFile, file.Path);
            //    }
            //}
            image.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                return stream;
            });
        }
    }
}
