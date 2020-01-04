using log4net;
using MahApps.Metro;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Shapes;
using UI.DataTransfer.Objects;
using UI.Models.Models;
using UI.ViewModels.Helpers;
using UI.ViewModels.RelayCommands;

namespace UI.ViewModels.ViewModels
{
    public class StyleViewModel : ViewModelBase
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region Fields
        private Accent _SelectedAccent;
        private bool _SelectedNight;
        private Language _SelectedLanguage;

        #endregion

        #region Properties
        public Accent SelectedAccent
        {
            get { return _SelectedAccent; }
            set
            {
                _SelectedAccent = value;
                var theme = ThemeManager.DetectAppStyle(Application.Current);
                ThemeManager.ChangeAppStyle(Application.Current, _SelectedAccent, theme.Item1);
                Application.Current.MainWindow.Activate();

                Log.Debug(String.Format("Set Selected Accent {0}", value.Name));
                SaveSetting("Accent", _SelectedAccent.Name);
                OnPropertyChanged("SelectedAccent");
            }
        }
        public bool SelectedNight
        {
            get { return _SelectedNight; }
            set
            {
                _SelectedNight = value;
                var theme = ThemeManager.DetectAppStyle(Application.Current);
                if (_SelectedNight)
                {
                    ThemeManager.ChangeAppStyle(Application.Current, theme.Item2, ThemeManager.GetAppTheme("Night"));
                    Log.Debug(String.Format("Set App Theme to Night"));
                    SaveSetting("AppTheme", "Night");
                }
                else
                {
                    ThemeManager.ChangeAppStyle(Application.Current, theme.Item2, ThemeManager.GetAppTheme("Day"));
                    Log.Debug(String.Format("Set App Theme to Day"));
                    SaveSetting("AppTheme", "Day");
                }
                OnPropertyChanged("SelectedNight");
            }
        }
        public List<Language> Languages { get; set; }
        public Language SelectedLanguage
        {
            get { return _SelectedLanguage; }
            set
            {
                if (value != _SelectedLanguage)
                {
                    _SelectedLanguage = value;
                    DTO.AppLocalize.ChangeLanguage(value.Culture);
                    DTO.LocalizeMenuItemsInterface.LocalizeMenuItems();
                    Log.Debug(String.Format("Set Language to {0} - {1}", value.Name, value.Culture));
                    SaveSetting("Language", value.Culture);
                    OnPropertyChanged("SelectedLanguage");
                }
            }
        }
        public RelayCommand Loaded { get; set; }
        public RelayCommand Unloaded { get; set; }
        public RelayCommand SetAccent { get; set; }

        #endregion

        #region Constructor
        public StyleViewModel()
        {
            Log.Debug(String.Format("Constructor"));
            Loaded = new RelayCommand(OnLoaded);
            Unloaded = new RelayCommand(OnUnloaded);
            SetAccent = new RelayCommand(OnSetAccent);


            Languages = new List<Language>
            {
                new Language { Name = "ENGLISH", Culture = "en" },
                new Language { Name = "РУССКИЙ", Culture = "ru" },
                new Language { Name = "PORTUGUÊS", Culture = "pt" }
            };

            GetSetting();
        }

        #endregion

        #region Commands
        private void OnSetAccent(object parameter)
        {
            Log.Debug(String.Format("Set Accent Event"));
            Ellipse ellipse = parameter as Ellipse;
            Accent accent = ThemeManager.GetAccent(ellipse.Tag.ToString());

            var theme = ThemeManager.DetectAppStyle(Application.Current);
            ThemeManager.ChangeAppStyle(Application.Current, accent, theme.Item1);
            Application.Current.MainWindow.Activate();

            Log.Debug(String.Format("Set Selected Accent {0}", accent.Name));
            SaveSetting("Accent", accent.Name);
        }
        private void OnUnloaded()
        {
            Log.Debug(String.Format("Unloaded"));
        }
        private void OnLoaded()
        {
            Log.Debug(String.Format("Loaded"));
        }

        #endregion

        #region Methods
        private void GetSetting()
        {
            Log.Debug(String.Format("Get Setting"));
            RegistryKey subKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\MP\UI DEV\PrevSesParameters");
            _SelectedAccent = ThemeManager.GetAccent(subKey.GetValue("Accent", "Lime").ToString());
            OnPropertyChanged("SelectedAccent");
            _SelectedNight = subKey.GetValue("AppTheme", "Night").ToString() == "Night";
            OnPropertyChanged("SelectedNight");
            _SelectedLanguage = Languages.FirstOrDefault(l => l.Culture == subKey.GetValue("Language", "en").ToString());
            OnPropertyChanged("SelectedLanguage");
            subKey.Close();
        }
        private void SaveSetting(string key, object value)
        {
            Log.Debug(String.Format("Save Session Parameter: {0} -> {1}", key, value));
            RegistryKey subKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\MP\UI DEV\PrevSesParameters", true);

            subKey.SetValue(key, value);
            subKey.Close();

        }
        #endregion
    }
}
