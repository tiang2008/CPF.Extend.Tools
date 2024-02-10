namespace CPF.Extend.Tools.Controls.MessageBox
{
    /// <summary>
    /// 指定 CPF.Controls.Window 首次打开时显示的位置。
    /// 由 CPF.Controls.MessageBoxImpl.WindowStartupLocation 属性使用。
    /// </summary>
    public enum WindowStartupLocation
    {
        /// <summary>
        /// CPF.Controls.Window 的启动位置从代码设置，或者使用默认的 Windows 位置。
        /// </summary>
        Manual,  
        /// <summary>
        /// CPF.Controls.Window 的启动位置是包含鼠标光标的屏幕的中心。
        /// </summary>
        CenterScreen,
        /// <summary>
        /// CPF.Controls.Window 的启动位置是由 CPF.Controls.Window.Owner 属性指定的 CPF.Controls.Window的中心。
        /// </summary>
        CenterOwner
    }
}
