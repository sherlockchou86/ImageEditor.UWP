using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Windows.Foundation;
using Windows.UI;
using Microsoft.Graphics.Canvas.Text;

namespace ImageEditor.DrawingObjects
{
    public class TagUI : IDrawingUI
    {
        public double X
        {
            get;set;
        }
        public double Y
        {
            get; set;
        }
        public string TagText  //显示Text
        {
            get; set;
        }
        public int TagType  //Tag类型  0表示At好友  1表示热门话题  2表示插入位置
        {
            get; set;
        }
        public Size Bound  //画布边界
        {
            get; set;
        }
        public void Draw(CanvasDrawingSession graphics)
        {
            var radius = 4;
            var color = Color.FromArgb(100, 0x00, 0x00, 0x00);
            graphics.FillCircle((float)X, (float)Y, radius, color);

            
            var ctFormat = new CanvasTextFormat { FontSize = 30.0f, WordWrapping = CanvasWordWrapping.NoWrap };
            var ctLayout = new CanvasTextLayout(graphics, TagText, ctFormat, 0.0f, 0.0f);
            //字体占用高度、宽度
            var width = ctLayout.DrawBounds.Width;
            var height = ctLayout.DrawBounds.Height;

            var w = X + width + 10;
            if (w > Bound.Width)  //超出画布边界
            {

            }
            else
            {
                
            }
        }
    }
}
