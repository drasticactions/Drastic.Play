// <copyright file="FileHandler.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.Play.Interfaces;
using Drastic.Play.Models;
using Drastic.Play.Tools;
using Drastic.Play.Win.Views;

namespace Drastic.Play.Win.Services
{
    /// <summary>
    /// File Handler.
    /// </summary>
    public class FileHandler : IFileHandler
    {
        private MainWindow window;

        public FileHandler(MainWindow window)
        {
            this.window = window;
        }

        /// <inheritdoc/>
        public async Task<Stream> GetFileStreamFromPathAsync(string path)
        {
            var file = await Windows.Storage.StorageFile.GetFileFromPathAsync(path);
            return await file.OpenStreamForReadAsync().ConfigureAwait(false);
        }

        public async Task<string> PickUrl()
        {
            var openUrlContentDialog = new OpenUrlContentDialog();
            var result = await openUrlContentDialog.ShowAsync();
            if (result != Microsoft.UI.Xaml.Controls.ContentDialogResult.Primary)
            {
                return string.Empty;
            }

            return openUrlContentDialog.UrlHolder.Text;
        }

        public async Task<string> PickVideo()
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker
            {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.VideosLibrary,
            };
            foreach (var ext in VLCFileExtensions.VideoExtensions)
            {
                picker.FileTypeFilter.Add(ext);
            }

            IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this.window);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file == null)
            {
                return string.Empty;
            }

            return file.Path;
        }

        public async Task<List<string>> PickVideos()
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker
            {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.VideosLibrary,
            };
            foreach (var ext in VLCFileExtensions.VideoExtensions)
            {
                picker.FileTypeFilter.Add(ext);
            }

            IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this.window);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            var files = await picker.PickMultipleFilesAsync();
            if (files == null)
            {
                return new List<string>();
            }

            return files.Select(x => x.Path).ToList();
        }
    }
}
