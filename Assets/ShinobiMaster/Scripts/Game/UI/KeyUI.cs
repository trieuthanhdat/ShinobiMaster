using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class KeyUI: MonoBehaviour
	{
		[SerializeField] private Sprite noKeySprite;
		[SerializeField] private Sprite keySprite;
		[SerializeField] private Image image;




		public void SetKey()
		{
			this.image.sprite = this.keySprite;
			var color = this.image.color;
			color.a = 1f;
			this.image.color = color;
		}

		public void SetNoKey()
		{
			this.image.sprite = this.noKeySprite;
			var color = this.image.color;
			color.a = 0.3f;
			this.image.color = color;
		}
	}
}