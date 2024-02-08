using CPF.Controls;
using System.Linq;

namespace CPF.Extend.Tools.Controls.MessageBox
{
    /// <summary>
    /// 文本的消息框
    /// </summary>
    public static class MessageBox
    {
        /// <summary>
        /// 显示一个带有指定文本的消息框。
        /// </summary>
        /// <param name="messageBoxText">要显示的消息框文本。</param>
        /// <param name="owner">拥有该消息框的窗口。</param>
        /// <returns>表示用户单击的消息框按钮的结果。</returns>
        public static MessageBoxResult Show(string messageBoxText, Window owner = null)
        {
            // 创建消息框实例
            var msg = new MessageBoxImpl(messageBoxText);
            // 获取消息框的窗口并显示
            return GetWindow(msg, owner);
        }

        /// <summary>
        /// 显示一个带有指定文本和标题的消息框。
        /// </summary>
        /// <param name="messageBoxText">要显示的消息框文本。</param>
        /// <param name="caption">消息框的标题。</param>
        /// <param name="owner">拥有该消息框的窗口。</param>
        /// <returns>表示用户单击的消息框按钮的结果。</returns>
        public static MessageBoxResult Show(string messageBoxText, string caption, Window owner = null)
        {
            // 创建消息框实例
            var msg = new MessageBoxImpl(messageBoxText, caption);
            // 获取消息框的窗口并显示
            return GetWindow(msg, owner);
        }

        /// <summary>
        /// 显示一个带有指定文本、标题和按钮的消息框。
        /// </summary>
        /// <param name="messageBoxText">要显示的消息框文本。</param>
        /// <param name="caption">消息框的标题。</param>
        /// <param name="button">要显示的消息框按钮。</param>
        /// <param name="owner">拥有该消息框的窗口。</param>
        /// <returns>表示用户单击的消息框按钮的结果。</returns>
        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, Window owner = null)
        {
            // 创建消息框实例
            var msg = new MessageBoxImpl(messageBoxText, caption, button);
            // 获取消息框的窗口并显示
            return GetWindow(msg, owner);
        }

        /// <summary>
        /// 显示一个带有指定文本、标题和图标的消息框。
        /// </summary>
        /// <param name="messageBoxText">要显示的消息框文本。</param>
        /// <param name="caption">消息框的标题。</param>
        /// <param name="icon">要显示的消息框图标。</param>
        /// <param name="owner">拥有该消息框的窗口。</param>
        /// <returns>表示用户单击的消息框按钮的结果。</returns>
        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxImage icon, Window owner = null)
        {
            // 创建消息框实例
            var msg = new MessageBoxImpl(messageBoxText, caption, icon);
            // 获取消息框的窗口并显示
            return GetWindow(msg, owner);
        }

        /// <summary>
        /// 显示一个带有指定文本、标题、按钮和图标的消息框。
        /// </summary>
        /// <param name="messageBoxText">要显示的消息框文本。</param>
        /// <param name="caption">消息框的标题。</param>
        /// <param name="button">要显示的消息框按钮。</param>
        /// <param name="icon">要显示的消息框图标。</param>
        /// <param name="owner">拥有该消息框的窗口。</param>
        /// <returns>表示用户单击的消息框按钮的结果。</returns>
        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button,
            MessageBoxImage icon, Window owner = null)
        {
            // 创建消息框实例
            var msg = new MessageBoxImpl(messageBoxText, caption, button, icon);
            // 获取消息框的窗口并显示
            return GetWindow(msg, owner);
        }

        /// <summary>
        /// 获取消息框窗口并显示。
        /// </summary>
        /// <param name="msg">消息框实例。</param>
        /// <param name="owner">拥有该消息框的窗口。</param>
        /// <returns>表示用户单击的消息框按钮的结果。</returns>
        private static MessageBoxResult GetWindow(MessageBoxImpl msg, Window owner = null)
        {
            var main = owner;
            if (main == null)
            {
                // 如果没有指定拥有者窗口，则尝试查找具有键盘焦点的窗口
                main = Window.Windows.FirstOrDefault(a => a.IsKeyboardFocusWithin);
                if (main == null)
                {
                    // 如果没有具有键盘焦点的窗口，则查找主窗口
                    main = Window.Windows.FirstOrDefault(a => a.IsMain);
                }
                // 检查操作系统类型以确定是否需要主窗口
                var os = CPF.Platform.Application.OperatingSystem;
                if (main == null && (os == Platform.OperatingSystemType.Windows || os == Platform.OperatingSystemType.Linux || os == Platform.OperatingSystemType.OSX))
                {
                    // 如果未找到主窗口且操作系统为 Windows、Linux 或 OSX，则抛出异常
                    throw new System.Exception("需要有主窗体");
                }
            }
            // 将消息框的图标设置为拥有者窗口的图标
            msg.Icon = main.Icon;
            // 加载拥有者窗口的样式
            msg.LoadStyle(main);
            // 同步显示消息框
            msg.ShowDialogSync();
            // 返回用户单击的消息框按钮的结果
            return msg.Result;
        }
    }
}
