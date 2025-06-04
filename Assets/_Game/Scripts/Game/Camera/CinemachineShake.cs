using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(CinemachineCamera))]
public class CinemachineShake : GenericSingleton<CinemachineShake>
{
    [SerializeField] private CameraShakeConfig shakeConfig;
    
    private CinemachineCamera _virtualCamera;
    private CinemachineBasicMultiChannelPerlin _perlin;
    private float _shakeTimer;
    private float _shakeTimerTotal;
    private float _startingIntensity;
    private bool _isShaking;

    public override void Awake()
    {
        base.Awake();
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        shakeConfig = GameConfig.Instance.CameraShakeConfig;
        _virtualCamera = GetComponent<CinemachineCamera>();
        if (_virtualCamera == null)
        {
            Debug.LogError($"CinemachineCamera not found on {gameObject.name}");
            return;
        }

        _perlin = _virtualCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
        if (_perlin == null)
        {
            Debug.LogError($"Noise component not found on {gameObject.name}'s Camera. Please add a Noise profile to the Camera.");
        }
    }

    public void ShakeCamera(float intensity, float time)
    {
        if (_perlin == null) return;

        _perlin.AmplitudeGain = intensity;
        _startingIntensity = intensity;
        _shakeTimerTotal = time;
        _shakeTimer = time;
        _isShaking = true;
    }

    // Convenience methods using config
    public void ShakeCameraLight()
    {
        ShakeCamera(shakeConfig.lightShakeIntensity, shakeConfig.lightShakeDuration);
    }

    public void ShakeCameraMedium()
    {
        ShakeCamera(shakeConfig.mediumShakeIntensity, shakeConfig.mediumShakeDuration);
    }

    public void ShakeCameraHeavy()
    {
        ShakeCamera(shakeConfig.heavyShakeIntensity, shakeConfig.heavyShakeDuration);
    }

    public void ShakeCameraPreset(string presetName)
    {
        if (shakeConfig.customShakePresets == null) return;
        
        ShakePreset preset = System.Array.Find(shakeConfig.customShakePresets, 
            x => x.presetName.Equals(presetName, System.StringComparison.OrdinalIgnoreCase));
            
        if (preset != null)
        {
            ShakeCamera(preset.intensity, preset.duration);
        }
        else
        {
            Debug.LogWarning($"Shake preset '{presetName}' not found!");
        }
    }

    private void Update()
    {
        if (!_isShaking) return;
        
        _shakeTimer -= Time.deltaTime;
        
        if (_shakeTimer > 0)
        {
            float shakeProgress = 1 - (_shakeTimer / _shakeTimerTotal);
            UpdateShakeIntensity(shakeProgress);
        }
        else
        {
            StopShaking();
        }
    }

    private void UpdateShakeIntensity(float progress)
    {
        if (_perlin == null) return;
        _perlin.AmplitudeGain = Mathf.Lerp(_startingIntensity, 0f, progress);
    }

    private void StopShaking()
    {
        if (_perlin != null)
        {
            _perlin.AmplitudeGain = 0f;
        }
        _isShaking = false;
        _shakeTimer = 0f;
    }

    private void OnDisable()
    {
        StopShaking();
    }
} 