using System.IO;
using Skins;
using UnityEditor;
using UnityEngine;

namespace Scripts.Editor
{
	public class MenuItems
	{
		[MenuItem("Tools/Clear PlayerPrefs")]
		private static void ClearPlayerPrefs()
		{
			PlayerPrefs.DeleteAll();

			var skinsPaths = Directory.GetFiles($"{Application.dataPath}/Skins/", "*.asset",
				SearchOption.AllDirectories);

			for (var i = 0; i < skinsPaths.Length; i++)
			{
				skinsPaths[i] = "Assets" + skinsPaths[i].Replace('\\', '/' )
					.Replace(Application.dataPath, "");
			}

			foreach (var skinPath in skinsPaths)
			{
				var skin = (Skin)AssetDatabase.LoadAssetAtPath(skinPath, typeof(Skin));

				if (skin.Id == 1)
				{
					continue;
				}

				skin.Available = false;
				skin.Skipped = false;
				skin.CurrentViewCount = 0;
			}

			var gameSaveFilePath = Application.dataPath + "/ghostWalker.gw";
			
			File.Delete(gameSaveFilePath);
			
			Debug.Log("PlayerPrefs cleared.");
		}
	}
}