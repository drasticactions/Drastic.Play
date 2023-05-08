using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage.Streams;
using WinUIEx;

namespace Drastic.Play.Win.Tools
{
    /// <summary>
    /// WinUI Extensions.
    /// </summary>
    public static class WinUIExtensions
    {
        /// <summary>
        /// Small Icon.
        /// </summary>
        private const int ICONSMALL = 0;

        /// <summary>
        /// Big Icon.
        /// </summary>
        private const int ICONBIG = 1;

        /// <summary>
        /// Icon Small 2.
        /// </summary>
        private const int ICONSMALL2 = 2;

        /// <summary>
        /// Get Icon.
        /// </summary>
        private const int WMGETICON = 0x007F;

        /// <summary>
        /// Set Icon.
        /// </summary>
        private const int WMSETICON = 0x0080;

        private const long APPMODELERRORNOPACKAGE = 15700L;

        /// <summary>
        /// Is the app running as a UWP.
        /// </summary>
        /// <returns>bool.</returns>
        public static bool IsRunningAsUwp()
        {
            int length = 0;
            StringBuilder sb = new StringBuilder(0);
            int result = GetCurrentPackageFullName(ref length, sb);

            sb = new StringBuilder(length);
            result = GetCurrentPackageFullName(ref length, sb);

            return result != APPMODELERRORNOPACKAGE;
        }

        /// <summary>
        /// Get the current version of app. Returns the store version if UWP. Returns the assembly version if unpackaged.
        /// </summary>
        /// <returns>String.</returns>
        public static string GetAppVersion()
        {
            if (IsRunningAsUwp())
            {
                Package package = Package.Current;
                PackageId packageId = package.Id;
                PackageVersion version = packageId.Version;

                return string.Format("{0}.{1}.{2}.{3}-Store", version.Major, version.Minor, version.Build, version.Revision);
            }

            return Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Missing";
        }

        /// <summary>
        /// Create a random access stream from a byte array.
        /// </summary>
        /// <param name="array">The byte array.</param>
        /// <returns><see cref="IRandomAccessStream"/>.</returns>
        public static IRandomAccessStream ToRandomAccessStream(this byte[] array)
        {
            InMemoryRandomAccessStream ms = new InMemoryRandomAccessStream();
            using (DataWriter writer = new DataWriter(ms.GetOutputStreamAt(0)))
            {
                writer.WriteBytes(array);
                writer.StoreAsync().GetResults();
            }

            return ms;
        }

        /// <summary>
        /// Set the Icon for this <see cref="Window"/> out from the current process, which is the same as the ApplicationIcon set in the *.csproj.
        /// </summary>
        /// <param name="window">Window.</param>
        public static void SetIconFromApplicationIcon(this Window window)
        {
            if (IsRunningAsUwp())
            {
                return;
            }

            // https://learn.microsoft.com/en-us/answers/questions/822928/app-icon-windows-app-sdk.html
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            string sExe = System.Diagnostics.Process.GetCurrentProcess().MainModule!.FileName;
            var ico = System.Drawing.Icon.ExtractAssociatedIcon(sExe);
            SendMessage(hWnd, WMSETICON, ICONBIG, ico!.Handle);
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int GetCurrentPackageFullName(ref int packageFullNameLength, StringBuilder packageFullName);

        /// <summary>
        /// Send Message to App.
        /// </summary>
        /// <param name="hWnd">Pointer.</param>
        /// <param name="msg">Message.</param>
        /// <param name="wParam">W Parameter.</param>
        /// <param name="lParam">L Parameter.</param>
        /// <returns>Int.</returns>
        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, uint msg, int wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(out Windows.Foundation.Point point);

        public static void ChangeCursor(this UIElement uiElement, InputCursor cursor)
        {
            Type type = typeof(UIElement);
            type.InvokeMember("ProtectedCursor", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty | BindingFlags.Instance, null, uiElement, new object[] { cursor });
        }

        /// <summary>
        /// Gets a value indicating whether if the current view is full screen.
        /// </summary>
        /// <returns>Bool.</returns>
        public static bool IsFullScreen(this Window window) => false;

        /// <summary>
        /// Gets a value indicating whether the current view is compact overlay.
        /// </summary>
        public static bool IsCompactOverlay(this Window window) => window.GetAppWindow().Presenter.Kind == Microsoft.UI.Windowing.AppWindowPresenterKind.CompactOverlay;

        /// <summary>
        /// Gets the title bar height.
        /// </summary>
        public static double TitleBarHeight(this Window window) => window.GetAppWindow().TitleBar.Height;

        /// <summary>
        /// Go to full screen.
        /// </summary>
        /// <param name="fullScreen">Use Full Screen.</param>
        public static void GoFullScreen(this Window window, bool fullScreen)
        {
            if (!fullScreen)
            {
                window.GetAppWindow().SetPresenter(AppWindowPresenterKind.Default);
            }
            else
            {
                window.GetAppWindow().SetPresenter(AppWindowPresenterKind.FullScreen);
            }
        }

        /// <summary>
        /// Go to compact view.
        /// </summary>
        /// <param name="compactMode">Use compact mode.</param>
        public static async void GoCompact(this Window window, bool compactMode)
        {
            if (!compactMode)
            {
                window.GetAppWindow().SetPresenter(AppWindowPresenterKind.Default);
            }
            else
            {
                window.GetAppWindow().SetPresenter(AppWindowPresenterKind.CompactOverlay);
            }
        }
    }
}
