using EZLocalizeNS;
using UI.DataTransfer.Interfaces;

namespace UI.DataTransfer.Objects
{
    public class DTO
    {
        public static EZLocalize AppLocalize { get; set; }
        public static ILocalizeMenuItems LocalizeMenuItemsInterface { get; set; }
    }
}
