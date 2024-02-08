namespace CPF.Extend.Tools.Controls.MessageBox
{
    /// <summary>
    /// 指定用户点击了哪个消息框按钮。
    /// CPF.Extend.Tools.Controls.MessageBox由Overload:CPF.Extend.Tools.Controls.MessageBox.MessageBox.Show方法返回。
    /// </summary>
    public enum MessageBoxResult
    {
        /// <summary>
        /// 消息框没有返回结果。
        /// </summary>
        None = 0,
        /// <summary>
        /// 消息框的结果值为“确定”。
        /// </summary>
        OK = 1,
        /// <summary>
        /// 消息框的结果值为“取消”。
        /// </summary>
        Cancel = 2,
        /// <summary>
        /// 消息框的结果值为“取消”。
        /// </summary>
        Yes = 6,
        /// <summary>
        /// 消息框的结果值为“否”。
        /// </summary>
        No = 7
    }
}
