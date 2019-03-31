using System;
using System.Diagnostics;

using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;

using FFramework.Overlay.PInvoke;

using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using TextAntialiasMode = SharpDX.Direct2D1.TextAntialiasMode;
using FactoryType = SharpDX.Direct2D1.FactoryType;

using Factory = SharpDX.Direct2D1.Factory;
using FontFactory = SharpDX.DirectWrite.Factory;

namespace FFramework.Overlay.Drawing
{
    public class Graphics : IDisposable
    {
        private HwndRenderTargetProperties _deviceProperties;
        private WindowRenderTarget _device;

        private Factory _factory;
        private FontFactory _fontFactory;

        private StrokeStyle _strokeStyle;

        private Stopwatch _watch;

        private volatile bool _resize;

        private volatile int _resizeWidth;
        private volatile int _resizeHeight;

        private volatile int _fpsCount;
        
        public bool IsResizing => _resize;

        public bool IsInitialized { get; private set; }
        public bool IsDrawing { get; private set; }

        public bool MeasureFPS { get; set; }

        public bool PerPrimitiveAntiAliasing { get; set; }
        public bool TextAntiAliasing { get; set; }
        public bool VSync { get; set; }
        public bool UseMultiThreadedFactories { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public int FPS { get; private set; }

        public IntPtr WindowHandle { get; set; }

        public Graphics()
        {
            _watch = new Stopwatch();

            PerPrimitiveAntiAliasing = false;
            TextAntiAliasing = true;
            VSync = false;
            UseMultiThreadedFactories = false;
        }

        public Graphics(IntPtr windowHandle) : this()
        {
            WindowHandle = windowHandle;
        }

        public Graphics(IntPtr windowHandle, int width, int height) : this()
        {
            WindowHandle = windowHandle;
            Width = width;
            Height = height;
        }

        ~Graphics()
        {
            Dispose(false);
        }

        public void Setup()
        {
            if (IsInitialized) throw new InvalidOperationException("Graphics device is already initialized");
            if (Width <= 0 || Height <= 0) throw new ArgumentOutOfRangeException("Width or Height is not valid");
            if (WindowHandle == IntPtr.Zero) throw new ArgumentOutOfRangeException("WindowHandle is zero");
            if (!User32.IsWindow(WindowHandle)) throw new ArgumentOutOfRangeException("WindowHandle is not valid");

            _factory = new Factory(UseMultiThreadedFactories ? FactoryType.MultiThreaded : FactoryType.SingleThreaded);
            _fontFactory = new FontFactory();

            _deviceProperties = new HwndRenderTargetProperties()
            {
                Hwnd = WindowHandle,
                PixelSize = new Size2(Width, Height),
                PresentOptions = VSync ? PresentOptions.None : PresentOptions.Immediately
            };

            var renderProperties = new RenderTargetProperties(
                RenderTargetType.Default,
                new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied),       
                96.0f,
                96.0f,
                RenderTargetUsage.None,
                FeatureLevel.Level_DEFAULT);

            try
            {
                _device = new WindowRenderTarget(_factory, renderProperties, _deviceProperties);
            }
            catch (SharpDXException)
            {
                try
                {
                    renderProperties.PixelFormat = new PixelFormat(Format.R8G8B8A8_UNorm, AlphaMode.Premultiplied);    
                    _device = new WindowRenderTarget(_factory, renderProperties, _deviceProperties);
                }
                catch (SharpDXException)
                {
                    renderProperties.PixelFormat = new PixelFormat(Format.Unknown, AlphaMode.Premultiplied);      
                    _device = new WindowRenderTarget(_factory, renderProperties, _deviceProperties);
                }
            }

            _device.AntialiasMode = PerPrimitiveAntiAliasing ? AntialiasMode.PerPrimitive : AntialiasMode.Aliased;        
            _device.TextAntialiasMode = TextAntiAliasing ? TextAntialiasMode.Grayscale : TextAntialiasMode.Aliased;            

            _strokeStyle = new StrokeStyle(_factory, new StrokeStyleProperties
            {
                DashCap = CapStyle.Flat,
                DashOffset = -1.0f,
                DashStyle = DashStyle.Dash,
                EndCap = CapStyle.Flat,
                LineJoin = LineJoin.MiterOrBevel,
                MiterLimit = 1.0f,
                StartCap = CapStyle.Flat
            });

            IsInitialized = true;
        }

        public void Destroy()
        {
            if (!IsInitialized) throw new InvalidOperationException("D2DDevice needs to be initialized first");

            try
            {
                _strokeStyle.Dispose();
                _fontFactory.Dispose();
                _factory.Dispose();
                _device.Dispose();
            }
            catch { }
            
            IsInitialized = false;
        }

        public void Resize(int width, int height)
        {
            if (Width == width && Height == height) return;

            if (IsInitialized)
            {
                _resizeWidth = width;
                _resizeHeight = height;
                _resize = true;
            }
            else
            {
                Width = width;
                Height = height;
            }
        }

