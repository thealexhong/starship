#region Usings

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

#endregion

namespace QA5_Trainer.CSharp.UI
{
    public class ColorMap : UserControl
    {
        private Color _BackColor;
        private Bitmap colorMap;
        private Color _ForeColor;

        public ColorMap()
        {
            InitializeComponent();
            SetStyle(
                ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                ControlStyles.Opaque | ControlStyles.UserPaint, true);
            colorMap = CreateColorMap();
        }

        private void ColorMap_Resize(object sender, EventArgs e)
        {
            colorMap = CreateColorMap();
        }

        private void ColorPicker_MouseUp(object sender, MouseEventArgs e)
        {
            var point = new Point(e.X, e.Y);
            switch (e.Button)
            {
                case MouseButtons.Left:
                    ForeColor = LocationToColor(point);
                    break;

                case MouseButtons.Right:
                    BackColor = LocationToColor(point);
                    return;
            }
        }

        private void ColorPicker_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImageUnscaled(colorMap, 0, 0);
        }

        private Bitmap CreateColorMap()
        {
            var image = new Bitmap(ClientSize.Width, ClientSize.Height);
            using (Graphics graphics = Graphics.FromImage(image))
            {
                int x = ClientRectangle.Width/7;
                var rect = new Rectangle(x/2, 0, x*2, ClientRectangle.Height);
                graphics.CompositingMode = CompositingMode.SourceOver;
                var brush2 = new LinearGradientBrush(ClientRectangle, Color.FromArgb(0xff, 0, 0, 0),
                                                                     Color.FromArgb(0xff, 0xff, 0xff, 0xff), 0f);
                graphics.FillRectangle(brush2, ClientRectangle);
                var path = new GraphicsPath();
                path.StartFigure();
                path.AddEllipse(rect);
                path.CloseFigure();
                var brush = new PathGradientBrush(path)
                                              {
                                                  CenterColor = Color.FromArgb(0xff, 0xff, 0, 0)
                                              };
                var colorArray = new[] {Color.FromArgb(0, 0, 0, 0)};
                brush.SurroundColors = colorArray;
                graphics.FillEllipse(brush, rect);
                rect.Offset(x, 0);
                path = new GraphicsPath();
                path.StartFigure();
                path.AddEllipse(rect);
                path.CloseFigure();
                brush = new PathGradientBrush(path)
                            {
                                CenterColor = Color.FromArgb(0xff, 0xff, 0xff, 0)
                            };
                colorArray = new[] {Color.FromArgb(0, 0, 0, 0)};
                brush.SurroundColors = colorArray;
                graphics.FillEllipse(brush, rect);
                rect.Offset(x, 0);
                path = new GraphicsPath();
                path.StartFigure();
                path.AddEllipse(rect);
                path.CloseFigure();
                brush = new PathGradientBrush(path)
                            {
                                CenterColor = Color.FromArgb(0xff, 0, 0xff, 0)
                            };
                colorArray = new[] {Color.FromArgb(0, 0, 0, 0)};
                brush.SurroundColors = colorArray;
                graphics.FillEllipse(brush, rect);
                rect.Offset(x, 0);
                path = new GraphicsPath();
                path.StartFigure();
                path.AddEllipse(rect);
                path.CloseFigure();
                brush = new PathGradientBrush(path)
                            {
                                CenterColor = Color.FromArgb(0xff, 0, 0xff, 0xff)
                            };
                colorArray = new[] {Color.FromArgb(0, 0, 0, 0)};
                brush.SurroundColors = colorArray;
                graphics.FillEllipse(brush, rect);
                rect.Offset(x, 0);
                path = new GraphicsPath();
                path.StartFigure();
                path.AddEllipse(rect);
                path.CloseFigure();
                brush = new PathGradientBrush(path)
                            {
                                CenterColor = Color.FromArgb(0xff, 0, 0, 0xff)
                            };
                colorArray = new[] {Color.FromArgb(0, 0, 0, 0)};
                brush.SurroundColors = colorArray;
                graphics.FillEllipse(brush, rect);
                rect.Offset(x, 0);
                path = new GraphicsPath();
                path.StartFigure();
                path.AddEllipse(rect);
                path.CloseFigure();
                brush = new PathGradientBrush(path)
                            {
                                CenterColor = Color.FromArgb(0xff, 0xff, 0, 0xff)
                            };
                colorArray = new[] {Color.FromArgb(0, 0, 0, 0)};
                brush.SurroundColors = colorArray;
                graphics.FillEllipse(brush, rect);
                rect.Offset(x, 0);
                path = new GraphicsPath();
                path.StartFigure();
                path.AddEllipse(rect);
                path.CloseFigure();
                brush = new PathGradientBrush(path)
                            {
                                CenterColor = Color.FromArgb(0xff, 0xff, 0, 0)
                            };
                colorArray = new[] {Color.FromArgb(0, 0, 0, 0)};
                brush.SurroundColors = colorArray;
                graphics.FillEllipse(brush, rect);
            }
            return image;
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            Name = "ColorMap";
            Size = new Size(240, 0x2d);
            Paint += ColorPicker_Paint;
            Resize += ColorMap_Resize;
            MouseUp += ColorPicker_MouseUp;
            ResumeLayout(false);
        }

        private Color LocationToColor(Point point)
        {
            return colorMap.GetPixel(point.X, point.Y);
        }

        public new Color BackColor
        {
            get { return _BackColor; }
            set
            {
                _BackColor = value;
                OnBackColorChanged(new EventArgs());
            }
        }

        public new Color ForeColor
        {
            get { return _ForeColor; }
            set
            {
                _ForeColor = value;
                OnForeColorChanged(new EventArgs());
            }
        }
    }
}