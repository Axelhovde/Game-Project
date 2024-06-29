using System.Collections;
using UnityEngine;

public class TransparencyHandler : MonoBehaviour
{
    public float transparency = 0.7f; // Desired transparency when the player is inside the collider
    public float transitionDuration = 0.1f; // Duration of the transparency transition
    public SpriteRenderer spriteRenderer;
    private float originalAlpha;
    private Coroutine currentCoroutine;

    void Start()
    {
        if (spriteRenderer != null)
        {
            originalAlpha = spriteRenderer.color.a;
        }
        else
        {
            Debug.LogError("SpriteRenderer not found on " + gameObject.name);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            if (currentCoroutine != null) StopCoroutine(currentCoroutine);
            currentCoroutine = StartCoroutine(FadeTo(transparency, transitionDuration));
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            if (currentCoroutine != null) StopCoroutine(currentCoroutine);
            currentCoroutine = StartCoroutine(FadeTo(originalAlpha, transitionDuration));
        }
    }

    IEnumerator FadeTo(float targetAlpha, float duration)
    {
        float startAlpha = spriteRenderer.color.a;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float blend = Mathf.Clamp01(time / duration);
            Color color = spriteRenderer.color;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, blend);
            spriteRenderer.color = color;
            yield return null;
        }

        // Ensure the final alpha is set
        Color finalColor = spriteRenderer.color;
        finalColor.a = targetAlpha;
        spriteRenderer.color = finalColor;
    }
}
