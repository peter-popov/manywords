﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;

namespace ManyWords
{
    public class StatusToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool b = (bool)value;

            return b ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }
    }

    public partial class App : Application
    {
        private static WordStorage.Storage wordStorgae = null;
        private static Model.LanguageListModel languagesListModel = null;
        private static Model.VocabularyViewModel vocabularyListModel = null;
        private static Model.TextToSpeech tts = null;

        /// <summary>
        /// A static word storage used to access words database.
        /// </summary>
        /// <returns>The MainViewModel object.</returns>
        public static WordStorage.Storage WordStorage
        {
            get
            {
                // Delay creation of the view model until necessary
                if (wordStorgae == null)
                    wordStorgae = new WordStorage.Storage();

                return wordStorgae;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static Model.LanguageListModel LanguagesListModel
        {
            get
            {
                // Delay creation of the view model until necessary
                if (languagesListModel == null)
                    languagesListModel = new Model.LanguageListModel();

                return languagesListModel;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static Model.VocabularyViewModel VocabularyListModel
        {
            get
            {
                // Delay creation of the view model until necessary
                if (vocabularyListModel == null)
                    vocabularyListModel = new Model.VocabularyViewModel();

                return vocabularyListModel;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static Model.TextToSpeech TextToSpeech
        {
            get
            {
                // Delay creation of the view model until necessary
                if (tts == null)
                    tts = new Model.TextToSpeech(Translator.TranslatorFactory.CreateInstance());

                return tts;
            }
        }


        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            this.SetCustomColorTheme();

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }
        }


        private void SetCustomColorTheme()
        {
            var accentColor = Color.FromArgb(255, 224, 65, 70);
            var mainForeground = Colors.White;
            var mainBackground = Color.FromArgb(255, 25, 117, 150);

           /* (App.Current.Resources["PhoneAccentBrush"] as SolidColorBrush).Color = accentColor;
            (App.Current.Resources["PhoneForegroundBrush"] as SolidColorBrush).Color = mainForeground;
            (App.Current.Resources["PhoneBackgroundBrush"] as SolidColorBrush).Color = mainBackground;

            (App.Current.Resources["PhoneContrastBackgroundBrush"] as SolidColorBrush).Color = Colors.White;
            (App.Current.Resources["PhoneContrastForegroundBrush"] as SolidColorBrush).Color = Colors.Black;

            (App.Current.Resources["PhoneSubtleBrush"] as SolidColorBrush).Color = Colors.LightGray;

            (App.Current.Resources["PhoneTextBoxBrush"] as SolidColorBrush).Color = Colors.White;
            (App.Current.Resources["PhoneTextCaretBrush"] as SolidColorBrush).Color = Colors.Black;
            (App.Current.Resources["PhoneTextBoxForegroundBrush"] as SolidColorBrush).Color = Colors.Black;
            (App.Current.Resources["PhoneTextBoxEditBackgroundBrush"] as SolidColorBrush).Color = Colors.White;
            (App.Current.Resources["PhoneTextBoxEditBorderBrush"] as SolidColorBrush).Color = Colors.White;
            (App.Current.Resources["PhoneTextBoxReadOnlyBrush"] as SolidColorBrush).Color = Colors.Gray;*/
        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {

        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            // Ensure that required application state is persisted here.
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}