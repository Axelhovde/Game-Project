using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{

    public Slider slider;

    public void SetMaxStamina(int stamina)
    {
        slider.maxValue = stamina;
        slider.value = stamina;
    }
    public void UpdateStamina(int stamina)
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
    public void SetStamina(int stamina)
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
