using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;

namespace ImageEditor.DrawingObjects
{
    public class WallPaperUI:IDrawingUI
    {
        public double X
        {
            get;set;
        }
        public double Y
        {
            get;set;
        }
        public double Rotate
        {
            get;set;
        }
        public CanvasBitmap Image
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
        public bool Editing
        {
            get;set;
        }
        public Size Bound
        {
            get;set;
        }
        public Rect Region
        {
            get
            {
                return new Rect(X - Width / 2, Y - Height / 2, Width, Height);
            }
        }
        public Rect RightTopRegion
        {
            get
            {
                return new Rect(X + Width / 2 + 2 - 8, Y - Height / 2 - 2 - 8, 16, 16);
            }
        }
        public Rect RightBottomRegion
        {
            get
            {
                return new Rect(X + Width / 2 + 2 - 8, Y + Height / 2 + 2 - 8, 16, 16);
            }
        }
        public void SyncWH()
        {
            Height = (Image.Size.Height / Image.Size.Width) * Width;
        }
        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="graphics"></param>
        public void Draw(CanvasDrawingSession graphics, float scale)
        {
            if (Image != null)
            {
                graphics.DrawImage(Image, new Rect((X - (Width / 2)) * scale, (Y - (Height / 2)) * scale, Width * scale, Height * scale));
            }
            else
            {
                graphics.DrawText("loading...", ((float)X - 12) * scale, ((float)Y - 2) * scale, Colors.Orange, new Microsoft.Graphics.Canvas.Text.CanvasTextFormat() { FontSize = 11 });
            }

            if (Editing && scale == 1) //如果是编辑状态 且 没有缩放
            {
                graphics.DrawRectangle(new Rect(X - Width / 2 - 2, Y - Height / 2 - 2, Width + 4, Height + 4), Windows.UI.Colors.Orange, 0.5f, new CanvasStrokeStyle() { DashStyle = CanvasDashStyle.DashDot });
                graphics.FillCircle((float)(X + Width / 2 + 2), (float)(Y - Height / 2 - 2), 8, Colors.Orange);  //取消
                graphics.DrawLine((float)(X + Width/2 + 2) - 4, (float)(Y - Height/2 - 2) - 4, (float)(X + Width / 2 + 2) + 4, (float)(Y - Height / 2 - 2) + 4, Colors.White);
                graphics.DrawLine((float)(X + Width / 2 + 2) - 4, (float)(Y - Height / 2 - 2) + 4, (float)(X + Width / 2 + 2) + 4, (float)(Y - Height / 2 - 2) - 4, Colors.White);
                graphics.FillCircle((float)(X + Width / 2 + 2), (float)(Y + Height / 2 + 2), 8, Colors.Orange); //缩放
                graphics.DrawLine((float)(X + Width / 2 + 2 - 4), (float)(Y + Height / 2 + 2 - 4), (float)(X + Width / 2 + 2 + 4), (float)(Y + Height / 2 + 2 + 4), Colors.White);
                graphics.DrawLine((float)(X + Width / 2 + 2 - 4), (float)(Y + Height / 2 + 2 - 4), (float)(X + Width / 2 + 2 - 4), (float)(Y + Height / 2 + 2), Colors.White);
                graphics.DrawLine((float)(X + Width / 2 + 2 - 4), (float)(Y + Height / 2 + 2 - 4), (float)(X + Width / 2 + 2), (float)(Y + Height / 2 + 2 - 4), Colors.White);
                graphics.DrawLine((float)(X + Width / 2 + 2 + 4), (float)(Y + Height / 2 + 2 + 4), (float)(X + Width / 2 + 2), (float)(Y + Height / 2 + 2 + 4), Colors.White);
                graphics.DrawLine((float)(X + Width / 2 + 2 + 4), (float)(Y + Height / 2 + 2 + 4), (float)(X + Width / 2 + 2 + 4), (float)(Y + Height / 2 + 2), Colors.White);
            }
        }
    }
}
