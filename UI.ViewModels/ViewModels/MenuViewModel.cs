using log4net;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using UI.DataTransfer.Interfaces;
using UI.DataTransfer.Objects;
using UI.ViewModels.Helpers;

namespace UI.ViewModels.ViewModels
{
    public class MenuViewModel : ViewModelBase, ILocalizeMenuItems
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private HamburgerMenuItemCollection _menuItems;
        private HamburgerMenuItemCollection _menuOptionItems;

        public HamburgerMenuItemCollection MenuItems
        {
            get { return _menuItems; }
            set
            {
                if (Equals(value, _menuItems))
                    return;
                _menuItems = value;
                Log.Debug(String.Format("Set MenuItems"));
                OnPropertyChanged("MenuItems");
            }
        }
        public HamburgerMenuItemCollection MenuOptionItems
        {
            get { return _menuOptionItems; }
            set
            {
                if (Equals(value, _menuOptionItems))
                    return;
                _menuOptionItems = value;
                Log.Debug(String.Format("Set MenuOptionItems"));
                OnPropertyChanged("MenuOptionItems");
            }
        }



        public MenuViewModel()
        {
            DTO.LocalizeMenuItemsInterface = this as ILocalizeMenuItems;
            this.CreateMenuItems();
        }

        public void CreateMenuItems()
        {
            // In case of multi-language use label as localizer key
            Log.Debug(String.Format("Creating menu items"));
            MenuItems = new HamburgerMenuItemCollection
            {
                new HamburgerMenuIconItem()
                {
                    Icon = new PackIconMaterial() {Kind = PackIconMaterialKind.Home},
                    Label = "Home",
                    ToolTip = (string)Application.Current.TryFindResource("Home"),
                    Tag = new HomeViewModel()
                },

            };

            MenuOptionItems = new HamburgerMenuItemCollection
            {
                new HamburgerMenuIconItem()
                {
                    Icon = new PackIconMaterial() {Kind = PackIconMaterialKind.InformationVariant},
                    Label = "About",
                    ToolTip = (string)Application.Current.TryFindResource("About"),
                    Tag = new AboutViewModel()
                }
            };
        }

        public void LocalizeMenuItems()
        {
            Log.Debug(String.Format("Localize menu items"));
            foreach (var item in MenuItems)
            {
                item.ToolTip = (string)Application.Current.TryFindResource(item.Label);
            }
            foreach (var item in MenuOptionItems)
            {
                item.ToolTip = (string)Application.Current.TryFindResource(item.Label);
            }
        }
    }
}