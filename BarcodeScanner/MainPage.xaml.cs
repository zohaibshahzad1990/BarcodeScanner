using BarcodeScanner.Mobile.Core;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using static System.Net.WebRequestMethods;
using static Xamarin.Essentials.Permissions;

namespace BarcodeScanner
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            image.IsVisible = false;
            Camera.IsVisible = true;
            // image.Source = ImageSource.FromResource("BarcodeScanner.defautimage.jpg", typeof(MainPage).GetType().Assembly);
            bool allowed = await BarcodeScanner.Mobile.XamarinForms.Methods.AskForRequiredPermission();
        }
        private void CameraView_OnDetected(object sender, OnDetectedEventArg e)
        {
            List<BarcodeResult> barCodeResultList = e.BarcodeResults;

            Device.BeginInvokeOnMainThread(async () =>
            {
                if (barCodeResultList != null && barCodeResultList.Count > 0)
                {
                    Camera.IsVisible = false;
                    bool isTakePhoto = await DisplayAlert("Barcode Scanned", "Barcode Detected", "Take Photo", "Scan Again");
                    if (isTakePhoto)
                    {
                        (Stream takePhotoStream, MediaFile takenPhotoFile) = await TakePhoto();
                        if (takenPhotoFile != null && takePhotoStream != null)
                        {
                            loader.IsVisible = true;
                            UploadPhotoStream(barCodeResultList, takenPhotoFile);
                            loader.IsVisible = false;
                            image.Source = ImageSource.FromStream(() =>
                            {
                                return takenPhotoFile.GetStream();

                            });
                            image.IsVisible = true;
                            RescanButton.IsVisible = true;
                            Camera.IsScanning = true;
                            return;
                        }
                    }
                    else
                    {
                        Camera.IsVisible = true;
                        Camera.IsScanning = true;
                        //await DisplayAlert("Barcode Scanned Failed", "Barcode is not detected", "Try Again");
                        return ;
                    }
                }

                await DisplayAlert("Barcode Scanned Failed", "Barcode is not detected", "Try Again");
                Camera.IsVisible = true;
                Camera.IsScanning = true;


            });

        }

        async Task<(Stream, MediaFile)> TakePhoto()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return default;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = "test.png"
            });

            if (file == null)
                return default;

            //await DisplayAlert("File Location", file.Path, "OK");
            image.IsVisible = false;
            loader.IsVisible = true;
            Stream stream = file.GetStream();
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            return (stream, file);
        }
        void UploadPhotoStream(List<BarcodeResult> barCodeResultList, MediaFile file)
        {
            using (var client = new WebClient())
            {
                client.Credentials = new NetworkCredential("product_ftp@thesat.co.uk", "123456");
                client.UploadFile($"ftp://thesat.co.uk/{barCodeResultList.FirstOrDefault().DisplayValue}.png", WebRequestMethods.Ftp.UploadFile, file.Path);
            }
        }
        private  void Button_Clicked(object sender, EventArgs e)
        {
            loader.IsVisible = false;
            Camera.IsVisible = true;
            image.IsVisible = false;
            RescanButton.IsVisible = false ;
        }
    }
}
