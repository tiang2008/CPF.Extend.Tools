namespace CPF.Extend.Tools.Controls.MessageBox
{
    /// <summary>
    /// 指定消息框上显示的按钮。
    /// 作为Overload:PF.Extend.Tools.Controls.MessageBox.MessageBox.Show方法的参数使用。
    /// </summary>
    public enum MessageBoxButton
    {
        /// <summary>
        /// 消息框显示一个“确定”按钮。
        /// </summary>
        OK = 0,
        /// <summary>
        /// 消息框显示“确定”和“取消”按钮。
        /// </summary>
        OKCancel = 1,
        /// <summary>
        /// 消息框显示“是”、“否”和“取消”按钮。
        /// </summary>
        YesNoCancel = 3,
        /// <summary>
        /// 消息框显示“是”和“否”按钮。
        /// </summary>
        YesNo = 4
    }
}
