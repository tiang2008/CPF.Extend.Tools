using CPF.Controls;
using CPF.Extend.Tools.Controls.MessageBox;
using MessageBox = CPF.Extend.Tools.Controls.MessageBox.MessageBox;

namespace CPF.Extend.Tools.SamplesCode
{
    /// <summary>
    /// 主窗体
    /// </summary>
    public class MainWindow : Window
    {
        /// <summary>
        /// 初始化组件。
        /// </summary>
        protected override void InitializeComponent()
        {
            Title = "示例";
            Width = 500;
            Height = 400;
            Background = null;
            Children.Add(new WindowFrame(this, new Panel
            {
                Width = "100%",
                Height = "100%",
                Children =
                {
                    new StackPanel{ 
                        Orientation= Orientation.Horizontal,
                        Children=
                        {
                            new Button
                            {
                                MarginLeft=10,
                                Content="Info" ,
                                Commands=
                                {
                                    {
                                        nameof(Button.Click),
                                        btnInformation_Click
                                    }

                                }
                            },
                            new Button
                            {
                                MarginLeft=10,
                                Content="Warning" ,
                                Commands=
                                {
                                    {
                                        nameof(Button.Click),
                                        btnWarning_Click
                                    }

                                }
                            },
                            new Button
                            {
                                MarginLeft=10,
                                Content="Error" ,
                                Commands=
                                {
                                    {
                                        nameof(Button.Click),
                                        btnError_Click
                                    }

                                }
                            },
                            new Button
                            {
                                MarginLeft=10,
                                Content="Question" ,
                                Commands=
                                {
                                    {
                                        nameof(Button.Click),
                                        btnQuestion_Click
                                    }

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
        }

        /// <summary>
        /// 当信息按钮被点击时执行的操作。
        /// </summary>
        /// <param name="s">按钮对象。</param>
        /// <param name="e">事件参数。</param>
        private void btnInformation_Click(CpfObject s, object e)
        {
            // 显示信息消息框
            MessageBox.Show("操作成功。", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// 当警告按钮被点击时执行的操作。
        /// </summary>
        /// <param name="s">按钮对象。</param>
        /// <param name="e">事件参数。</param>
        private void btnWarning_Click(CpfObject s, object e)
        {
            // 显示警告消息框
            MessageBox.Show("执行此操作可能导致失败！", "警告", MessageBoxImage.Warning);
        }

        /// <summary>
        /// 当错误按钮被点击时执行的操作。
        /// </summary>
        /// <param name="s">按钮对象。</param>
        /// <param name="e">事件参数。</param>
        private void btnError_Click(CpfObject s, object e)
        {
            // 显示错误消息框
            MessageBox.Show("操作失败。", "错误", MessageBoxImage.Error);
        }

        /// <summary>
        /// 当询问按钮被点击时执行的操作。
        /// </summary>
        /// <param name="s">按钮对象。</param>
        /// <param name="e">事件参数。</param>
        private void btnQuestion_Click(CpfObject s, object e)
        {
            // 显示询问消息框
            MessageBox.Show("是否继续执行该操作?", "询问", MessageBoxButton.OKCancel, MessageBoxImage.Question);
        }
    }
}
