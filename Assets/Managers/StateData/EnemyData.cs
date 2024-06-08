using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text.RegularExpressions;


[System.Serializable]
public class EnemyData
{
    public float health;
    public float maxHealth;
    public float[] position;
    public bool targetable;
    public string type;

    public static string CleanName(string name)
    {
        // Remove spaces, numbers, and parentheses
        //remove "Clone" from the name
        name = name.Replace("(Clone)", "");
        return Regex.Replace(name, @"[\d\s\(\)]", "");
    }
    public EnemyData(DamageableCharacter enemy)
    {
        health = enemy.GetHealth();
        maxHealth = enemy.GetMaxHealth();
        targetable = enemy.Targetable;
        type = CleanName(enemy.gameObject.name);

        position = new float[2];
        position[0] = enemy.transform.position.x;
        position[1] = enemy.transform.position.y;

    }

}
