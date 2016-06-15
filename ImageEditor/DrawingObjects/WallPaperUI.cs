using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
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
        public void Draw(CanvasDrawingSession graphics)
        {
            if (Image != null)
            {
                graphics.DrawImage(Image, new Rect(X - (Width / 2), Y - (Height / 2), Width, Height));
            }
            else
            {
                graphics.DrawText("loading...", (float)X - 10, (float)Y - 2, Colors.Orange, new Microsoft.Graphics.Canvas.Text.CanvasTextFormat() { FontSize = 11 });
            }

            if (Editing)
            {
                graphics.DrawRectangle(new Rect(X - Width / 2 - 2, Y - Height / 2 - 2, Width + 4, Height + 4), Windows.UI.Colors.Orange);
                graphics.FillCircle((float)(X + Width / 2 + 2), (float)(Y - Height / 2 - 2), 8, Colors.Orange);
                graphics.FillCircle((float)(X + Width / 2 + 2), (float)(Y + Height / 2 - 2), 8, Colors.Orange);
            }
        }
    }
}
