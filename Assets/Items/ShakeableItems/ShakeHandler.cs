using UnityEngine;
using System.Collections;

public class ShakeHandler : MonoBehaviour
{
    public float shakeDuration = 0.8f; // Duration of the shake effect
    public float rotationMagnitude = 3f; // Magnitude of the shake effect for rotation
    public float rotationFrequency = 10f; // Frequency of the shake effect for rotation
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Coroutine shakeCoroutine;

    void Start()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("PlayerHitbox"))
        {
            if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
            shakeCoroutine = StartCoroutine(Shake());
        }
    }

    IEnumerator Shake()
    {
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            // Use Mathf.Sin for a smooth back-and-forth motion
            float rotationZ = Mathf.Sin(Time.time * rotationFrequency) * rotationMagnitude;
            transform.localRotation = originalRotation * Quaternion.Euler(0, 0, rotationZ);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;
    }
}
