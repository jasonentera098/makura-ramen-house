using System;
using System.Linq;
using System.Windows;

namespace IT_Helpdesk
{
    /// <summary>
    /// Static helper that prevents multiple instances of the same window type from opening.
    /// 
    /// Usage:
    ///   - Non-modal: WindowManager.ShowSingle&lt;MyWindow&gt;(() => new MyWindow());
    ///   - Modal dialog: WindowManager.ShowSingleDialog&lt;MyWindow&gt;(() => new MyWindow(arg), ownerWindow);
    /// 
    /// If a window of the requested type is already open, it is brought to the foreground
    /// instead of creating a new instance.
    /// </summary>
    public static class WindowManager
    {
        /// <summary>
        /// Opens a non-modal window, ensuring only one instance of <typeparamref name="TWindow"/>
        /// is open at a time. If an instance already exists it is activated (brought to focus).
        /// </summary>
        /// <typeparam name="TWindow">The window type to manage.</typeparam>
        /// <param name="factory">Factory delegate used to create the window when needed.</param>
        /// <param name="owner">Optional owner window.</param>
        public static void ShowSingle<TWindow>(Func<TWindow> factory, Window? owner = null)
            where TWindow : Window
        {
            // Check if an instance is already open
            var existing = Application.Current.Windows
                .OfType<TWindow>()
                .FirstOrDefault();

            if (existing != null)
            {
                // Bring existing window to the foreground
                if (existing.WindowState == WindowState.Minimized)
                    existing.WindowState = WindowState.Normal;

                existing.Activate();
                return;
            }

            // Create and show a new instance
            var window = factory();
            if (owner != null)
                window.Owner = owner;

            window.Show();
        }

        /// <summary>
        /// Opens a modal dialog window, ensuring only one instance of <typeparamref name="TWindow"/>
        /// is open at a time. If an instance already exists it is activated instead of opening a new one.
        /// Returns the <see cref="bool?"/> DialogResult when the dialog closes, or <c>null</c> if an
        /// existing instance was activated instead.
        /// </summary>
        /// <typeparam name="TWindow">The window type to manage.</typeparam>
        /// <param name="factory">Factory delegate used to create the window when needed.</param>
        /// <param name="owner">Optional owner window.</param>
        /// <returns>
        /// The <see cref="Window.DialogResult"/> of the newly opened dialog, or <c>null</c> if an
        /// existing instance was brought to focus instead.
        /// </returns>
        public static bool? ShowSingleDialog<TWindow>(Func<TWindow> factory, Window? owner = null)
            where TWindow : Window
        {
            // Check if an instance is already open
            var existing = Application.Current.Windows
                .OfType<TWindow>()
                .FirstOrDefault();

            if (existing != null)
            {
                // Bring existing window to the foreground
                if (existing.WindowState == WindowState.Minimized)
                    existing.WindowState = WindowState.Normal;

                existing.Activate();
                return null; // Indicate that no new dialog was opened
            }

            // Create and show a new modal dialog
            var window = factory();
            if (owner != null)
                window.Owner = owner;

            bool? result = window.ShowDialog();

            // Return focus to the parent window after the dialog closes (Requirement 18.3)
            owner?.Activate();

            return result;
        }

        /// <summary>
        /// Checks whether a window of the given type is currently open.
        /// </summary>
        /// <typeparam name="TWindow">The window type to check.</typeparam>
        /// <returns><c>true</c> if at least one instance is open; otherwise <c>false</c>.</returns>
        public static bool IsOpen<TWindow>() where TWindow : Window
        {
            return Application.Current.Windows.OfType<TWindow>().Any();
        }
    }
}
