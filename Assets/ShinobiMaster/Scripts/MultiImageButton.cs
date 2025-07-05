using System;
using UnityEngine;
using UnityEngine.UI;

public class MultiImageButton : Button
{
	public Graphic[] Graphics;


	protected override void DoStateTransition(SelectionState state, bool instant)
	{
		Color color;
		switch (state)
		{
			case (Selectable.SelectionState.Normal):
				color = this.colors.normalColor;
				break;

			case (Selectable.SelectionState.Highlighted):
				color = this.colors.highlightedColor;
				break;

			case (Selectable.SelectionState.Pressed):
				color = this.colors.pressedColor;
				break;

			case (Selectable.SelectionState.Disabled):
				color = this.colors.disabledColor;
				break;

			default:
				color = this.colors.normalColor;
				
				break;
		}

		if (base.gameObject.activeInHierarchy)
		{
			switch (this.transition)
			{
				case (Selectable.Transition.ColorTint):
					this.ColorTween(color * this.colors.colorMultiplier, instant);
					break;

				default:
					throw new System.NotSupportedException();
			}
		}
	}


	private void ColorTween(Color targetColor, bool instant)
	{
		if (this.targetGraphic == null)
		{
			return;
		}

		foreach (var g in this.Graphics)
		{
			g.CrossFadeColor(targetColor, (!instant) ? this.colors.fadeDuration : 0f, true, true);
		}
	}
}