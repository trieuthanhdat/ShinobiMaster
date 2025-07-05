using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimatorExtension
{
    /// <summary>  Return the Hash value of a parameter if it exists on the Animator. If it not exists it returns 0 </summary>
    public static int TryOptionalParameter(this Animator m_Animator, string param)
    {
        var AnimHash = Animator.StringToHash(param);

        foreach (var p in m_Animator.parameters)
        {
            if (p.nameHash == AnimHash) return AnimHash;
        }

        return 0;
    }
    public static int GetAnimationNumberOfEvents(AnimationClip clip, string eventName)
    {
        if (clip == null) return 0;
        int amount = 0;
        foreach(var evnt in clip.events)
        {
            if(evnt.functionName == eventName)
                amount++;
        }
        return amount;
    }
    public static Transform GetBoneTransform(this Animator animator, HumanBodyBones bones)
    {
        return animator.GetBoneTransform(bones).transform;
    }
}
