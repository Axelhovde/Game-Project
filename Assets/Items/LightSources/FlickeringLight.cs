using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlickeringLight : MonoBehaviour
{
    public Light2D light2D; // The Light2D component

    // Intensity parameters
    public float intensityMin = 0.8f; // Minimum intensity
    public float intensityMax = 1.2f; // Maximum intensity

    // Radius parameters
    public float innerRadiusMin = 0.4f; // Minimum inner radius
    public float innerRadiusMax = 0.6f; // Maximum inner radius
    public float outerRadiusMin = 1.4f; // Minimum outer radius
    public float outerRadiusMax = 1.6f; // Maximum outer radius

    // Flicker speed
    public float flickerSpeed = 1.5f; // Speed of the flicker

    // Movement speed
    public float movementSpeed = 0.1f; // Speed of the movement

    private float initialIntensity;
    private float initialInnerRadius;
    private float initialOuterRadius;

    void Start()
    {
        // Ensure we have a Light2D component
        if (light2D == null)
        {
            light2D = GetComponent<Light2D>();
        }

        initialIntensity = light2D.intensity; // Store the initial intensity
        initialInnerRadius = light2D.pointLightInnerRadius; // Store the initial inner radius
        initialOuterRadius = light2D.pointLightOuterRadius; // Store the initial outer radius
    }

    void Update()
    {
        // Change the intensity over time to create a flicker effect
        light2D.intensity = Mathf.Lerp(intensityMin, intensityMax, Mathf.PerlinNoise(Time.time * flickerSpeed, 0.0f));

        // Change the inner radius over time to create a flicker effect
        light2D.pointLightInnerRadius = Mathf.Lerp(innerRadiusMin, innerRadiusMax, Mathf.PerlinNoise(Time.time * flickerSpeed, 1.0f));

        // Change the outer radius over time to create a flicker effect
        light2D.pointLightOuterRadius = Mathf.Lerp(outerRadiusMin, outerRadiusMax, Mathf.PerlinNoise(Time.time * flickerSpeed, 2.0f));

        // Add slight position variation for a more dynamic flicker
        light2D.transform.localPosition = new Vector3(
            (Mathf.PerlinNoise(Time.time * movementSpeed, 3.0f) - 0.5f) * 2 * movementSpeed,
            (Mathf.PerlinNoise(Time.time * movementSpeed, 4.0f) - 0.5f) * 2 * movementSpeed,
            light2D.transform.localPosition.z
        );
    }
}
