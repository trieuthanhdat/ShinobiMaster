using System;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.Analytics;

namespace SendAppMetrica
{
    public class AnalyticsManager : MonoBehaviour
    {
        public static AnalyticsManager Instance;

        private readonly Dictionary<string, object> eventParams = new Dictionary<string, object>();

        private static string LevelStartEventName = "level_start";
        private static string LevelFinishEventName = "level_finish";

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                this.transform.SetParent(null);
                DontDestroyOnLoad(this);
            }
            else
            {
                if (Instance != this)
                {
                    Destroy(this.gameObject);
                }

                return;
            }
        }

        private void OnDestroy()
        {
        }

        public void Event_VideoAdsAvailable(string ad_type, string placement, string result, bool connection)
        {
            var eventName = "video_ads_available";

            this.eventParams.Add("ad_type", ad_type);
            this.eventParams.Add("placement", placement);
            this.eventParams.Add("result", result);
            this.eventParams.Add("connection", connection.ToString());

            Analytics.CustomEvent(eventName, this.eventParams);
            this.eventParams.Clear();
        }

        public void Event_VideoAdsStarted(string ad_type, string placement, string result, bool connection)
        {
            var eventName = "video_ads_started";

            this.eventParams.Add("ad_type", ad_type);
            this.eventParams.Add("placement", placement);
            this.eventParams.Add("result", result);
            this.eventParams.Add("connection", connection.ToString());

            Analytics.CustomEvent(eventName, this.eventParams);
            this.eventParams.Clear();
        }

        public void Event_VideoAdsWatch(string ad_type, string placement, string result, bool? connection)
        {
            var eventName = "video_ads_watch";

            if (ad_type != null) this.eventParams.Add("ad_type", ad_type);
            if (placement != null) this.eventParams.Add("placement", placement);
            if (result != null) this.eventParams.Add("result", result);
            if (connection != null) this.eventParams.Add("connection", Convert.ToInt16(connection).ToString());

            Analytics.CustomEvent(eventName, this.eventParams);
            this.eventParams.Clear();
        }

        public void Event_LevelStart(int numLevel, int numStage, int location, int idSkin, int idWeapon, int config)
        {
            var param = new Dictionary<string, object>
               {
                   {"level", numLevel + "." + numStage},
                   {"location", location},
                   {"idSkin", idSkin},
                   {"idWeapon", idWeapon},
                   {"config", config}
               };

            Analytics.CustomEvent(LevelStartEventName, param);
        }

        public void Event_LevelFinish(string result, float time, int numLevel, int numStage, int location, int config,
            int aliveEnemies, int health, int loseCountHp, int pickHp)
        {
            var param = new Dictionary<string, object>
               {
                   {"level", numLevel + "." + numStage},
                   {"location", location},
                   {"time", Mathf.RoundToInt(time)},
                   {"result", result},
                   {"config", config},
                   {"NonKillEnemy", aliveEnemies},
                   {"Balance_hp", health},
                   {"Lose_count_hp", loseCountHp},
                   {"Pick_hp", pickHp}
               };

            Analytics.CustomEvent(LevelFinishEventName, param);
        }

        public void Event_BPSkin(int level, int stage, int idSkin, int idWeapon)
        {
            var param = new Dictionary<string, object>
               {
                   {"level", level + "." + stage},
                   {"idSkin", idSkin},
                   {"idWeapon", idWeapon}
               };

            Analytics.CustomEvent("BP_Skin", param);
        }
    }
}