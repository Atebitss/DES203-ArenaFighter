using UnityEngine;
using System.Collections;
using System;
using UnityEngine.InputSystem;

public class HapticController : MonoBehaviour
{
    public Haptics[] haptics;
    private Gamepad currentGamepad;
    

    public void PlayHaptics(string name, Gamepad gamepad)
    {
        Haptics hapticToPlay = Array.Find(haptics, haptic => haptic.name == name);
        currentGamepad = gamepad;
        gamepad.SetMotorSpeeds(hapticToPlay.leftRumble, hapticToPlay.rightRumble);

        StartCoroutine(StopHaptics(hapticToPlay.duration, gamepad));

    }
    public IEnumerator StopHaptics(float duration, Gamepad gamepad)
    {

        yield return new WaitForSeconds(duration);
        gamepad.SetMotorSpeeds(0f, 0f);
    }
}

