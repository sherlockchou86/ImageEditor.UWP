using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Search;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ImageEditor.Controls
{
    public sealed partial class ImageSeletorControl : UserControl,INotifyPropertyChanged
    {
        public ImageSeletorControl()
        {
            this.InitializeComponent();

            Loaded += ImageSeletorControl_Loaded;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event ImageSelectedEventHandler ImageSelected;

        private ObservableCollection<BitmapImage> _images = new ObservableCollection<BitmapImage>();
        public ObservableCollection<BitmapImage> Images
        {
            get
            {
                return _images;
            }
            set
            {
                _images = value;
                OnPropertyChanged("Images");
            }
        }

        private Dictionary<BitmapImage, StorageFile> _cache = new Dictionary<BitmapImage, StorageFile>();
        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseBtn_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }
        /// <summary>
        /// 打开相机拍照
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void FromCamera_Tapped(object sender, TappedRoutedEventArgs e)
        {
            CameraCaptureUI captureUI = new CameraCaptureUI();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            captureUI.PhotoSettings.CroppedSizeInPixels = new Size(200, 200);
            StorageFile photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

            if (ImageSelected != null && photo != null)
            {
                ImageSelected(photo);
            }
        }
        /// <summary>
        /// 打开目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void FromAlbum_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".bmp");

            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;

            var image_file = await picker.PickSingleFileAsync();
            if (image_file != null)
            {
                if(ImageSelected !=null)
                {
                    ImageSelected(image_file);
                }
            }
        }
        /// <summary>
        /// 快速选择图片列表中的图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (ImageSelected != null)
            {
                ImageSelected(_cache[e.ClickedItem as BitmapImage]);
            }
        }

        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageSeletorControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadImages();
        }

        /// <summary>
        /// 加载picture library中前9张图片
        /// </summary>
        private async void LoadImages()
        {
            WaitingRing.IsActive = true;
            var pictures = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Pictures);
            if (pictures != null)
            {
                var folder = pictures.SaveFolder;
                StorageFileQueryResult query = folder.CreateFileQuery(Windows.Storage.Search.CommonFileQuery.OrderByDate);

                var images = await query.GetFilesAsync();
                if (images != null)
                {
                    if (images.Count > 9)  //只显示最前面的9张
                    {
                        images = images.Take(9).ToList();
                    }
                    images.ToList().ForEach(async (image) =>
                    {
                        try
                        {
                            BitmapImage img = new BitmapImage();
                            var f = await image.OpenAsync(FileAccessMode.Read);
                            if (f != null)
                            {
                                f.Seek(0);
                                await img.SetSourceAsync(f);
                                Images.Add(img);
                                _cache.Add(img, image);
                            }
                        }
                        catch
                        {

                        }
                    });
                }
            }
            if (Images.Count == 0)
            {
                WaitingRing.Visibility = Visibility.Visible;
            }
            WaitingRing.IsActive = false;
        }
    }

    public delegate void ImageSelectedEventHandler(StorageFile image_file);
}
