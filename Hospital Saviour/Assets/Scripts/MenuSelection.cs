﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//https://www.c-sharpcorner.com/article/unity-change-scene-on-button-click-using-c-sharp-scripts-in-unity/
using UnityEngine.SceneManagement;

public class MenuSelection : MonoBehaviour
{
    public static int levelNum;
    public static int playerNum;

    //holder for current level selected
    private int levelSet = 1;

    //holder for players selected
    private int playersSet = 1;

    //holder for high score to display
    private int currHighScore;


    public void StartLevel(int level)
    {
        levelSet = level;
        //Debug.Log("Loading Level " + levelSet);

        DisplayHighScore();
    }


    //https://stackoverflow.com/questions/32680348/unity3d-slider-onvaluechanged-sending-only-0-or-other-defined-value accessed 3/8/23
    //public void NumberPlayers(int players)
    public void NumberPlayers(Slider players)
    {
        //playersSet = players;
        playersSet = (int)players.value;

        //Debug.Log(players + " Player(s)");
        //Debug.Log(playersSet + " Player(s)");
    }

    public void StartGame()
    {
        SetLevelNum(levelSet, playersSet);
        //Debug.Log("Starting Level " + levelSet + " with " + playersSet + " players");
        SceneManager.LoadScene("GameScene");
    }

    /// <summary>
    /// Set the PlayerPrefs before starting a level.
    /// PlayerPrefs are utilized to essentially transfer data between scenes.
    /// https://docs.unity3d.com/ScriptReference/PlayerPrefs.html
    /// </summary>
    /// <param name="_level"></param>
    /// <param name="_players"></param>
    public static void SetLevelNum(int _level, int _players)
    {
        levelNum = _level;
        playerNum = _players;

        PlayerPrefs.SetInt("LevelNum", levelNum);
        PlayerPrefs.SetInt("PlayerNum", playerNum);

        //MenuSelection menu = new MenuSelection();
        
        //menu.DisplayHighScore();


    }

    private void DisplayHighScore()
    {
        
        
        string levelName = "Level" + levelSet + "Data";
        //https://docs.unity3d.com/2020.1/Documentation/ScriptReference/Resources.Load.html accessed 14/8/23
        var levelToLoad = Resources.Load<Levels>("Level Data/" + levelName);
        
        currHighScore = levelToLoad.highScore;

        Debug.Log(currHighScore);

    }

}