        public void BeginScene()
        {
            if (!IsInitialized) throw new InvalidOperationException("The DirectX device is not initialized");
            if (IsDrawing) return;
            
            if (_resize)
            {
                try
                {
                    _resize = false;

                    Width = _resizeWidth;
                    Height = _resizeHeight;

                    _device.Resize(new Size2(_resizeWidth, _resizeHeight));
                }
                catch { }    
            }

            if (MeasureFPS && !_watch.IsRunning)
            {
                _watch.Restart();
            }

            _device.BeginDraw();

            IsDrawing = true;
        }

        public void ClearScene()
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            _device.Clear(null);
        }

        public void ClearScene(Color color)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            _device.Clear(color);
        }

        public void ClearScene(SolidBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            _device.Clear(brush.Color);
        }

        public void EndScene()
        {
            if (!IsInitialized) throw new InvalidOperationException("The DirectX device is not initialized");
            if (!IsDrawing) return;

            var result = _device.TryEndDraw(out long _, out long _);

            if (result.Failure)
            {
                Destroy();
                Setup();
            }
            
            if (MeasureFPS && _watch.IsRunning)
            {
                _fpsCount++;

                if (_watch.ElapsedMilliseconds >= 1000)
                {
                    FPS = _fpsCount;

                    _fpsCount = 0;

                    _watch.Stop();
                }
            }

            IsDrawing = false;
        }

        public Scene UseScene()
        {
            return new Scene(this);
        }

        public SolidBrush CreateSolidBrush(float r, float g, float b, float a = 1.0f)
        {
            if (!IsInitialized) throw new InvalidOperationException("The DirectX device is not initialized");

            return new SolidBrush(_device, new Color(r, g, b, a));
        }

        public SolidBrush CreateSolidBrush(int r, int g, int b, int a = 255)
        {
            if (!IsInitialized) throw new InvalidOperationException("The DirectX device is not initialized");

            return new SolidBrush(_device, new Color(r, g, b, a));
        }

        public SolidBrush CreateSolidBrush(Color color)
        {
            if (!IsInitialized) throw new InvalidOperationException("The DirectX device is not initialized");

            return new SolidBrush(_device, color);
        }

        public Font CreateFont(string fontFamilyName, float size, bool bold = false, bool italic = false, bool wordWrapping = false)
        {
            if (!IsInitialized) throw new InvalidOperationException("The DirectX device is not initialized");

            return new Font(_fontFactory, fontFamilyName, size, bold, italic, wordWrapping);
        }

        public Image CreateImage(byte[] bytes)
        {
            if (!IsInitialized) throw new InvalidOperationException("The DirectX device is not initialized");

            return new Image(_device, bytes);
        }

        public Image CreateImage(string path)
        {
            if (!IsInitialized) throw new InvalidOperationException("The DirectX device is not initialized");

            return new Image(_device, path);
        }

        public Geometry CreateGeometry()
        {
            return new Geometry(this);
        }

        public void DrawCircle(IBrush brush, float x, float y, float radius, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            _device.DrawEllipse(new SharpDX.Direct2D1.Ellipse(new RawVector2(x, y), radius, radius), brush.Brush, stroke);
        }

        public void DrawCircle(IBrush brush, Point location, float radius, float stroke) => DrawCircle(brush, location.X, location.Y, radius, stroke);

        public void DrawCircle(IBrush brush, Circle circle, float stroke) => DrawCircle(brush, circle.Location.X, circle.Location.Y, circle.Radius, stroke);

        public void OutlineCircle(IBrush outline, IBrush fill, float x, float y, float radius, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            var ellipse = new SharpDX.Direct2D1.Ellipse(new RawVector2(x, y), radius, radius);
            
            _device.DrawEllipse(ellipse, fill.Brush, stroke);

            float halfStroke = stroke * 0.5f;

            ellipse.RadiusX += halfStroke;
            ellipse.RadiusY += halfStroke;

            _device.DrawEllipse(ellipse, outline.Brush, halfStroke);

            ellipse.RadiusX -= stroke;
            ellipse.RadiusY -= stroke;

            _device.DrawEllipse(ellipse, outline.Brush, halfStroke);
        }

        public void OutlineCircle(IBrush outline, IBrush fill, Point location, float radius, float stroke) => OutlineCircle(outline, fill, location.X, location.Y, radius, stroke);

        public void OutlineCircle(IBrush outline, IBrush fill, Circle circle, float stroke) => OutlineCircle(outline, fill, circle.Location.X, circle.Location.Y, circle.Radius, stroke);

        public void DashedCircle(IBrush brush, float x, float y, float radius, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            _device.DrawEllipse(new SharpDX.Direct2D1.Ellipse(new RawVector2(x, y), radius, radius), brush.Brush, stroke, _strokeStyle);
        }

        public void DashedCircle(IBrush brush, Point location, float radius, float stroke) => DashedCircle(brush, location.X, location.Y, radius, stroke);

        public void DashedCircle(IBrush brush, Circle circle, float stroke) => DashedCircle(brush, circle.Location.X, circle.Location.Y, circle.Radius, stroke);

        public void FillCircle(IBrush brush, float x, float y, float radius)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            _device.FillEllipse(new SharpDX.Direct2D1.Ellipse(new RawVector2(x, y), radius, radius), brush.Brush);
        }

        public void FillCircle(IBrush brush, Point location, float radius) => FillCircle(brush, location.X, location.Y, radius);

        public void FillCircle(IBrush brush, Circle circle) => FillCircle(brush, circle.Location.X, circle.Location.Y, circle.Radius);

        public void OutlineFillCircle(IBrush outline, IBrush fill, float x, float y, float radius, float stroke)
        {
            var ellipseGeometry = new EllipseGeometry(_factory, new SharpDX.Direct2D1.Ellipse(new RawVector2(x, y), radius, radius));

            var geometry = new PathGeometry(_factory);

            var sink = geometry.Open();

            ellipseGeometry.Outline(sink);

            sink.Close();

            _device.FillGeometry(geometry, fill.Brush);
            _device.DrawGeometry(geometry, outline.Brush, stroke);

            sink.Dispose();
            geometry.Dispose();
            ellipseGeometry.Dispose();
        }

        public void OutlineFillCircle(IBrush outline, IBrush fill, Point location, float radius, float stroke) => OutlineFillCircle(outline, fill, location.X, location.Y, radius, stroke);

        public void OutlineFillCircle(IBrush outline, IBrush fill, Circle circle, float stroke) => OutlineFillCircle(outline, fill, circle.Location.X, circle.Location.Y, circle.Radius, stroke);

        public void DrawEllipse(IBrush brush, float x, float y, float radiusX, float radiusY, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            _device.DrawEllipse(new SharpDX.Direct2D1.Ellipse(new RawVector2(x, y), radiusX, radiusY), brush.Brush, stroke);
        }

        public void DrawEllipse(IBrush brush, Point location, float radiusX, float radiusY, float stroke) => DrawEllipse(brush, location.X, location.Y, radiusX, radiusY, stroke);

        public void DrawEllipse(IBrush brush, Ellipse ellipse, float stroke) => DrawEllipse(brush, ellipse.Location.X, ellipse.Location.Y, ellipse.RadiusX, ellipse.RadiusY, stroke);

        public void OutlineEllipse(IBrush outline, IBrush fill, float x, float y, float radiusX, float radiusY, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            var ellipse = new SharpDX.Direct2D1.Ellipse(new RawVector2(x, y), radiusX, radiusY);

            _device.DrawEllipse(ellipse, fill.Brush, stroke);

            float halfStroke = stroke * 0.5f;

            ellipse.RadiusX += halfStroke;
            ellipse.RadiusY += halfStroke;

            _device.DrawEllipse(ellipse, outline.Brush, halfStroke);

            ellipse.RadiusX -= stroke;
            ellipse.RadiusY -= stroke;

            _device.DrawEllipse(ellipse, outline.Brush, halfStroke);
        }

        public void OutlineEllipse(IBrush outline, IBrush fill, Point location, float radiusX, float radiusY, float stroke) => OutlineEllipse(outline, fill, location.X, location.Y, radiusX, radiusY, stroke);

        public void OutlineEllipse(IBrush outline, IBrush fill, Ellipse ellipse, float stroke) => OutlineEllipse(outline, fill, ellipse.Location.X, ellipse.Location.Y, ellipse.RadiusX, ellipse.RadiusY, stroke);

        public void DashedEllipse(IBrush brush, float x, float y, float radiusX, float radiusY, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            _device.DrawEllipse(new SharpDX.Direct2D1.Ellipse(new RawVector2(x, y), radiusX, radiusY), brush.Brush, stroke, _strokeStyle);
        }

        public void DashedEllipse(IBrush brush, Point location, float radiusX, float radiusY, float stroke) => DashedEllipse(brush, location.X, location.Y, radiusX, radiusY, stroke);

        public void DashedEllipse(IBrush brush, Ellipse ellipse, float stroke) => DashedEllipse(brush, ellipse.Location.X, ellipse.Location.Y, ellipse.RadiusX, ellipse.RadiusY, stroke);

        public void FillEllipse(IBrush brush, float x, float y, float radiusX, float radiusY)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            _device.FillEllipse(new SharpDX.Direct2D1.Ellipse(new RawVector2(x, y), radiusX, radiusY), brush.Brush);
        }

        public void FillEllipse(IBrush brush, Point location, float radiusX, float radiusY) => FillEllipse(brush, location.X, location.Y, radiusX, radiusY);

        public void FillEllipse(IBrush brush, Ellipse ellipse) => FillEllipse(brush, ellipse.Location.X, ellipse.Location.Y, ellipse.RadiusX, ellipse.RadiusY);

        public void OutlineFillEllipse(IBrush outline, IBrush fill, float x, float y, float radiusX, float radiusY, float stroke)
        {
            var ellipseGeometry = new EllipseGeometry(_factory, new SharpDX.Direct2D1.Ellipse(new RawVector2(x, y), radiusX, radiusY));

            var geometry = new PathGeometry(_factory);

            var sink = geometry.Open();

            ellipseGeometry.Outline(sink);

            sink.Close();

            _device.FillGeometry(geometry, fill.Brush);
            _device.DrawGeometry(geometry, outline.Brush, stroke);

            sink.Dispose();
            geometry.Dispose();
            ellipseGeometry.Dispose();
        }

        public void OutlineFillEllipse(IBrush outline, IBrush fill, Point location, float radiusX, float radiusY, float stroke) => OutlineFillEllipse(outline, fill, location.X, location.Y, radiusX, radiusY, stroke);

        public void OutlineFillEllipse(IBrush outline, IBrush fill, Ellipse ellipse, float stroke) => OutlineFillEllipse(outline, fill, ellipse.Location.X, ellipse.Location.Y, ellipse.RadiusX, ellipse.RadiusY, stroke);

        public void DrawLine(IBrush brush, float startX, float startY, float endX, float endY, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            _device.DrawLine(new RawVector2(startX, startY), new RawVector2(endX, endY), brush.Brush, stroke);
        }

        public void DrawLine(IBrush brush, Point start, Point end, float stroke) => DrawLine(brush, start.X, start.Y, end.X, end.Y, stroke);

        public void DrawLine(IBrush brush, Line line, float stroke) => DrawLine(brush, line.Start.X, line.Start.Y, line.End.X, line.End.Y, stroke);

        public void OutlineLine(IBrush outline, IBrush fill, float startX, float startY, float endX, float endY, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            var geometry = new PathGeometry(_factory);

            var sink = geometry.Open();

            float half = stroke / 2.0f;

            sink.BeginFigure(new RawVector2(startX, startY - half), FigureBegin.Filled);

            sink.AddLine(new RawVector2(endX, endY - half));
            sink.AddLine(new RawVector2(endX, endY + half));
            sink.AddLine(new RawVector2(startX, startY + half));

            sink.EndFigure(FigureEnd.Closed);

            _device.DrawGeometry(geometry, outline.Brush, half);
            _device.FillGeometry(geometry, fill.Brush);

            sink.Dispose();
            geometry.Dispose();
        }

        public void OutlineLine(IBrush outline, IBrush fill, Point start, Point end, float stroke) => OutlineLine(outline, fill, start.X, start.Y, end.X, end.Y, stroke);

        public void OutlineLine(IBrush outline, IBrush fill, Line line, float stroke) => OutlineLine(outline, fill, line.Start.X, line.Start.Y, line.End.X, line.End.Y, stroke);

        public void DashedLine(IBrush brush, float startX, float startY, float endX, float endY, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            _device.DrawLine(new RawVector2(startX, startY), new RawVector2(endX, endY), brush.Brush, stroke, _strokeStyle);
        }

        public void DashedLine(IBrush brush, Point start, Point end, float stroke) => DashedLine(brush, start.X, start.Y, end.X, end.Y, stroke);

        public void DashedLine(IBrush brush, Line line, float stroke) => DashedLine(brush, line.Start.X, line.Start.Y, line.End.X, line.End.Y, stroke);

        public void DrawRectangle(IBrush brush, float left, float top, float right, float bottom, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            _device.DrawRectangle(new RawRectangleF(left, top, right, bottom), brush.Brush, stroke);
        }

        public void DrawRectangle(IBrush brush, Rectangle rectangle, float stroke) => DrawRectangle(brush, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom, stroke);

        public void OutlineRectangle(IBrush outline, IBrush fill, float left, float top, float right, float bottom, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            float halfStroke = stroke / 2.0f;

            float width = right;
            float height = bottom;

            _device.DrawRectangle(new RawRectangleF(left - halfStroke, top - halfStroke, width + halfStroke, height + halfStroke), outline.Brush, halfStroke);

            _device.DrawRectangle(new RawRectangleF(left + halfStroke, top + halfStroke, width - halfStroke, height - halfStroke), outline.Brush, halfStroke);

            _device.DrawRectangle(new RawRectangleF(left, top, width, height), fill.Brush, halfStroke);
        }

        public void OutlineRectangle(IBrush outline, IBrush fill, Rectangle rectangle, float stroke) => OutlineRectangle(outline, fill, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom, stroke);

        public void DashedRectangle(IBrush brush, float left, float top, float right, float bottom, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            _device.DrawRectangle(new RawRectangleF(left, top, right, bottom), brush.Brush, stroke, _strokeStyle);
        }

        public void DashedRectangle(IBrush brush, Rectangle rectangle, float stroke) => DashedRectangle(brush, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom, stroke);

        public void FillRectangle(IBrush brush, float left, float top, float right, float bottom)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            _device.FillRectangle(new RawRectangleF(left, top, right, bottom), brush.Brush);
        }

        public void FillRectangle(IBrush brush, Rectangle rectangle) => FillRectangle(brush, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);

        public void OutlineFillRectangle(IBrush outline, IBrush fill, float left, float top, float right, float bottom, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            var rectangleGeometry = new RectangleGeometry(_factory, new RawRectangleF(left, top, right, bottom));

            var geometry = new PathGeometry(_factory);

            var sink = geometry.Open();

            rectangleGeometry.Widen(stroke, sink);
            rectangleGeometry.Outline(sink);

            sink.Close();

            _device.FillGeometry(geometry, fill.Brush);
            _device.DrawGeometry(geometry, outline.Brush, stroke);

            sink.Dispose();
            geometry.Dispose();
            rectangleGeometry.Dispose();
        }

        public void OutlineFillRectangle(IBrush outline, IBrush fill, Rectangle rectangle, float stroke) => OutlineFillRectangle(outline, fill, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom, stroke);

        public void DrawRoundedRectangle(IBrush brush, float left, float top, float right, float bottom, float radius, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            var rect = new SharpDX.Direct2D1.RoundedRectangle()
            {
                RadiusX = radius,
                RadiusY = radius,
                Rect = new RawRectangleF(left, top, right, bottom)
            };

            _device.DrawRoundedRectangle(rect, brush.Brush, stroke);
        }

        public void DrawRoundedRectangle(IBrush brush, RoundedRectangle rectangle, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");
            
            _device.DrawRoundedRectangle(rectangle, brush.Brush, stroke);
        }

        public void DashedRoundedRectangle(IBrush brush, float left, float top, float right, float bottom, float radius, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            var rect = new SharpDX.Direct2D1.RoundedRectangle()
            {
                RadiusX = radius,
                RadiusY = radius,
                Rect = new RawRectangleF(left, top, right, bottom)
            };

            _device.DrawRoundedRectangle(rect, brush.Brush, stroke, _strokeStyle);
        }

        public void DashedRoundedRectangle(IBrush brush, RoundedRectangle rectangle, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            _device.DrawRoundedRectangle(rectangle, brush.Brush, stroke, _strokeStyle);
        }

        public void FillRoundedRectangle(IBrush brush, float left, float top, float right, float bottom, float radius)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            var rect = new SharpDX.Direct2D1.RoundedRectangle()
            {
                RadiusX = radius,
                RadiusY = radius,
                Rect = new RawRectangleF(left, top, right, bottom)
            };

            _device.FillRoundedRectangle(rect, brush.Brush);
        }

        public void FillRoundedRectangle(IBrush brush, RoundedRectangle rectangle)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");
            
            _device.FillRoundedRectangle(rectangle, brush.Brush);
        }

        public void DrawTriangle(IBrush brush, float aX, float aY, float bX, float bY, float cX, float cY, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            var geometry = new PathGeometry(_factory);

            var sink = geometry.Open();

            sink.BeginFigure(new RawVector2(aX, aY), FigureBegin.Hollow);
            sink.AddLine(new RawVector2(bX, bY));
            sink.AddLine(new RawVector2(cX, cY));
            sink.EndFigure(FigureEnd.Closed);

            sink.Close();

            _device.DrawGeometry(geometry, brush.Brush, stroke);

            sink.Dispose();
            geometry.Dispose();
        }

        public void DrawTriangle(IBrush brush, Point a, Point b, Point c, float stroke) => DrawTriangle(brush, a.X, a.Y, b.X, b.Y, c.X, c.Y, stroke);

        public void DrawTriangle(IBrush brush, Triangle triangle, float stroke) => DrawTriangle(brush, triangle.A.X, triangle.A.Y, triangle.B.X, triangle.B.Y, triangle.C.X, triangle.C.Y, stroke);

        public void DashedTriangle(IBrush brush, float aX, float aY, float bX, float bY, float cX, float cY, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            var geometry = new PathGeometry(_factory);

            var sink = geometry.Open();

            sink.BeginFigure(new RawVector2(aX, aY), FigureBegin.Hollow);
            sink.AddLine(new RawVector2(bX, bY));
            sink.AddLine(new RawVector2(cX, cY));
            sink.EndFigure(FigureEnd.Closed);

            sink.Close();

            _device.DrawGeometry(geometry, brush.Brush, stroke, _strokeStyle);

            sink.Dispose();
            geometry.Dispose();
        }

        public void DashedTriangle(IBrush brush, Point a, Point b, Point c, float stroke) => DashedTriangle(brush, a.X, a.Y, b.X, b.Y, c.X, c.Y, stroke);

        public void DashedTriangle(IBrush brush, Triangle triangle, float stroke) => DashedTriangle(brush, triangle.A.X, triangle.A.Y, triangle.B.X, triangle.B.Y, triangle.C.X, triangle.C.Y, stroke);

        public void FillTriangle(IBrush brush, float aX, float aY, float bX, float bY, float cX, float cY)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            var geometry = new PathGeometry(_factory);

            var sink = geometry.Open();

            sink.BeginFigure(new RawVector2(aX, aY), FigureBegin.Filled);
            sink.AddLine(new RawVector2(bX, bY));
            sink.AddLine(new RawVector2(cX, cY));
            sink.EndFigure(FigureEnd.Closed);

            sink.Close();

            _device.FillGeometry(geometry, brush.Brush);

            sink.Dispose();
            geometry.Dispose();
        }

        public void FillTriangle(IBrush brush, Point a, Point b, Point c) => FillTriangle(brush, a.X, a.Y, b.X, b.Y, c.X, c.Y);

        public void FillTriangle(IBrush brush, Triangle triangle) => FillTriangle(brush, triangle.A.X, triangle.A.Y, triangle.B.X, triangle.B.Y, triangle.C.X, triangle.C.Y);

        public void DrawHorizontalProgressBar(IBrush outline, IBrush fill, float left, float top, float right, float bottom, float stroke, float percentage)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            var outer = new RawRectangleF(left, top, right, bottom);

            if (percentage < 1.0f)
            {
                _device.DrawRectangle(outer, outline.Brush, stroke);
            }
            else
            {
                float height = bottom - top;
                float filledHeight = (height / 100.0f) * percentage;

                float halfStroke = stroke * 0.5f;

                var inner = new RawRectangleF(left + halfStroke, top + (height - filledHeight) + halfStroke, right - halfStroke, bottom - halfStroke);

                _device.FillRectangle(inner, fill.Brush);
                _device.DrawRectangle(outer, outline.Brush, stroke);
            }
        }

        public void DrawHorizontalProgressBar(IBrush outline, IBrush fill, Rectangle rectangle, float stroke, float percentage) => DrawHorizontalProgressBar(outline, fill, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom, stroke, percentage);

        public void DrawVerticalProgressBar(IBrush outline, IBrush fill, float left, float top, float right, float bottom, float stroke, float percentage)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");
            
            var outer = new RawRectangleF(left, top, right, bottom);

            if (percentage < 1.0f)
            {
                _device.DrawRectangle(outer, outline.Brush, stroke);
            }
            else
            {
                float width = right - left;
                float filledWidth = (width / 100.0f) * percentage;

                float halfStroke = stroke * 0.5f;

                var inner = new RawRectangleF(left + halfStroke, top + halfStroke, right - (width - filledWidth) - halfStroke, bottom - halfStroke);

                _device.FillRectangle(inner, fill.Brush);
                _device.DrawRectangle(outer, outline.Brush, stroke);
            }
        }

        public void DrawVerticalProgressBar(IBrush outline, IBrush fill, Rectangle rectangle, float stroke, float percentage) => DrawVerticalProgressBar(outline, fill, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom, stroke, percentage);

        public void DrawCrosshair(IBrush brush, float x, float y, float size, float stroke, CrosshairStyle style)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            if (style == CrosshairStyle.Dot)
            {
                FillCircle(brush, x, y, size);
            }
            else if (style == CrosshairStyle.Plus)
            {
                DrawLine(brush, x - size, y, x + size, y, stroke);
                DrawLine(brush, x, y - size, x, y + size, stroke);
            }
            else if (style == CrosshairStyle.Cross)
            {
                DrawLine(brush, x - size, y - size, x + size, y + size, stroke);
                DrawLine(brush, x + size, y - size, x - size, y + size, stroke);
            }
            else if (style == CrosshairStyle.Gap)
            {
                DrawLine(brush, x - size - stroke, y, x - stroke, y, stroke);
                DrawLine(brush, x + size + stroke, y, x + stroke, y, stroke);

                DrawLine(brush, x, y - size - stroke, x, y - stroke, stroke);
                DrawLine(brush, x, y + size + stroke, x, y + stroke, stroke);
            }
            else if (style == CrosshairStyle.Diagonal)
            {
                DrawLine(brush, x - size, y - size, x + size, y + size, stroke);
                DrawLine(brush, x + size, y - size, x - size, y + size, stroke);
            }
        }

        public void DrawCrosshair(IBrush brush, Point location, float size, float stroke, CrosshairStyle style) => DrawCrosshair(brush, location.X, location.Y, size, stroke, style);

        public void DrawArrowLine(IBrush brush, float startX, float startY, float endX, float endY, float size)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            float deltaX = endX >= startX ? endX - startX : startX - endX;
            float deltaY = endY >= startY ? endY - startY : startY - endY;

            float length = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            float xm = length - size;
            float xn = xm;

            float ym = size;
            float yn = -ym;

            float sin = deltaY / length;
            float cos = deltaX / length;

            float x = xm * cos - ym * sin + endX;
            ym = xm * sin + ym * cos + endY;
            xm = x;

            x = xn * cos - yn * sin + endX;
            yn = xn * sin + yn * cos + endY;
            xn = x;

            FillTriangle(brush, startX, startY, xm, ym, xn, yn);
        }

        public void DrawArrowLine(IBrush brush, Point start, Point end, float size) => DrawArrowLine(brush, start.X, start.Y, end.X, end.Y, size);

        public void DrawArrowLine(IBrush brush, Line line, float size) => DrawArrowLine(brush, line.Start.X, line.Start.Y, line.End.X, line.End.Y, size);

        public void DrawBox2D(IBrush outline, IBrush fill, float left, float top, float right, float bottom, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            float width = right - left;
            float height = bottom - top;

            var geometry = new PathGeometry(_factory);

            var sink = geometry.Open();

            sink.BeginFigure(new RawVector2(left, top), FigureBegin.Filled);
            sink.AddLine(new RawVector2(left + width, top));
            sink.AddLine(new RawVector2(left + width, top + height));
            sink.AddLine(new RawVector2(left, top + height));
            sink.EndFigure(FigureEnd.Closed);

            sink.Close();
            _device.DrawGeometry(geometry, outline.Brush, stroke);
            _device.FillGeometry(geometry, fill.Brush);
            

            sink.Dispose();
            geometry.Dispose();
        }

        public void DrawBox2D(IBrush outline, IBrush fill, Rectangle rectangle, float stroke) => DrawBox2D(outline, fill, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom, stroke);

        public void DrawRectangleEdges(IBrush brush, float left, float top, float right, float bottom, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            float width = right - left;
            float height = bottom - top;

            int length = (int)((width + height) / 2.0f * 0.2f);

            var first = new RawVector2(left, top);
            var second = new RawVector2(left, top + length);
            var third = new RawVector2(left + length, top);

            _device.DrawLine(first, second, brush.Brush, stroke);
            _device.DrawLine(first, third, brush.Brush, stroke);

            first.Y += height;
            second.Y = first.Y - length;
            third.Y = first.Y;
            third.X = first.X + length;

            _device.DrawLine(first, second, brush.Brush, stroke);
            _device.DrawLine(first, third, brush.Brush, stroke);

            first.X = left + width;
            first.Y = top;
            second.X = first.X - length;
            second.Y = first.Y;
            third.X = first.X;
            third.Y = first.Y + length;

            _device.DrawLine(first, second, brush.Brush, stroke);
            _device.DrawLine(first, third, brush.Brush, stroke);

            first.Y += height;
            second.X += length;
            second.Y = first.Y - length;
            third.Y = first.Y;
            third.X = first.X - length;

            _device.DrawLine(first, second, brush.Brush, stroke);
            _device.DrawLine(first, third, brush.Brush, stroke);
        }

        public void DrawRectangleEdges(IBrush brush, Rectangle rectangle, float stroke) => DrawRectangle(brush, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom, stroke);

        public void DrawText(Font font, float fontSize, IBrush brush, float x, float y, string text)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            if (text == null) throw new ArgumentNullException(nameof(text));
            if (text == string.Empty) return;

            float clippedWidth = Width - x;
            float clippedHeight = Height - y;

            if (clippedWidth <= fontSize)
            {
                clippedWidth = Width;
            }
            if (clippedHeight <= fontSize)
            {
                clippedHeight = Height;
            }

            var layout = new TextLayout(_fontFactory, text, font.TextFormat, clippedWidth, clippedHeight);

            if (fontSize != font.FontSize)
            {
                layout.SetFontSize(fontSize, new TextRange(0, text.Length));
            }
            
            _device.DrawTextLayout(new RawVector2(x, y), layout, brush.Brush, DrawTextOptions.Clip);

            layout.Dispose();
        }

        public void DrawText(Font font, float fontSize, IBrush brush, Point location, string text) => DrawText(font, fontSize, brush, location.X, location.Y, text);

        public void DrawText(Font font, IBrush brush, float x, float y, string text) => DrawText(font, font.FontSize, brush, x, y, text);

        public void DrawText(Font font, IBrush brush, Point location, string text) => DrawText(font, font.FontSize, brush, location.X, location.Y, text);

        public void DrawTextWithBackground(Font font, float fontSize, IBrush brush, IBrush background, float x, float y, string text)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");
            
            if (text == null) throw new ArgumentNullException(nameof(text));
            if (text == string.Empty) return;

            float clippedWidth = Width - x;
            float clippedHeight = Height - y;

            if (clippedWidth <= fontSize)
            {
                clippedWidth = Width;
            }
            if (clippedHeight <= fontSize)
            {
                clippedHeight = Height;
            }

            var layout = new TextLayout(_fontFactory, text, font.TextFormat, clippedWidth, clippedHeight);

            if (fontSize != font.FontSize)
            {
                layout.SetFontSize(fontSize, new TextRange(0, text.Length));
            }

            float modifier = layout.FontSize * 0.25f;
            var rectangle = new RawRectangleF(x - modifier, y - modifier, x + layout.Metrics.Width + modifier, y + layout.Metrics.Height + modifier);

            _device.FillRectangle(rectangle, background.Brush);

            _device.DrawTextLayout(new RawVector2(x, y), layout, brush.Brush, DrawTextOptions.Clip);

            layout.Dispose();
        }

        public void DrawTextWithBackground(Font font, float fontSize, IBrush brush, IBrush background, Point location, string text) => DrawTextWithBackground(font, fontSize, brush, background, location.X, location.Y, text);

        public void DrawTextWithBackground(Font font, IBrush brush, IBrush background, float x, float y, string text) => DrawTextWithBackground(font, font.FontSize, brush, background, x, y, text);

        public void DrawTextWithBackground(Font font, IBrush brush, IBrush background, Point location, string text) => DrawTextWithBackground(font, font.FontSize, brush, background, location.X, location.Y, text);

        public void DrawImage(Image image, float x, float y, float opacity = 1.0f)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            float destRight = x + image.Bitmap.PixelSize.Width;
            float destBottom = y + image.Bitmap.PixelSize.Height;

            _device.DrawBitmap(
                image.Bitmap,
                new RawRectangleF(x, y, destRight, destBottom),
                opacity,
                BitmapInterpolationMode.Linear);
        }

        public void DrawImage(Image image, Point location, float opacity = 1.0f) => DrawImage(image, location.X, location.Y, opacity);

        public void DrawImage(Image image, Rectangle rectangle, float opacity = 1.0f, bool linearScale = true)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            _device.DrawBitmap(
                image.Bitmap,
                rectangle,
                opacity,
                linearScale ? BitmapInterpolationMode.Linear : BitmapInterpolationMode.NearestNeighbor,
                new RawRectangleF(0, 0, image.Bitmap.PixelSize.Width, image.Bitmap.PixelSize.Height));
        }

        public void DrawImage(Image image, float left, float top, float right, float bottom, float opacity = 1.0f, bool linearScale = true) => DrawImage(image, new Rectangle(left, top, right, bottom), opacity, linearScale);

        public void DrawGeometry(Geometry geometry, IBrush brush, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            _device.DrawGeometry(geometry, brush.Brush, stroke);
        }

        public void DashedGeometry(Geometry geometry, IBrush brush, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            _device.DrawGeometry(geometry, brush.Brush, stroke, _strokeStyle);
        }

        public void FillGeometry(Geometry geometry, IBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            _device.FillGeometry(geometry, brush.Brush);
        }

        public void FillMesh(Mesh mesh, IBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            _device.FillMesh(mesh, brush.Brush);
        }
        
        public RenderTarget GetRenderTarget()
        {
            if (!IsInitialized) throw new InvalidOperationException("The DirectX device is not initialized");

            return _device;
        }

        public Factory GetFactory()
        {
            if (!IsInitialized) throw new InvalidOperationException("The DirectX device is not initialized");

            return _factory;
        }

        public FontFactory GetFontFactory()
        {
            if (!IsInitialized) throw new InvalidOperationException("The DirectX device is not initialized");

            return _fontFactory;
        }

        public override bool Equals(object obj)
        {
            var gfx = obj as Graphics;

            if (gfx == null)
            {
                return false;
            }
            else
            {
                return gfx.WindowHandle == WindowHandle
                    && gfx.IsInitialized == IsInitialized
                    && gfx._device.NativePointer == _device.NativePointer;
            }
        }

        public bool Equals(Graphics value)
        {
            return value != null
                && value.WindowHandle == WindowHandle
                && value.IsInitialized == IsInitialized
                && value._device.NativePointer == _device.NativePointer;
        }

        public override int GetHashCode()
        {
            return OverrideHelper.HashCodes(
                WindowHandle.GetHashCode(),
                _watch.GetHashCode());
        }

        public override string ToString()
        {
            return OverrideHelper.ToString(
                "WindowHandle", WindowHandle.ToString("X"),
                "Width", Width.ToString(),
                "Height", Height.ToString(),
                "IsInitialized", IsInitialized.ToString(),
                "IsDrawing", IsDrawing.ToString(),
                "AntiAliasing", (PerPrimitiveAntiAliasing || TextAntiAliasing).ToString(),
                "VSync", VSync.ToString());
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (IsInitialized)
                {
                    if (IsDrawing)
                    {
                        try
                        {
                            _device.EndDraw();
                        }
                        catch { }
                    }

                    Destroy();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        public static bool Equals(Graphics left, Graphics right)
        {
            return left != null
                && left.Equals(right);
        }
    }
}