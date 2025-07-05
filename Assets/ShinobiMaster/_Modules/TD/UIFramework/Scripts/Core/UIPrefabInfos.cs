using System.Collections;
using System.Collections.Generic;

namespace TD.UIFramework.Core
{
    public static class UIPrefabInfos
    {
        public enum Scenes
        {
            //To be added
            NotificationPopup,
            SamplePopup,
            EquipmentPopup,
            CharacterStatbarScreen,
            YouDiePopup,
            MissionChoosingScreen,
            SettingScreen
        }
        public static Dictionary<Scenes, string> ScenesMapper = new Dictionary<Scenes, string>()
        {
            { Scenes.NotificationPopup, "NotificationPopup" },
            { Scenes.SamplePopup, "SamplePopup" },
            { Scenes.EquipmentPopup, "EquipmentPopup" },
            { Scenes.CharacterStatbarScreen, "CharacterStatbarScreen"},
            { Scenes.YouDiePopup, "YouDiePopup"},
            { Scenes.MissionChoosingScreen, "MissionChoosingScreen"},
            { Scenes.SettingScreen, "SettingScreen"}
        };
        static string pathPopup = "UIFramework/Popups/";
        public static string PathPopupPrefabs
        {
            get
            {
                return pathPopup;
            }
        }
        static string pathScreen = "UIFramework/Screens/";
        public static string PatScreenPrefabs
        {
            get
            {
                return pathScreen;
            }
        }
        static string pathOverlay = "UIFramework/Overlays/";
        public static string PathOverlayPrefabs
        {
            get
            {
                return pathOverlay;
            }
        }
    }

}
