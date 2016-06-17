using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Windows.Foundation;
using Windows.UI;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Brushes;

namespace ImageEditor.DrawingObjects
{
    public class CropUI : IDrawingUI
    {
        public double Left
        {
            get;set;
        }
        public double Top
        {
            get;set;
        }
        public double Width
        {
            get;set;
        }
        public double Height
        {
            get;set;
        }
        public Color DrawColor
        {
            get;set;
        }
        public Rect Region
        {
            get
            {
                return new Rect(Left, Top, Width, Height);
            }
        }
        /// <summary>
        /// 左上角 点击取消区域
        /// </summary>
        public Rect LeftTopRegion
        {
            get
            {
                return new Rect(Left - 8, Top - 8, 16, 16);
            }
        }
        /// <summary>
        /// 右上角 点击确定区域
        /// </summary>
        public Rect RightTopRegion
        {
            get
            {
                return new Rect(Left + Width - 8, Top - 8, 16, 16);
            }
        }
        /// <summary>
        /// 右下角 放大区域
        /// </summary>
        public Rect RightBottomRegion
        {
            get
            {
                return new Rect(Left + Width - 8, Top + Height - 8, 16, 16);
            }
        }
        public Size Bound  //画布边界
        {
            get; set;
        }
        public void Draw(CanvasDrawingSession graphics, float scale)
        {
            var stickness = 1;
            var radius = 8;
            CanvasStrokeStyle style = new CanvasStrokeStyle();
            style.DashCap = CanvasCapStyle.Square;
            style.DashStyle = CanvasDashStyle.Dash;
            style.StartCap = CanvasCapStyle.Round;
            style.EndCap = CanvasCapStyle.Round;

            graphics.FillRectangle(Region, Color.FromArgb(100, 0XFF, 0XFF, 0XFF));
            graphics.DrawRectangle(Region, DrawColor, stickness); //矩形
            if (Width > 50 && Height > 50)  //当满足条件时  绘制九宫格
            {
                graphics.DrawLine((float)Left, (float)(Top + (Height / 3)), (float)(Left + Width), (float)(Top + Height / 3), Colors.Orange, 0.3f, style);
                graphics.DrawLine((float)Left, (float)(Top + (Height * 2 / 3)), (float)(Left + Width), (float)(Top + Height * 2 / 3), Colors.Orange, 0.3f, style);
                graphics.DrawLine((float)(Left + Width / 3), (float)Top, (float)(Left + Width / 3), (float)(Top + Height), Colors.Orange, 0.3f, style);
                graphics.DrawLine((float)(Left + Width * 2 / 3), (float)Top, (float)(Left + Width * 2 / 3), (float)(Top + Height), Colors.Orange, 0.3f, style);
            }
            graphics.FillCircle((float)Left, (float)Top, radius, DrawColor);  //×
            graphics.FillCircle((float)(Left + Width), (float)Top, radius, DrawColor); //√
            graphics.FillCircle((float)(Left + Width), (float)(Top + Height), radius, DrawColor); //缩放
        }
    }
}
