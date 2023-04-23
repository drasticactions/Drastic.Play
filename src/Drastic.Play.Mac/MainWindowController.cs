// <copyright file="MainWindowController.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;

namespace Drastic.Play.Mac
{
    public class MainWindowController : NSWindowController
    {
        public MainWindowController()
            : base()
        {
            // Construct the window from code here
            CGRect contentRect = new CGRect(0, 0, 1000, 500);
            base.Window = new MainWindow(contentRect, NSWindowStyle.Titled | NSWindowStyle.Closable | NSWindowStyle.Miniaturizable | NSWindowStyle.Resizable, NSBackingStore.Buffered, false);

            // Simulate Awaking from Nib
            this.Window.AwakeFromNib();
        }

        public new MainWindow Window
        {
            get { return (MainWindow)base.Window; }
        }
    }
}