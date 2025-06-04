using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class BloodScreen : MonoBehaviour
{
    [Header("Blood Overlay Images")]
    [SerializeField] private Image[] directionalBloods; // [0]Top, [1]Bottom, [2]Left, [3]Right

    [Header("Effect Settings")]
    [SerializeField] private float flashDuration = 0.3f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private float maxAlpha = 0.8f;
    
    [Header("Blood Effect")]
    [SerializeField] private Image[] bloodScreenEffects;

    private void Start()
    {
        // Initialize all blood images to be transparent
        SetAllBloodAlpha(0);
        InitializeBloodEffects();
    }

    private void InitializeBloodEffects()
    {
        if (bloodScreenEffects != null)
        {
            foreach (var effect in bloodScreenEffects)
            {
                if (effect != null)
                {
                    var color = effect.color;
                    color.a = 0;
                    effect.color = color;
                }
            }
        }
    }

    private void OnEnable()
    {
        EventBusManager.Instance.Subscribe(EventType.PlayerHit, OnPlayerHitGeneral);
    }

    private void OnDisable()
    {
        EventBusManager.Instance.Unsubscribe(EventType.PlayerHit, OnPlayerHitGeneral);
    }

    private void SetAllBloodAlpha(float alpha)
    {
        if (directionalBloods == null) return;

        foreach (var blood in directionalBloods)
        {
            if (blood != null)
            {
                var color = blood.color;
                color.a = alpha;
                blood.color = color;
            }
        }
    }

    public void OnPlayerHit(Vector3 hitDirection)
    {
        if (directionalBloods == null || directionalBloods.Length < 4) return;

        // Kill any ongoing tweens
        foreach (var blood in directionalBloods)
        {
            if (blood != null)
                DOTween.Kill(blood);
        }

        // Normalize the hit direction
        hitDirection.Normalize();

        // Flash the blood overlays based on direction
        if (hitDirection.z > 0.3f)
            FlashBloodOverlay(directionalBloods[0]); // Top
        if (hitDirection.z < -0.3f)
            FlashBloodOverlay(directionalBloods[1]); // Bottom
        if (hitDirection.x > 0.3f)
            FlashBloodOverlay(directionalBloods[2]); // Right
        if (hitDirection.x < -0.3f)
            FlashBloodOverlay(directionalBloods[3]); // Left

        // Flash blood screen effects
        FlashBloodEffects();
    }

    private void FlashBloodOverlay(Image bloodImage)
    {
        if (bloodImage == null) return;

        // Reset any ongoing animation
        DOTween.Kill(bloodImage);

        // Create flash sequence
        Sequence flashSequence = DOTween.Sequence();

        // Quick fade in to maxAlpha
        flashSequence.Append(bloodImage.DOFadeImage(maxAlpha, flashDuration));
        // Slower fade out to 0
        flashSequence.Append(bloodImage.DOFadeImage(0, fadeOutDuration));

        // Play the sequence
        flashSequence.Play();
    }

    private void FlashBloodEffects()
    {
        if (bloodScreenEffects == null) return;

        foreach (var effect in bloodScreenEffects)
        {
            if (effect != null)
            {
                // Kill any ongoing tweens
                DOTween.Kill(effect);

                // Create flash sequence
                Sequence flashSequence = DOTween.Sequence();

                // Quick fade in to maxAlpha
                flashSequence.Append(effect.DOFadeImage(maxAlpha, flashDuration));
                // Slower fade out to 0
                flashSequence.Append(effect.DOFadeImage(0, fadeOutDuration));

                // Play the sequence
                flashSequence.Play();
            }
        }
    }

    // Call this when player gets hit from a specific direction
    public void OnPlayerHitFromDirection(Vector3 hitPosition)
    {
        // Calculate hit direction relative to player
        Vector3 hitDirection = hitPosition - transform.position;
        OnPlayerHit(hitDirection);
    }

    // Call this for a general hit (will show all blood overlays)
    public void OnPlayerHitGeneral(object data)
    {
        if (directionalBloods == null) return;

        // Flash all directional blood overlays
        foreach (var blood in directionalBloods)
        {
            if (blood != null)
                FlashBloodOverlay(blood);
        }

        // Flash blood screen effects
        FlashBloodEffects();
    }

    private void OnDestroy()
    {
        // Clean up DOTween animations for directional bloods
        if (directionalBloods != null)
        {
            foreach (var blood in directionalBloods)
            {
                if (blood != null)
                    DOTween.Kill(blood);
            }
        }

        // Clean up DOTween animations for blood effects
        if (bloodScreenEffects != null)
        {
            foreach (var effect in bloodScreenEffects)
            {
                if (effect != null)
                    DOTween.Kill(effect);
            }
        }
    }
}

// Extension method for UI Image fade
public static class DOTweenExtensions
{
    public static Tweener DOFadeImage(this Image image, float endValue, float duration)
    {
        return DOTween.To(() => image.color.a,
            x => {
                var color = image.color;
                color.a = x;
                image.color = color;
            },
            endValue,
            duration);
    }
} 