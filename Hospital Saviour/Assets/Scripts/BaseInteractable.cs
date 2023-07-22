using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseInteractable : MonoBehaviour
{
    public GameObject player { get; private set; }

    public virtual void MainInteract(GameObject playerObject)
    {
        //this needs to be change based on the type of interactable object
    }

    public void setPlayer(GameObject _player)
    {
        player = _player;
    }
}
