using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoupMachine : MonoBehaviour
{
    //holder for the soup
    [SerializeField] GameObject soupPrefab;

    //variable to hold if it is interactable ot not
    public bool isInteractable = true;

    //HOlder for the inactive material 
    [SerializeField]
    Material inactiveObjectMaterial;

    //holder for current soup
    public GameObject currentSoup { get; private set; } = null;

    // Start is called before the first frame update
    void Start()
    {
        //runs generateSoup over multiple frames
        StartCoroutine(generateSoup());
    }

    /// <summary>
    /// Called when player picks up the soup
    /// </summary>
    public void soupPickUp()
    {
        //resets current soup
        currentSoup = null;
        //runs generateSoup over multiple frames
        StartCoroutine(generateSoup());
    }

    /// <summary>
    /// Regenerates soup after a certain amount of time
    /// </summary>
    IEnumerator generateSoup()
    {
        yield return new WaitForSeconds(1.0f);
        Vector3 spawnLoc = new Vector3(0, 
                                       1.125f, 
                                       0.3f);
        currentSoup = Instantiate(soupPrefab, transform);
        currentSoup.transform.localPosition = spawnLoc;
    }

    public void disableSelf()
    {
        //disable the interactable variable
        isInteractable = false;

        //change material to inactive
        changeMaterial(transform);
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
