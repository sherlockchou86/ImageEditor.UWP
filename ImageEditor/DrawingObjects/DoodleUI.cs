using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Windows.UI;
using Windows.Foundation;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Brushes;

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
        CanvasBitmap _brush_image;
        public async void InitImageBrush()
        {
            if (DrawingColor == Colors.Transparent)
            {
                if (_brush_image == null)
                {
                    CanvasDevice cd = CanvasDevice.GetSharedDevice();
                    _brush_image = await CanvasBitmap.LoadAsync(cd, new Uri("ms-appx:///Images/default_back.png"));
                }
            }
        }
        public void Draw(CanvasDrawingSession graphics, float scale)
        {
            if (_points != null && _points.Count>0)
            {
                ICanvasBrush brush;
                if (DrawingColor == Colors.Transparent)
                {
                    if (_brush_image == null)
                        return;
                    brush = new CanvasImageBrush(graphics, _brush_image);
                }
                else
                {
                    brush = new CanvasSolidColorBrush(graphics, DrawingColor);
                }
                if (_points.Count == 1)
                {
                    graphics.DrawLine((float)_points[0].X * scale, (float)_points[0].Y * scale, (float)_points[0].X * scale, (float)_points[0].Y * scale, brush, DrawingSize * scale);
                }
                else
                {
                    var style = new CanvasStrokeStyle();
                    style.DashCap = CanvasCapStyle.Round;
                    style.StartCap = CanvasCapStyle.Round;
                    style.EndCap = CanvasCapStyle.Round;
                    for (int i = 0; i < _points.Count - 1; ++i)
                    {
                        graphics.DrawLine((float)_points[i].X * scale, (float)_points[i].Y * scale, (float)_points[i + 1].X * scale, (float)_points[i + 1].Y * scale, brush, DrawingSize * scale, style);

                    }
                }
            }
        }
    }
}
