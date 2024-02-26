using CPF.Drawing;
using CPF.Input;
using CPF.Platform;
using System.Collections.Generic;

namespace CPF.Extend.Tools.Controls.ScreenCapturer
{
    /// <summary>
    /// 屏幕截图类。
    /// </summary>
    public class ScreenCapture
    {
        /// <summary>
        /// 截图完成委托
        /// </summary>
        public delegate void ScreenShotDone(Bitmap bitmap);
        /// <summary>
        /// 截图完成事件
        /// </summary>
        public event ScreenShotDone SnapCompleted;
        /// <summary>
        /// 截图取消委托
        /// </summary>
        public delegate void ScreenShotCanceled();
        /// <summary>
        /// 截图取消事件
        /// </summary>
        public event ScreenShotCanceled SnapCanceled;
        /// <summary>
        /// 是否将截图结果复制
        /// 默认复制
        /// </summary>
        private bool copyToClipboard;
        /// <summary>
        /// 屏幕截图实例列表
        /// </summary>
        List<ScreenCutImpl> ScreenCuts = new List<ScreenCutImpl>();

        /// <summary>
        /// 初始化 ScreenCapture 类的新实例，并指定是否将截图复制到剪贴板。
        /// </summary>
        /// <param name="copyToClipboard">指定是否将截图复制到剪贴板。</param>
        public ScreenCapture(bool copyToClipboard = true)
        {
            // 初始化复制到剪贴板的标志
            this.copyToClipboard = copyToClipboard;
            // 遍历所有屏幕并进行截图
            for (var i = 0; i < Screen.AllScreens.Count; i++)
            {
                // 将截图添加到截图列表中
                ScreenCuts.Add(CaptureScreen(i));
            }
        }

        /// <summary>
        /// 对指定索引的屏幕进行截图。
        /// </summary>
        /// <param name="index">要截图的屏幕索引。</param>
        /// <returns>表示截图结果的 ScreenCutImpl 实例。</returns>
        private ScreenCutImpl CaptureScreen(int index)
        {
            // 创建 ScreenCutImpl 实例
            ScreenCutImpl screenCut = new ScreenCutImpl(index);
            // 订阅截图完成事件
            screenCut.CutCompleted += ScreenCut_CutCompleted;
            // 订阅截图取消事件
            screenCut.CutCanceled += ScreenCut_CutCanceled;
            // 订阅关闭事件
            screenCut.Closed += ScreenCut_Closed;
            return screenCut;
        }

        /// <summary>
        /// 当截图被取消时调用。
        /// </summary>
        private void ScreenCut_CutCanceled()
        {
            // 触发截图取消事件
            if (SnapCanceled != null) SnapCanceled();
        }

        /// <summary>
        /// 对所有屏幕进行截图操作。
        /// </summary>
        public void Capture()
        {
            // 对每个屏幕进行截图
            foreach (var screenCut in ScreenCuts)
            {
                // 显示截图界面
                screenCut.Show();
                // 将焦点设置到截图界面
                screenCut.Focus();
            }
        }

        /// <summary>
        /// 当截图界面关闭时调用。
        /// </summary>
        /// <param name="sender">事件源对象。</param>
        /// <param name="e">事件参数。</param>
        private void ScreenCut_Closed(object sender, System.EventArgs e)
        {
            // 检查是否存在于截图列表中
            if (ScreenCuts.Contains((ScreenCutImpl)sender))
            {
                // 从截图列表中移除
                ScreenCuts.Remove((ScreenCutImpl)sender);
            }
            // 关闭所有截图界面
            CloseCutters();
            // 清除截图的屏幕标识
            ScreenCutImpl.ClearCaptureScreenID();
        }

        /// <summary>
        /// 关闭所有截图界面。
        /// </summary>
        private void CloseCutters()
        {
            // 如果没有截图界面，直接返回
            if (ScreenCuts.Count == 0) return;
            // 逐个关闭截图界面
            while (ScreenCuts.Count > 0)
            {
                ScreenCuts[0].Close();
            }
            // 清空截图列表
            ScreenCuts.Clear();
        }

        /// <summary>
        /// 当截图完成时调用。
        /// </summary>
        /// <param name="bitmap">截取的位图图像。</param>
        private void ScreenCut_CutCompleted(Bitmap bitmap)
        {
            // 触发截图完成事件
            SnapCompleted?.Invoke(bitmap);
            // 如果设置了复制到剪贴板，则将截图复制到剪贴板
            if (copyToClipboard)
                Clipboard.SetData((DataFormat.Image, bitmap));
        }
    }
}