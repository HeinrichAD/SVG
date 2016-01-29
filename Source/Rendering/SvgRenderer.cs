using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Svg
{
    /// <summary>
    /// Convenience wrapper around a graphics object
    /// </summary>
    public class SvgRenderer : IDisposable, IGraphicsProvider, ISvgRenderer
    {
        protected virtual Graphics InnerGraphics { get; set; }
        protected virtual Stack<ISvgBoundable> Boundables { get; set; }

        public virtual void SetBoundable(ISvgBoundable boundable)
        {
            Boundables.Push(boundable);
        }
        public virtual ISvgBoundable GetBoundable()
        {
            return Boundables.Peek();
        }
        public virtual ISvgBoundable PopBoundable()
        {
            return Boundables.Pop();
        }

        public virtual float DpiY
        {
            get { return InnerGraphics.DpiY; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ISvgRenderer"/> class.
        /// </summary>
        protected SvgRenderer(Graphics graphics)
        {
            this.Boundables = new Stack<ISvgBoundable>();
            this.InnerGraphics = graphics;
        }

        public virtual void DrawImage(Image image, RectangleF destRect, RectangleF srcRect, GraphicsUnit graphicsUnit)
        {
            InnerGraphics.DrawImage(image, destRect, srcRect, graphicsUnit);
        }
        public virtual void DrawImageUnscaled(Image image, Point location)
        {
            this.InnerGraphics.DrawImageUnscaled(image, location);
        }
        public virtual void DrawPath(Pen pen, GraphicsPath path)
        {
            this.InnerGraphics.DrawPath(pen, path);
        }
        public virtual void FillPath(Brush brush, GraphicsPath path)
        {
            this.InnerGraphics.FillPath(brush, path);
        }
        public virtual Region GetClip()
        {
            return this.InnerGraphics.Clip;
        }
        public virtual void RotateTransform(float fAngle, MatrixOrder order = MatrixOrder.Append)
        {
            this.InnerGraphics.RotateTransform(fAngle, order);
        }
        public virtual void ScaleTransform(float sx, float sy, MatrixOrder order = MatrixOrder.Append)
        {
            this.InnerGraphics.ScaleTransform(sx, sy, order);
        }
        public virtual void SetClip(Region region, CombineMode combineMode = CombineMode.Replace)
        {
            this.InnerGraphics.SetClip(region, combineMode);
        }
        public virtual void TranslateTransform(float dx, float dy, MatrixOrder order = MatrixOrder.Append)
        {
            this.InnerGraphics.TranslateTransform(dx, dy, order);
        }

        public virtual CompositingMode CompositingMode
        {
          get { return this.InnerGraphics.CompositingMode; }
          set { this.InnerGraphics.CompositingMode = value; }
        }

        public virtual SmoothingMode SmoothingMode
        {
            get { return this.InnerGraphics.SmoothingMode; }
            set { this.InnerGraphics.SmoothingMode = value; }
        }

        public virtual Matrix Transform
        {
            get { return this.InnerGraphics.Transform; }
            set { this.InnerGraphics.Transform = value; }
        }

        public virtual void Dispose()
        {
            this.InnerGraphics.Dispose();
        }

        Graphics IGraphicsProvider.GetGraphics()
        {
            return InnerGraphics;
        }

        /// <summary>
        /// Creates a new <see cref="ISvgRenderer"/> from the specified <see cref="Image"/>.
        /// </summary>
        /// <param name="image"><see cref="Image"/> from which to create the new <see cref="ISvgRenderer"/>.</param>
        public static ISvgRenderer FromImage(Image image)
        {
            var g = Graphics.FromImage(image);
            g.TextRenderingHint = TextRenderingHint.AntiAlias;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.TextContrast = 1;
            return new SvgRenderer(g);
        }

        /// <summary>
        /// Creates a new <see cref="ISvgRenderer"/> from the specified <see cref="Graphics"/>.
        /// </summary>
        /// <param name="graphics">The <see cref="Graphics"/> to create the renderer from.</param>
        public static ISvgRenderer FromGraphics(Graphics graphics)
        {
            return new SvgRenderer(graphics);
        }

        public static ISvgRenderer FromNull()
        {
            var img = new Bitmap(1, 1);
            return SvgRenderer.FromImage(img);
        }
    }
}