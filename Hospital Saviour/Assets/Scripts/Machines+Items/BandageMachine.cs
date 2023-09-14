using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BandageMachine : MonoBehaviour
{
    //holder for the bandage
    [SerializeField] GameObject bandagePrefab;

    //variable to hold if it is interactable ot not
    public bool isInteractable = true;

    //HOlder for the inactive material 
    [SerializeField]
    Material inactiveObjectMaterial;

    //Holder for the object when covered and inactive
    [SerializeField]
    GameObject coveredObject;

    //holder for current bandage
    public GameObject currentBandage { get; private set; } = null;


    // Start is called before the first frame update
    void Start()
    {
        //runs generateBandage over multiple frames
        StartCoroutine(generateBandage());
    }

    /// <summary>
    /// Called when player picks up the bandage
    /// </summary>
    public void bandagePickUp()
    {
        //resets currentBandage
        currentBandage = null;

        //runs generateBandage over multiple frames
        StartCoroutine(generateBandage());
    }

    /// <summary>
    /// Regenerates bandages after a certain amount of time
    /// </summary>
    IEnumerator generateBandage()
    {
        yield return new WaitForSeconds(1.0f);
        Vector3 spawnLoc = new Vector3(-0.28f,
                                       1.94f,
                                       0.28f);
        Quaternion spawnRot = new Quaternion();
        spawnRot.eulerAngles = new Vector3(0, 0, 90);
        currentBandage = Instantiate(bandagePrefab, spawnLoc, spawnRot, transform);
        currentBandage.transform.localPosition = spawnLoc;
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

    private void coverObject()
    {
        Vector3 spawnLoc = new Vector3(transform.position.x - .19f,
                                       transform.position.y,
                                       transform.position.z);
        Quaternion spawnRot = new Quaternion();
        spawnRot.eulerAngles = new Vector3(-90, 0, 180);
        Instantiate(coveredObject, spawnLoc, spawnRot);
        gameObject.SetActive(false);
    }

    private void changeMaterial(Transform objectToChange)
    {
        //https://gamedev.stackexchange.com/questions/168803/looping-through-children-in-a-foreach-loop accessed 7/8/23
        //change material for all elements to InactiveMaterial
        foreach (Transform child in objectToChange.transform)
        {
            //https://gamedev.stackexchange.com/questions/84160/how-do-i-change-the-material-of-an-object-with-script-in-unity accessed 7/8/23
            //get the meshrenderer of the child
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
