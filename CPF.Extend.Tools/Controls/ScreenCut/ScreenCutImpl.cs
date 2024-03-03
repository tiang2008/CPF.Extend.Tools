using CPF.Controls;
using CPF.Drawing;
using CPF.Effects;
using CPF.Extend.Tools.Controls.ScreenCut;
using CPF.Input;
using CPF.Platform;
using CPF.Shapes;
using System;

namespace CPF.Extend.Tools.Controls
{
    /// <summary>
    /// 表示屏幕截图的实现类。
    /// </summary>
    public class ScreenCutImpl : Window
    {
        // 边框、编辑栏、弹出框边框
        private Border _border, _editBar, _popupBorder;
        // 保存按钮、取消按钮、完成按钮
        private Button _buttonSave, _buttonCancel, _buttonComplete;
        // 画布
        private Panel _canvas;
        // 标签为“Draw”
        private const string _tag = "Draw";

        /// <summary>
        /// 当前选择颜色
        /// </summary>
        private ViewFill _currentBrush;
        // 弹出窗口
        private Popup _popup;

        private RadioButton _radioButtonRectangle, // 矩形单选按钮
                    _radioButtonEllipse, // 椭圆单选按钮
                    _radioButtonArrow, // 箭头单选按钮
                    _radioButtonInk, // 墨水单选按钮
                    _radioButtonText; // 文本单选按钮

        private Rectangle _rectangleLeft, _rectangleTop, _rectangleRight, _rectangleBottom;
        private WrapPanel _wrapPanel;

        /// <summary>
        ///     当前绘制矩形
        /// </summary>
        private Border borderRectangle;

        /// <summary>
        ///     当前箭头
        /// </summary>
        private Control controlArrow;

        /// <summary>
        ///     绘制当前椭圆
        /// </summary>
        private Ellipse drawEllipse;

        private UIElement frameworkElement;
        private bool isMouseUp;
        private Point? pointStart, pointEnd;

        /// <summary>
        ///     当前画笔
        /// </summary>
        private Polyline polyLine;

        private Rect rect;
        private ScreenCutAdorner screenCutAdorner;
        private ScreenCutMouseType screenCutMouseType = ScreenCutMouseType.Default;

        /// <summary>
        ///     当前文本
        /// </summary>
        private Border textBorder;

        /// <summary>
        /// 截图完成委托
        /// </summary>
        public delegate void ScreenShotDone(Bitmap bitmap);
        /// <summary>
        /// 截图完成事件
        /// </summary>
        public event ScreenShotDone CutCompleted;
        /// <summary>
        /// 截图取消委托
        /// </summary>
        public delegate void ScreenShotCanceled();
        /// <summary>
        /// 截图取消事件
        /// </summary>
        public event ScreenShotCanceled CutCanceled;
        private double y1;
        private int ScreenIndex;
        public static int CaptureScreenID = -1;
        private Bitmap ScreenCapture;
        private ScreenDPI screenDPI;
        public ScreenCutImpl(int index)
        {
            ScreenIndex = index;
            Screen screen = Screen.AllScreens[ScreenIndex];
            base.MarginLeft = screen.WorkingArea.Left;
            base.MarginTop = screen.WorkingArea.Top;
            ShowInTaskbar = false;
            screenDPI = GetScreenDPI(ScreenIndex);
            this.Closed += OnClosed;
        }

        private void OnClosed(object sender, EventArgs e)
        {
            this._popup.Hide();
            this._popup.Dispose();
            GC.SuppressFinalize(this);
        }

        public static void ClearCaptureScreenID()
        {
            CaptureScreenID = -1;
        }

