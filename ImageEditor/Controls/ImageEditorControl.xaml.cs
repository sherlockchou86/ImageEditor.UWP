using ImageEditor.Tools;
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
using Microsoft.Graphics.Canvas;
using ImageEditor.DrawingObjects;
using System.Threading.Tasks;

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
            Loaded += ImageEditorControl_Loaded;
        }

        #region event handlers
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageEditorControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetCanvas();
        }

        /// <summary>
        /// 显示编辑器
        /// </summary>
        public void Show()
        {
            var height = ApplicationView.GetForCurrentView().VisibleBounds.Height;
            var width = ApplicationView.GetForCurrentView().VisibleBounds.Width;

            border.Background = new SolidColorBrush(Color.FromArgb(0XAA, 0X00, 0X00, 0X00));

            if (UWPPlatformTool.IsMobile)
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
            tab1.Width = tab2.Width = tab3.Width = tab4.Width = this.Width / 4;

            SetCanvas();

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
            popup.IsOpen = true;
        }
        /// <summary>
        /// PC窗体大小改变时，保证居中显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            var height = ApplicationView.GetForCurrentView().VisibleBounds.Height;
            var width = ApplicationView.GetForCurrentView().VisibleBounds.Width;

            if (UWPPlatformTool.IsMobile)
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
            tab1.Width = tab2.Width = tab3.Width = tab4.Width = this.Width / 4;
            SetCanvas();
            MainCanvas.Invalidate();
            await Task.Delay(10);
            SetCanvas();
            MainCanvas.Invalidate();
        }
        /// <summary>
        /// 画布绘制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void MainCanvas_Draw(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasDrawEventArgs args)
        {
            var target = GetDrawings(true);  //
            if (target != null)
            {
                args.DrawingSession.DrawImage(target);
            }
        }
        /// <summary>
        /// 点击画布
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainCanvas_Tapped(object sender, TappedRoutedEventArgs e)
        {
            TagInsertControl tic = new TagInsertControl();
            tic.Show();
        }
        #endregion

        #region fields
        private Color _back_color = Colors.White;   //画布背景色
        private Stretch _stretch = Stretch.Uniform;  //底图图片填充方式
        private int _size_mode = 2;  //画布长宽比  0/ 1:1  1/ 4:3  2/ 3:4
        private int _rotate = 0;   //底图图片旋转度数（360度==0度）

        private CanvasBitmap _image;  //底图

        IDrawingUI _cropUI = new CropUI() { Left = 10, Top = 10, Width = 200, Height = 100, DrawColor=Colors.Gray };  //剪切UI  
        List<IDrawingUI> _tagsUIs;  //Tags
        Stack<IDrawingUI> _doodleUIs;  //涂鸦
        IDrawingUI _wall_paperUI; // 墙纸

        #endregion

        #region methods
        /// <summary>
        /// 获取绘图结果
        /// </summary>
        /// <param name="edit">是否编辑状态，编辑状态需要绘制编辑工具</param>
        /// <returns></returns>
        private CanvasRenderTarget GetDrawings(bool edit)
        {
            CanvasDevice device = CanvasDevice.GetSharedDevice();
            CanvasRenderTarget target = new CanvasRenderTarget(device, (float)MainCanvas.ActualWidth, (float)MainCanvas.ActualHeight, 96);
            using (CanvasDrawingSession graphics = target.CreateDrawingSession())
            {
                //绘制背景
                graphics.Clear(_back_color);

                //绘制底图
                //绘制涂鸦
                //绘制贴图
                //绘制Tag
                //绘制Crop裁剪工具
                if (_cropUI != null)
                {
                    _cropUI.Draw(graphics);
                }
            }

            return target;
        }
        /// <summary>
        /// 
        /// </summary>
        private void SetCanvas()
        {
            var w = MainWorkSapce.ActualWidth - 40;  //
            var h = MainWorkSapce.ActualHeight - 40;  //
            if (w <= 0 || h <= 0)
            {
                return;
            }
            if (_size_mode == 0)  //1:1
            {
                var l = w > h ? h : w;
                MainCanvas.Width = MainCanvas.Height = l;
            }
            else if (_size_mode == 1)  //4:3
            {
                if (w <= h)
                {
                    MainCanvas.Width = w;
                    MainCanvas.Height = MainCanvas.Width * 3 / 4;
                }
                else
                {
                    if (w / h <= 4 / 3)
                    {
                        MainCanvas.Width = w;
                        MainCanvas.Height= MainCanvas.Width * 3 / 4;
                    }
                    else
                    {
                        MainCanvas.Height = h;
                        MainCanvas.Width = MainCanvas.Height * 4 / 3;
                    }
                }
            }
            else  //3:4
            {
                if (h <= w)
                {
                    MainCanvas.Height = h;
                    MainCanvas.Width = MainCanvas.Height * 3 / 4;
                }
                else
                {
                    if (h / w <= 4 / 3)
                    {
                        MainCanvas.Height = h;
                        MainCanvas.Width = MainCanvas.Height * 3 / 4;
                    }
                    else
                    {
                        MainCanvas.Width = w;
                        MainCanvas.Height = MainCanvas.Width * 4 / 3;
                    }
                }
            }
        }
        #endregion
    }
}
