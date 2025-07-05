using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageParamScene : MonoBehaviour
{
    [Header("Sky")]
    [SerializeField] private Material sceneSky;

    public void SetParams() {
        //RenderSettings.skybox = sceneSky;
    }
}
