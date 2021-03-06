﻿using ImageEditor.Tools;
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
using Windows.UI.Xaml.Shapes;
using Windows.Storage;
using Microsoft.Graphics.Canvas.Effects;
using System.Collections.ObjectModel;
using Windows.Storage.Streams;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ImageEditor.Controls
{
    public sealed partial class ImageEditorControl : UserControl, INotifyPropertyChanged
    {
        public event ImageEditedCompletedEventHandler ImageEditedCompleted;
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

        private ObservableCollection<string> _wallpapers = new ObservableCollection<string>();
        public ObservableCollection<string> WallPapers
        {
            get
            {
                return _wallpapers;
            }
            set
            {
                _wallpapers = value;
                OnPropertyChanged("WallPapers");
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
            LoadWallPapers();
        }

        /// <summary>
        /// 显示编辑器
        /// </summary>
        private void Show()
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
            tab1.Width = tab2.Width = tab3.Width = tab4.Width = tab5.Width = this.Width / 5;

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
        /// 显示编辑器（带local底图参数）
        /// </summary>
        /// <param name="image"></param>
        public async void Show(StorageFile image)
        {
            try
            {
                Show();
                WaitLoading.IsActive = true;
                CanvasDevice cd = CanvasDevice.GetSharedDevice();
                var stream = await image.OpenAsync(FileAccessMode.Read);
                _image = await CanvasBitmap.LoadAsync(cd, stream);
                WaitLoading.IsActive = false;
                MainCanvas.Invalidate();
            }
            catch
            {

            }
        }
        /// <summary>
        /// 显示编辑器（带底图片url）
        /// </summary>
        /// <param name="uri"></param>
        public async void Show(Uri uri)
        {
            try
            {
                Show();
                WaitLoading.IsActive = true;
                CanvasDevice cd = CanvasDevice.GetSharedDevice();
                _image = await CanvasBitmap.LoadAsync(cd, uri, 96);
                WaitLoading.IsActive = false;
                MainCanvas.Invalidate();
            }
            catch
            {

            }
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
            tab1.Width = tab2.Width = tab3.Width = tab4.Width = tab5.Width = this.Width / 5;
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
            if (!ClickEmpty(e.GetPosition(MainCanvas)))
            {
                return;
            }
            TagInsertControl tic = new TagInsertControl();
            tic.TagSelected += (tag, type) =>
              {
                  if (_tagsUIs == null)
                      _tagsUIs = new List<IDrawingUI>();
                  _tagsUIs.Add(new TagUI() { Bound = MainCanvas.Size, TagText = tag, TagType = type, X = (float)e.GetPosition(MainCanvas).X, Y = (float)e.GetPosition(MainCanvas).Y });
                  MainCanvas.Invalidate();

              };
            tic.Show();
        }
        /// <summary>
        /// pivot切换  下面的tabs同步显示效果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = (sender as Pivot).SelectedIndex;
            RelativePanel tab = null;
            switch (selected)
            {
                case 0:
                    {
                        tab = tab1;
                        break;
                    }
                case 1:
                    {
                        tab = tab5;
                        break;
                    }
                case 2:
                    {
                        tab = tab2;
                        break;
                    }
                case 3:
                    {
                        tab = tab3;
                        break;
                    }
                case 4:
                    {
                        tab = tab4;
                        break;
                    }
            }
            List<RelativePanel> l = new List<RelativePanel> { tab1, tab5, tab2, tab3, tab4 };
            foreach (RelativePanel t in l)
            {
                (t.Children[0] as TextBlock).Foreground = new SolidColorBrush(Colors.Gray);
                (t.Children[1] as Rectangle).Fill = new SolidColorBrush(Colors.Gray);
            }
            (tab.Children[0] as TextBlock).Foreground = new SolidColorBrush(Colors.Orange);
            (tab.Children[1] as Rectangle).Fill = new SolidColorBrush(Colors.Orange);
        }
        /// <summary>
        /// 点击tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tab_Tapped(object sender, TappedRoutedEventArgs e)
        {
            List<RelativePanel> tabs = new List<RelativePanel> { tab1, tab5, tab2, tab3, tab4 };
            int selected = tabs.IndexOf(sender as RelativePanel);

            MainCommandPanel.SelectedIndex = selected;
        }
        /// <summary>
        /// 涂鸦撤销
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SymbolIcon_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (_doodleUIs != null && _doodleUIs.Count>0)
            {
                _doodleUIs.Pop();  //删除最近一次涂鸦 立即重绘
                MainCanvas.Invalidate();
            }
        }
        /// <summary>
        /// 选择涂鸦画笔粗细
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PenSize_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var pen_size = sender as Border;
            List<Border> l = new List<Border> { PenSize1, PenSize2, PenSize3 };

            l.ForEach((b) => { (b as Border).Child.Visibility = Visibility.Collapsed; }); //不选中状态

            pen_size.Child.Visibility = Visibility.Visible; //选中

            _pen_size = int.Parse(pen_size.Tag.ToString());
        }
        /// <summary>
        /// 选择涂鸦画笔颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PenColor_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var pen_color = sender as Border;

            List<Border> l = new List<Border> { PenColor1, PenColor2, PenColor3, PenColor4, PenColor5, PenColor6 };
            l.ForEach((b) => { b.Child.Visibility = Visibility.Collapsed; });

            pen_color.Child.Visibility = Visibility.Visible;

            if (pen_color.Background is ImageBrush)  //图片刷子
            {
                _pen_color = Colors.Transparent;
                PenSize1.Background = PenSize2.Background = PenSize3.Background = pen_color.Background;
            }
            else
            {
                _pen_color = (pen_color.Background as SolidColorBrush).Color;
                PenSize1.Background = PenSize2.Background = PenSize3.Background = new SolidColorBrush(_pen_color);
            }
        }
        /// <summary>
        /// 操作画布开始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainCanvas_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            if (MainCommandPanel.SelectedIndex == 4)  //涂鸦状态
            {
                if (_current_editing_doodleUI == null)
                {
                    _current_editing_doodleUI = new DoodleUI() { DrawingColor = _pen_color, DrawingSize = _pen_size };
                    _current_editing_doodleUI.InitImageBrush();  //可能是图片图片画刷  需要提前初始化
                }
                return;
            }
            if (_tagsUIs != null)
            {
                foreach (var tag in _tagsUIs)
                {
                    if ((tag as TagUI).Region.Contains(e.Position))
                    {
                        _current_tag = tag;
                        _pre_manipulation_position = e.Position;
                        _manipulation_type = 2;
                        break;
                    }
                }
            }
            if (MainCommandPanel.SelectedIndex == 0) //可能是剪切状态
            {
                if (_cropUI != null)  //确实是剪切状态
                {
                    if ((_cropUI as CropUI).Region.Contains(e.Position)) //移动剪切对象
                    {
                        _manipulation_type = 0;
                        _pre_manipulation_position = e.Position;
                    }
                    if ((_cropUI as CropUI).RightBottomRegion.Contains(e.Position)) //缩放剪切区域
                    {
                        _manipulation_type = 1;
                        _pre_manipulation_position = e.Position;
                    }

                }
                return;
            }
            if (MainCommandPanel.SelectedIndex == 2)  //可能是墙纸编辑状态
            {
                if(_wall_paperUI != null)
                {
                    if ((_wall_paperUI as WallPaperUI).Region.Contains(e.Position))  //移动墙纸
                    {
                        _manipulation_type = 3;
                        _pre_manipulation_position = e.Position;
                        (_wall_paperUI as WallPaperUI).Editing = true;
                    }
                    if ((_wall_paperUI as WallPaperUI).RightBottomRegion.Contains(e.Position) && (_wall_paperUI as WallPaperUI).Editing)  //缩放墙纸
                    {
                        _manipulation_type = 4;
                        _pre_manipulation_position = e.Position;
                    }
                    MainCanvas.Invalidate();
                }
                return;
            }
        }
        /// <summary>
        /// 操作画布结束
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainCanvas_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (MainCommandPanel.SelectedIndex == 4)
            {
                if (_current_editing_doodleUI != null)
                {
                    _doodleUIs.Push(_current_editing_doodleUI);
                    _current_editing_doodleUI = null;
                    MainCanvas.Invalidate();
                }
                return;
            }
            if (_current_tag != null)
            {
                if (_manipulation_type == 2)
                {
                    _pre_manipulation_position = null;
                    _current_tag = null;
                }
            }
            if (MainCommandPanel.SelectedIndex == 0)
            {
                if (_cropUI != null)
                {
                    _pre_manipulation_position = null;
                }
                return;
            }
            if (MainCommandPanel.SelectedIndex == 2)
            {
                if (_wall_paperUI != null)
                {
                    _pre_manipulation_position = null;
                }
                return;
            }
        }

        /// <summary>
        /// 操作画布进行时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainCanvas_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (MainCommandPanel.SelectedIndex == 4) //涂鸦状态
            {
                if (_current_editing_doodleUI != null)
                {
                    _current_editing_doodleUI.Points.Add(e.Position);
                    MainCanvas.Invalidate();
                }
                return;
            }
            if (_current_tag != null)
            {
                if (_manipulation_type == 2)
                {
                    var deltaX = e.Position.X - _pre_manipulation_position.Value.X;
                    var deltaY = e.Position.Y - _pre_manipulation_position.Value.Y;

                    (_current_tag as TagUI).X += deltaX;
                    (_current_tag as TagUI).Y += deltaY;
                    _pre_manipulation_position = e.Position;

                    MainCanvas.Invalidate();
                }
            }
            if (MainCommandPanel.SelectedIndex == 0)  //剪切
            {
                if (_cropUI != null && _pre_manipulation_position != null)
                {
                    var deltaX = e.Position.X - _pre_manipulation_position.Value.X;
                    var deltaY = e.Position.Y - _pre_manipulation_position.Value.Y;

                    if (_manipulation_type == 0) //移动
                    {
                        (_cropUI as CropUI).Left += deltaX;
                        (_cropUI as CropUI).Top += deltaY;
                    }
                    else if(_manipulation_type == 1) //缩放
                    {
                        (_cropUI as CropUI).Width += deltaX;
                        (_cropUI as CropUI).Height += deltaY;
                    }

                    _pre_manipulation_position = e.Position;
                    MainCanvas.Invalidate();
                }
                return;
            }
            if (MainCommandPanel.SelectedIndex == 2) //墙纸
            {
                if (_wall_paperUI != null && _pre_manipulation_position != null)
                {
                    var deltaX = e.Position.X - _pre_manipulation_position.Value.X;
                    var deltaY = e.Position.Y - _pre_manipulation_position.Value.Y;
                    if (_manipulation_type == 3)  //移动
                    {
                        (_wall_paperUI as WallPaperUI).X += deltaX;
                        (_wall_paperUI as WallPaperUI).Y += deltaY;
                    }
                    else if (_manipulation_type == 4)  //缩放
                    {
                        (_wall_paperUI as WallPaperUI).Width += deltaX * 2;
                        (_wall_paperUI as WallPaperUI).SyncWH();  //只需要设置宽度  高度自动同步
                    }
                    _pre_manipulation_position = e.Position;

                    MainCanvas.Invalidate();
                }
            }
        }
        /// <summary>
        /// 布局命令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LayoutCommand_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var layout_command = sender as GridViewItem;
            if (layout_command == LayoutCommand1) //白边 填充
            {
                _stretch = (_stretch == Stretch.Uniform) ? Stretch.UniformToFill : Stretch.Uniform;
                (LayoutCommand1_Panel.Children[1] as TextBlock).Text = (_stretch == Stretch.Uniform) ? "填充" : "白边";
            }
            else if (layout_command == LayoutCommand2)  //黑底 白底
            {
                _back_color = (_back_color == Colors.White) ? Colors.Black : Colors.White;
                (LayoutCommand2_Panel.Children[1] as TextBlock).Text = (_back_color == Colors.White) ? "黑底" : "白底";
            }
            else if (layout_command == LayoutCommand3)  //画布比例
            {
                _size_mode = (_size_mode + 1) % 3;
                var t = "";
                if (_size_mode == 0)
                {
                    t = "4:3";
                }
                else if (_size_mode == 1)
                {
                    t = "3:4";
                }
                else
                {
                    t = "1:1";
                }
                (LayoutCommand3_Panel.Children[1] as TextBlock).Text = t;
            }
            else if (layout_command == LayoutCommand4)  //旋转
            {
                _rotate = (_rotate + 90) % 360;
            }
            else if (layout_command == LayoutCommand5)  //剪切
            {
                if (_cropUI == null)
                {
                    _cropUI = new CropUI() { Top = 20, Left = 20, Height = 100, Width = 100, DrawColor = Colors.Orange };
                }
            }
            else if (layout_command == LayoutCommand6)  //重选底图
            {
                SelectImage();
            }
            MainCanvas.Invalidate();
            SetCanvas();
        }
        /// <summary>
        /// Slider的值发生变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            MainCanvas.Invalidate();
        }
        /// <summary>
        /// 选择滤镜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Filters_ItemClick(object sender, ItemClickEventArgs e)
        {
            foreach (var item in Filters.Items)
            {
                (((item as GridViewItem).Content as StackPanel).Children[1] as Border).Background = new SolidColorBrush(Colors.Transparent);
                ((((item as GridViewItem).Content as StackPanel).Children[1] as Border).Child as TextBlock).Foreground = new SolidColorBrush(Colors.Gray);
            }
            ((e.ClickedItem as StackPanel).Children[1] as Border).Background = new SolidColorBrush(Colors.Orange);
            (((e.ClickedItem as StackPanel).Children[1] as Border).Child as TextBlock).Foreground = new SolidColorBrush(Colors.White);

            _filter_index = int.Parse((e.ClickedItem as StackPanel).Tag.ToString());

            MainCanvas.Invalidate();
        }
        /// <summary>
        /// 选择墙纸
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void WallPapersList_ItemClick(object sender, ItemClickEventArgs e)
        {
            foreach (var item in WallPapersList.Items)
            {
                (((WallPapersList.ContainerFromItem(item) as GridViewItem).ContentTemplateRoot as RelativePanel).Children[1] as Rectangle).Visibility = Visibility.Collapsed;
            }

            (((WallPapersList.ContainerFromItem(e.ClickedItem) as GridViewItem).ContentTemplateRoot as RelativePanel).Children[1] as Rectangle).Visibility = Visibility.Visible;

            //创建墙纸
            _wall_paperUI = new WallPaperUI() { Editing = true, Height = 100, Width = 100, Image = null, X = 150, Y = 150 };
            MainCanvas.Invalidate();

            CanvasDevice device = CanvasDevice.GetSharedDevice();
            var img = await CanvasBitmap.LoadAsync(device, new Uri(e.ClickedItem.ToString()));
            if (img != null)
            {
                if (_wallpapers != null)
                {
                    (_wall_paperUI as WallPaperUI).Width = img.Size.Width;
                    (_wall_paperUI as WallPaperUI).Height = img.Size.Height;
                    (_wall_paperUI as WallPaperUI).Image = img;

                    MainCanvas.Invalidate();
                }
            }
;        }

        /// <summary>
        /// 点击取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelBtn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (popup != null)
            {
                popup.IsOpen = false;
            }
        }
        /// <summary>
        /// 点击确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OKBtn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            GenerateResultImage();
        }
        #endregion

        #region fields
        private Color _back_color = Colors.White;   //画布背景色
        private Stretch _stretch = Stretch.Uniform;  //底图图片填充方式
        private int _size_mode = 2;  //画布长宽比  0/ 1:1  1/ 4:3  2/ 3:4
        private int _rotate = 0;   //底图图片旋转度数（360度==0度）
        private int _pen_size = 2;   //涂鸦画笔粗细
        private Color _pen_color = Colors.Orange;  //涂鸦画笔颜色
        private DoodleUI _current_editing_doodleUI;  //当前涂鸦对象
        private CanvasBitmap _image;  //底图
        private Point? _pre_manipulation_position;  //操作起始点
        private int _manipulation_type = 0; // 0表示移动剪切对象 1表示缩放剪切对象 2表示移动tag 3表示移动墙纸 4表示缩放墙纸
        private IDrawingUI _current_tag; //当前移动的tag

        private int _filter_index = 0;  //滤镜模板  0表示无滤镜

        IDrawingUI _cropUI;//剪切UI  
        List<IDrawingUI> _tagsUIs;  //Tags
        Stack<IDrawingUI> _doodleUIs = new Stack<IDrawingUI>();  //涂鸦
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
            double w, h;  //画布大小
            if (edit)  //编辑状态
            {
                w = MainCanvas.ActualWidth;
                h = MainCanvas.ActualHeight;
            }
            else  //最终生成图片  有一定的scale
            {
                Rect des = GetImageDrawingRect();

                w = (_image.Size.Width / des.Width) * MainCanvas.Width;
                h = (_image.Size.Height / des.Height) * MainCanvas.Height;
            }
            var scale = edit ? 1 : w / MainCanvas.Width;  //缩放比例

            CanvasDevice device = CanvasDevice.GetSharedDevice();
            CanvasRenderTarget target = new CanvasRenderTarget(device, (float)w, (float)h, 96);
            using (CanvasDrawingSession graphics = target.CreateDrawingSession())
            {
                //绘制背景
                graphics.Clear(_back_color);

                //绘制底图
                DrawBackImage(graphics, scale);
                //绘制涂鸦
                if (_doodleUIs != null && _doodleUIs.Count > 0)
                {
                    var list = _doodleUIs.ToList();list.Reverse();
                    list.ForEach((d) => { d.Draw(graphics, (float)scale); });
                }
                if (_current_editing_doodleUI != null)
                {
                    _current_editing_doodleUI.Draw(graphics, (float)scale); //正在涂鸦对象 在上面
                }
                //绘制贴图
                if (_wall_paperUI != null)
                {
                    _wall_paperUI.Draw(graphics, (float)scale);
                }
                //绘制Tag
                if (_tagsUIs != null)
                {
                    _tagsUIs.ForEach((t) => { t.Draw(graphics, (float)scale); });
                }
                //绘制Crop裁剪工具
                if (_cropUI != null && edit)
                {
                    _cropUI.Draw(graphics, (float)scale);
                }
            }

            return target;
        }
        /// <summary>
        /// 设置画布尺寸比例
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
                    if (w / h <= (double)4 / 3)
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
                    if (h / w <= (double)4 / 3)
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
            MainCanvas.Invalidate();
        }
        /// <summary>
        /// 绘制底图
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="scale"></param>
        private void DrawBackImage(CanvasDrawingSession graphics, double scale)
        {
            if (_image != null)
            {
                Rect des = GetImageDrawingRect();
                des.X *= scale;
                des.Y *= scale;
                des.Width *= scale;
                des.Height *= scale;
                // 滤镜特效

                //亮度
                ICanvasImage image = GetBrightnessEffect(_image);
                //锐化
                image = GetSharpenEffect(image);
                //模糊
                image = GetBlurEffect(image);

                //应用滤镜模板
                image = ApplyFilterTemplate(image);

                graphics.DrawImage(image, des, _image.Bounds);
            }
        }
        /// <summary>
        /// 判断是否点击空白区域 若是，返回true；否则进行相应操作
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private bool ClickEmpty(Point p)
        {
            if (_cropUI != null)
            {
                if ((_cropUI as CropUI).LeftTopRegion.Contains(p))  //取消
                {
                    _cropUI = null;
                    MainCanvas.Invalidate();
                    return false;
                }
                if ((_cropUI as CropUI).RightTopRegion.Contains(p))  //OK
                {
                    //
                    return false;
                }
                if ((_cropUI as CropUI).Region.Contains(p))  //点击的是 剪切对象 区域
                {
                    return false;
                }
            }
            if (_wall_paperUI != null)
            {
                if((_wall_paperUI as WallPaperUI).RightTopRegion.Contains(p)) //取消墙纸
                {
                    _wall_paperUI = null;
                    MainCanvas.Invalidate();
                    return false;
                }
                if ((_wall_paperUI as WallPaperUI).Region.Contains(p)) //点击墙纸
                {
                    (_wall_paperUI as WallPaperUI).Editing = !(_wall_paperUI as WallPaperUI).Editing;
                    MainCanvas.Invalidate();
                    return false;
                }
            }
            if (_tagsUIs != null)
            {
                foreach (var tag in _tagsUIs)
                {
                    if ((tag as TagUI).Region.Contains(p))  //点击的是 tag 区域
                    {
                        (tag as TagUI).ShowCloseBtn = !(tag as TagUI).ShowCloseBtn;
                        MainCanvas.Invalidate();

                        return false;
                    }

                    if ((tag as TagUI).CloseRegion.Contains(p) && (tag as TagUI).ShowCloseBtn) //点击的是tag 关闭按钮
                    {
                        _tagsUIs.Remove(tag);
                        MainCanvas.Invalidate();
                        return false;
                    }
                }
            }
            //点击空白区域  所有元素失去编辑（选中）状态
            if (_wall_paperUI != null)
            {
                (_wall_paperUI as WallPaperUI).Editing = false;
            }
            if (_tagsUIs != null)
            {
                _tagsUIs.ForEach((tag) => { (tag as TagUI).ShowCloseBtn = false; });
            }
            
            return true;
        }
        /// <summary>
        /// 异步从网络位置加载墙纸
        /// </summary>
        private async void LoadWallPapers()
        {
            var url = "http://files.cnblogs.com/files/xiaozhi_5638/Papers.zip" + "?t="+DateTime.Now.Ticks;
            var json = await HttpTool.GetJson(url);
            if (json != null)
            {
                var papers = json["papers"].GetArray();
                var image_url = ""; 
                foreach (var paper in papers)
                {
                    var p =  paper.GetObject();
                    image_url = p["image_url"].GetString();
                    if (!String.IsNullOrEmpty(image_url))
                    {
                        WallPapers.Add(image_url);
                    }
                }
            }
        }
        /// <summary>
        /// 选择底图
        /// </summary>
        private void SelectImage()
        {
            Popup pop = new Popup();

            Border bod = new Border();
            bod.Background = new SolidColorBrush(Color.FromArgb(100, 0x00, 0x00, 0x00));
            bod.Width = this.Width;
            bod.Height = this.Height;

            MainGrid.Children.Add(pop);
            Grid.SetRow(pop, 0);
            Grid.SetRowSpan(pop, 3);

            var imageSelector = new ImageSeletorControl();
            imageSelector.Height = 175;
            imageSelector.Width = bod.Width;
            
            bod.Child = imageSelector;
            imageSelector.VerticalAlignment = VerticalAlignment.Bottom;
             
            bod.Tapped += (sender, e) =>  //点击空白区域关闭popup
              {
                  pop.IsOpen = false;
                  MainGrid.Children.Remove(pop);
              };

            imageSelector.ImageSelected += async (image_file) =>  //选择image
                  {
                      CanvasDevice cd = CanvasDevice.GetSharedDevice();
                      var stream = await image_file.OpenAsync(FileAccessMode.Read);
                      var tmp = await CanvasBitmap.LoadAsync(cd, stream);
                      if (tmp != null)
                      {
                          _image = tmp;
                          MainCanvas.Invalidate();
                      }
                  };
            pop.Child = bod;
            pop.IsOpen = true;
        }
        /// <summary>
        /// 生成最终结果
        /// </summary>
        private async void GenerateResultImage()
        {
            var img = GetDrawings(false);
            if (img != null)
            {
                IRandomAccessStream stream = new InMemoryRandomAccessStream();
                await img.SaveAsync(stream, CanvasBitmapFileFormat.Jpeg);
                BitmapImage result = new BitmapImage();
                stream.Seek(0);
                await result.SetSourceAsync(stream);
                if (ImageEditedCompleted != null)
                {
                    ImageEditedCompleted(result);
                }
                if (popup != null)
                {
                    popup.IsOpen = false;
                }
            }
        }
        /// <summary>
        /// 底图绘制区域
        /// </summary>
        /// <returns></returns>
        private Rect GetImageDrawingRect()
        {
            Rect des;

            var image_w = _image.Size.Width;
            var image_h = _image.Size.Height;

            if (_stretch == Stretch.Uniform)
            {
                var w = MainCanvas.Width - 20;
                var h = MainCanvas.Height - 20;
                if (image_w / image_h > w / h)
                {
                    var left = 10;

                    var width = w;
                    var height = (image_h / image_w) * width;

                    var top = (h - height) / 2 + 10;

                    des = new Rect(left, top, width, height);
                }
                else
                {
                    var top = 10;
                    var height = h;
                    var width = (image_w / image_h) * height;
                    var left = (w - width) / 2 + 10;
                    des = new Rect(left, top, width, height);
                }
            }
            else
            {
                var w = MainCanvas.Width;
                var h = MainCanvas.Height;
                var left = 0;
                var top = 0;
                if (image_w / image_h > w / h)
                {
                    var height = h;
                    var width = (image_w / image_h) * height;
                    des = new Rect(left, top, width, height);
                }
                else
                {
                    var width = w;
                    var height = (image_h / image_w) * width;

                    des = new Rect(left, top, width, height);
                }
            }
            return des;
        }
        #endregion

        #region 滤镜特效
        /// <summary>
        /// 亮度
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private ICanvasImage GetBrightnessEffect(ICanvasImage source)
        {
            var t = Slider1.Value / 500 * 2;
            var exposureEffect = new ExposureEffect
            {
                Source = source,
                Exposure = (float)t
            };

            return exposureEffect;
        }
        /// <summary>
        /// 模糊
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private ICanvasImage GetBlurEffect(ICanvasImage source)
        {
            var t = Slider3.Value / 100 * 12;
            var blurEffect = new GaussianBlurEffect
            {
                Source = source,
                BlurAmount = (float)t
            };
            return blurEffect;
        }
        /// <summary>
        /// 锐化
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private ICanvasImage GetSharpenEffect(ICanvasImage source)
        {
            var sharpenEffect = new SharpenEffect
            {
                Source = source,
                Amount = (float)(Slider2.Value * 0.1)
            };
            return sharpenEffect;
        }
        /// <summary>
        /// 应用滤镜模板
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private ICanvasImage ApplyFilterTemplate(ICanvasImage source)
        {
            if (_filter_index == 0)  //无滤镜
            {
                return source;
            }
            else if (_filter_index == 1)  // 黑白
            {
                return new GrayscaleEffect
                {
                    Source = source
                };
            }
            else if (_filter_index == 2)  //反色
            {
                return new InvertEffect
                {
                    Source = source
                };
            }
            else if (_filter_index == 3) //冷淡
            {
                var hueRotationEffect = new HueRotationEffect
                {
                    Source = source,
                    Angle = 0.5f
                };
                return hueRotationEffect;
            }
            else if (_filter_index == 4)  //美食
            {
                var temperatureAndTintEffect = new TemperatureAndTintEffect
                {
                    Source = source
                };
                temperatureAndTintEffect.Temperature = 0.6f;
                temperatureAndTintEffect.Tint = 0.6f;

                return temperatureAndTintEffect;
            }
            else if (_filter_index == 5) //非主流
            {
                var temperatureAndTintEffect = new TemperatureAndTintEffect
                {
                    Source = source
                };
                temperatureAndTintEffect.Temperature = -0.6f;
                temperatureAndTintEffect.Tint = -0.6f;

                return temperatureAndTintEffect;
            }
            else if (_filter_index == 6) //梦幻
            {
                var vignetteEffect = new VignetteEffect
                {
                    Source = source
                };
                vignetteEffect.Color = Color.FromArgb(255, 0xFF, 0xFF, 0xFF);
                vignetteEffect.Amount = 0.6f;

                return vignetteEffect;
            }
            else if (_filter_index == 7) //雕刻
            {
                var embossEffect = new EmbossEffect
                {
                    Source = source
                };
                embossEffect.Amount = 5;
                embossEffect.Angle = 0;
                return embossEffect;
            }
            else if (_filter_index == 8) //怀旧
            {
                var sepiaEffect = new SepiaEffect
                {
                    Source = source
                };
                sepiaEffect.Intensity = 1;
                return sepiaEffect;
            }
            else
            {
                return source;
            }
        }
        #endregion
    }

    public delegate void ImageEditedCompletedEventHandler(BitmapImage image);
}
