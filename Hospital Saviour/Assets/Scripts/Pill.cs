using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pill : MonoBehaviour
{
    public void transferTo(GameObject obj)
    {
        transform.parent = obj.transform; //changes the parent of folder to the transfered object
    }
    public void changePosToPlayer()
    {
        transform.localPosition = new Vector3(0f, 0.5f, 0.85f);
        transform.localRotation = new Quaternion(0f, 0f, 0f, 0f); //resets rotation
    }

    public void changePosToBed()
    {
        transform.localPosition = new Vector3(0f, 1.75f, 0f); //Values will need to be changed
        transform.localRotation = new Quaternion(0f, 0f, 0f, 0f); //resets rotation
    }

    public IEnumerator destroySelf()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }
}