        /// <summary>
        /// 初始化组件。
        /// </summary>
        protected override void InitializeComponent()
        {
            MarginLeft = Screen.AllScreens[ScreenIndex].Bounds.Left / screenDPI.scaleX;
            MarginTop = Screen.AllScreens[ScreenIndex].Bounds.Top / screenDPI.scaleY;
            Width = Screen.AllScreens[ScreenIndex].Bounds.Width / screenDPI.scaleX;
            Height = Screen.AllScreens[ScreenIndex].Bounds.Height / screenDPI.scaleY;
            Background = null;
            Children.Add(new Panel
            {
                Height="100%",
                Width="100%",
                BorderStroke = new Stroke(2, DashStyles.Dash),
                BorderFill=Color.Red,
                Children ={
                    new Panel
                    {
                        PresenterFor=this,
                        IsAntiAlias=true,
                        Name="Canvas",
                        Height="100%",
                        Width="100%",
                    },
                    new Rectangle
                    {
                        PresenterFor=this,
                        Name="RectangleLeft",
                        Fill="#000000",
                        Effect=new OpacityEffect
                        { 
                            Opacity=0.3f
                        }
                    },
                    new Rectangle
                    {
                        PresenterFor=this,
                        Name="RectangleTop",
                        Fill="#000000",
                        Effect=new OpacityEffect
                        {
                            Opacity=0.3f
                        }
                    },
                    new Rectangle
                    {
                        PresenterFor = this,
                        Name="RectangleRight",
                        Fill="#000000",
                        Effect=new OpacityEffect
                        {
                            Opacity=0.3f
                        }
                    },
                    new Rectangle
                    {
                        PresenterFor = this,
                        Name="RectangleBottom",
                        Fill="#000000",
                        Effect=new OpacityEffect
                        {
                            Opacity=0.3f
                        }
                    },
                    new Border
                    {
                        PresenterFor=this,
                        Name="Border",
                        Background=null,
                        BorderFill="#409EFF",
                        BorderThickness=new Thickness(3),
                        Cursor=Cursors.SizeAll
                    },
                    new Border
                    {
                       ZIndex=99,
                       PresenterFor=this,
                       Name="EditBar",
                       Visibility= Visibility.Hidden,
                       Background="#EBF4FFFF",
                       CornerRadius=new CornerRadius(12),
                       ShadowBlur=2,
                       ShadowColor=Color.Black,
                       Cursor= Cursors.Arrow,
                       Child=new WrapPanel
                       {
                           MarginLeft=10,
                           MarginRight=10,
                           MarginTop=5,
                           MarginBottom=5,
                           Children =
                           {
                               new RadioButton
                               {
                                   PresenterFor=this,
                                   Name="RadioButtonRectangle",
                                   MarginLeft=4,
                                   MarginRight=4,
                                   ToolTip="方框",
                                   Content=new Path
                                   {
                                       Width=18,
                                       Height=18,
                                       Stretch= Stretch.Fill,
                                       Fill="#626660",
                                       Data="M640 146.286h-475.429q-37.714 0-64.571 26.857t-26.857 64.571v475.429q0 37.714 26.857 64.571t64.571 26.857h475.429q37.714 0 64.571-26.857t26.857-64.571v-475.429q0-37.714-26.857-64.571t-64.571-26.857zM804.571 237.714v475.429q0 68-48.286 116.286t-116.286 48.286h-475.429q-68 0-116.286-48.286t-48.286-116.286v-475.429q0-68 48.286-116.286t116.286-48.286h475.429q68 0 116.286 48.286t48.286 116.286z"
                                   }
                               },
                               new RadioButton
                               {
                                   PresenterFor=this,
                                   Name="RadioButtonEllipse",
                                   MarginLeft=4,
                                   MarginRight=4,
                                   ToolTip="椭圆",
                                   Content=new Ellipse
                                   {
                                       Width=19,
                                       Height=19,
                                       StrokeStyle= new Stroke(1.5f),
                                       StrokeFill="#626660",
                                   }
                               },
                               new RadioButton
                               {
                                   PresenterFor=this,
                                   Name="RadioButtonArrow",
                                   MarginLeft=4,
                                   MarginRight=4,
                                   ToolTip="箭头",
                                   Content=new Path
                                   {
                                       Width=18,
                                       Height=18,
                                       StrokeStyle= new Stroke(3f),
                                       StrokeFill=Color.Transparent,
                                       Stretch= Stretch.Fill,
                                       Fill="#626660",
                                       Data="M384 214h426v426h-84v-282l-496 496-60-60 496-496h-282v-84z"
                                   }
                               },
                               new RadioButton
                               {
                                   PresenterFor=this,
                                   Name="RadioButtonInk",
                                   MarginLeft=4,
                                   MarginRight=4,
                                   ToolTip="画笔",
                                   Content=new Path
                                   {
                                       Width=18,
                                       Height=18,
                                       Stretch= Stretch.Fill,
                                       Fill="#626660",
                                       Data="M986.466 37.502c-49.982-49.982-131.004-49.982-180.986 0l-483.984 484.144c43.262 11.264 83.262 32.99 115.74 65.502 32.99 32.99 54.014 73.246 65.246 115.484l483.984-484.144c50.014-49.982 50.014-131.002 0-180.986zM346.742 858.63c49.982-49.982 49.982-131.004 0-180.986s-131.004-49.982-180.986 0l-165.756 346.356 346.742-165.37z"
                                   }
                               },
                               new RadioButton
                               {
                                   PresenterFor=this,
                                   Name="RadioButtonText",
                                   MarginLeft=4,
                                   MarginRight=4,
                                   ToolTip="文字",
                                   Content=new Path
                                   {
                                       Width=18,
                                       Height=18,
                                       Stretch= Stretch.Fill,
                                       Fill="#626660",
                                       Data="M8.394,7.3850001 L18,7.3850001 18,8.9495381 14.071471,8.9495381 14.071471,21 12.304952,21 12.304952,8.9495381 8.394,8.9495381 z M5.2364039,2.4000326 C4.4364113,2.4000326 3.7515685,2.6937805 3.1818757,3.2812766 2.612183,3.8687721 2.3273365,4.5750159 2.3273365,5.4000081 L2.3273365,20.999992 C2.3273365,21.824983 2.612183,22.531227 3.1818757,23.118723 3.7515685,23.706219 4.4364113,23.999966 5.2364039,23.999966 L20.363661,23.999966 C21.163652,23.999966 21.848495,23.706219 22.418188,23.118723 22.987883,22.531227 23.272728,21.824983 23.272728,20.999992 L23.272728,5.4000081 C23.272728,4.5750159 22.987883,3.8687721 22.418188,3.2812766 21.848495,2.6937805 21.163652,2.4000326 20.363661,2.4000326 z M5.2363719,0 L20.36363,0 C21.806051,0 23.039387,0.52812719 24.063631,1.5843812 25.087877,2.6406356 25.6,3.912511 25.6,5.4000081 L25.6,20.999992 C25.6,22.487488 25.087877,23.759365 24.063631,24.815618 23.039387,25.871874 21.806051,26.399999 20.36363,26.399999 L5.2363719,26.399999 C3.7939503,26.399999 2.5606164,25.871874 1.5363699,24.815618 0.51212323,23.759365 -5.3697355E-08,22.487488 3.5527137E-15,20.999992 L3.5527137E-15,5.4000081 C-5.3697355E-08,3.912511 0.51212323,2.6406356 1.5363699,1.5843812 2.5606164,0.52812719 3.7939503,0 5.2363719,0 z"
                                   }
                               },
                               new Rectangle
                               {
                                   Width=1,
                                   Height=18,
                                   MarginLeft=4,
                                   MarginRight=4,
                                   StrokeFill="#939990",
                               },
                               new Button
                               {
                                   PresenterFor=this,
                                   Name="ButtonSave",
                                   MarginLeft=4,
                                   MarginRight=4,
                                   ToolTip="保存",
                                   Content=new Path
                                   {
                                       Width=18,
                                       Height=18,
                                       Stretch= Stretch.Fill,
                                       Fill="#626660",
                                       Data="M512 42.667q17.667 0 30.167 12.5t12.5 30.167v494.333l140.333-140.667q12.333-12.333 30.333-12.333 18.333 0 30.5 12.167t12.167 30.5q0 18-12.333 30.333l-213.333 213.333q-12.333 12.333-30.333 12.333t-30.333-12.333l-213.333-213.333q-12.333-13-12.333-30.333 0-17.667 12.5-30.167t30.167-12.5q18 0 30.333 12.333l140.333 140.667v-494.333q0-17.667 12.5-30.167t30.167-12.5zM938.667 640q17.667 0 30.167 12.5t12.5 30.167v170.667q0 53.333-37 90.333-37.667 37.667-90 37.667h-683.667q-52.333 0-90.667-37.333-37.333-38.333-37.333-90.667v-170.667q0-17.667 12.5-30.167t30.167-12.5 30.167 12.5 12.5 30.167v170.667q0 17.667 12.5 30.167t30.167 12.5h683.667q17.333 0 29.5-12.5t12.167-30.167v-170.667q0-17.667 12.5-30.167t30.167-12.5z"
                                   }
                               },
                               new Button
                               {
                                    PresenterFor=this,
                                   Name="ButtonCancel",
                                   MarginLeft=4,
                                   MarginRight=4,
                                   ToolTip="取消",
                                   Content=new Path
                                   {
                                       Width=14,
                                       Height=14,
                                       Stretch= Stretch.Fill,
                                       Fill="#626660",
                                       Data="M810.667 170.667q18.333 0 30.5 12.167t12.167 30.5q0 18-12.333 30.333l-268.667 268.333 268.667 268.333q12.333 12.333 12.333 30.333 0 18.333-12.167 30.5t-30.5 12.167q-18 0-30.333-12.333l-268.333-268.667-268.333 268.667q-12.333 12.333-30.333 12.333-18.333 0-30.5-12.167t-12.167-30.5q0-18 12.333-30.333l268.667-268.333-268.667-268.333q-12.333-12.333-12.333-30.333 0-18.333 12.167-30.5t30.5-12.167q18 0 30.333 12.333l268.333 268.667 268.333-268.667q12.333-12.333 30.333-12.333z"
                                   }
                               },
                               new Button
                               {
                                    PresenterFor=this,
                                   Name="ButtonComplete",
                                   MarginLeft=4,
                                   MarginRight=4,
                                   ToolTip="完成",
                                   Content=new Path
                                   {
                                       Width=20,
                                       Height=15,
                                       Stretch= Stretch.Fill,
                                       Fill="#626660",
                                       Data="M384 690l452-452 60 60-512 512-238-238 60-60z"
                                   }
                               },
                           } 
                       }
                    }
                }
            });
            _popup = new Popup
            {
                Visibility= Visibility.Hidden,
                MarginTop = 30,
                CanActivate = true,
                StaysOpen = true,
                Placement = PlacementMode.Padding,
                BorderFill = "#aaa",
                Background = "#fff",
                BorderStroke = "1",
            };
            _popupBorder = new Border
            {
                PresenterFor = this,
                Name = "EditBar",
                Visibility = Visibility.Hidden,
                Background = "#FFEBF4FF",
                CornerRadius = new CornerRadius(12),
                Effect = new OpacityEffect { Opacity = 0.1f },
                Child=new Panel 
                {
                    Children = 
                    {
                        new Path
                        {
                            Width=10,
                            Margin="0,-8,0,0",
                            Data="M768 716.8h-512l256-460.8 256 460.8z",
                            Fill = "#FFEBF4FF",
                            Stretch= Stretch.Uniform,
                        },
                        new WrapPanel
                        {
                            Margin="10",
                        }
                    }
                }
            };
            _popup.Children.Add(_popupBorder);
        }

