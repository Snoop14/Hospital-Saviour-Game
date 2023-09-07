using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//https://www.c-sharpcorner.com/article/unity-change-scene-on-button-click-using-c-sharp-scripts-in-unity/
using UnityEngine.SceneManagement;
//https://www.kindacode.com/snippet/unity-how-to-show-a-confirmation-dialog/ accessed 3/9/23
using UnityEditor;

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

    [SerializeField]
    Text infoDisplay;

    [SerializeField]
    Text scoreDisplay;

    [SerializeField]
    Transform levelsParent;

    [SerializeField]
    Transform StarsDisplay;

    private void Start()
    {
        ResetButtons();

        //reset playersset to last value in playerprefs
        if (PlayerPrefs.GetInt("PlayerNum") > 0)
        {
            playersSet = PlayerPrefs.GetInt("PlayerNum");
        }
        levelSet = PlayerPrefs.GetInt("LevelNum");
        GameObject.Find("PlayersSlider").GetComponent<Slider>().value = playersSet;
    }

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
        playersSet = (int)players.value;
        ResetButtons();
        DisplayHighScore();
    }

    /// <summary>
    /// Loads the GameScene
    /// </summary>
    public void StartGame()
    {
        SetLevelNum(levelSet, playersSet);
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
    }

    /// <summary>
    /// Changes the score that is displayed on screen
    /// as well as the 'stars' for the level
    /// </summary>
    private void DisplayHighScore()
    {
        string levelName = "Level " + levelSet;

        if (playersSet == 1)
        {
            infoDisplay.text = levelName + " Solo";
            levelName += "_1p";
        }
        else if (playersSet == 2)
        {
            infoDisplay.text = levelName + " Duo";
            levelName += "_2p";
        }

        currHighScore = PlayerPrefs.GetInt(levelName);

        levelName = "Level" + levelSet + "Data";
        int[] stars = Resources.Load<Levels>("Level Data/" + levelName).stars;

        for(int i = 0; i <= 2; i++)
        {
            StarsDisplay.GetChild(3).GetChild(i).GetComponent<Text>().text = (stars[i] * playersSet).ToString();

            if(currHighScore >= (stars[i])*playersSet)
            {
                StarsDisplay.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                StarsDisplay.GetChild(i).gameObject.SetActive(false);
            }
            
        }

        scoreDisplay.text = currHighScore.ToString();
    }

    /// <summary>
    /// Deletes all playerPrefs and reloads the scene
    /// Should this be included in the final game?
    /// </summary>
    public void ResetGame()
    {
        //https://www.kindacode.com/snippet/unity-how-to-show-a-confirmation-dialog/ accessed 3/9/23
        bool decision = EditorUtility.DisplayDialog(
            "Reset Game", // title
            "Are you sure want to reset all game data?", // description
            "Yes", // OK button
            "No" // Cancel button
        );

        if (decision)
        {
            PlayerPrefs.DeleteAll();
            SceneManager.LoadScene("MenuScene");
        }
                
    }

    private void ResetButtons()
    {
        //turn off buttons
        for (int i = 1; i <= 4; i++)
        {
            Button thisButton = levelsParent.GetChild(i).GetComponent<Button>();
            thisButton.interactable = false;
        }

        int numLevelsComplete = PlayerPrefs.GetInt("Highest_Level_Complete_" + playersSet + "p");
        //ensure button unlock remains within set buttons
        if (numLevelsComplete >= 1 && numLevelsComplete <= 4)
        {
            for (int i = 1; i <= numLevelsComplete; i++)
            {
                Button thisButton = levelsParent.GetChild(i).GetComponent<Button>();
                thisButton.interactable = true;
            }
        }
        else if (numLevelsComplete >= 1 && numLevelsComplete == 5)
        {
            for (int i = 1; i <= 4; i++)
            {
                Button thisButton = levelsParent.GetChild(i).GetComponent<Button>();
                thisButton.interactable = true;
            }
        }
    }


}
