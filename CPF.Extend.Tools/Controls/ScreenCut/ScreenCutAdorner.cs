using CPF.Controls;
using CPF.Drawing;
using CPF.Input;
using CPF.Shapes;

namespace CPF.Extend.Tools.Controls
{
    /// <summary>
    /// 表示屏幕截图的装饰器。
    /// </summary>
    public class ScreenCutAdorner : Control
    {
        // 图标的大小
        private const float THUMB_SIZE = 15;
        // 最小尺寸
        private const float MINIMAL_SIZE = 20; 
        // 左中
        private readonly Thumb lc; 
        // 左上
        private readonly Thumb tl; 
        // 上中
        private readonly Thumb tc; 
        // 右上
        private readonly Thumb tr; 
        // 右中
        private readonly Thumb rc; 
        // 右下
        private readonly Thumb br; 
        // 下中
        private readonly Thumb bc; 
        // 左下
        private readonly Thumb bl; 
        // 可见图标集合
        private readonly Collection<Thumb> visCollec; 
        // 画布
        private readonly Panel canvas; 
        // 装饰元素
        private readonly UIElement adorned;

        /// <summary>
        /// 构造函数：创建一个 ScreenCutAdorner 对象。
        /// </summary>
        /// <param name="adorned">要装饰的 UI 元素。</param>
        public ScreenCutAdorner(UIElement adorned)
        {
            this.adorned = adorned;
            canvas = FindParent(adorned) as Panel;

            visCollec = new Collection<Thumb>();
            visCollec.Add(lc = GetResizeThumb(Cursors.SizeWestEast, TextAlignment.Left, VerticalAlignment.Center));
            visCollec.Add(tl = GetResizeThumb(Cursors.TopLeftCorner, TextAlignment.Left, VerticalAlignment.Top));
            visCollec.Add(tc = GetResizeThumb(Cursors.SizeNorthSouth, TextAlignment.Center, VerticalAlignment.Top));
            visCollec.Add(tr = GetResizeThumb(Cursors.TopRightCorner, TextAlignment.Right, VerticalAlignment.Top));
            visCollec.Add(rc = GetResizeThumb(Cursors.SizeWestEast, TextAlignment.Right, VerticalAlignment.Center));
            visCollec.Add(br = GetResizeThumb(Cursors.BottomRightCorner, TextAlignment.Right, VerticalAlignment.Bottom));
            visCollec.Add(bc = GetResizeThumb(Cursors.SizeNorthSouth, TextAlignment.Center, VerticalAlignment.Bottom));
            visCollec.Add(bl = GetResizeThumb(Cursors.BottomLeftCorner, TextAlignment.Left, VerticalAlignment.Bottom));
        }

        /// <summary>
        /// 当拇指的预览鼠标按下事件发生时调用的方法。
        /// </summary>
        /// <param name="sender">事件的发送者。</param>
        /// <param name="e">鼠标按下事件参数。</param>
        protected void OnThumbPreviewMouseDown(object sender,MouseButtonEventArgs e)
        {
            RaiseEvent(in e, "PreviewMouseDown");
        }

        /// <summary>
        /// 查找指定元素的父级 UI 元素。
        /// </summary>
        /// <param name="element">要查找父级元素的元素。</param>
        /// <returns>指定元素的父级 UI 元素，如果未找到父级则返回 null。</returns>
        private static UIElement FindParent(UIElement element)
        {
            UIElement obj = element.Parent;
            return obj;
        }

        /// <summary>
        /// 调整指定框架元素的大小。
        /// </summary>
        /// <param name="frameworkElement">要调整大小的框架元素。</param>
        private void Resize(UIElement frameworkElement)
        {
            if (double.IsNaN(frameworkElement.Width.Value))
                frameworkElement.Width = frameworkElement.RenderBounds.Width;
            if (double.IsNaN(frameworkElement.Height.Value))
                frameworkElement.Height = frameworkElement.RenderBounds.Height;
        }

