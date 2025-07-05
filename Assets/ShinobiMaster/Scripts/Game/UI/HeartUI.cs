using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class HeartUI: MonoBehaviour
	{
		private static readonly int Show = Animator.StringToHash("Show");
		private static readonly int Hide = Animator.StringToHash("Hide");
	
		[SerializeField]
		private Image noHeartImage;
		[SerializeField]
		private Image heartImage;
		[SerializeField] 
		private Animator animator;
		public bool IsVisible { get; private set; }




		private void Awake()
		{
			IsVisible = true;
		}

		private void OnEnable()
		{
			if (this.heartImage != null)
			{
				this.heartImage.enabled = IsVisible;
			}
		}


		public void SetHeartVisibility(bool visible)
		{
			if (IsVisible && !visible)
			{
				PlayHideAnim();
			}
			else if (!IsVisible && visible)
			{
				if (this.heartImage != null)
				{
					this.heartImage.enabled = true;
				}

				PlayShowAnim();
			}
		
			IsVisible = visible;
		}


		public void PlayShowAnim()
		{
			if (this.animator != null)
			{
				this.animator.SetBool(Show, true);
			}
		}

		public void PlayHideAnim()
		{
			if (this.animator != null)
			{
				this.animator.SetBool(Show, false);
			}
		}
	}
}