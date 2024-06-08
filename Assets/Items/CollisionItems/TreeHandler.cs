using UnityEngine;
using System.Collections;

public class TreeHandler : MonoBehaviour
{
    public float shakeDuration = 0.5f; // Duration of the shake effect
    public float rotationMagnitude = 3f; // Magnitude of the shake effect for rotation
    public float rotationFrequency = 20f; // Frequency of the shake effect for rotation
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Coroutine shakeCoroutine;

    void Start()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }

    public void ShakeTree()
    {
        if (shakeCoroutine == null)
        {
            shakeCoroutine = StartCoroutine(Shake());
        }
    }

    IEnumerator Shake()
    {
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            float rotationZ = Mathf.Sin(Time.time * rotationFrequency) * rotationMagnitude;
            transform.localRotation = originalRotation * Quaternion.Euler(0, 0, rotationZ);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;
        shakeCoroutine = null;
    }
}
