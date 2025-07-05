using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCountUI : MonoBehaviour
{
    public static EnemyCountUI EnemyCount;

    [SerializeField] private Text text;
    [SerializeField] private Sprite soldierIcon;
    [SerializeField] private Image enemyImage;
    
    

    private void Awake()
    {
        if (EnemyCount)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            EnemyCount = this;
        }
    }

    public void SetCount(int count)
    {
        text.text = count.ToString();
    }

    public void SetIcon(Sprite icon)
    {
        this.enemyImage.sprite = icon;
        this.enemyImage.preserveAspect = true;
    }

    public void SetSoldierIcon()
    {
        this.enemyImage.sprite = this.soldierIcon;
        this.enemyImage.preserveAspect = true;
    }
}
