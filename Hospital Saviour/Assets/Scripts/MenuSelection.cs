using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSelection : MonoBehaviour
{
    //holder for current level selected
    private int levelSet = 1;

    //holder for players selected
    private int playersSet = 1;

    public void StartLevel(int level)
    {
        levelSet = level;
        Debug.Log("Loading Level " + levelSet);
    }


    //https://stackoverflow.com/questions/32680348/unity3d-slider-onvaluechanged-sending-only-0-or-other-defined-value accessed 3/8/23
    //public void NumberPlayers(int players)
    public void NumberPlayers(Slider players)
    {
        //playersSet = players;
        playersSet = (int)players.value;

        //Debug.Log(players + " Player(s)");
        Debug.Log(playersSet + " Player(s)");
    }

    public void StartGame()
    {
        Debug.Log("Starting Level " + levelSet + " with " + playersSet + " players");
    }
}
