using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCountdown : MonoBehaviour
{
    public CameraShake CameraShake;
    public void ShakeCamera()
    {
        StartCoroutine(CameraShake.Shake(0.4f, 0.2f));
    }
}
