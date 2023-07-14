using UnityEngine;
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

        Invoke(nameof(StopHaptics), hapticToPlay.duration);
    }
    public void StopHaptics()
    {
        currentGamepad.SetMotorSpeeds(0f, 0f);
    }
}

