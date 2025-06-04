using UnityEngine;
using System.Collections;
using System;

public class EnemyEffectHandler : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

    [Header("Flash Effect")]
    [SerializeField] private Material flashMaterial;
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private Color flashColor = Color.white;

    [Header("Dissolve Effect")]
    [SerializeField] private Material dissolveMaterial;
    [SerializeField] private float dissolveDuration = 1f;
    [SerializeField] private Color dissolveEdgeColor1 = new Color(0.8f, 0.7f, 0.5f, 1f); // Sandy color
    [SerializeField] private Color dissolveEdgeColor2 = new Color(0.6f, 0.5f, 0.3f, 1f); // Darker sandy color
    [SerializeField] private float edgeWidth = 0.1f;
    [SerializeField] private Vector2 noiseScale = new Vector2(2f, 2f);
    [SerializeField] private float verticalBias = 1f;
    [SerializeField] private float upwardForce = 2f;
    [SerializeField] private float sandNoiseScale = 3f;
    [SerializeField] private float displacementStrength = 0.5f;
    [SerializeField] private float displacementSpeed = 2f;

    private Material originalMaterial;
    private bool isDissolving = false;
    private MaterialPropertyBlock propertyBlock;
    
    // Shader property IDs
    private static readonly int FlashColorID = Shader.PropertyToID("_FlashColor");
    private static readonly int FlashAmountID = Shader.PropertyToID("_FlashAmount");
    private static readonly int DissolveAmountID = Shader.PropertyToID("_DissolveAmount");
    private static readonly int EdgeColor1ID = Shader.PropertyToID("_EdgeColor1");
    private static readonly int EdgeColor2ID = Shader.PropertyToID("_EdgeColor2");
    private static readonly int EdgeWidthID = Shader.PropertyToID("_EdgeWidth");
    private static readonly int NoiseScaleID = Shader.PropertyToID("_NoiseScale");
    private static readonly int VerticalBiasID = Shader.PropertyToID("_VerticalBias");
    private static readonly int UpwardForceID = Shader.PropertyToID("_UpwardForce");
    private static readonly int SandNoiseScaleID = Shader.PropertyToID("_SandNoiseScale");
    private static readonly int DisplacementStrengthID = Shader.PropertyToID("_DisplacementStrength");
    private static readonly int DisplacementSpeedID = Shader.PropertyToID("_DisplacementSpeed");

    // Event that will be called when dissolve effect is complete
    public event Action OnDissolveComplete;

    private void Awake()
    {
        if (skinnedMeshRenderer != null)
        {
            originalMaterial = skinnedMeshRenderer.sharedMaterial;
        }
        propertyBlock = new MaterialPropertyBlock();
    }

    public void PlayHitEffect()
    {
        if (isDissolving) return;
        StartCoroutine(FlashRoutine());
    }

    public void PlayDeathEffect()
    {
        if (isDissolving) return;
        StartCoroutine(DissolveRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        // Set flash material
        skinnedMeshRenderer.sharedMaterial = flashMaterial;
        
        // Initialize property block
        skinnedMeshRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor(FlashColorID, flashColor);

        // Animate flash
        float elapsedTime = 0;
        while (elapsedTime < flashDuration)
        {
            float flashAmount = Mathf.Lerp(1f, 0f, elapsedTime / flashDuration);
            propertyBlock.SetFloat(FlashAmountID, flashAmount);
            skinnedMeshRenderer.SetPropertyBlock(propertyBlock);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset material if not dissolving
        if (!isDissolving)
        {
            skinnedMeshRenderer.sharedMaterial = originalMaterial;
            skinnedMeshRenderer.SetPropertyBlock(null);
        }
    }

    private IEnumerator DissolveRoutine()
    {
        isDissolving = true;

        // Set dissolve material
        skinnedMeshRenderer.sharedMaterial = dissolveMaterial;
        
        // Initialize property block
        skinnedMeshRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor(EdgeColor1ID, dissolveEdgeColor1);
        propertyBlock.SetColor(EdgeColor2ID, dissolveEdgeColor2);
        propertyBlock.SetFloat(EdgeWidthID, edgeWidth);
        propertyBlock.SetVector(NoiseScaleID, noiseScale);
        propertyBlock.SetFloat(VerticalBiasID, verticalBias);
        propertyBlock.SetFloat(UpwardForceID, upwardForce);
        propertyBlock.SetFloat(SandNoiseScaleID, sandNoiseScale);
        propertyBlock.SetFloat(DisplacementStrengthID, displacementStrength);
        propertyBlock.SetFloat(DisplacementSpeedID, displacementSpeed);
        propertyBlock.SetFloat(DissolveAmountID, 0f);
        skinnedMeshRenderer.SetPropertyBlock(propertyBlock);

        // Animate dissolve
        float elapsedTime = 0;
        while (elapsedTime < dissolveDuration)
        {
            float dissolveAmount = Mathf.Lerp(0f, 1f, elapsedTime / dissolveDuration);
            propertyBlock.SetFloat(DissolveAmountID, dissolveAmount);
            skinnedMeshRenderer.SetPropertyBlock(propertyBlock);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset material and dissolve state
        skinnedMeshRenderer.sharedMaterial = originalMaterial;
        skinnedMeshRenderer.SetPropertyBlock(null);
        isDissolving = false;

        // Notify that dissolve is complete
        OnDissolveComplete?.Invoke();
    }

    private void OnDisable()
    {
        // Reset materials when object is disabled (returned to pool)
        if (skinnedMeshRenderer != null)
        {
            skinnedMeshRenderer.sharedMaterial = originalMaterial;
            skinnedMeshRenderer.SetPropertyBlock(null);
        }
        isDissolving = false;
    }

    private void OnDestroy()
    {
        // Clean up materials if needed
        //if (flashMaterial != null)
        //    Destroy(flashMaterial);
        //if (dissolveMaterial != null)
        //    Destroy(dissolveMaterial);
    }
} 