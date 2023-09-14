using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillMachine : MonoBehaviour
{
    //holder for the pill prefab
    [SerializeField] GameObject pillPrefab;

    //disable the interactable variable
    public bool isInteractable = true;

    //HOlder for the inactive material 
    [SerializeField]
    Material inactiveObjectMaterial;

    //Holder for the object when covered and inactive
    [SerializeField]
    GameObject coveredObject;

    //variable to hold current pill
    public GameObject currentPill { get; private set; } = null;

    // Start is called before the first frame update
    void Start()
    {
        //Runs generatePill across multiple frames
        StartCoroutine(generatePill());
    }

    /// <summary>
    /// Called when player picks up the pill
    /// </summary>
    public void pillPickUp()
    {
        currentPill = null;
        StartCoroutine(generatePill());
    }

    /// <summary>
    /// Regenerates pills after a certain amount of time
    /// </summary>
    IEnumerator generatePill()
    {
        yield return new WaitForSeconds(1.0f);
        Vector3 spawnLoc = new Vector3(0,
                                       1.8f,
                                       -0.55f);
        currentPill = Instantiate(pillPrefab, transform);
        currentPill.transform.localPosition = spawnLoc;
    }


    public void disableSelf()
    {
        //disable the interactable variable
        isInteractable = false;

        //loads the covered object
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
        Vector3 spawnLoc = new Vector3(transform.position.x - .1f,
                                       transform.position.y,
                                       transform.position.z + .4f);
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
