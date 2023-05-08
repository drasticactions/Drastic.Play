// <copyright file="MainPage.xaml.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Reflection;
using CommunityToolkit.Mvvm.DependencyInjection;
using Drastic.Play.Interfaces;
using Drastic.Play.Services;
using Drastic.Play.Win.Services;
using Drastic.Play.Win.Tools;
using Drastic.Play.Win.ViewModels;
using Drastic.Services;
using Drastic.Tools;
using LibVLCSharp.Platforms.Windows;
using LibVLCSharp.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace Drastic.Play.Win
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MediaPlayer? player;
        private LibVLC? libVLC;
        private MediaPlayerViewModel? vm;
        private MainWindow window;

        public MainPage(MainWindow window)
        {
            this.InitializeComponent();
            this.window = window;
            this.KibouVideoView.Initialized += this.KibouVideoView_Initialized;
        }

        private void KibouVideoView_Initialized(object? sender, InitializedEventArgs e)
        {
            this.libVLC = new LibVLC(enableDebugLogs: true, e.SwapChainOptions);
            this.player = new MediaPlayer(this.libVLC);

            var dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
            string logLocation = WinUIExtensions.IsRunningAsUwp() ? System.IO.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "drasticplay-error.txt") : Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly()!.Location!)!, "drasticplay-error.txt");
            Ioc.Default.ConfigureServices(
                new ServiceCollection()
                .AddSingleton<IErrorHandlerService>(new WinUIErrorHandlerService(logLocation))
                .AddSingleton<IAppDispatcher>(new AppDispatcher(dispatcherQueue))
                //.AddSingleton<INavigationHandler, NavigationHandler>()
                .AddSingleton<LibVLC>(this.libVLC)
                .AddSingleton<IMouseService>(new MouseService(this.window))
                .AddSingleton<IFileHandler>(new FileHandler(this.window))
                .AddSingleton<MediaPlayer>(this.player)
                .AddSingleton<PlaybackService>()
                .AddSingleton<MediaPlayerViewModel>()
                .BuildServiceProvider());

            this.DataContext = this.vm = Ioc.Default.ResolveWith<MediaPlayerViewModel>(this.window);

         //   this.vm.FilePickerCommand.ExecuteAsync();
        }

        private void Grid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            //WindowsUIHelpers.GoFullScreen(!WindowsUIHelpers.IsFullScreen);
            //WindowsUIHelpers.GoCompact(!WindowsUIHelpers.IsCompactOverlay);
        }
    }
}
