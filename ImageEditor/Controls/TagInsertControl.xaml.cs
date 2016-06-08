using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using ImageEditor.Tools;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ImageEditor.Controls
{
    public sealed partial class TagInsertControl : UserControl
    {
        Border border = new Border();
        Popup popup = new Popup();

        public TagInsertControl()
        {
            this.InitializeComponent();
        }
        /// <summary>
        /// 显示
        /// </summary>
        public void Show()
        {
            var height = ApplicationView.GetForCurrentView().VisibleBounds.Height;
            var width = ApplicationView.GetForCurrentView().VisibleBounds.Width;

            border.Background = new SolidColorBrush(Color.FromArgb(0X88, 0X00, 0X00, 0X00));

            border.Width = width;
            border.Height = height;

            this.Width = this.Height = 200;

            if (UWPPlatformTool.IsMobile)
            {
                popup.VerticalOffset = 24;
            }
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
            border.Tapped += (s, e) =>
              {
                  popup.IsOpen = false;
              };
            popup.IsOpen = true;
        }

        private void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            var height = ApplicationView.GetForCurrentView().VisibleBounds.Height;
            var width = ApplicationView.GetForCurrentView().VisibleBounds.Width;

            border.Width = width;
            border.Height = height;
        }

        /// <summary>
        /// 插入定位tag
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InsertLocation_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }
        /// <summary>
        /// 插入热门话题tag
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InsertTopic_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }
        /// <summary>
        /// 插入@好友
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InsertAt_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }
    }
}
