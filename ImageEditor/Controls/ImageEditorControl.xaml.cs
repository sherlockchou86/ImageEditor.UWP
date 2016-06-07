using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
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
    public sealed partial class ImageEditorControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property_name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property_name));
            }
        }
        #region properties
        private string _left_button_text = "取消";
        public string LeftButtonText
        {
            get
            {
                return _left_button_text;
            }
            set
            {
                _left_button_text = value;
                OnPropertyChanged("LeftButtonText");
            }
        }

        private string _right_button_text = "确定";
        public string RightButtonText
        {
            get
            {
                return _right_button_text;
            }
            set
            {
                _right_button_text = value;
                OnPropertyChanged("RightButtonText");
            }
        }

        private string _title ="图片 DIY";
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                OnPropertyChanged("Title");
            }
        }
        #endregion

        Border border = new Border();
        Popup popup = new Popup();
        public ImageEditorControl()
        {
            this.InitializeComponent();
        }

        public void Show()
        {
            var height = ApplicationView.GetForCurrentView().VisibleBounds.Height;
            var width = ApplicationView.GetForCurrentView().VisibleBounds.Width;

            border.Background = new SolidColorBrush(Color.FromArgb(0XAA, 0X00, 0X00, 0X00));

            if (height < 800 && width < 600)
            {
                border.Width = this.Width = width;
                border.Height = this.Height = height;
            }
            else
            {
                border.Width = width;
                border.Height = height;

                this.Height = height * 3 / 4;
                this.Width = this.Height * 3 / 4;
            }
            tab1.Width = tab2.Width = tab3.Width = this.Width / 3;

            border.Child = this;

            popup.Child = border;
            popup.Opened += (s, e) =>
              {
                  Window.Current.SizeChanged += Current_SizeChanged;
              };
            popup.Closed += (s, e) =>
              {
                  Window.Current.SizeChanged -= Current_SizeChanged;
              };
            popup.IsOpen = true;
        }


        /// <summary>
        /// PC窗体大小改变时，保证居中显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            var height = ApplicationView.GetForCurrentView().VisibleBounds.Height;
            var width = ApplicationView.GetForCurrentView().VisibleBounds.Width;

            if (height < 800 && width < 600)
            {
                border.Width = this.Width  = width;
                border.Height = this.Height = height;
            }
            else
            {
                border.Width = width;
                border.Height = height;

                this.Height = height * 3 / 4;
                this.Width = this.Height * 3 / 4;
            }
            tab1.Width = tab2.Width = tab3.Width = this.Width / 3;
        }
    }
}
