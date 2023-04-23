// <copyright file="PlaybackService.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Drastic.Play.Interfaces;
using Drastic.Services;
using Drastic.Tools;
using LibVLCSharp.Shared;

namespace Drastic.Play.Services
{
    /// <summary>
    /// Playback Service.
    /// </summary>
    public class PlaybackService : INotifyPropertyChanged
    {
        private MediaPlayer player;
        private LibVLC libVLC;
        private IErrorHandlerService error;
        private IMediaItem? currentMedia;
        private Media? currentVlcMedia;
        private IFileHandler files;
        private int volume;
        private IAppDispatcher appDispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaybackService"/> class.
        /// </summary>
        /// <param name="libVLC">LibVLC Instance.</param>
        /// <param name="player">Media Player Instance.</param>
        /// <param name="error">Error Handler.</param>
        public PlaybackService(LibVLC libVLC, MediaPlayer player, IFileHandler files, IAppDispatcher dispatcher, IErrorHandlerService error)
        {
            this.libVLC = libVLC ?? throw new ArgumentNullException(nameof(libVLC));
            this.player = player ?? throw new ArgumentNullException(nameof(player));
            this.appDispatcher = dispatcher;
            this.files = files;
            this.error = error;
            this.player.Playing += this.Player_Event;
            this.player.Paused += this.Player_Event;
            this.player.EndReached += this.EndPlayer_Event;
            this.player.Stopped += this.Player_Event;
            this.player.TimeChanged += this.Player_Event;
            this.player.LengthChanged += this.Player_Event;
            this.Volume = 75;
        }

        /// <summary>
        /// Raised when CanExecute is changed.
        /// </summary>
        public event EventHandler? RaiseCanExecuteChanged;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets the media playlist.
        /// </summary>
        public ObservableCollection<IMediaItem> MediaPlaylist { get; } = new ObservableCollection<IMediaItem>();

        /// <summary>
        /// Gets or sets the currently playing media.
        /// </summary>
        public IMediaItem? CurrentMedia
        {
            get
            {
                return this.currentMedia;
            }

            set
            {
                this.SetProperty(ref this.currentMedia, value);
                this.OnPropertyChanged(nameof(this.CanGoBack));
                this.OnPropertyChanged(nameof(this.CanGoForward));
                this.RaiseCanExecuteChanged?.Invoke(this, new EventArgs());
                this.appDispatcher.Dispatch(async () => await this.PlayMediaAsync(this.CurrentMedia).ConfigureAwait(false));
            }
        }

