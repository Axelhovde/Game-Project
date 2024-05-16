using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public Slider slider;

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetHealth(int health)
    {
        if (slider != null && slider.maxValue > health && health >= 0)
        {
            slider.value = health;
        }
        else
        {
            Debug.LogWarning("Slider is not assigned in HealthBar.");
        }
    }
}
