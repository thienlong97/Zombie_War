using UnityEngine;

public class AudioManager : GenericSingleton<AudioManager>
{
    [SerializeField] private AudioSource audioSource;

    public override void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        EventBusManager.Instance.Subscribe(EventType.PlayerFired, OnPlaySound);
        EventBusManager.Instance.Subscribe(EventType.PlayerGrabWeapon, OnPlaySound);
    }

    private void OnDisable()
    {
        EventBusManager.Instance.Unsubscribe(EventType.PlayerFired, OnPlaySound);
        EventBusManager.Instance.Unsubscribe(EventType.PlayerGrabWeapon, OnPlaySound);
    }

    private void OnPlaySound(object eventData)
    {
        if (eventData is AudioClip data)
        {
            PlaySFX(data);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        audioSource.PlayOneShot(clip);
    }
}
