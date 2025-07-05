using System;
using Game;
using UnityEngine;

public class GDPR: MonoBehaviour
{
	public const string GDPRAcceptedKey = "gdpr_accepted";
	
		
	public bool IsGDPRAccepted
	{
		get => Convert.ToBoolean(PlayerPrefs.GetInt(GDPRAcceptedKey, 0));
		set
		{
			PlayerPrefs.SetInt(GDPRAcceptedKey, Convert.ToInt32(value));
			PlayerPrefs.Save();
		}
	}

	[SerializeField] private GameObject gdprWindow;


	public void SetActiveWindow(bool state)
	{
		this.gdprWindow.SetActive(state);
	}

	public void OnClickAcceptButton()
	{
		AudioManager.Instance.PlayClickButtonSound();
	
		AcceptGDPR();
		SetActiveWindow(false);
	}

	public void OpenPrivacyPolicyLink()
	{
		Application.OpenURL("https://ancestralcode.vercel.app/private-policy");
	}

	public void OpenTermOfUseLink()
	{
		Application.OpenURL("https://ancestralcode.vercel.app/terms-of-use");
	}

	public void AcceptGDPR()
	{
		if (IsGDPRAccepted)
		{
			return;
		}

		IsGDPRAccepted = true;
		
		MaxSdk.SetHasUserConsent(true);
	}
}