// <copyright file="MainWindow.xaml.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Controls;
using WinUIEx;

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

            var manager = WinUIEx.WindowManager.Get(this);
            manager.Backdrop = new WinUIEx.MicaSystemBackdrop();

            this.MainFrame.Content = new MainPage(this);
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