        /// <summary>
        /// 在初始化完成时调用。
        /// </summary>
        protected override void OnInitialized()
        {
            _canvas = FindPresenterByName<Panel>("Canvas");
            _canvas.Background = new TextureFill(CopyScreen()) {  Stretch= Stretch.Fill };
            Cursor = Cursors.Cross;
            _rectangleLeft = FindPresenterByName<Rectangle>("RectangleLeft");
            _rectangleTop = FindPresenterByName<Rectangle>("RectangleTop");
            _rectangleRight = FindPresenterByName<Rectangle>("RectangleRight");
            _rectangleBottom = FindPresenterByName<Rectangle>("RectangleBottom");
            _rectangleLeft.MarginLeft = 0;
            _rectangleLeft.MarginTop = 0;
            _rectangleLeft.Width = _canvas.Width;
            _rectangleLeft.Height = _canvas.Height;
            _rectangleTop.MarginTop = 0;
            _border = FindPresenterByName<Border>("Border");
            _border.Effect = new OpacityEffect { Opacity = 0f };
            _editBar = FindPresenterByName<Border>("EditBar");
            _buttonSave = FindPresenterByName<Button>("ButtonSave"); 
            if (_buttonSave != null)
                _buttonSave.Click += _buttonSave_Click;
            _buttonCancel = FindPresenterByName<Button>("ButtonCancel");
            if (_buttonCancel != null)
                _buttonCancel.Click += _buttonCancel_Click;
            _buttonComplete = FindPresenterByName<Button>("ButtonComplete");
            if (_buttonComplete != null)
                _buttonComplete.Click += _buttonComplete_Click;
            _radioButtonRectangle = FindPresenterByName<RadioButton>("RadioButtonRectangle");
            if (_radioButtonRectangle != null)
            {
                _radioButtonRectangle.Checked += _radioButtonRectangle_Checked;
                _radioButtonRectangle.Unchecked += _radioButtonRectangle_Unchecked;
            }
            _radioButtonEllipse = FindPresenterByName<RadioButton>("RadioButtonEllipse");
            if (_radioButtonEllipse != null)
            {
                _radioButtonEllipse.Checked += _radioButtonEllipse_Checked;
                _radioButtonEllipse.Unchecked += _radioButtonEllipse_Unchecked;
            }
            _radioButtonArrow = FindPresenterByName<RadioButton>("RadioButtonArrow");
            if (_radioButtonArrow != null) 
            {
                _radioButtonArrow.Checked += _radioButtonArrow_Checked;
                _radioButtonArrow.Unchecked += _radioButtonArrow_Unchecked;
            }
            _radioButtonInk = FindPresenterByName<RadioButton>("RadioButtonInk");
            if (_radioButtonInk != null) 
            {
                _radioButtonInk.Checked += _radioButtonInk_Checked;
                _radioButtonInk.Unchecked += _radioButtonInk_Unchecked;
            }
            _radioButtonText = FindPresenterByName<RadioButton>("RadioButtonText");
            if (_radioButtonText != null) 
            {
                _radioButtonText.Checked += _radioButtonText_Checked;
                _radioButtonText.Unchecked += _radioButtonText_Unchecked;
            }
            _popup.MarginLeft = -_popupBorder.ActualSize.Width / 3;
        }

        //public override void OnApplyTemplate()
        //{
        //    base.OnApplyTemplate();
        //    _canvas = GetTemplateChild(CanvasTemplateName) as Canvas;
        //    _rectangleLeft = GetTemplateChild(RectangleLeftTemplateName) as Rectangle;
        //    _rectangleTop = GetTemplateChild(RectangleTopTemplateName) as Rectangle;
        //    _rectangleRight = GetTemplateChild(RectangleRightTemplateName) as Rectangle;
        //    _rectangleBottom = GetTemplateChild(RectangleBottomTemplateName) as Rectangle;
        //    _border = GetTemplateChild(BorderTemplateName) as Border;
        //    _border.MouseLeftButtonDown += _border_MouseLeftButtonDown;