        /// <summary>
        /// 根据鼠标光标和文本对齐方式获取调整大小的拇指。
        /// </summary>
        /// <param name="cur">鼠标光标。</param>
        /// <param name="hor">文本的水平对齐方式。</param>
        /// <param name="ver">文本的垂直对齐方式。</param>
        /// <returns>调整大小的拇指。</returns>
        private Thumb GetResizeThumb(Cursor cur, TextAlignment hor, VerticalAlignment ver)
        {
            var thumb = new Thumb
            {
                Width = THUMB_SIZE,
                Height = THUMB_SIZE,
                Background=null,
                Cursor = cur,
                Child = new Ellipse
                {
                    Height = "100%",
                    Width = "100%",
                    Fill = Color.White,
                    StrokeFill = "#409EFF"
                }
            };
            canvas.Children.Add(thumb);
            var offset = THUMB_SIZE / 2;
            if (hor== TextAlignment.Left)
            {
                thumb.Bindings.Add(nameof(Thumb.MarginLeft), nameof(UIElement.MarginLeft), adorned, BindingMode.OneWay, x => (FloatField)x - offset);
            }
            if (hor == TextAlignment.Right)
            {
                thumb.Bindings.Add(nameof(Thumb.MarginLeft), nameof(UIElement.MarginLeft), adorned, BindingMode.OneWay, x => (FloatField)x + adorned.Width.Value - offset);
                thumb.Bindings.Add(nameof(Thumb.MarginLeft), nameof(UIElement.Width), adorned, BindingMode.OneWay, x => adorned.MarginLeft.Value + adorned.Width.Value - offset);
            }
            if (hor == TextAlignment.Center)
            {
                thumb.Bindings.Add(nameof(Thumb.MarginLeft), nameof(UIElement.MarginLeft), adorned, BindingMode.OneWay, x => (FloatField)x + adorned.Width.Value/2- offset);
                thumb.Bindings.Add(nameof(Thumb.MarginLeft), nameof(UIElement.Width), adorned, BindingMode.OneWay, x => adorned.MarginLeft.Value + adorned.Width.Value / 2 - offset);
            }
            if (ver == VerticalAlignment.Top)
            {
                thumb.Bindings.Add(nameof(Thumb.MarginTop), nameof(UIElement.MarginTop), adorned, BindingMode.OneWay, x => (FloatField)x - offset);
            }
            if (ver == VerticalAlignment.Bottom)
            {
                thumb.Bindings.Add(nameof(Thumb.MarginTop), nameof(UIElement.MarginTop), adorned, BindingMode.OneWay, x => (FloatField)x + adorned.Height.Value - offset);
                thumb.Bindings.Add(nameof(Thumb.MarginTop), nameof(UIElement.Height), adorned, BindingMode.OneWay, x => adorned.MarginTop.Value + adorned.Height.Value - offset);
            }
            if (ver == VerticalAlignment.Center)
            {
                thumb.Bindings.Add(nameof(Thumb.MarginTop), nameof(UIElement.MarginTop), adorned, BindingMode.OneWay, x => (FloatField)x + adorned.Height.Value / 2 - offset);
                thumb.Bindings.Add(nameof(Thumb.MarginTop), nameof(UIElement.Height), adorned, BindingMode.OneWay, x => adorned.MarginTop.Value + adorned.Height.Value / 2 - offset);
            }
            var maxWidth = double.IsNaN(canvas.Width.Value) ? canvas.ActualSize.Width : canvas.Width;
            var maxHeight = double.IsNaN(canvas.Height.Value) ? canvas.ActualSize.Height : canvas.Height;
            thumb.PreviewMouseDown += OnThumbPreviewMouseDown;
            thumb.DragDelta += (s, e) =>
            {
                var element = adorned;
                if (element == null)
                    return;
                Resize(element);

                if (s == tc|| s==tl || s==tr)
                {
                    if (element.ActualSize.Height - e.VerticalChange > MINIMAL_SIZE)
                    {
                        var newHeight = element.ActualSize.Height - e.VerticalChange;
                        var top = element.MarginTop.Value;
                        if (newHeight > 0 && top + e.VerticalChange >= 0)
                        {
                            element.MarginTop = top + e.VerticalChange;
                            element.Height = newHeight;
                        }
                    }
                }

                if (s == bc || s == bl || s == br)
                {
                    if (element.ActualSize.Height - e.VerticalChange > MINIMAL_SIZE)
                    {
                        var newHeight = element.Height.Value + e.VerticalChange;
                        var top = element.MarginTop.Value + newHeight;
                        if (newHeight > 0 && top <= canvas.ActualSize.Height)
                            element.Height = newHeight;
                    }
                }

                if (s == lc || s == bl || s == tl)
                {
                    if (element.ActualSize.Width - e.HorizontalChange > MINIMAL_SIZE)
                    {
                        var newWidth = element.ActualSize.Width - e.HorizontalChange;
                        var left = element.MarginLeft.Value;
                        if (newWidth > 0 && left + e.HorizontalChange >= 0)
                        {
                            element.MarginLeft = left + e.HorizontalChange;
                            element.Width = newWidth;
                        }
                    }
                }

                if (s == rc || s == br || s == tr)
                {
                    if (element.ActualSize.Width + e.HorizontalChange > MINIMAL_SIZE)
                    {
                        var newWidth = element.Width.Value + e.HorizontalChange;
                        var left = element.MarginLeft.Value + newWidth;
                        if (newWidth > 0 && left <= canvas.ActualSize.Width)
                            element.Width = newWidth;
                    }
                }

                e.Handled = true;
            };
            return thumb;
        }

        /// <summary>
        /// 初始化组件。
        /// </summary>
        protected override void InitializeComponent()
        {
            Height = "100%";
            Width = "100%";
        }
    }
}