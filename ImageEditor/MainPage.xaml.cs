using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using ImageEditor.Controls;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Pickers;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ImageEditor
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }
        private async void EditLocal_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FileOpenPicker fo = new FileOpenPicker();
            fo.FileTypeFilter.Add(".png");
            fo.FileTypeFilter.Add(".jpg");
            fo.SuggestedStartLocation = PickerLocationId.Desktop;

            var f = await fo.PickSingleFileAsync();
            if (f != null)
            {
                // create instance and show it
                ImageEditorControl editor = new ImageEditorControl();
                editor.Show(f);

                //register event to get edited image
                editor.ImageEditedCompleted += (image_edited) =>
                {
                    image.Source = image_edited;
                };
            }
        }

        private void EditRemote_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var url = "http://imgstore.cdn.sogou.com/app/a/100540002/714860.jpg";
            
            
            // create instance and show it
            ImageEditorControl editor = new ImageEditorControl();
            editor.Show(new Uri(url));

            //register event to get edited image
            editor.ImageEditedCompleted += (image_edited) =>
            {
                image.Source = image_edited;
            };

        }
    }
}