        //    _editBar = GetTemplateChild(EditBarTemplateName) as Border;
        //    _buttonSave = GetTemplateChild(ButtonSaveTemplateName) as Button;
        //    if (_buttonSave != null)
        //        _buttonSave.Click += _buttonSave_Click;
        //    _buttonCancel = GetTemplateChild(ButtonCancelTemplateName) as Button;
        //    if (_buttonCancel != null)
        //        _buttonCancel.Click += _buttonCancel_Click;
        //    _buttonComplete = GetTemplateChild(ButtonCompleteTemplateName) as Button;
        //    if (_buttonComplete != null)
        //        _buttonComplete.Click += _buttonComplete_Click;
        //    _radioButtonRectangle = GetTemplateChild(RadioButtonRectangleTemplateName) as RadioButton;
        //    if (_radioButtonRectangle != null)
        //        _radioButtonRectangle.Click += _radioButtonRectangle_Click;
        //    _radioButtonEllipse = GetTemplateChild(RadioButtonEllipseTemplateName) as RadioButton;
        //    if (_radioButtonEllipse != null)
        //        _radioButtonEllipse.Click += _radioButtonEllipse_Click;
        //    _radioButtonArrow = GetTemplateChild(RadioButtonArrowTemplateName) as RadioButton;
        //    if (_radioButtonArrow != null)
        //        _radioButtonArrow.Click += _radioButtonArrow_Click;
        //    _radioButtonInk = GetTemplateChild(RadioButtonInkTemplateName) as RadioButton;
        //    if (_radioButtonInk != null)
        //        _radioButtonInk.Click += _radioButtonInk_Click;
        //    _radioButtonText = GetTemplateChild(RadioButtonTextTemplateName) as RadioButton;
        //    if (_radioButtonText != null)
        //        _radioButtonText.Click += _radioButtonText_Click;
        //    _canvas.Width = Screen.AllScreens[ScreenIndex].Bounds.Width;
        //    _canvas.Height = Screen.AllScreens[ScreenIndex].Bounds.Height;
        //    //_canvas.Background = new ImageBrush(ControlsHelper.Capture());
        //    _canvas.Background = new ImageBrush(ConvertBitmap(CopyScreen()));
        //_rectangleLeft.Width = _canvas.Width;
        //    _rectangleLeft.Height = _canvas.Height;
        //    _border.Opacity = 0;
        //    _popup = GetTemplateChild(PopupTemplateName) as Popup;
        //    _popupBorder = GetTemplateChild(PopupBorderTemplateName) as Border;
        //    _popupBorder.Loaded += (s, e) => { _popup.HorizontalOffset = -_popupBorder.ActualWidth / 3; };
        //    _wrapPanel = GetTemplateChild(WrapPanelColorTemplateName) as WrapPanel;
        //    _wrapPanel.PreviewMouseDown += _wrapPanel_PreviewMouseDown;
        //    controlTemplate = (ControlTemplate)FindResource("WD.PART_DrawArrow");
        //}
        //public static BitmapSource BitmapSourceFromBrush(Brush drawingBrush, int x = 32, int y = 32, int dpi = 96)
        //{
        //    // RenderTargetBitmap = builds a bitmap rendering of a visual
        //    var pixelFormat = PixelFormats.Pbgra32;
        //    RenderTargetBitmap rtb = new RenderTargetBitmap(x, y, dpi, dpi, pixelFormat);

        //    // Drawing visual allows us to compose graphic drawing parts into a visual to render
        //    var drawingVisual = new DrawingVisual();
        //    using (DrawingContext context = drawingVisual.RenderOpen())
        //    {
        //        // Declaring drawing a rectangle using the input brush to fill up the visual
        //        context.DrawRectangle(drawingBrush, null, new Rect(0, 0, x, y));
        //    }

        //    // Actually rendering the bitmap
        //    rtb.Render(drawingVisual);
        //    return rtb;
        //}

        //private BitmapSource ConvertBitmap(Bitmap bitmap)
        //{
        //    BitmapSource img;
        //    IntPtr hBitmap;
        //    hBitmap = bitmap.GetHbitmap();
        //    img = Imaging.CreateBitmapSourceFromHBitmap(
        //        hBitmap,
        //        IntPtr.Zero,
        //        Int32Rect.Empty,
        //        BitmapSizeOptions.FromEmptyOptions());
        //    return img;
        //}

        private ScreenDPI GetScreenDPI(int screenIndex)
        {
            ScreenDPI dpi = new ScreenDPI();
            Screen screen = Screen.AllScreens[ScreenIndex];
            dpi.scaleX = this.RenderScaling/Application.BaseScale;
            dpi.scaleY = this.RenderScaling/Application.BaseScale;
            return dpi;
        }

        private Bitmap CopyScreen()
        {
            Bitmap ScreenCapture = Screen.AllScreens[ScreenIndex].Screenshot();
            return ScreenCapture;
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (e.MouseButton == MouseButton.Right)
                OnCanceled();
            if (e.MouseButton == MouseButton.Left)
                OnPreviewMouseLeftButtonDown(e);
        }

        private void _radioButtonInk_Checked(object sender, EventArgs e)
        {
            RadioButtonChecked(_radioButtonInk, ScreenCutMouseType.DrawInk);
        }

        private void _radioButtonInk_Unchecked(object sender, EventArgs e)
        {
            RadioButtonUnchecked(_radioButtonInk, ScreenCutMouseType.DrawInk);
        }

        private void _radioButtonText_Checked(object sender, EventArgs e)
        {
            RadioButtonChecked(_radioButtonText, ScreenCutMouseType.DrawText);
        }

        private void _radioButtonText_Unchecked(object sender, EventArgs e)
        {
            RadioButtonUnchecked(_radioButtonText, ScreenCutMouseType.DrawText);
        }

        private void _radioButtonArrow_Checked(object sender, EventArgs e)
        {
            RadioButtonChecked(_radioButtonArrow, ScreenCutMouseType.DrawArrow);
        }

        private void _radioButtonArrow_Unchecked(object sender, EventArgs e)
        {
            RadioButtonUnchecked(_radioButtonArrow, ScreenCutMouseType.DrawArrow);
        }

