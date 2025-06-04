using UnityEngine;
[System.Serializable]
public class CameraShakeConfig
{
    [Header("Light Shake")]
    public float lightShakeIntensity = 1f;
    public float lightShakeDuration = 0.2f;

    [Header("Medium Shake")]
    public float mediumShakeIntensity = 2.5f;
    public float mediumShakeDuration = 0.3f;

    [Header("Heavy Shake")]
    public float heavyShakeIntensity = 4f;
    public float heavyShakeDuration = 0.5f;

    [Header("Custom Shake Presets")]
    public ShakePreset[] customShakePresets;
}

[System.Serializable]
public class ShakePreset
{
    public string presetName;
    public float intensity;
    public float duration;
} 