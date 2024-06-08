using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PlayerData
{
    public int level;
    public float health;
    public int xp;
    public float maxHealth;
    public float[] position;

    public PlayerData(PlayerStats player)
    {
        level = player.GetLevel();
        health = player.GetHealth();
        maxHealth = player.GetMaxHealth();
        xp = player.GetXp();

        position = new float[2];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;

    }

}
