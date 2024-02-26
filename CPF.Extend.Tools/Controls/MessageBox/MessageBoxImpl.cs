using CPF.Controls;
using CPF.Drawing;
using CPF.Shapes;

namespace CPF.Extend.Tools.Controls.MessageBox
{
    /// <summary>
    /// 文本的消息框的具体实现
    /// </summary>
    internal class MessageBoxImpl: Window
    {
        private const string PATH_INFORMATION = "M426 726l384-384-60-62-324 324-152-152-60 60zM512 86c236 0 426 190 426 426s-190 426-426 426-426-190-426-426 190-426 426-426z";
        private const string PATH_QUESTION = "M10 20a10 10 0 1 1 0-20 10 10 0 0 1 0 20zm2-13c0 .28-.21.8-.42 1L10 9.58c-.57.58-1 1.6-1 2.42v1h2v-1c0-.29.21-.8.42-1L13 9.42c.57-.58 1-1.6 1-2.42a4 4 0 1 0-8 0h2a2 2 0 1 1 4 0zm-3 8v2h2v-2H9z";
        private const string PATH_WARNING = "M512 960c-281.6 0-512-230.4-512-512s230.4-512 512-512 512 230.4 512 512-230.4 512-512 512zM576 128h-128v448h128v-448zM576 640h-128v128h128v-128z";
        private const string PATH_ERROR = "M264 456Q210 456 164 429 118 402 91 356 64 310 64 256 64 202 91 156 118 110 164 83 210 56 264 56 318 56 364 83 410 110 437 156 464 202 464 256 464 310 437 356 410 402 364 429 318 456 264 456ZM264 288L328 352 360 320 296 256 360 192 328 160 264 224 200 160 168 192 232 256 168 320 200 352 264 288Z";

        private readonly string _messageString;
        private readonly string _titleString;

        private Button _buttonCancel;
        private Button _buttonOK;
        private Visibility _cancelVisibility = Visibility.Collapsed;
        private PathGeometry _geometry;
        private TextBox _message;
        private Visibility _okVisibility;
        private Color _color;
        private Path _path;

        /// <summary>
        /// 使用指定的消息文本初始化消息框实例。
        /// </summary>
        /// <param name="message">要显示的消息文本。</param>
        public MessageBoxImpl(string message)
        {
            // 初始化消息框实例，并设置消息文本
            _messageString = message;
        }

        /// <summary>
        /// 使用指定的消息文本和标题初始化消息框实例。
        /// </summary>
        /// <param name="message">要显示的消息文本。</param>
        /// <param name="caption">消息框的标题。</param>
        public MessageBoxImpl(string message, string caption)
        {
            // 初始化消息框实例，并设置标题和消息文本
            _titleString = caption;
            _messageString = message;
        }

        /// <summary>
        /// 使用指定的消息文本、标题和按钮类型初始化消息框实例。
        /// </summary>
        /// <param name="message">要显示的消息文本。</param>
        /// <param name="caption">消息框的标题。</param>
        /// <param name="button">要显示的按钮类型。</param>
        public MessageBoxImpl(string message, string caption, MessageBoxButton button)
        {
            // 初始化消息框实例，并设置标题、消息文本和按钮类型
            _titleString = caption;
            _messageString = message;
        }

        /// <summary>
        /// 使用指定的消息文本、标题和图像类型初始化消息框实例。
        /// </summary>
        /// <param name="message">要显示的消息文本。</param>
        /// <param name="caption">消息框的标题。</param>
        /// <param name="image">要显示的图像类型。</param>
        public MessageBoxImpl(string message, string caption, MessageBoxImage image)
        {
            // 初始化消息框实例，并设置标题、消息文本和图像类型
            _titleString = caption;
            _messageString = message;
            DisplayImage(image);
        }

        /// <summary>
        /// 使用指定的消息文本、标题、按钮类型和图像类型初始化消息框实例。
        /// </summary>
        /// <param name="message">要显示的消息文本。</param>
        /// <param name="caption">消息框的标题。</param>
        /// <param name="button">要显示的按钮类型。</param>
        /// <param name="image">要显示的图像类型。</param>
        public MessageBoxImpl(string message, string caption, MessageBoxButton button, MessageBoxImage image)
        {
            // 初始化消息框实例，并设置标题、消息文本、按钮类型和图像类型
            _titleString = caption;
            _messageString = message;
            DisplayImage(image);
            DisplayButtons(button);
        }