        /// <summary>
        /// Gets or sets the currently playing media.
        /// </summary>
        public Media? CurrentVlcMedia
        {
            get
            {
                return this.currentVlcMedia;
            }

            set
            {
                this.SetProperty(ref this.currentVlcMedia, value);
                this.RaiseCanExecuteChanged?.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        /// Gets a value indicating whether the player is playing.
        /// </summary>
        public bool IsPlaying => this.player != null && this.player.IsPlaying;

        /// <summary>
        /// Gets the Media Player.
        /// </summary>
        public MediaPlayer Player => this.player;

        /// <summary>
        /// Gets or Sets the volume of the VLC player.
        /// </summary>
        public int Volume
        {
            get
            {
                return this.volume;
            }

            set
            {
                this.SetProperty(ref this.volume, value);
                this.Player.Volume = this.volume;
            }
        }

        /// <summary>
        /// Gets a value indicating whether you can go back in the playlist.
        /// </summary>
        public bool CanGoBack => this.CurrentMediaItemPlace > 1;

        /// <summary>
        /// Gets a value indicating whether you can go forward in the playlist.
        /// </summary>
        public bool CanGoForward => this.CurrentMediaItemPlace > 0 && this.CurrentMediaItemPlace < this.MediaPlaylist.Count;

        private int CurrentMediaItemPlace => this.CurrentMedia != null && this.MediaPlaylist.IndexOf(this.CurrentMedia) > -1 ? this.MediaPlaylist.IndexOf(this.CurrentMedia) + 1 : -1;

        /// <summary>
        /// Go Forward In Playlist.
        /// </summary>
        /// <returns>Task.</returns>
        public async Task GoForwardInPlaylistAsync()
        {
            this.appDispatcher.Dispatch(() =>
            {
                if (this.CurrentMedia is null)
                {
                    this.CurrentMedia = this.MediaPlaylist.FirstOrDefault();
                }
                else
                {
                    this.CurrentMedia = this.MediaPlaylist[this.MediaPlaylist.IndexOf(this.CurrentMedia) + 1];
                }
            });
        }

        /// <summary>
        /// Go Back in Playlist.
        /// </summary>
        /// <returns>Task.</returns>
        public async Task GoBackInPlaylistAsync()
        {
            this.appDispatcher.Dispatch(() =>
            {
                if (this.CurrentMedia is null)
                {
                    this.CurrentMedia = this.MediaPlaylist.FirstOrDefault();
                }
                else
                {
                    this.CurrentMedia = this.MediaPlaylist[this.MediaPlaylist.IndexOf(this.CurrentMedia) - 1];
                }
            });
        }

        /// <summary>
        /// Plays a single media item.
        /// Clears the existing playlist.
        /// </summary>
        /// <param name="item">Media Item.</param>
        /// <returns>Task.</returns>
        public async Task PlaySingleMediaItemAsync(IMediaItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            this.appDispatcher.Dispatch(() =>
            {
                this.MediaPlaylist.Clear();
                this.MediaPlaylist.Add(item);
                this.CurrentMedia = item;
            });
        }

        /// <summary>
        /// Adds an item to the playlist.
        /// If it's the first item in the list, autoplay.
        /// </summary>
        /// <param name="item">Media Item.</param>
        /// <returns>Task.</returns>
        public async Task AddItemToPlaylistAsync(IMediaItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            this.appDispatcher.Dispatch(() =>
            {
                this.MediaPlaylist.Add(item);

                // If first item in the list, autoplay it.
                if (this.MediaPlaylist.Count == 1)
                {
                    this.CurrentMedia = item;
                }

                this.OnPropertyChanged(nameof(this.CanGoBack));
                this.OnPropertyChanged(nameof(this.CanGoForward));
            });
        }

#pragma warning disable SA1600 // Elements should be documented
        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action? onChanged = null)
#pragma warning restore SA1600 // Elements should be documented
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
            {
                return false;
            }

            backingStore = value;
            onChanged?.Invoke();
            this.OnPropertyChanged(propertyName);
            return true;
        }

        private async Task PlayMediaAsync(IMediaItem? item)
        {
            if (item == null)
            {
                return;
            }

            switch (item.FromType)
            {
                case FromType.FromPath:
                    var stream = await this.files.GetFileStreamFromPathAsync(item.Path).ConfigureAwait(false);
                    this.CurrentVlcMedia = new Media(this.libVLC, new StreamMediaInput(stream));
                    break;
                case FromType.FromLocation:
                    this.CurrentVlcMedia = new Media(this.libVLC, new Uri(item.Path));
                    break;
                case FromType.AsNode:
                    break;
            }

            if (this.CurrentVlcMedia is not null)
            {
                this.player.Play(this.CurrentVlcMedia);
            }
        }

        private void EndPlayer_Event(object? sender, EventArgs e)
        {
            if (this.CanGoForward)
            {
                this.GoForwardInPlaylistAsync().FireAndForgetSafeAsync(this.error);
            }
            else
            {
                this.player.Position = 0;
            }

            this.Player_Event(sender, e);
        }

        private void Player_Event(object? sender, EventArgs e)
        {
            this.OnPropertyChanged(nameof(this.IsPlaying));
            this.OnPropertyChanged(nameof(this.CurrentMedia));
            this.OnPropertyChanged(nameof(this.CurrentVlcMedia));
            this.OnPropertyChanged(nameof(this.Player));
            this.RaiseCanExecuteChanged?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// On Property Changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            this.appDispatcher.Dispatch(() =>
            {
                var changed = this.PropertyChanged;
                if (changed == null)
                {
                    return;
                }

                changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
            });
        }
    }
}
