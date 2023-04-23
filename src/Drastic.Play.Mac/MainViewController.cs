// <copyright file="MainViewController.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.Play.Mac.Services;
using Drastic.Play.Services;
using LibVLCSharp.Platforms.Mac;
using LibVLCSharp.Shared;
using Masonry;

namespace Drastic.Play.Mac
{
    public class MainViewController : NSViewController
    {
        private VideoView videoView;
        private LibVLC libVLC;
        private MacAppDispatcher appDispatcher;
        private MacErrorHandler errorHandler;
        private MacFileHandler fileHandler;
        private PlaybackService playback;
        private NSDrawer? bottomDrawer;
        private NSWindow parentWindow;

        private LibVLCSharp.Shared.MediaPlayer mediaPlayer;

        public MainViewController(NSWindow parentWindow)
        {
            this.parentWindow = parentWindow;
            this.appDispatcher = new MacAppDispatcher();
            this.errorHandler = new MacErrorHandler();
            this.fileHandler = new MacFileHandler();

            this.libVLC = new LibVLC(enableDebugLogs: true);
            this.mediaPlayer = new LibVLCSharp.Shared.MediaPlayer(this.libVLC);
            this.videoView = new VideoView { MediaPlayer = this.mediaPlayer };
            this.playback = new PlaybackService(this.libVLC, this.mediaPlayer, this.fileHandler, this.appDispatcher, this.errorHandler);
        }

        public override void LoadView()
        {
            this.View = new NSView(new CGRect(0, 0, 1000, 500));

#pragma warning disable CA1422 // プラットフォームの互換性を検証
            this.bottomDrawer = new NSDrawer(new CGSize(100, 300), NSRectEdge.MinYEdge) { MinContentSize = new CGSize(100, 300), ParentWindow = this.parentWindow };
#pragma warning restore CA1422 // プラットフォームの互換性を検証

            this.View.AddSubview(this.videoView);

            this.videoView.MakeConstraints(make =>
            {
                make.Top.And.Left.And.Right.And.Bottom.EqualTo(this.View);
            });

            this.playback.Player.Play(new Media(this.libVLC, new Uri("http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4")));
        }
    }
}