using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Windows.UI;
using Windows.Foundation;
using Microsoft.Graphics.Canvas.Geometry;

namespace ImageEditor.DrawingObjects
{
    /// <summary>
    /// 涂鸦
    /// </summary>
    public class DoodleUI : IDrawingUI
    {
        public Color DrawingColor
        {
            get; set;
        }
        public int DrawingSize
        {
            get; set;
        }
        private List<Point> _points;
        public List<Point> Points
        {
            get
            {
                if (_points == null)
                {
                    _points = new List<Point>();
                }
                return _points;
            }
        }
        public void Draw(CanvasDrawingSession graphics, float scale)
        {
            if (_points != null && _points.Count>0)
            {
                if (_points.Count == 1)
                {
                    graphics.DrawLine((float)_points[0].X * scale, (float)_points[0].Y * scale, (float)_points[0].X * scale, (float)_points[0].Y * scale, DrawingColor, DrawingSize * scale);
                }
                else
                {
                    var style = new CanvasStrokeStyle();
                    style.DashCap = CanvasCapStyle.Round;
                    style.StartCap = CanvasCapStyle.Round;
                    style.EndCap = CanvasCapStyle.Round;
                    for (int i = 0; i < _points.Count - 1; ++i)
                    {
                        graphics.DrawLine((float)_points[i].X * scale, (float)_points[i].Y * scale, (float)_points[i + 1].X * scale, (float)_points[i + 1].Y * scale, DrawingColor, DrawingSize * scale, style);

                    }
                }
            }
        }
    }
}
