using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Windows.Foundation;
using Windows.UI;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.Geometry;

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
        private Rect _region;
        public Rect Region
        {
            get
            {
                return _region;
            }
        }
        public void Draw(CanvasDrawingSession graphics)
        {
            var radius = 4;
            var color = Color.FromArgb(200, 0xFF, 0xFF, 0xFF);
            var color2 = Color.FromArgb(200, 0x00, 0x00, 0x00);

            graphics.FillCircle((float)X, (float)Y, radius, color);
            graphics.DrawCircle((float)X, (float)Y, radius, color2);
            
            var ctFormat = new CanvasTextFormat { FontSize = 10.0f, WordWrapping = CanvasWordWrapping.NoWrap, FontFamily="微软雅黑" };
            var ctLayout = new CanvasTextLayout(graphics, TagText, ctFormat, 0.0f, 0.0f);
            //字体占用高度、宽度
            var width = ctLayout.DrawBounds.Width + 10;
            var height = ctLayout.DrawBounds.Height + 10;

            var w = X + width + 10;
            var pathBuilder = new CanvasPathBuilder(graphics);

            if (w > Bound.Width)  //超出画布边界
            {
                pathBuilder.BeginFigure((float)X - 5, (float)Y);
                pathBuilder.AddLine((float)X - 5 - 6, (float)Y - (float)height / 2);
                pathBuilder.AddLine((float)X - (float)width - 5 - 6, (float)Y - (float)height / 2);
                pathBuilder.AddLine((float)X - (float)width - 5 - 6, (float)Y + (float)height / 2);
                pathBuilder.AddLine((float)X - 5 - 6, (float)Y + (float)height / 2);

                pathBuilder.EndFigure(CanvasFigureLoop.Closed);

                var geometry = CanvasGeometry.CreatePath(pathBuilder);

                graphics.FillGeometry(geometry, color2);
                graphics.DrawText(TagText, (float)X - (float)width - 5 - 6 + 5, (float)Y - (float)height / 2 + 3, Colors.White, ctFormat);

                _region = new Rect((float)X - (float)width - 5 - 6, (float)Y - (float)height / 2, width, height);
            }
            else
            {
                pathBuilder.BeginFigure((float)X + 5, (float)Y);
                pathBuilder.AddLine((float)X + 5 + 6, (float)Y - (float)height / 2);
                pathBuilder.AddLine((float)X + (float)width + 5 + 6, (float)Y - (float)height / 2);
                pathBuilder.AddLine((float)X + (float)width + 5 + 6, (float)Y + (float)height / 2);
                pathBuilder.AddLine((float)X + 5 + 6, (float)Y + (float)height / 2);

                pathBuilder.EndFigure(CanvasFigureLoop.Closed);

                var geo = CanvasGeometry.CreatePath(pathBuilder);

                graphics.FillGeometry(geo, color2);
                graphics.DrawText(TagText, (float)X + 5 + 4 + 5, (float)Y - (float)height / 2 + 3, Colors.White, ctFormat);

                _region = new Rect((float)X + 5 + 6, (float)Y - (float)height / 2, width, height);
            }

        }
    }
}
