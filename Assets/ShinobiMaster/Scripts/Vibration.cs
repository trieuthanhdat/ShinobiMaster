using UnityEngine;

namespace Development
{
	public static class Vibration
	{
		private const string VibroActiveKey = "vibro_active";
	
		public static bool VibroActive
		{
			get => bool.Parse(PlayerPrefs.GetString(VibroActiveKey, "True"));
			private set
			{
				PlayerPrefs.SetString(VibroActiveKey, value.ToString());
				PlayerPrefs.Save();
			}
		}


#if UNITY_ANDROID && !UNITY_EDITOR
    public static AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    public static AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    public static AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
#else
		public static AndroidJavaClass unityPlayer;
		public static AndroidJavaObject currentActivity;
		public static AndroidJavaObject vibrator;
#endif

		public static void SetActiveVibro(bool active)
		{
			VibroActive = active;
		}

		public static void Vibrate()
		{
			if (!VibroActive)
			{
				return;
			}
		
			if (isAndroid())
				vibrator.Call("vibrate");
			else
				Handheld.Vibrate();
		}


		public static void Vibrate(long milliseconds)
		{
			if (!VibroActive)
			{
				return;
			}
			
			#if !UNITY_EDITOR
			if (isAndroid())
				vibrator.Call("vibrate", milliseconds);
			else
				Handheld.Vibrate();
			#endif
		}

		public static void Vibrate(long[] pattern, int repeat)
		{
			if (!VibroActive)
			{
				return;
			}
		
			if (isAndroid())
				vibrator.Call("vibrate", pattern, repeat);
			else
				Handheld.Vibrate();
		}

		public static bool HasVibrator()
		{
			return isAndroid();
		}

		public static void Cancel()
		{
			if (isAndroid())
				vibrator.Call("cancel");
		}

		private static bool isAndroid()
		{
#if UNITY_ANDROID && !UNITY_EDITOR
	return true;
#else
			return false;
#endif
		}
	}
}