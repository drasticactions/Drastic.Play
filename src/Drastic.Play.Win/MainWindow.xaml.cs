using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Drastic.Play.Win
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : WindowEx
    {
        public MainWindow()
        {
            this.InitializeComponent();

            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(this.AppTitleBar);

            //this.GetAppWindow().TitleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(0, 0, 0, 0);
            //this.GetAppWindow().TitleBar.ButtonInactiveBackgroundColor = Windows.UI.Color.FromArgb(0, 0, 0, 0);

            var manager = WinUIEx.WindowManager.Get(this);
            manager.Backdrop = new WinUIEx.MicaSystemBackdrop();

            // this.MainFrame.Content = new MainPage(this);
            this.MainWindowGrid.DataContext = this;
        }

        public InputCursor? InputCursor {

            get { return this.MainFrame.InputCursor; }
            set { this.MainFrame.InputCursor = value; }
        }

        /// <summary>
        /// Gets the app logo path.
        /// </summary>
        public string AppLogo => "Icon.logo_header.png";

        public double TitleBarHeight => this.GetAppWindow().TitleBar.Height;
    }

    public class ContextHolder : Frame
    {
        public InputCursor? InputCursor {

            get { return this.ProtectedCursor; }
            set { this.ProtectedCursor = value; }
        }
    }
}
