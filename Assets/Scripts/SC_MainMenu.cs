﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SC_MainMenu : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject CreditsMenu;

    // Start is called before the first frame update
    void Start()
    {
        MainMenuButton();
    }

    public void PlayNowButton(String gameLevel)
    {
        // Play Now Button has been pressed, here you can initialize your game (For example Load a Scene called GameLevel etc.)
        UnityEngine.SceneManagement.SceneManager.LoadScene(gameLevel);
    }

    public void CreditsButton()
    {
        // Show Credits Menu
        MainMenu.SetActive(false);
        CreditsMenu.SetActive(true);
    }

    public void MainMenuButton()
    {
        // Show Main Menu
        MainMenu.SetActive(true);
        CreditsMenu.SetActive(false);
    }

    public void QuitButton()
    {
        // Quit Game
        Application.Quit();
    }

    public void deleteSave(String gameLevel)
    {
        String buffPath = "Save_Cube (1).json";
        String path;
#if UNITY_ANDROID && !UNITY_EDITOR
        path = Path.Combine(Application.persistentDataPath, buffPath);
#else
        path = Path.Combine(Application.dataPath, buffPath);
#endif
        if (File.Exists(path))
        {
            for (int i = 0; i < 64; i++)
            {
                path = Path.Combine(Application.dataPath, "Save_Cube (" + i + ").json");
                File.Delete(path);
            }
            path = Path.Combine(Application.dataPath, "Save_data.json");
            File.Delete(path);
           
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene(gameLevel);
    }
}