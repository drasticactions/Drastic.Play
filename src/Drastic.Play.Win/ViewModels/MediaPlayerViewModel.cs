using Drastic.Play.Interfaces;
using Drastic.Play.Models;
using Drastic.Play.Services;
using Drastic.Play.Tools;
using Drastic.Play.Win.Services;
using Drastic.Play.Win.Tools;
using Drastic.Play.Win.Views;
using Drastic.Tools;
using Drastic.ViewModels;
using Kibou.Tools;
using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drastic.Play.Win.ViewModels
{
    public class MediaPlayerViewModel : BaseViewModel
    {
        private PlaybackService playback;
        private IFileHandler fileHandler;
        private IMouseService mouse;
        private AsyncCommand? filePickerCommand;
        private AsyncCommand? playPauseCommand;
        private AsyncCommand? stopCommand;
        private AsyncCommand? openUrlCommand;
        private AsyncCommand? fullscreenMode;
        private AsyncCommand? initPiPCommand;
        private AsyncCommand? addToPlaylistFileCommand;
        private AsyncCommand? addToPlaylistUrlCommand;
        private AsyncCommand? showPlaylistCommand;
        private AsyncCommand? goForwardCommand;
        private AsyncCommand? goBackCommand;
        private bool showSidePane;
        private string currentState = string.Empty;
        private MainWindow window;

        public MediaPlayerViewModel(IServiceProvider services, MainWindow window)
            : base(services)
        {
            this.window = window;
            this.playback = services.GetService(typeof(PlaybackService)) as PlaybackService ?? throw new NullReferenceException(nameof(PlaybackService));
            this.mouse = services.GetService(typeof(IMouseService)) as IMouseService ?? throw new NullReferenceException(nameof(IMouseService));
            this.fileHandler = services.GetService(typeof(IFileHandler)) as IFileHandler ?? throw new NullReferenceException(nameof(IFileHandler));
            this.playback.RaiseCanExecuteChanged += this.VideoPlayerViewModel_RaiseCanExecuteChanged;
            this.mouse.OnMoved += this.Mouse_OnMoved;
            this.mouse.OnHidden += this.Mouse_OnHidden;
            this.mouse.StartService();
        }

        /// <summary>
        /// Gets Playback Service.
        /// </summary>
        public PlaybackService Playback => this.playback;

        /// <summary>
        /// Gets or sets the current visible state of the controls on screen.
        /// </summary>
        public string CurrentState {
            get {
                return this.currentState;
            }

            set {
                this.SetProperty(ref this.currentState, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show the side pane.
        /// </summary>
        public bool ShowSidePane {
            get {
                return this.showSidePane;
            }

            set {
                this.SetProperty(ref this.showSidePane, value);
            }
        }

        /// <summary>
        /// Gets the file picker command.
        /// </summary>
        public AsyncCommand FilePickerCommand {
            get {
                return this.filePickerCommand ??= new AsyncCommand(this.OpenFile, null, this.Dispatcher, this.ErrorHandler);
            }
        }

        /// <summary>
        /// Gets the open url command.
        /// </summary>
        public AsyncCommand OpenUrlCommand {
            get {
                return this.openUrlCommand ??= new AsyncCommand(this.OpenUrl, null, this.Dispatcher, this.ErrorHandler);
            }
        }

        /// <summary>
        /// Gets the go forward command.
        /// </summary>
        public AsyncCommand GoForwardCommand {
            get {
                return this.goForwardCommand ??= new AsyncCommand(this.playback.GoForwardInPlaylistAsync, () => this.playback.CanGoForward, this.Dispatcher, this.ErrorHandler);
            }
        }

        /// <summary>
        /// Gets the go forward command.
        /// </summary>
        public AsyncCommand GoBackCommand {
            get {
                return this.goBackCommand ??= new AsyncCommand(this.playback.GoBackInPlaylistAsync, () => this.playback.CanGoBack, this.Dispatcher, this.ErrorHandler);
            }
        }

        /// <summary>
        /// Gets the add file to playlist command.
        /// </summary>
        public AsyncCommand AddToPlaylistFileCommand {
            get {
                return this.addToPlaylistFileCommand ??= new AsyncCommand(this.AddFilePathToPlaylist, null, this.Dispatcher, this.ErrorHandler);
            }
        }

        /// <summary>
        /// Gets the add url to playlist command.
        /// </summary>
        public AsyncCommand AddToPlaylistUrlCommand {
            get {
                return this.addToPlaylistUrlCommand ??= new AsyncCommand(this.AddURLToPlaylist, null, this.Dispatcher, this.ErrorHandler);
            }
        }

        /// <summary>
        /// Gets the play pause command.
        /// </summary>
        public AsyncCommand PlayPauseCommand {
            get {
                return this.playPauseCommand ??= new AsyncCommand(this.PlayPause, () => this.playback.CurrentMedia != null, this.Dispatcher, this.ErrorHandler);
            }
        }

        /// <summary>
        /// Gets the file picker command.
        /// </summary>
        public AsyncCommand ShowPlaylistCommand {
            get {
                return this.showPlaylistCommand ??= new AsyncCommand(async () => { this.ShowSidePane = true; }, null, this.Dispatcher, this.ErrorHandler);
            }
        }

        /// <summary>
        /// Gets the stop command.
        /// </summary>
        public AsyncCommand StopCommand {
            get {
                return this.stopCommand ??= new AsyncCommand(this.Stop, () => this.playback.CurrentMedia != null, this.Dispatcher, this.ErrorHandler);
            }
        }

        /// <summary>
        /// Gets the FullScreen Command.
        /// </summary>
        public AsyncCommand FullScreenCommand {
            get {
                return this.fullscreenMode ??= new AsyncCommand(this.FullScreen, () => this.playback.CurrentMedia != null, this.Dispatcher, this.ErrorHandler);
            }
        }

        /// <summary>
        /// Gets the Init PiP Command.
        /// </summary>
        public AsyncCommand InitPiPCommand {
            get {
                return this.initPiPCommand ??= new AsyncCommand(this.InitPiP, () => this.playback.CurrentMedia != null, this.Dispatcher, this.ErrorHandler);
            }
        }

        /// <summary>
        /// Gets the app logo path.
        /// </summary>
        public string AppLogo => "Icon.logo_header.png";

        /// <summary>
        /// Gets a value indicating whether the player is playing.
        /// </summary>
        public bool IsPlaying => this.playback != null && this.playback.IsPlaying;

        private void SetCurrentState(bool mouseVisible)
        {
            if (!this.IsPlaying)
            {
                this.CurrentState = "Visible";
                return;
            }

            if (this.IsPlaying && mouseVisible)
            {
                this.CurrentState = "Visible";
                return;
            }
            else
            {
                this.CurrentState = "Hidden";
                return;
            }
        }

        private Task InitPiP()
        {
            if (this.window.IsFullScreen())
            {
                this.window.GoFullScreen(false);
            }

            this.window.GoCompact(!this.window.IsCompactOverlay());
            return Task.CompletedTask;
        }

        private Task FullScreen()
        {
            if (this.window.IsCompactOverlay())
            {
                this.window.GoCompact(false);
            }

            this.window.GoFullScreen(!this.window.IsFullScreen());
            return Task.CompletedTask;
        }

        private void VideoPlayerViewModel_RaiseCanExecuteChanged(object? sender, EventArgs e)
        {
            this.PlayPauseCommand.RaiseCanExecuteChanged();
            this.StopCommand.RaiseCanExecuteChanged();
            this.FullScreenCommand.RaiseCanExecuteChanged();
            this.InitPiPCommand.RaiseCanExecuteChanged();
            this.GoBackCommand.RaiseCanExecuteChanged();
            this.GoForwardCommand.RaiseCanExecuteChanged();
        }

        private void Mouse_OnHidden(object? sender, EventArgs e)
        {
            this.SetCurrentState(false);
        }

        private void Mouse_OnMoved(object? sender, EventArgs e)
        {
            this.SetCurrentState(true);
        }

        private async Task OpenUrl()
        {
            var videoItem = await this.GetVideoUrl();
            if (videoItem != null)
            {
                await this.playback.PlaySingleMediaItemAsync(videoItem);
            }
        }

        private async Task<VideoItem?> GetVideoUrl()
        {
            var urlString = await this.fileHandler.PickUrl();
            if (string.IsNullOrEmpty(urlString))
            {
                return null;
            }

            return MediaHelpers.GenerateVideoItem(FromType.FromLocation, urlString, string.Empty);
        }

        private async Task<VideoItem?> GetVideoItemFromPath()
        {
            var file = await this.fileHandler.PickVideo();

            if (string.IsNullOrEmpty(file))
            {
                return null;
            }

            return MediaHelpers.GenerateVideoItem(FromType.FromPath, file, string.Empty);
        }

        private async Task<List<VideoItem>> GetVideoItemsFromPath()
        {
            var videoItems = new List<VideoItem>();
            var files = await this.fileHandler.PickVideos();

            foreach (var file in files)
            {
                videoItems.Add(MediaHelpers.GenerateVideoItem(FromType.FromPath, file, string.Empty));
            }

            return videoItems;
        }

        private Task PlayPause()
        {
            if (this.playback.Player.State == VLCState.Ended)
            {
                this.playback.Player.Stop();
                this.playback.Player.Play();
            }
            else
            {
                this.playback.Player.Pause();
            }

            return Task.CompletedTask;
        }

        private Task Stop()
        {
            this.playback.Player.Stop();
            return Task.CompletedTask;
        }

        private async Task OpenFile()
        {
            var result = await this.GetVideoItemFromPath();
            if (result != null)
            {
                await this.playback.PlaySingleMediaItemAsync(result);
            }
        }

        private async Task AddFilePathToPlaylist()
        {
            var result = await this.GetVideoItemsFromPath();
            if (result.Any())
            {
                foreach (var item in result)
                {
                    await this.playback.AddItemToPlaylistAsync(item);
                }
            }
        }

        private async Task AddURLToPlaylist()
        {
            var result = await GetVideoUrl();
            if (result != null)
            {
                await this.playback.AddItemToPlaylistAsync(result);
            }
        }
    }
}
