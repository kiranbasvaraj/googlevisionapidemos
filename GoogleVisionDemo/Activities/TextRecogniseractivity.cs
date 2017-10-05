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
using Android.Gms.Vision.Texts;
using Android.Util;
using Android.Gms.Vision;
using Android.Graphics;

namespace GoogleVisionDemo.Activities
{
    [Activity(Label = "TextRecogniseractivity")]
    public class TextRecogniserActivity : Activity
    {
        Button _button;
        ImageView _image;
        TextView _textViewLabel;
        Bitmap _bitmap;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.TextDetectionLayout);
            FindView();
            HandlClickEvents();
        }

        void FindView()
        {
            _button = FindViewById<Button>(Resource.Id.DetectButton);
            _image = FindViewById<ImageView>(Resource.Id.TextDetectionImageView);
            _textViewLabel = FindViewById<TextView>(Resource.Id.RecognisedTextLabel);
            _bitmap = BitmapFactory.DecodeResource(ApplicationContext.Resources, Resource.Drawable.slide5new);

        }
        void HandlClickEvents()
        {
            _button.Click += _button_Click;
            _image.Click += _image_Click;


        }

        private void _image_Click(object sender, EventArgs e)
        {
            var imageIntent = new Intent();
            imageIntent.SetType("image/*");
            imageIntent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(
            Intent.CreateChooser(imageIntent, "Select photo"), 0);
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok)
            {
                Android.Net.Uri uri = data.Data;
                _bitmap = Android.Provider.MediaStore.Images.Media.GetBitmap(ContentResolver, uri);
                _image.SetImageBitmap(_bitmap);

            }
        }

        private void _button_Click(object sender, EventArgs e)
        {
            TextRecognizer textRecogniser = new TextRecognizer.Builder(ApplicationContext).Build();
            if (!textRecogniser.IsOperational)
            {
                Log.Error("error", "detecotor dependencies are not yet available");
            }
            else
            {
                try
                {

                    Frame frame = new Frame.Builder().SetBitmap(_bitmap).Build();
                    SparseArray items = textRecogniser.Detect(frame);
                    StringBuilder strBuilder = new StringBuilder();

                    var size = items.Size();
                    for (int i = 0; i <= items.Size(); ++i)
                    {
                        TextBlock item = (TextBlock)items.ValueAt(i);
                        if (item != null)
                        {
                            strBuilder.Append(item.Value);

                        }

                    }


                    _textViewLabel.Text = strBuilder.ToString();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }
    }
}