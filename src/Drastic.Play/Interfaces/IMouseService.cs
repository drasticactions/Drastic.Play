// <copyright file="IMouseService.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drastic.Play.Interfaces
{
    /// <summary>
    /// Mouse Service.
    /// </summary>
    public interface IMouseService
    {
        /// <summary>
        /// Fired when mouse cursor is hidden.
        /// </summary>
        public event EventHandler OnHidden;

        /// <summary>
        /// Fired when mouse cursor moves.
        /// </summary>
        public event EventHandler OnMoved;

        /// <summary>
        /// Starts Mouse Service.
        /// </summary>
        public void StartService();

        /// <summary>
        /// Ends Mouse Service.
        /// </summary>
        public void StopService();

        /// <summary>
        /// Hides Mouse Cursor.
        /// </summary>
        public void HideCursor();

        /// <summary>
        /// Show Mouse Cursor.
        /// </summary>
        public void ShowCursor();
    }
}
