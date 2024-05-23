using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public Slider slider;

    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetHealth(float health)
    {
        if (slider != null && slider.maxValue > health && health >= 0)
        {
            slider.value = health;
        }
        else if (health < 0)
        {
            slider.value = 0;
        }
        else
        {
            Debug.LogWarning("Slider is not assigned in HealthBar.");
        }
    }
}
