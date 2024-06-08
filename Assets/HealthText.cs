using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthText : MonoBehaviour
{
    public float timeToLive = 0.5f;
    public float floatSpeed = 200;
    public Vector3 floatDirection = new Vector3(0, 1, 0);
    public TextMeshProUGUI textMesh;
    float timeElapsed = 0.0f;
    Color startingColor;

    RectTransform rectTransform;

    void Start()
    {
        startingColor = textMesh.color;
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;
        rectTransform.position += floatDirection * floatSpeed * Time.deltaTime;

        textMesh.color = new Color(startingColor.r, startingColor.g, startingColor.b, 1 - timeElapsed / timeToLive);
        if (timeElapsed >= timeToLive)
        {
            Destroy(gameObject);
        }
    }
}