        /// <summary>
        /// 获取或设置窗口的启动位置。
        /// </summary>
        public WindowStartupLocation WindowStartupLocation
        {
            get
            {
                return GetValue<WindowStartupLocation>();
            }
            set
            {
                SetValue<WindowStartupLocation>(value);
            }
        }

        /// <summary>
        /// 获取或设置窗口的拥有者。
        /// </summary>
        public Window Owner
        {
            get
            {
                return GetValue<Window>();
            }
            set
            {
                SetValue<Window>(value);
            }
        }

        /// <summary>
        /// 获取或设置消息框的结果。
        /// </summary>
        public MessageBoxResult Result { get; set; }

        /// <summary>
        /// 初始化组件。
        /// </summary>
        protected override void InitializeComponent()
        {
            CanResize = false;
            Background = null;
            Title = _titleString;
            MinWidth = 260;
            Children.Add(new WindowFrame(this, new Panel
            {
                Width = "100%",
                Height = "100%",
                Children =
                {
                    new Grid
                    {
                        Width = "100%",
                        Height = "100%",
                        RowDefinitions =
                        {
                            new RowDefinition{ Height="auto" },
                            new RowDefinition{ Height="auto" },
                        },
                        Children=
                        { 
                            new Panel
                            {
                                Margin=new ThicknessField(20),
                                Children=
                                {
                                    new DockPanel
                                    {
                                        MarginLeft=0,
                                        Children={
                                            new Path
                                            {
                                                PresenterFor = this,
                                                Name="Path",
                                                IsAntiAlias=true,
                                                Width=25,
                                                Height=25,
                                                Stretch= Stretch.Fill
                                            },
                                            new TextBox
                                            {
                                                PresenterFor = this,
                                                Name="Message",
                                                BorderThickness= new Thickness(0),
                                                MaxWidth=500,
                                                Padding=new Thickness(10,0),
                                                IsReadOnly=true,
                                                WordWarp= true,
                                            }
                                        }

                                    }
                                },
                                Attacheds =
                                {
                                    {Grid.RowIndex,0}
                                }
                            },
                            new Panel
                            {
                                Margin="140,20,10,10",
                                Children={ 
                                    new StackPanel
                                    { 
                                        Orientation= Orientation.Horizontal,
                                        MarginRight=0,
                                        Children =
                                        {
                                            new Button{ PresenterFor = this,Name="ButtonCancel",Content="取消" , Visibility= Visibility.Collapsed},
                                            new Button{ PresenterFor = this,Name="ButtonOK",Content="确认",MarginLeft=10 },
                                        }
                                    }
                                },
                                Attacheds =
                                {
                                    {Grid.RowIndex,1 }
                                }
                            }
                        }
                    } 
                }
            }));
        }

        /// <summary>
        /// 在初始化完成时调用。
        /// </summary>
        protected override void OnInitialized()
        {
            base.OnInitialized();
           
            _message = FindPresenterByName<TextBox>("Message");
            if (_message != null)
            {
                _message.Text = _messageString;
            }

            _path = FindPresenterByName<Path>("Path");
            if (_path != null)
            {
                _path.Data = _geometry;
                _path.Fill = _color;
            }

            _buttonCancel = FindPresenterByName<Button>("ButtonCancel");
            if (_buttonCancel != null)
            {
                _buttonCancel.Visibility = _cancelVisibility;
                _buttonCancel.Click += _buttonCancel_Click;
            }

            _buttonOK = FindPresenterByName<Button>("ButtonOK");
            if (_buttonOK != null)
            {
                _buttonOK.Visibility = _okVisibility;
                _buttonOK.Click += _buttonOK_Click;
            }

            LayoutManager.ExecuteLayoutPass();
            int leftDeviceUnits = this.Position.X;
            int topDeviceUnits = this.Position.Y;
            Size currentSizeDeviceUnits = this.ActualSize;
            if ((CalculateWindowLocation(ref leftDeviceUnits, ref topDeviceUnits, currentSizeDeviceUnits)) && WindowState == WindowState.Normal)
            {
                this.Position = new PixelPoint(leftDeviceUnits, topDeviceUnits);
            }
        }

