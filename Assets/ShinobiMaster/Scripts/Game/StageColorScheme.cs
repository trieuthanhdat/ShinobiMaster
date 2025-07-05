using Game.LevelControll;
using UnityEngine;

namespace Game
{
	[CreateAssetMenu(fileName = "StageColorScheme")]
	public class StageColorScheme: ScriptableObject
	{
		public EnvVisualType EnvVisualType;
		public Material WallstripesMat;
		public Material WallsMat;
		public Material FloorMat;
		public Material ElevatorDoorsMat;
		public Material ElevatorFramesMat;
		public Material BalksMat;
		public Material BackgroundMat;
		public Material SkyboxMat;
		public Material WindowMat;
		

		public void Apply(Stage stage)
		{
			foreach (var wallstripe in stage.Wallstripes)
			{
				if (wallstripe == null)
				{
					continue;
				}
			
				wallstripe.material = WallstripesMat;
			}
		
			foreach (var wall in stage.Walls)
			{
				if (wall == null)
				{
					continue;
				}
			
				wall.material = WallsMat;
			}
			
			foreach (var floor in stage.Floor)
			{
				if (floor == null)
				{
					continue;
				}
				
				floor.material = FloorMat;
			}
			
			foreach (var elevatorDoor in stage.ElevatorDoors)
			{
				if (elevatorDoor == null)
				{
					continue;
				}
			
				elevatorDoor.material = ElevatorDoorsMat;
			}
			
			foreach (var elevatorFrame in stage.ElevatorFrames)
			{
				if (elevatorFrame == null)
				{
					continue;
				}
			
				elevatorFrame.material = ElevatorFramesMat;
			}
			
			foreach (var balk in stage.Balks)
			{
				if (balk == null)
				{
					continue;
				}
			
				balk.material = BalksMat;
			}

			if (WindowMat != null)
			{
				foreach (var window in stage.Windows)
				{
					if (window == null)
					{
						continue;
					}
				
					var mats = window.materials;
					mats[1] = WindowMat;
					window.materials = mats;
				}
			}

			var backgroundTrs = stage.Background.GetComponentsInChildren<Transform>();
			
			foreach (Transform tr in backgroundTrs)
			{
				if (tr.name.Equals("SmokeWhiteSoft"))
				{
					continue;
				}
			
				var rend = tr.GetComponent<Renderer>();

				if (rend != null)
				{
					rend.material = BackgroundMat;
				}
			}

			RenderSettings.skybox = SkyboxMat;
		}
	}
}