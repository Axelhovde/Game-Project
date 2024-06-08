using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{

    public Slider slider;

    public void SetMaxStamina(float stamina)
    {
        slider.maxValue = stamina;
        slider.value = stamina;
    }
    public void AddStamina(float stamina)
    {
        if (slider != null)
        {
            if (slider.maxValue > stamina && slider.value + stamina >= 0)
            {
                slider.value += stamina;
            }
        }
        else
        {
            Debug.LogWarning("Slider is not assigned in HealthBar.");
        }
    }
    public void SetStamina(float stamina)
    {
        if (slider != null)
        {
            if (slider.maxValue > stamina && stamina >= 0)
            {
                slider.value = stamina;
            }
        }
        else
        {
            Debug.LogWarning("Slider is not assigned in HealthBar.");
        }
    }
}
