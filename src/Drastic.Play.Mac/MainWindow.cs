// <copyright file="MainWindow.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;

namespace Drastic.Play.Mac
{
    public class MainWindow : NSWindow
    {
        private MainViewController viewController;

        public MainWindow(CGRect contentRect, NSWindowStyle aStyle, NSBackingStore bufferingType, bool deferCreation)
            : base(contentRect, aStyle, bufferingType, deferCreation)
        {
            this.ContentViewController = this.viewController = new MainViewController(this);
        }
    }
}