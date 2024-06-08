using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelBar : MonoBehaviour
{

    public Slider slider;
    public PlayerStats playerStats;
    public LvlDisplay lvlDisplay;
    private int level = 0;
    private int xp = 100;

    // Start is called before the first frame update
    void Start()
    {
        if (slider != null)
        {
            lvlDisplay.UpdateLvlDisplay(level);
        }
        else
        {
            Debug.LogWarning("Slider is not assigned in LevelBar.");
        }
    }

    public void SetLevel(int level)
    {
        this.level = level;
        playerStats.SetLevel(level);
        if (lvlDisplay != null)
        {
            lvlDisplay.UpdateLvlDisplay(level);
        }
        else
        {
            Debug.LogWarning("LvlDisplay is not assigned in LevelBar.");
        }
    }
    public void SetMaxXp()
    {
        slider.maxValue = (xp + (20 * level));
    }

    public void AddXp(int xp)
    {
        if (slider != null)
        {
            Debug.Log("slider plus xp: " + (slider.value + xp));
            if (slider.maxValue <= (slider.value + xp))
            {
                Debug.Log("Leveling up and Adding xp: " + xp);
                SetLevel(level + 1);
                playerStats.SetLevel(level);
                SetXp((int)((slider.value + xp) - slider.maxValue));
                SetMaxXp();
            }
            else if (slider.maxValue > (slider.value + xp) && xp >= 0)
            {
                Debug.Log("Adding xp: " + xp);
                slider.value += xp;
                playerStats.SetXp((int)(slider.value));
                Debug.Log("Total xp: " + slider.value);
            }
        }
        else
        {
            Debug.LogWarning("Slider is not assigned in HealthBar.");
        }
    }


    public void SetXp(int xp)
    {
        if (slider != null)
        {
            if (slider.maxValue > xp && xp >= 0)
            {
                slider.value = xp;
                playerStats.SetXp(xp);
                Debug.Log("Xp set to playerstats: " + slider.value);
            }
            else
            {
                slider.value = 0;
                playerStats.SetXp(xp);
            }
        }
        else
        {
            Debug.LogWarning("Slider is not assigned in HealthBar.");
        }
    }
}
