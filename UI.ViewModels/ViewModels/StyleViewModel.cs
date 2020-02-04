using log4net;
using MahApps.Metro;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
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
        private Ellipse _SelectedAccent;
        private Language _SelectedLanguage;
        private string _SelectedTheme;
        #endregion

        #region Properties

        public List<Ellipse> Accents { get; set; }
        public Ellipse SelectedAccent
        {
            get { return _SelectedAccent; }
            set
            {
                if (value != null)
                {
                    Accent accent = ThemeManager.GetAccent(value.Tag.ToString());
                    var theme = ThemeManager.DetectAppStyle(Application.Current);
                    ThemeManager.ChangeAppStyle(Application.Current, accent, theme.Item1);
                    Application.Current.MainWindow.Activate();

                    Log.Debug(String.Format("Set Selected Accent {0}", accent.Name));
                    
                    if (theme.Item1.Name != "DAY")
                    {
                        value.StrokeThickness = 1;
                        value.Stroke = Brushes.FloralWhite;
                    }
                    else
                    {
                        value.StrokeThickness = 2;
                        value.Stroke = Brushes.DimGray;
                    }


                    if (_SelectedAccent != null)
                    {
                        _SelectedAccent.StrokeThickness = 0;
                        SaveSetting("Accent", accent.Name);
                    }
                        
                }
                _SelectedAccent = value;
                OnPropertyChanged("SelectedAccent");
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
                    if (_SelectedLanguage != null)
                    {
                        DTO.AppLocalize.ChangeLanguage(value.Culture);
                        DTO.LocalizeMenuItemsInterface.LocalizeMenuItems();
                        Log.Debug(String.Format("Set Language to {0} - {1}", value.Name, value.Culture));
                        SaveSetting("Language", value.Culture);
                    }

                    _SelectedLanguage = value;
                    OnPropertyChanged("SelectedLanguage");
                }
            }
        }

        public List<string> Themes { get; set; }
        public string SelectedTheme
        {
            get { return _SelectedTheme; }
            set
            {
                if (value != _SelectedTheme)
                {
                    if (_SelectedTheme != null)
                    {
                        var theme = ThemeManager.DetectAppStyle(Application.Current);
                        ThemeManager.ChangeAppStyle(Application.Current, theme.Item2, ThemeManager.GetAppTheme(value));
                        Log.Debug(String.Format("Set App Theme to {0}", value));
                        SaveSetting("AppTheme", value);

                        if (value != "DAY")
                        {
                            SelectedAccent.StrokeThickness = 1;
                            SelectedAccent.Stroke = Brushes.FloralWhite;
                        }
                        else
                        {
                            SelectedAccent.StrokeThickness = 2;
                            SelectedAccent.Stroke = Brushes.DimGray;
                        }
                    }

                    _SelectedTheme = value;
                    OnPropertyChanged("SelectedTheme");
                }
            }
        }

        public RelayCommand Loaded { get; set; }
        public RelayCommand Unloaded { get; set; }

        #endregion

        #region Constructor
        public StyleViewModel()
        {
            Log.Debug(String.Format("Constructor"));
            Loaded = new RelayCommand(OnLoaded);
            Unloaded = new RelayCommand(OnUnloaded);

            
            GetLanguages();
            GetStyle();
            GetSetting();
        }

        #endregion

        #region Commands

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

            SelectedAccent = Accents.FirstOrDefault(e => e.Tag.ToString() == subKey.GetValue("Accent", "Lime").ToString());
            SelectedTheme = Themes.FirstOrDefault(e => e == subKey.GetValue("AppTheme", "NIGHT").ToString());
            SelectedLanguage = Languages.FirstOrDefault(l => l.Culture == subKey.GetValue("Language", "en").ToString());

            subKey.Close();
        }
        private void GetStyle()
        {
            Log.Debug(String.Format("Get Accents"));
            Accents = new List<Ellipse>();
            foreach (var item in ThemeManager.Accents)
            {
                Accents.Add(new Ellipse
                {
                    Tag = item.Name,
                    Fill = item.Resources["AccentColorBrush"] as SolidColorBrush,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Width = 32,
                    Height = 32,
                    Margin = new Thickness(0, 0, 2, 2)
                });
            }

            Log.Debug(String.Format("Get Themes"));
            Themes = new List<string>();
            foreach (var item in ThemeManager.AppThemes)
            {
                if (item.Name.IndexOf("Base") == -1)
                    Themes.Add(item.Name);
            }
        }
        private void GetLanguages()
        {
            Log.Debug(String.Format("Get Languages"));
            Languages = new List<Language>
            {
                new Language { Name = "ENGLISH", Culture = "en" },
                new Language { Name = "РУССКИЙ", Culture = "ru" },
                new Language { Name = "PORTUGUÊS", Culture = "pt" }
            };
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
