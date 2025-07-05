using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public static SettingsUI Instance;
    
    [SerializeField] private Button settingsButton;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Image settingsButtonImage;
    [SerializeField] private Button soundsButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button vibroButton;
    [SerializeField] private Sprite buttonFrameSprite;
    [SerializeField] private Sprite buttonSprite;

    private bool soundsButtonActive;
    private bool musicButtonActive;
    private bool vibroButtonActive;




    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetActiveSoundsButton(AudioManager.Instance.SoundsActive);
        SetActiveMusicButton(AudioManager.Instance.MusicActive);
        SetActiveVibroButton(Development.Vibration.VibroActive);
    }


    public void SetSettingsButtonInteractable(bool active)
    {
        this.settingsButton.interactable = active;
    }

    public void SetSettingsPanelActive(bool active)
    {
        this.settingsPanel.SetActive(active);
    }

    public bool IsSettingsPanelActive()
    {
        return this.settingsPanel.activeInHierarchy;
    }

    public void ShowButtonSmoothly(float time, float alpha)
    {
        StartCoroutine(ShowButtonSmoothlyProcess(time, alpha));
    }
    
    public void HideButtonSmoothly(float time)
    {
        StartCoroutine(HideButtonSmoothlyProcess(time));
    }

    public void OnClickSoundsButton()
    {
        AudioManager.Instance.PlayClickButtonSound();
        SetActiveSoundsButton(!this.soundsButtonActive);
    }
    
    public void OnClickMusicButton()
    {
        AudioManager.Instance.PlayClickButtonSound();
        SetActiveMusicButton(!this.musicButtonActive);
    }
    
    public void OnClickVibroButton()
    {
        AudioManager.Instance.PlayClickButtonSound();
        SetActiveVibroButton(!this.vibroButtonActive);
    }

    public void SetActiveSoundsButton(bool state)
    {
        this.soundsButton.image.sprite = state ? this.buttonSprite : this.buttonFrameSprite;
        
        var icon = this.soundsButton.transform.GetChild(0).GetComponent<Image>();
        
        var color = icon.color;

        color.a = state ? 1f : 0.5f;

        icon.color = color;

        this.soundsButtonActive = state;
        
        AudioManager.Instance.SetActiveSounds(state);
    }
    
    public void SetActiveMusicButton(bool state)
    {
        this.musicButton.image.sprite = state ? this.buttonSprite : this.buttonFrameSprite;
        
        var icon = this.musicButton.transform.GetChild(0).GetComponent<Image>();
        
        var color = icon.color;

        color.a = state ? 1f : 0.5f;

        icon.color = color;

        this.musicButtonActive = state;
        
        AudioManager.Instance.SetActiveMusic(state);
    }
    
    public void SetActiveVibroButton(bool state)
    {
        this.vibroButton.image.sprite = state ? this.buttonSprite : this.buttonFrameSprite;
        
        var icon = this.vibroButton.transform.GetChild(0).GetComponent<Image>();
        
        var color = icon.color;

        color.a = state ? 1f : 0.5f;

        icon.color = color;

        this.vibroButtonActive = state;
        
        Development.Vibration.SetActiveVibro(state);
    }

    private IEnumerator ShowButtonSmoothlyProcess(float time, float alpha)
    {
        SetSettingsButtonInteractable(true);

        var step = alpha / time;

        var currTime = time;

        while (currTime > 0)
        {
            var color = this.settingsButtonImage.color;

            color.a += step * Time.deltaTime;

            this.settingsButtonImage.color = color;

            currTime -= Time.deltaTime;

            yield return null;
        }
    }
    
    private IEnumerator HideButtonSmoothlyProcess(float time)
    {
        SetSettingsButtonInteractable(false);

        var step = this.settingsButtonImage.color.a / time;

        var currTime = time;

        while (currTime > 0)
        {
            var color = this.settingsButtonImage.color;

            color.a -= step * Time.deltaTime;

            this.settingsButtonImage.color = color;

            currTime -= Time.deltaTime;

            yield return null;
        }
    }
}
