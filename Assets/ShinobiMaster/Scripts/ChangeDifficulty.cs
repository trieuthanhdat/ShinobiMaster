using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.UI;

public class ChangeDifficulty : MonoBehaviour
{
	[SerializeField] Button set;

	[SerializeField] InputField difficultyInputField;

	private void Start()
	{
		set.onClick.AddListener(Change);
	}

	private void Change()
	{
		
	
		if (int.TryParse(this.difficultyInputField.text, out var num))
		{
			num--;
			DifficultyRepository.Instance.CurrentDifficultyConfig = num;
		}
	}
}
