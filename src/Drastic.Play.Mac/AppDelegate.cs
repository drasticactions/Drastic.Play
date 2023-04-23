// <copyright file="AppDelegate.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace Drastic.Play.Mac;

[Register("AppDelegate")]
public class AppDelegate : NSApplicationDelegate
{
    private NSWindowController? mainWindowController;

    /// <inheritdoc/>
    public override void DidFinishLaunching(NSNotification notification)
    {
        this.mainWindowController = new MainWindowController();
        this.mainWindowController.Window.MakeKeyAndOrderFront(this);
    }

    public override void WillTerminate(NSNotification notification)
    {
        // Insert code here to tear down your application
    }
}