        private bool CalculateWindowLocation(ref int leftDeviceUnits, ref int topDeviceUnits, Size currentSizeDeviceUnits)
        {
            double value = leftDeviceUnits;
            double value2 = topDeviceUnits;
            switch (WindowStartupLocation)
            {
                case WindowStartupLocation.CenterScreen:
                    {
                        CalculateCenterScreenPosition(currentSizeDeviceUnits, ref leftDeviceUnits, ref topDeviceUnits);
                        break;
                    }
                case WindowStartupLocation.CenterOwner:
                    {
                        Rect rect = Rect.Empty;
                        if (WindowState == WindowState.Maximized || WindowState == WindowState.Minimized)
                        {
                            goto case WindowStartupLocation.CenterScreen;
                        }

                        Size windowSize = Owner.ActualSize;
                        PixelPoint point = new PixelPoint((int)windowSize.Width, (int)windowSize.Height);


                        PixelPoint point2 = this.Owner.Position;
                        rect = new Rect(point2.X, point2.Y, point.X, point.Y);


                        if (!rect.IsEmpty)
                        {
                            leftDeviceUnits = (int)(rect.X + (int)(rect.Width - currentSizeDeviceUnits.Width) / 2.0);
                            topDeviceUnits = (int)(rect.Y + (rect.Height - currentSizeDeviceUnits.Height) / 2.0);
                            Rect rECT = this.Screen.WorkingArea;
                            leftDeviceUnits = (int)System.Math.Min(leftDeviceUnits, (double)rECT.Right - currentSizeDeviceUnits.Width);
                            leftDeviceUnits = (int)System.Math.Max(leftDeviceUnits, rECT.Left);
                            topDeviceUnits = (int)System.Math.Min(topDeviceUnits, (double)rECT.Bottom - currentSizeDeviceUnits.Height);
                            topDeviceUnits = System.Math.Max(topDeviceUnits, (int)rECT.Top);
                        }

                        break;
                    }
            }

            return true;
        }

        internal void CalculateCenterScreenPosition(Size currentSizeDeviceUnits, ref int leftDeviceUnits, ref int topDeviceUnits)
        {
            Rect rECT = this.Screen.WorkingArea;
            float num = rECT.Right - rECT.Left;
            float num2 = rECT.Bottom - rECT.Top;
            leftDeviceUnits = (int)(rECT.Left + (num - currentSizeDeviceUnits.Width) / 2.0);
            topDeviceUnits = (int)(rECT.Top + (num2 - currentSizeDeviceUnits.Height) / 2.0);
        }

        /// <summary>
        /// 当“确定”按钮被点击时执行的操作。
        /// </summary>
        /// <param name="sender">事件源对象。</param>
        /// <param name="e">事件参数。</param>
        private void _buttonOK_Click(object sender, RoutedEventArgs e)
        {
            // 设置结果为“确定”并关闭窗口
            Result = MessageBoxResult.OK;
            Close();
        }

        /// <summary>
        /// 当“取消”按钮被点击时执行的操作。
        /// </summary>
        /// <param name="sender">事件源对象。</param>
        /// <param name="e">事件参数。</param>
        private void _buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            // 设置结果为“取消”并关闭窗口
            Result = MessageBoxResult.Cancel;
            Close();
        }

        /// <summary>
        /// 根据指定的按钮类型显示相应的按钮。
        /// </summary>
        /// <param name="button">要显示的按钮类型。</param>
        private void DisplayButtons(MessageBoxButton button)
        {
            switch (button)
            {
                case MessageBoxButton.OKCancel:
                case MessageBoxButton.YesNo:
                    // 显示“确定”和“取消”按钮
                    _cancelVisibility = Visibility.Visible;
                    _okVisibility = Visibility.Visible;
                    break;
                default:
                    // 显示“确定”按钮
                    _okVisibility = Visibility.Visible;
                    break;
            }
        }


        /// <summary>
        /// 显示指定图像的路径几何形状。
        /// </summary>
        /// <param name="image">要显示的图像类型。</param>
        private void DisplayImage(MessageBoxImage image)
        {
            switch (image)
            {
                case MessageBoxImage.Warning:
                    // 显示警告图像
                    _geometry =  PATH_WARNING;
                    _color = Color.Parse("#E6A23C");
                    break;
                case MessageBoxImage.Error:
                    // 显示错误图像
                    _geometry =  PATH_ERROR;
                    _color = Color.Parse("#F88C8C");
                    break;
                case MessageBoxImage.Information:
                    // 显示信息图像
                    _geometry =  PATH_INFORMATION;
                    _color = Color.Parse("#909399");
                    break;
                case MessageBoxImage.Question:
                    // 显示询问图像
                    _geometry =  PATH_QUESTION;
                    _color = Color.Parse("#409EFF");
                    break;
            }
        }

    }
}
