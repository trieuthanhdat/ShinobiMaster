using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageIconParams
{
    public bool Current { get; set; }
    public bool Boss { get; set; }
    public bool Bonus { get; set; }
    public bool Skin { get; set; }
    public bool Completed { get; set; }
}

public class StageIconUI : MonoBehaviour
{
    [SerializeField] private Sprite bonusSprite;
    [SerializeField] private Sprite bossSprite;
    [SerializeField] private Sprite skinSprite;
    [SerializeField] private Sprite progressLongSprite;
    [SerializeField] private Sprite progressShortSprite;

    [SerializeField] private Image progressImage;
    [SerializeField] private Image additionalIcon;

    [SerializeField] private Color completedColor;
    [SerializeField] private Color currentColor;
    [SerializeField] private Color incompletedColor;
    

    private StageIconParams stageIconParams;




    public void SetParams(StageIconParams stageIconParams)
    {
        this.stageIconParams = stageIconParams;
        
        if (stageIconParams.Completed)
        {
            this.additionalIcon.color = this.completedColor;
            this.progressImage.color = this.completedColor;
        }
        else
        {
            this.additionalIcon.color = this.incompletedColor;
            this.progressImage.color = this.incompletedColor;
        }

        if (stageIconParams.Current)
        {
            this.additionalIcon.color = this.currentColor;
            this.progressImage.color = this.currentColor;
        }

        if (!stageIconParams.Completed && (stageIconParams.Boss || stageIconParams.Bonus || stageIconParams.Skin))
        {
            this.progressImage.sprite = this.progressShortSprite;
            
            this.additionalIcon.gameObject.SetActive(!stageIconParams.Completed);

            if (stageIconParams.Bonus)
            {
                this.additionalIcon.sprite = this.bonusSprite;
            }

            if (stageIconParams.Boss)
            {
                this.additionalIcon.sprite = this.bossSprite;
            }

            if (stageIconParams.Skin)
            {
                this.additionalIcon.sprite = this.skinSprite;
            }
        }
        else
        {
            this.additionalIcon.gameObject.SetActive(false);
            this.progressImage.sprite = this.progressLongSprite;
        }
    }
}
