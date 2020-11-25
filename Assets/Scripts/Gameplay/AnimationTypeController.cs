using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTypeController : MonoBehaviour
{
    [SerializeField] private AnimatorOverrideController[] overrideControllers;
    [SerializeField] private AnimatorOverrider overrider;
    public void Set(int value)
    {
        overrider.SetAnimations(overrideControllers[value]);
    }
}
