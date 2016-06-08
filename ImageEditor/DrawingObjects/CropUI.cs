using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Windows.Foundation;
using Windows.UI;

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
        public void Draw(CanvasDrawingSession graphics)
        {
            var stickness = 3;
            var radius = 6;
            graphics.DrawRectangle(Region, DrawColor, stickness);  //矩形
            graphics.FillCircle((float)Left, (float)Top, radius, DrawColor);  //×
            graphics.DrawText("×", (float)Left, (float)Top, Colors.White);
            graphics.DrawCircle((float)(Left + Width), (float)Top, radius, DrawColor); //√
            graphics.DrawText("√", (float)(Left + Width), (float)Top, Colors.Orange);
            graphics.DrawCircle((float)(Left + Width), (float)(Top + Height), radius, DrawColor); //缩放
            graphics.DrawText(@"\", (float)(Left + Width), (float)(Top + Height), Colors.Orange);
        }
    }
}
