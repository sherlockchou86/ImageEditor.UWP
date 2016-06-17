using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;

namespace ImageEditor.DrawingObjects
{
    interface IDrawingUI
    {
        void Draw(CanvasDrawingSession graphics, float scale);
    }
}
