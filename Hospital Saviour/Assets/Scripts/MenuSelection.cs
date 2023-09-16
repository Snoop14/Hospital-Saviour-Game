using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//https://www.c-sharpcorner.com/article/unity-change-scene-on-button-click-using-c-sharp-scripts-in-unity/
using UnityEngine.SceneManagement;
//https://www.kindacode.com/snippet/unity-how-to-show-a-confirmation-dialog/ accessed 3/9/23
using UnityEditor;
using TMPro;

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
    TMP_Text infoDisplay;

    [SerializeField]
    TMP_Text scoreDisplay;

    [SerializeField]
    Transform levelsParent;

    [SerializeField]
    Transform StarsDisplay;

    private void Start()
    {
        //sets active and inactive buttons as per settings
        ResetButtons();

        //reset playersset to last value in playerprefs
        if (PlayerPrefs.GetInt("PlayerNum") > 0)
        {
            playersSet = PlayerPrefs.GetInt("PlayerNum");
        }

        //gets last level from playerprefs
        levelSet = PlayerPrefs.GetInt("LevelNum");
        //if the last level is 0 (has not been played)...
        if(levelSet == 0)
        {
            //the level is 1
            levelSet = 1;
        }

        //find the players slider and set it to the value of players set
        GameObject.Find("PlayersSlider").GetComponent<Slider>().value = playersSet;
    }

    /// <summary>
    /// This function is called when a level is selected
    /// </summary>
    /// <param name="level"></param>
    public void StartLevel(int level)
    {
        //levelSet value is updated to value of the button
        levelSet = level;

        //runs display high sore for the level
        DisplayHighScore();
    }


    //https://stackoverflow.com/questions/32680348/unity3d-slider-onvaluechanged-sending-only-0-or-other-defined-value accessed 3/8/23
    //public void NumberPlayers(int players)
    /// <summary>
    /// Change the number of players based on slider value change
    /// </summary>
    /// <param name="players"></param>
    public void NumberPlayers(Slider players)
    {
        //set value of playersSet based upon the slider value
        playersSet = (int)players.value;

        //reset level to 1
        levelSet = 1;

        //run rest buttons function
        ResetButtons();

        //run display high score function
        DisplayHighScore();
    }

    /// <summary>
    /// Loads the GameScene upon clicking "Start Game" button
    /// </summary>
    public void StartGame()
    {
        //runs SetLevelNum function with the level and player number as set
        SetLevelNum(levelSet, playersSet);

        //loads the gaame
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
        //sets the level to load
        levelNum = _level;

        //sets the number of players
        playerNum = _players;

        //sets the player prefs of level and players
        PlayerPrefs.SetInt("LevelNum", levelNum);
        PlayerPrefs.SetInt("PlayerNum", playerNum);
    }

    /// <summary>
    /// Changes the score that is displayed on screen
    /// as well as the 'stars' for the level
    /// </summary>
    private void DisplayHighScore()
    {
        //adds text of level name
        string levelName = "Level " + levelSet;

        //if 1 player mode is selected
        if (playersSet == 1)
        {
            //add "Solo" to the display words
            infoDisplay.text = levelName + " Solo";
            
            // adds 1player value to level name
            levelName += "_1p";
        }
        //otherwise if there is 2 players selected...
        else if (playersSet == 2)
        {
            //add "duo" to the displayed text
            infoDisplay.text = levelName + " Duo";

            // adds 2player value to level name
            levelName += "_2p";
        }

        //sets value from player prefs
        currHighScore = PlayerPrefs.GetInt(levelName);

        //sets text for levelname to load
        levelName = "Level" + levelSet + "Data";

        //loads star values for the level
        int[] stars = Resources.Load<Levels>("Level Data/" + levelName).stars;

        //iterate from 0 to 2 (3 iterations)
        for(int i = 0; i <= 2; i++)
        {
            //display the star values for the level
            StarsDisplay.GetChild(3).GetChild(i).GetComponent<TMP_Text>().text = (stars[i] * playersSet).ToString();

            //if the current high score is higher than the star value...
            if(currHighScore >= (stars[i])*playersSet)
            {
                //display the star image
                StarsDisplay.GetChild(i).gameObject.SetActive(true);
            }
            //otherwise ...
            else
            {
                //hide the star image
                StarsDisplay.GetChild(i).gameObject.SetActive(false);
            }
            
        }

        //display the high score value on screen
        scoreDisplay.text = currHighScore.ToString();
    }

    //turned off as only works within Unity
    /*
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
    */

    /// <summary>
    /// Disables and enables the buttons that for the levels that
    /// should and shouldn't be accessible to the player
    /// </summary>
    private void ResetButtons()
    {
        //turn off buttons
        for (int i = 1; i <= 4; i++)
        {
            Button thisButton = levelsParent.GetChild(i).GetComponent<Button>();
            thisButton.interactable = false;
        }

        //turns back on the buttons that are less than or equal to the highest level played plus one (so can play the next level, but no further)
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
        //if highest level is 5, activate all buttons, but don't try to activate the 6th button, bacause there isn't one
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
