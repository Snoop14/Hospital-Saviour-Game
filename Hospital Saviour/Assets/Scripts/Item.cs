using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Soup,
    Pill,
    Bandage,
    Folder,
    Empty
}
public class Item : MonoBehaviour
{
    public ItemType type;
    /// <summary>
    /// Transfers the object to be a child of another
    /// </summary>
    /// <param name="obj"></param>
    public void transferTo(GameObject obj)
    {
        if (obj.name.Contains("Patient"))
        {
            transform.parent = obj.transform.GetChild(0);
        }
        else
        {
            transform.parent = obj.transform; //changes the parent of folder to the transferred object
        }
    }

    /// <summary>
    /// Changes position of pill to the players hands
    /// </summary>
    public void changePosToPlayer()
    {
        transform.localPosition = new Vector3(0f, 0.5f, 0.85f);
        transform.localRotation = new Quaternion(0f, 0f, 0f, 0f); //resets rotation
    }

    /// <summary>
    /// Changes position of self to above patient on bed 
    /// </summary>
    public void changePosToBed()
    {
        transform.localPosition = new Vector3(0f, 0.1f, 0.85f); //Values will need to be changed
        transform.localRotation = new Quaternion(0f, 0f, 0f, 0f); //resets rotation
    }

    /// <summary>
    /// Destroy self after some time
    /// </summary>
    public IEnumerator destroySelf()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }
}
