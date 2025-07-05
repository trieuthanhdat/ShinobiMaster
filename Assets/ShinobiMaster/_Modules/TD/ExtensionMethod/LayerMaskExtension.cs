using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LayerMaskExtension
{
    public static bool IsLayermaskIncludeTargetLayer(this LayerMask targetLayerMask, GameObject target)
    {
        //Shift 1 to the left by the layer number and check it with the targetLayermask
        return (targetLayerMask.value & (1 << target.layer)) != 0; ;
    }
}
