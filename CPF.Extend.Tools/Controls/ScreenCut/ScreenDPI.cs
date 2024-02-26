namespace CPF.Extend.Tools.Controls.ScreenCut
{
    /// <summary>
    /// 表示屏幕的 DPI 信息。
    /// </summary>
    public struct ScreenDPI
    {
        /// <summary>
        /// 水平方向的 DPI 值。
        /// </summary>
        public uint dpiX;

        /// <summary>
        /// 垂直方向的 DPI 值。
        /// </summary>
        public uint dpiY;

        /// <summary>
        /// 水平方向的缩放比例。
        /// </summary>
        public float scaleX;

        /// <summary>
        /// 垂直方向的缩放比例。
        /// </summary>
        public float scaleY;
    }
}
