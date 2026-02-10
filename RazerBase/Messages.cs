using System;
using System.Windows;

namespace RazerBase
{
    /// <summary>
    /// Methods to display message boxes.
    /// </summary>
    public static class Messages
    {
        public static string ApplicationName = "Razer";

        /// <summary>
        /// Displays an error dialog with a given message.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        public static void ShowError(string message)
        {
            ShowMessage(message, MessageBoxImage.Error);
        }

        /// <summary>
        /// Displays information dialog with a given message.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        public static void ShowInformation(string message)
        {
            ShowMessage(message, MessageBoxImage.Information);
        }

        /// <summary>
        /// Displays warning dialog with a given message.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        public static void ShowWarning(string message)
        {
            ShowMessage(message, MessageBoxImage.Warning);
        }

        /// <summary>
        /// Displays message dialog with a given message and icon.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        /// <param name="icon">The icon to be displayed with the message.</param>
        public static void ShowMessage(string message, MessageBoxImage icon)
        {
            string appName = ApplicationName;
            MessageBox.Show(message, appName, MessageBoxButton.OK, icon);
        }

        /// <summary>
        /// Displays an OK / Cancel dialog and returns the user input.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        /// <param name="icon">The icon to be displayed.</param>
        /// <returns>User selection.</returns>
        public static MessageBoxResult ShowOkCancel(string message, MessageBoxImage icon)
        {
            string appName = ApplicationName;
            return MessageBox.Show(message, appName, MessageBoxButton.OKCancel, icon);
        }

        /// <summary>
        /// Displays a Yes/No dialog and returns the user input.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        /// <param name="icon">The icon to be displayed.</param>
        /// <returns>User selection.</returns>
        public static MessageBoxResult ShowYesNo(string message, MessageBoxImage icon)
        {
            string appName = ApplicationName;
            return MessageBox.Show(message, appName, MessageBoxButton.YesNo, icon);
        }

        /// <summary>
        /// Displays an Yes / No / Cancel dialog and returns the user input.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        /// <param name="icon">The icon to be displayed.</param>
        /// <returns>User selection.</returns>
        public static MessageBoxResult ShowYesNoCancel(string message, MessageBoxImage icon)
        {
            string appName = ApplicationName;
            return MessageBox.Show(message, appName, MessageBoxButton.YesNoCancel, icon);
        }

    }
}

//MUST HAVE using System.Windows in client code
//Client code example:

            //using System.Windows;

            //MessageBoxResult result = Messages.ShowYesNo("Add a new record?", System.Windows.MessageBoxImage.Question);
            //if (result == MessageBoxResult.Yes)
            //{
            //    //add new record
            //    Messages.ShowMessage("Added New Record", MessageBoxImage.Information);
            //}
            //else
            //{
            //    //do nothing
            //    Messages.ShowMessage("I did nothing", MessageBoxImage.Information);
            //}
