using UnityEngine;
using System;

public class VFXController : MonoBehaviour
{
    public VFX[] visualEffects;

    public void PlayVFX(Transform location, string name)
    {
        VFX visualEffect = Array.Find(visualEffects, vfx => vfx.name == name);

        Instantiate(visualEffect.effect, location.position, Quaternion.identity);
    }
}
