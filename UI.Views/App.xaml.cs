using EZLocalizeNS;
using log4net;
using MahApps.Metro;
using Microsoft.Win32;
using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using UI.DataTransfer.Objects;

namespace UI.Views
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        protected override void OnStartup(StartupEventArgs e)
        {
            Log.Info("        =============  Started New Session Logging  =============        ");

            
            ThemeManager.AddAppTheme("TINTED", new Uri("pack://application:,,,/Themes/Tinted.xaml"));
            ThemeManager.AddAppTheme("NIGHT", new Uri("pack://application:,,,/Themes/Night.xaml"));
            ThemeManager.AddAppTheme("DAY", new Uri("pack://application:,,,/Themes/Day.xaml"));


            EventManager.RegisterClassHandler(
                typeof(TextBox), TextBox.PreviewMouseDownEvent, new RoutedEventHandler(TextBox_PreviewMouseDown));
            EventManager.RegisterClassHandler(
                typeof(TextBox), TextBox.GotFocusEvent, new RoutedEventHandler(TextBox_GotFocus));

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            DTO.AppLocalize = new EZLocalize(App.Current.Resources, "en", null, "Languages\\", "Interface");


            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
                new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            LoadPreviousSessionParameters();

            // For catching Global uncaught exception
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionOccured);

            Log.Info("Application Startup");
            LogMachineDetails();
        }
        static void UnhandledExceptionOccured(object sender, UnhandledExceptionEventArgs args)
        {
            //// Here change path to the log.txt file
            //var path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
            //                                        + "\\Documents\\Spectrum Analyzer\\Sessions Logs\\SA.log";

            // // Show a message before closing application
            // var dialogService = new MvvmDialogs.DialogService();
            //dialogService.ShowMessageBox((INotifyPropertyChanged)(app.DataContext),
            //    "Oops, something went wrong and the application must close. Please find a " +
            //    "report on the issue at: " + path + Environment.NewLine +
            //    "If the problem persist, please contact your local system supplier.",
            //    "Unhandled Error",
            //    MessageBoxButton.OK);

            Exception e = (Exception)args.ExceptionObject;
            Log.Fatal("Application has crashed", e);
        }

        private void LogMachineDetails()
        {
            var computer = new Microsoft.VisualBasic.Devices.ComputerInfo();

            string text = "OS: " + computer.OSPlatform + " v" + computer.OSVersion + " " + computer.OSFullName +
                          "; RAM: " + computer.TotalPhysicalMemory.ToString() +
                          "; Language: " + computer.InstalledUICulture.EnglishName;
            Log.Info(text);
        }


        private void TextBox_PreviewMouseDown(object sender, RoutedEventArgs e)
        {
            //Log.Debug("Register Class Handler TextBox PreviewMouseDown");
            TextBox textBox = sender as TextBox;

            if (!textBox.IsFocused)
            {
                textBox.Focus();
                textBox.SelectAll();
                e.Handled = true;
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private void LoadPreviousSessionParameters()
        {
            Log.Debug("Load Previous Session Parameters");
            RegistryKey subKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\MP\UI DEV\PrevSesParameters");
            if (subKey == null)
                subKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\MP\UI DEV\PrevSesParameters");

            DTO.AppLocalize.ChangeLanguage(subKey.GetValue("Language", "ru").ToString());
            ThemeManager.ChangeAppStyle(Application.Current,
                ThemeManager.GetAccent(subKey.GetValue("Accent", "Lime").ToString()),
                ThemeManager.GetAppTheme(subKey.GetValue("AppTheme", "NIGHT").ToString()));
            subKey.Close();

        }
    }
}
