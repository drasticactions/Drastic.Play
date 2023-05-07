using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Core;
using Drastic.Play.Interfaces;
using Drastic.Play.Win.Tools;

namespace Drastic.Play.Win.Services
{
    /// <summary>
    /// Mouse Service.
    /// </summary>
    public class MouseService : IMouseService
    {
        private const int CursorHiddenAfterSeconds = 4;
        private InputCursor? oldCursor;
        private DispatcherTimer? cursorTimer;
        private bool isMouseVisible = true;
        private MainWindow window;

        /// <summary>
        /// Fired when mouse cursor is hidden.
        /// </summary>
        public event EventHandler? OnHidden;

        /// <summary>
        /// Fired when mouse cursor moves.
        /// </summary>
        public event EventHandler? OnMoved;

        public MouseService(MainWindow window)
        {
            this.window = window;
        }

        /// <summary>
        /// Starts Mouse Service.
        /// </summary>
        public void StartService()
        {
            this.cursorTimer = new DispatcherTimer();
            this.cursorTimer.Interval = TimeSpan.FromSeconds(CursorHiddenAfterSeconds);
            this.cursorTimer.Tick += this.CursorDisappearanceTimer;

            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                this.window.Content.PointerMoved += Content_PointerMoved;
            }
        }

        private void Content_PointerMoved(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            this.InputDetected();
        }

        /// <summary>
        /// Ends Mouse Service.
        /// </summary>
        public void StopService()
        {
            if (this.cursorTimer is not null)
            {
                this.cursorTimer.Tick -= this.CursorDisappearanceTimer;
                this.cursorTimer.Stop();
            }

            this.window.Content.PointerMoved -= Content_PointerMoved;
        }

        /// <summary>
        /// Hides Mouse Cursor.
        /// </summary>
        public void HideCursor()
        {
            if (!this.isMouseVisible)
            {
                return;
            }

            if (IsCursorInWindow())
            {
                this.oldCursor = this.window.InputCursor;
                this.window.InputCursor = null;
                this.isMouseVisible = false;
            }
        }

        /// <summary>
        /// Show Mouse Cursor.
        /// </summary>
        public void ShowCursor()
        {
            if (this.isMouseVisible)
            {
                return;
            }

            this.isMouseVisible = true;
            if (this.oldCursor != null)
            {
                this.window.InputCursor = this.oldCursor;
            }
        }

        /// <summary>
        /// Is the mouse cursor in the window.
        /// </summary>
        /// <returns>Bool.</returns>
        private bool IsCursorInWindow()
        {
            var pos = GetPointerPosition();

            return pos.Y > this.window.TitleBarHeight &&
                   pos.Y < this.window.Bounds.Height &&
                   pos.X > 0 &&
                   pos.X < this.window.Bounds.Width;
        }

        private Point GetPointerPosition()
        {
            Window currentWindow = this.window;
            Point point;

            try
            {
                WinUIExtensions.GetCursorPos(out point);
            }
            catch (UnauthorizedAccessException)
            {
                return new Point(double.NegativeInfinity, double.NegativeInfinity);
            }

            Rect bounds = currentWindow.Bounds;
            return new Point(point.X - bounds.X, point.Y - bounds.Y);
        }

        private void InputDetected()
        {
            if (!IsCursorInWindow())
            {
                return;
            }

            this.cursorTimer?.Stop();
            this.cursorTimer?.Start();

            // TODO: Enable/Disable Show Cursor.
            this.ShowCursor();
            this.OnMoved?.Invoke(this, new EventArgs());
        }

        private void OnCursorDisappearance()
        {
            this.cursorTimer?.Stop();
            this.HideCursor();
            this.OnHidden?.Invoke(this, new EventArgs());
        }

        private void CursorDisappearanceTimer(object? sender, object e)
        {
            this.OnCursorDisappearance();
        }
    }
}
