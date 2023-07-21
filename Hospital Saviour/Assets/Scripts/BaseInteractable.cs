using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseInteractable : MonoBehaviour
{
    public GameObject player { get; private set; }

    public virtual void MainInteract(GameObject gameObject)
    {
        //this needs to be change based on the object
    }

    public void setPlayer(GameObject _player)
    {
        player = _player;
    }

    public bool CheckIfCarrying()
    {
        return false;
    }


}
