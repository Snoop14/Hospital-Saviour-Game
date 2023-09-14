using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweezerMachine : MonoBehaviour
{
    //variable to hold if it is interactable ot not
    public bool isInteractable = true;

    //HOlder for the inactive material 
    [SerializeField]
    Material inactiveObjectMaterial;

    //Holder for the object when covered and inactive
    [SerializeField]
    GameObject coveredObject;

    public void disableSelf()
    {
        //disable the interactable variable
        isInteractable = false;

        coverObject();

        //change material to inactive
        //changeMaterial(transform);
    }

    /// <summary>
    /// Changes the object to a covered object 
    /// when this object is not active
    /// </summary>
    private void coverObject()
    {
        Vector3 spawnLoc = new Vector3(transform.position.x - 0.18f,
                                       transform.position.y - 0.7f,
                                       transform.position.z + 0.01f);
        Quaternion spawnRot = new Quaternion();
        spawnRot.eulerAngles = new Vector3(-90, 0, 180);
        Instantiate(coveredObject, spawnLoc, spawnRot);
        gameObject.SetActive(false);
    }

    //changes material of child and children into inactive material
    private void changeMaterial(Transform objectToChange)
    {
        //https://gamedev.stackexchange.com/questions/168803/looping-through-children-in-a-foreach-loop accessed 7/8/23
        //change material for all elements to InactiveMaterial
        foreach (Transform child in objectToChange.transform)
        {
            //https://gamedev.stackexchange.com/questions/84160/how-do-i-change-the-material-of-an-object-with-script-in-unity accessed 7/8/23
            //get the meshrendered of the child
            MeshRenderer my_renderer = child.GetComponent<MeshRenderer>();
            //if the child's meshrendered exists
            if (my_renderer != null)
            {
                //changeMaterial the material to the inactive material that is set
                my_renderer.material = inactiveObjectMaterial;
            }
            //otherwise
            else
            {
                //recursive call to the function on the child (to work on the childs children)
                changeMaterial(child);
            }
        }
    }
}
