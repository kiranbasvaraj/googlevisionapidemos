using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Gms.Vision;
using Android.Gms.Vision.Barcodes;
using Android.Graphics;
using Android.Support.V4.App;
using Android;
using Android.Content.PM;
using static Android.Gms.Vision.Detector;
using Android.Util;

namespace GoogleVisionDemo.Activities
{
    [Activity(Label = "BarcodeScannerActivity", MainLauncher = true)]
    public class BarcodeScannerActivity : Activity, ISurfaceHolderCallback, IProcessor
    {
        SurfaceView _cameraPreview;
        Button _scanButton;
        TextView _scannedText;
        CameraSource _cameraSource;
        BarcodeDetector _barcodeDetector;
        const int RequestCameraPermissionId = 1001;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.BarcodeScannerLayout);
            FindViews();
            HandleEvents();
            _barcodeDetector = new BarcodeDetector.Builder(this).SetBarcodeFormats(BarcodeFormat.QrCode).Build();
            _cameraSource = new CameraSource.Builder(this, _barcodeDetector).SetRequestedPreviewSize(640, 480).Build();
            _cameraPreview.Holder.AddCallback(this);
            _barcodeDetector.SetProcessor(this);

        }
        void FindViews()
        {
            _cameraPreview = FindViewById<SurfaceView>(Resource.Id.cameraPreview);
           // _scanButton = FindViewById<Button>(Resource.Id.scanButton);
            _scannedText = FindViewById<TextView>(Resource.Id.scannedText);
        }
        void HandleEvents()
        {
           // _scanButton.Click += _scanButton_Click;
        }

        private void _scanButton_Click(object sender, EventArgs e)
        {

        }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
            //throw new NotImplementedException();
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            if (ActivityCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.Camera) != Android.Content.PM.Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new string[]
                {
                    Manifest.Permission.Camera
                }, RequestCameraPermissionId);
                return;
            }

            try
            {
                _cameraSource.Start(_cameraPreview.Holder);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.StackTrace);
            }
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            _cameraSource.Stop();
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            switch (requestCode)
            {
                case RequestCameraPermissionId:
                    {
                        if (grantResults[0] == Permission.Granted)
                        {
                            if (ActivityCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.Camera) != Android.Content.PM.Permission.Granted)
                            {
                                ActivityCompat.RequestPermissions(this, new string[]
                                {
                    Manifest.Permission.Camera
                                }, RequestCameraPermissionId);
                                return;
                            }

                            try
                            {
                                _cameraSource.Start(_cameraPreview.Holder);
                            }
                            catch (Exception ex)
                            {

                                Console.WriteLine(ex.StackTrace);
                            }
                        }
                    }
                    break;
            }
        }

        public void ReceiveDetections(Detections detections)
        {
            SparseArray qrcodes = detections.DetectedItems;
            if (qrcodes.Size() != 0)
            {
                _scannedText.Post(() =>
                {
                    Vibrator vib = (Vibrator)GetSystemService(Context.VibratorService);
                    vib.Vibrate(1000);
                    _scannedText.Text = ((Barcode)qrcodes.ValueAt(0)).RawValue;
                });
            }
        }

        public void Release()
        {
           // throw new NotImplementedException();
        }
    }
}