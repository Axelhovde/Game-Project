using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public PlayerStats playerStats;
    public PlayerController playerController;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        playerController.enabled = true;
    }
    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        playerController.enabled = false;
    }
    public void LoadMenu()
    {
        //Time.timeScale = 1f;
        Debug.Log("Loading menu...");
        //SceneManager.LoadScene("Menu");
    }
    public void SaveGame()
    {
        SaveSystem.SavePlayer(playerStats);
        //for each collition object with enemy tag, make a enemyData object and save it
        //make list of enemies
        List<EnemyData> enemies = new List<EnemyData>();
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            EnemyData enemyData = new EnemyData(enemy.GetComponent<DamageableCharacter>());
            enemies.Add(enemyData);
        }
        SaveSystem.SaveEnemies(enemies);
        Debug.Log("Game saved");
    }
    public void LoadGame()
    {
        PlayerData data = SaveSystem.LoadPlayer();
        if (data == null)
        {
            Debug.LogError("Failed to load player data.");
            return;
        }

        Debug.Log("Game loaded");
        playerStats.SetMaxHealth(data.maxHealth);
        playerStats.SetHealth(data.health);
        playerStats.SetXp(data.xp);
        playerStats.SetLevel(data.level);
        playerStats.SetPosition(data.position[0], data.position[1]);

        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Destroy(enemy);
        }

        List<EnemyData> enemyData = SaveSystem.LoadEnemies();

        foreach (EnemyData enemy in enemyData)
        {
            string type = enemy.type;
            string path = "Prefabs/Enemies/" + type;

            GameObject enemyPrefab = Resources.Load<GameObject>(path);
            GameObject newEnemy = Instantiate(enemyPrefab, new Vector3(enemy.position[0], enemy.position[1], 0), Quaternion.identity);
            DamageableCharacter damageableCharacter = newEnemy.GetComponent<DamageableCharacter>();
            if (damageableCharacter != null)
            {
                damageableCharacter.SetMaxHealth(enemy.maxHealth);
                damageableCharacter.SetHealth(enemy.health);
            }
            else
            {
                Debug.LogError("DamageableCharacter component not found on instantiated enemy.");
            }
        }
        Resume();
    }


    public class AllEnemiesData
    {
        public List<EnemyData> enemies;
    }
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