        private void _wrapPanel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is RadioButton)
            {
                var radioButton = (RadioButton)e.OriginalSource;
                _currentBrush = radioButton.Background;
            }
        }

        private void _radioButtonRectangle_Checked(object sender, EventArgs e)
        {
            RadioButtonChecked(_radioButtonRectangle, ScreenCutMouseType.DrawRectangle);
        }

        private void _radioButtonRectangle_Unchecked(object sender, EventArgs e)
        {
            RadioButtonUnchecked(_radioButtonRectangle, ScreenCutMouseType.DrawRectangle);
        }

        private void _radioButtonEllipse_Checked(object sender, EventArgs e)
        {
            RadioButtonChecked(_radioButtonEllipse, ScreenCutMouseType.DrawEllipse);
        }

        private void _radioButtonEllipse_Unchecked(object sender, EventArgs e)
        {
            RadioButtonUnchecked(_radioButtonEllipse, ScreenCutMouseType.DrawEllipse);
        }

        private void RadioButtonUnchecked(RadioButton radioButton, ScreenCutMouseType screenCutMouseTypeRadio)
        {
            if (screenCutMouseType == screenCutMouseTypeRadio)
                Restore();
        }

        private void RadioButtonChecked(RadioButton radioButton, ScreenCutMouseType screenCutMouseTypeRadio)
        {
            screenCutMouseType = screenCutMouseTypeRadio;
            _border.Cursor = Cursors.Arrow;
            if (_popup.PlacementTarget != null && _popup.Visibility == Visibility.Visible)
                _popup.Visibility = Visibility.Collapsed;
            _popup.PlacementTarget = radioButton;
            _popup.Visibility = Visibility.Visible;
            DisposeControl();
        }

        private void Restore()
        {
            _border.Cursor = Cursors.SizeAll;
            if (screenCutMouseType == ScreenCutMouseType.Default) return;
            screenCutMouseType = ScreenCutMouseType.Default;
            if (_popup.PlacementTarget != null && _popup.Visibility == Visibility.Visible)
                _popup.Visibility = Visibility.Collapsed;
        }

        private void ResoreRadioButton()
        {
            _radioButtonRectangle.IsChecked = false;
            _radioButtonEllipse.IsChecked = false;
        }

        private void _border_SizeChanged(object sender, CPFPropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Border.Width) || e.PropertyName == nameof(Border.Height))
            {
                if (isMouseUp)
                {
                    var left = _border.MarginLeft.Value;
                    var top = _border.MarginTop.Value;
                    var beignPoint = new Point(left, top);
                    var endPoint = new Point(left + _border.Width.Value, top + _border.Height.Value);
                    rect = new Rect(beignPoint, endPoint);
                    pointStart = beignPoint;
                    MoveAllRectangle(endPoint);
                }
                EditBarPosition();
            }
        }

        private void _border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(e.MouseButton== MouseButton.Left)
            {
                if (screenCutMouseType == ScreenCutMouseType.Default)
                    screenCutMouseType = ScreenCutMouseType.MoveMouse;
            }
        }

        private async void _buttonSave_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog {
                Title = "保存文件",
                InitialFileName= $"ScreenCut{DateTime.Now.ToString("yyyyMMddHHmmss")}.jpg",
                Filters =
                {
                    new FileDialogFilter
                    {
                        Extensions="jpg",
                        Name="jpg文件",
                        
                    }
                }
            };
            var sf = await dlg.ShowAsync(this);
            if (!String.IsNullOrEmpty(sf))
            {
                Bitmap pngEncoder =CutBitmap();
                using (var fs = System.IO.File.OpenWrite(sf))
                {
                    pngEncoder.SaveToStream(ImageFormat.Png,fs);
                    fs.Dispose();
                    fs.Close();
                    Close();
                }
            }
        }

        private void _buttonComplete_Click(object sender, RoutedEventArgs e)
        {
            var bitmap = CutBitmap();
            if (CutCompleted != null)
                CutCompleted(bitmap);
            Close();
        }

        private Bitmap CutBitmap()
        {
            _border.Visibility = Visibility.Collapsed;
            _editBar.Visibility = Visibility.Collapsed;
            _rectangleLeft.Visibility = Visibility.Collapsed;
            _rectangleTop.Visibility = Visibility.Collapsed;
            _rectangleRight.Visibility = Visibility.Collapsed;
            _rectangleBottom.Visibility = Visibility.Collapsed;
            var renderTargetBitmap = new Bitmap((int)_canvas.ActualSize.Width,
                (int)_canvas.ActualSize.Height);
            using (var dc = DrawingContext.FromBitmap(renderTargetBitmap))
            {
                _canvas.Render(dc);
            }
            Bitmap croppedBitmap = new Bitmap((int)_border.Width.Value,
    (int)_border.Height.Value);
            using (var dc = DrawingContext.FromBitmap(croppedBitmap))
            {
                dc.DrawImage(renderTargetBitmap,
                    new Rect(0, 0, _border.ActualSize.Width, _border.ActualSize.Height),
                    new Rect(_border.MarginLeft.Value, _border.MarginTop.Value, _border.ActualSize.Width, _border.ActualSize.Height));
            }
            return croppedBitmap;
        }

        private void _buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            OnCanceled();
        }
        void OnCanceled()
        {
            Close();
            if (CutCanceled != null)
                CutCanceled();
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Keys.Escape)
            {
                OnCanceled();
            }
            else if (e.Key == Keys.Delete)
            {
                if (_canvas.Children.Count > 0)
                    _canvas.Children.Remove(frameworkElement);
            }
            else if (e.Key == Keys.Z && e.Modifiers == InputModifiers.Control )
            {
                if (_canvas.Children.Count > 0)
                    _canvas.Children.Remove(_canvas.Children[_canvas.Children.Count - 1]);
            }
        }

        protected void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e.OriginalSource is RadioButton)
                return;

            // if is multi-screen, only one screen is allowed.
            if (CaptureScreenID == -1)
            {
                CaptureScreenID = ScreenIndex;
            }
            if (CaptureScreenID != -1 && CaptureScreenID != ScreenIndex)
            {
                e.Handled = true;
                return;
            }

            var vPoint = e.Location;
            if (!isMouseUp)
            {
                pointStart = vPoint;
                screenCutMouseType = ScreenCutMouseType.DrawMouse;
                _editBar.Visibility = Visibility.Hidden;
                pointEnd = pointStart;
                rect = new Rect(pointStart.Value, pointEnd.Value);
            }
            else
            {
                if (vPoint.X < rect.Left || vPoint.X > rect.Right)
                    return;

                if (vPoint.Y < rect.Top || vPoint.Y > rect.Bottom)
                    return;
                pointStart = vPoint;
                if (textBorder != null)
                    Focus();

                switch (screenCutMouseType)
                {
                    case ScreenCutMouseType.DrawText:
                        y1 = vPoint.Y;
                        DrawText();
                        break;
                    default:
                        Focus();
                        break;
                }
            }
        }

        private void DrawText()
        {
            if (pointStart.Value.X < rect.Right
                &&
                pointStart.Value.X > rect.Left
                &&
                pointStart.Value.Y > rect.Top
                &&
                pointStart.Value.Y < rect.Bottom)
            {
                var currentWAndX = pointStart.Value.X + 40;
                if (textBorder == null)
                {
                    textBorder = new Border
                    {
                        BorderFill = _currentBrush == (ViewFill)Color.Empty ? (ViewFill)Color.Red : _currentBrush,
                        BorderThickness = new Thickness(1),
                        Tag = _tag
                    };

                    var textBox = new TextBox();
                    textBox.Background = null;
                    textBox.BorderThickness = new Thickness(0);
                    textBox.Foreground = textBorder.BorderFill;
                    //textBox.FontFamily = DrawingContextHelper.FontFamily;
                    textBox.FontSize = 16;
                    textBox.WordWarp = true;
                    textBox.FontStyle = FontStyles.Regular;
                    //textBox.MinWidth = _width;
                    textBox.MaxWidth = rect.Right - pointStart.Value.X;
                    textBox.MaxHeight = rect.Height - 4;
                    textBox.Cursor = Cursors.Hand;

                    textBox.Padding = new Thickness(4);
                    //textBox.LostKeyboardFocus += (s, e1) =>
                    //{
                    //    var tb = s as TextBox;

                    //    var parent = VisualTreeHelper.GetParent(tb);
                    //    if (parent != null && parent is Border border)
                    //    {
                    //        border.BorderThickness = new Thickness(0);
                    //        if (string.IsNullOrWhiteSpace(tb.Text))
                    //            _canvas.Children.Remove(border);
                    //    }
                    //};
                    //textBorder.SizeChanged += (s, e1) =>
                    //{
                    //    var tb = s as Border;
                    //    var y = y1;
                    //    if (y + tb.ActualHeight > rect.Bottom)
                    //    {
                    //        var v = Math.Abs(rect.Bottom - (y + tb.ActualHeight));
                    //        y1 = y - v;
                    //        Canvas.SetTop(tb, y1 + 2);
                    //    }
                    //};
                    //textBorder.PreviewMouseLeftButtonDown += (s, e) =>
                    //{
                    //    _radioButtonText.IsChecked = true;
                    //    _radioButtonText_Click(null, null);
                    //    SelectElement();
                    //    var border = s as Border;
                    //    frameworkElement = border;
                    //    frameworkElement.Opacity = .7;
                    //    border.BorderThickness = new Thickness(1);
                    //};
                    textBorder.Child = textBox;
                    _canvas.Children.Add(textBorder);
                    textBox.Focus();
                    var x = pointStart.Value.X;

                    if (currentWAndX > rect.Right)
                        x = x - (currentWAndX - rect.Right);
                    textBorder.MarginLeft= x - 2;
                    textBorder.MarginTop= pointStart.Value.Y - 2;
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.OriginalSource is RadioButton)
                return;

            if (pointStart is null)
                return;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var current = e.Location;
                switch (screenCutMouseType)
                {
                    case ScreenCutMouseType.DrawMouse:
                        MoveAllRectangle(current);
                        break;
                    case ScreenCutMouseType.MoveMouse:
                        MoveRect(current);
                        break;
                    case ScreenCutMouseType.DrawRectangle:
                    case ScreenCutMouseType.DrawEllipse:
                        DrawMultipleControl(current);
                        break;
                    case ScreenCutMouseType.DrawArrow:
                        DrawArrowControl(current);
                        break;
                    case ScreenCutMouseType.DrawInk:
                        DrwaInkControl(current);
                        break;
                }
            }
        }

        private void CheckPoint(Point current)
        {
            if (current == pointStart) return;

            if (current.X > rect.BottomRight.X
                ||
                current.Y > rect.BottomRight.Y)
                return;
        }

        private void DrwaInkControl(Point current)
        {
            CheckPoint(current);
            if (current.X >= rect.Left
                &&
                current.X <= rect.Right
                &&
                current.Y >= rect.Top
                &&
                current.Y <= rect.Bottom)
            {
                if (polyLine == null)
                {
                    polyLine = new Polyline();
                    //polyLine.Stroke = _currentBrush == null ? Brushes.Red : _currentBrush;
                    //polyLine.Cursor = Cursors.Hand;
                    //polyLine.StrokeThickness = 3;
                    //polyLine.StrokeLineJoin = PenLineJoin.Round;
                    //polyLine.StrokeStartLineCap = PenLineCap.Round;
                    //polyLine.StrokeEndLineCap = PenLineCap.Round;
                    //polyLine.MouseLeftButtonDown += (s, e) =>
                    //{
                    //    _radioButtonInk.IsChecked = true;
                    //    _radioButtonInk_Click(null, null);
                    //    SelectElement();
                    //    frameworkElement = s as Polyline;
                    //    frameworkElement.Opacity = .7;
                    //};
                    _canvas.Children.Add(polyLine);
                }

                polyLine.Points.Add(current);
            }
        }

        private void DrawArrowControl(Point current)
        {
            CheckPoint(current);
            if (screenCutMouseType != ScreenCutMouseType.DrawArrow)
                return;

            if (pointStart is null)
                return;

            var vPoint = pointStart.Value;

            var drawArrow = new Rect(vPoint, current);
            if (controlArrow == null)
            {
                controlArrow = new Control();
                //controlArrow.Background = _currentBrush == null ? Brushes.Red : _currentBrush;
                //controlArrow.Template = controlTemplate;
                //controlArrow.Cursor = Cursors.Hand;
                //controlArrow.Tag = _tag;
                //controlArrow.MouseLeftButtonDown += (s, e) =>
                //{
                //    _radioButtonArrow.IsChecked = true;
                //    _radioButtonArrow_Click(null, null);
                //    SelectElement();
                //    frameworkElement = s as Control;
                //    frameworkElement.Opacity = .7;
                //};
                //_canvas.Children.Add(controlArrow);
                //Canvas.SetLeft(controlArrow, drawArrow.Left);
                //Canvas.SetTop(controlArrow, drawArrow.Top - 7.5);
            }

            var rotate = new RotateTransform();
            //var renderOrigin = new Point(0, .5);
            //controlArrow.RenderTransformOrigin = renderOrigin;
            controlArrow.RenderTransform = rotate;
            //rotate.Angle = ControlsHelper.CalculeAngle(vPoint, current);
            if (current.X < rect.Left
                ||
                current.X > rect.Right
                ||
                current.Y < rect.Top
                ||
                current.Y > rect.Bottom)
            {
                if (current.X >= vPoint.X && current.Y < vPoint.Y)
                {
                    var a1 = (current.Y - vPoint.Y) / (current.X - vPoint.X);
                    var b1 = vPoint.Y - a1 * vPoint.X;
                    var xTop = (rect.Top - b1) / a1;
                    var yRight = a1 * rect.Right + b1;

                    if (xTop <= rect.Right)
                    {
                        current.X = xTop;
                        current.Y = rect.Top;
                    }
                    else
                    {
                        current.X = rect.Right;
                        current.Y = yRight;
                    }
                }
                else if (current.X > vPoint.X && current.Y > vPoint.Y)
                {
                    var a1 = (current.Y - vPoint.Y) / (current.X - vPoint.X);
                    var b1 = vPoint.Y - a1 * vPoint.X;
                    var xBottom = (rect.Bottom - b1) / a1;
                    var yRight = a1 * rect.Right + b1;

                    if (xBottom <= rect.Right)
                    {
                        current.X = xBottom;
                        current.Y = rect.Bottom;
                    }
                    else
                    {
                        current.X = rect.Right;
                        current.Y = yRight;
                    }
                }
                else if (current.X < vPoint.X && current.Y < vPoint.Y)
                {
                    var a1 = (current.Y - vPoint.Y) / (current.X - vPoint.X);
                    var b1 = vPoint.Y - a1 * vPoint.X;
                    var xTop = (rect.Top - b1) / a1;
                    var yLeft = a1 * rect.Left + b1;
                    if (xTop >= rect.Left)
                    {
                        current.X = xTop;
                        current.Y = rect.Top;
                    }
                    else
                    {
                        current.X = rect.Left;
                        current.Y = yLeft;
                    }
                }
                else if (current.X < vPoint.X && current.Y > vPoint.Y)
                {
                    var a1 = (current.Y - vPoint.Y) / (current.X - vPoint.X);
                    var b1 = vPoint.Y - a1 * vPoint.X;
                    var xBottom = (rect.Bottom - b1) / a1;
                    var yLeft = a1 * rect.Left + b1;

                    if (xBottom <= rect.Left)
                    {
                        current.X = rect.Left;
                        current.Y = yLeft;
                    }
                    else
                    {
                        current.X = xBottom;
                        current.Y = rect.Bottom;
                    }
                }
            }

            var x = current.X - vPoint.X;
            var y = current.Y - vPoint.Y;
            var width = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
            width = width < 15 ? 15 : width;
            controlArrow.Width = width;
        }

        private void DrawMultipleControl(Point current)
        {
            CheckPoint(current);
            if (pointStart is null)
                return;

            var vPoint = pointStart.Value;

            var drawRect = new Rect(vPoint, current);
            switch (screenCutMouseType)
            {
                case ScreenCutMouseType.DrawRectangle:
                    //if (borderRectangle == null)
                    //{
                    //    borderRectangle = new Border
                    //    {
                    //        BorderBrush = _currentBrush == null ? Brushes.Red : _currentBrush,
                    //        BorderThickness = new Thickness(3),
                    //        CornerRadius = new CornerRadius(3),
                    //        Tag = _tag,
                    //        Cursor = Cursors.Hand
                    //    };
                    //    borderRectangle.MouseLeftButtonDown += (s, e) =>
                    //    {
                    //        _radioButtonRectangle.IsChecked = true;
                    //        _radioButtonRectangle_Click(null, null);
                    //        SelectElement();
                    //        frameworkElement = s as Border;
                    //        frameworkElement.Opacity = .7;
                    //    };
                    //    _canvas.Children.Add(borderRectangle);
                    //}

                    break;
                case ScreenCutMouseType.DrawEllipse:
                    //if (drawEllipse == null)
                    //{
                    //    drawEllipse = new Ellipse
                    //    {
                    //        Stroke = _currentBrush == null ? Brushes.Red : _currentBrush,
                    //        StrokeThickness = 3,
                    //        Tag = _tag,
                    //        Cursor = Cursors.Hand
                    //    };
                    //    drawEllipse.MouseLeftButtonDown += (s, e) =>
                    //    {
                    //        _radioButtonEllipse.IsChecked = true;
                    //        _radioButtonEllipse_Click(null, null);
                    //        SelectElement();
                    //        frameworkElement = s as Ellipse;
                    //        frameworkElement.Opacity = .7;
                    //    };
                    //    _canvas.Children.Add(drawEllipse);
                    //}

                    break;
            }

            //var _borderLeft = drawRect.Left - Canvas.GetLeft(_border);

            //if (_borderLeft < 0)
            //    _borderLeft = Math.Abs(_borderLeft);
            //if (drawRect.Width + _borderLeft < _border.ActualWidth)
            //{
            //    var wLeft = Canvas.GetLeft(_border) + _border.ActualWidth;
            //    var left = drawRect.Left < Canvas.GetLeft(_border) ? Canvas.GetLeft(_border) :
            //        drawRect.Left > wLeft ? wLeft : drawRect.Left;
            //    if (borderRectangle != null)
            //    {
            //        borderRectangle.Width = drawRect.Width;
            //        Canvas.SetLeft(borderRectangle, left);
            //    }

            //    if (drawEllipse != null)
            //    {
            //        drawEllipse.Width = drawRect.Width;
            //        Canvas.SetLeft(drawEllipse, left);
            //    }
            //}

            //var _borderTop = drawRect.Top - Canvas.GetTop(_border);
            //if (_borderTop < 0)
            //    _borderTop = Math.Abs(_borderTop);
            //if (drawRect.Height + _borderTop < _border.ActualHeight)
            //{
            //    var hTop = Canvas.GetTop(_border) + _border.Height;
            //    var top = drawRect.Top < Canvas.GetTop(_border) ? Canvas.GetTop(_border) :
            //        drawRect.Top > hTop ? hTop : drawRect.Top;
            //    if (borderRectangle != null)
            //    {
            //        borderRectangle.Height = drawRect.Height;
            //        Canvas.SetTop(borderRectangle, top);
            //    }

            //    if (drawEllipse != null)
            //    {
            //        drawEllipse.Height = drawRect.Height;
            //        Canvas.SetTop(drawEllipse, top);
            //    }
            //}
        }

        private void SelectElement()
        {
            //foreach (var child in  _canvas.Children)
            //{
            //    if (child is Thumb frameworkElement && frameworkElement.Tag != null)
            //        if (frameworkElement.Tag.ToString() == _tag)
            //            frameworkElement.Background = Color.White;
            //}
        }

        private void MoveRect(Point current)
        {
            if (pointStart is null)
                return;

            var vPoint = pointStart.Value;

            if (current != vPoint)
            {
                var vector = Point.Subtract(current, vPoint);
                var left = _border.MarginLeft.Value+ vector.X;
                var top = _border.MarginTop.Value + vector.Y;
                if (left <= 0)
                    left = 0;
                if (top <= 0)
                    top = 0;
                if (left + _border.ActualSize.Width >= _canvas.ActualSize.Width)
                    left = _canvas.ActualSize.Width - _border.ActualSize.Width;
                if (top + _border.ActualSize.Height >= _canvas.ActualSize.Height)
                    top = _canvas.ActualSize.Height - _border.ActualSize.Height;
                pointStart = current;

                _border.MarginLeft= left;
                _border.MarginTop= top;
                rect = new Rect(new Point(left, top), new Point(left + _border.ActualSize.Width, top + _border.ActualSize.Height));
                _rectangleLeft.Height = _canvas.ActualSize.Height;
                _rectangleLeft.Width = left <= 0 ? 0 : left >= _canvas.ActualSize.Width ? _canvas.ActualSize.Width : left;


                _rectangleTop.MarginLeft = _rectangleLeft.Width;
                _rectangleTop.Height = top <= 0 ? 0 : top >= _canvas.ActualSize.Height ? _canvas.ActualSize.Height : top;

                _rectangleRight.MarginLeft= left + _border.Width.Value;
                var wRight = _canvas.ActualSize.Width - (_border.Width.Value + _rectangleLeft.Width.Value);
                _rectangleRight.Width = wRight <= 0 ? 0 : wRight;
                _rectangleRight.Height = _canvas.ActualSize.Height;

                _rectangleBottom.MarginLeft= _rectangleLeft.Width;
                _rectangleBottom.MarginTop= top + _border.Height.Value;
                _rectangleBottom.Width = _border.Width;
                var hBottom = _canvas.ActualSize.Height - (top + _border.Height.Value);
                _rectangleBottom.Height = hBottom <= 0 ? 0 : hBottom;
            }
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            if (e.MouseButton == MouseButton.Left) 
            {
                if (e.OriginalSource is ToggleButton)
                    return;
                if (pointStart == pointEnd)
                {
                    return;
                }
                var fElement = e.OriginalSource as UIElement;
                if (fElement != null && fElement.Tag == null)
                    SelectElement();
                isMouseUp = true;
                if (screenCutMouseType != ScreenCutMouseType.Default)
                {
                    if (screenCutMouseType == ScreenCutMouseType.MoveMouse)
                        EditBarPosition();

                    if (_radioButtonRectangle.IsChecked != true
                        &&
                        _radioButtonEllipse.IsChecked != true
                        &&
                        _radioButtonArrow.IsChecked != true
                        &&
                        _radioButtonText.IsChecked != true
                        &&
                        _radioButtonInk.IsChecked != true)
                        screenCutMouseType = ScreenCutMouseType.Default;
                    else
                        DisposeControl();
                }
            }
        }

        private void DisposeControl()
        {
            polyLine = null;
            textBorder = null;
            borderRectangle = null;
            drawEllipse = null;
            controlArrow = null;
            pointStart = null;
            pointEnd = null;
        }

        private void EditBarPosition()
        {
            _editBar.Visibility = Visibility.Visible;
            _editBar.MarginLeft = rect.X + rect.Width - _editBar.ActualSize.Width;
            var y = _border.MarginTop.Value + _border.ActualSize.Height + _editBar.ActualSize.Height + _popupBorder.ActualSize.Height +
                    24;
            if (y > _canvas.ActualSize.Height && _border.MarginTop.Value > _editBar.ActualSize.Height)
                y = _border.MarginTop.Value - _editBar.ActualSize.Height - 8;
            else if (y > _canvas.ActualSize.Height && _border.MarginTop.Value < _editBar.ActualSize.Height)
                y = _border.ActualSize.Height - _editBar.ActualSize.Height - 8;
            else
                y = _border.MarginTop.Value + _border.ActualSize.Height + 8;
            _editBar.MarginTop= y;
            if (_popup != null && _popup.Visibility== Visibility.Collapsed)
            {
                _popup.Visibility = Visibility.Visible;
            }
        }

        private void MoveAllRectangle(Point current)
        {
            if (pointStart is null)
                return;

            var vPoint = pointStart.Value;

            pointEnd = current;
            var vEndPoint = current;

            rect = new Rect(vPoint, vEndPoint);
            _rectangleLeft.Width = rect.X < 0 ? 0 : rect.X > _canvas.ActualSize.Width ? _canvas.ActualSize.Width : rect.X;
            _rectangleLeft.Height = _canvas.ActualSize.Height;

            _rectangleTop.MarginLeft=_rectangleLeft.Width;
            _rectangleTop.Width = rect.Width;
            var h = 0.0;
            if (current.Y < vPoint.Y)
                h = current.Y;
            else
                h = current.Y - rect.Height;

            _rectangleTop.Height = h < 0 ? 0 : h > _canvas.ActualSize.Height ? _canvas.ActualSize.Height : h;

            _rectangleRight.MarginLeft = _rectangleLeft.Width + rect.Width;
            var rWidth = _canvas.ActualSize.Width - (rect.Width + _rectangleLeft.Width.Value);
            _rectangleRight.Width = rWidth < 0 ? 0 : rWidth > _canvas.ActualSize.Width ? _canvas.ActualSize.Width : rWidth;

            _rectangleRight.Height = _canvas.ActualSize.Height;

            _rectangleBottom.MarginLeft = _rectangleLeft.Width;
            _rectangleBottom.MarginTop=rect.Height + _rectangleTop.Height.Value;
            _rectangleBottom.Width = rect.Width;
            var rBottomHeight = _canvas.ActualSize.Height - (rect.Height + _rectangleTop.Height.Value);
            _rectangleBottom.Height = rBottomHeight < 0 ? 0 : rBottomHeight;

            _border.MarginLeft = rect.X;
            _border.MarginTop = rect.Y;
            _border.Height = rect.Height;
            _border.Width = rect.Width;

            if (screenCutAdorner != null) return;
            _border.Effect = new OpacityEffect { Opacity=1};
            screenCutAdorner = new ScreenCutAdorner(_border);
            screenCutAdorner.PreviewMouseDown += (s, e) =>
            {
                Restore();
                ResoreRadioButton();
            };
            _border.MouseDown += _border_MouseLeftButtonDown;
            _border.Child = screenCutAdorner;
            _border.PropertyChanged += _border_SizeChanged;
        }

    }
}