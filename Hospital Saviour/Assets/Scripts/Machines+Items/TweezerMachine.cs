using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweezerMachine : MonoBehaviour
{
    //[SerializeField] GameObject bandagePrefab;

    public bool isInteractable = true;
    [SerializeField]
    Material inactiveObjectMaterial;
    [SerializeField]
    GameObject coveredObject;


    //public GameObject currentBandage { get; private set; } = null;


    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(generateBandage());
    }

    /// <summary>
    /// Called when player picks up the bandage
    /// </summary>
    //public void bandagePickUp()
    //{
    //  currentBandage = null;
    //StartCoroutine(generateBandage());
    //}

    /// <summary>
    /// Regenerates bandages after a certain amount of time
    /// </summary>
    //IEnumerator generateBandage()
    //{
    //  yield return new WaitForSeconds(1.0f);
    //Vector3 spawnLoc = new Vector3(transform.localPosition.x,
    //                             transform.localPosition.y + 0.95f,
    //                           transform.localPosition.z);
    //Quaternion spawnRot = new Quaternion();
    //spawnRot.eulerAngles = new Vector3(90, 0, 0);
    //currentBandage = Instantiate(bandagePrefab, spawnLoc, spawnRot, transform);
    //}

    public void disableSelf()
    {
        //disable the interactable variable
        isInteractable = false;

        coverObject();
        //changeMaterial(transform);
    }

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

    private void changeMaterial(Transform objectToChange)
    {
        //https://gamedev.stackexchange.com/questions/168803/looping-through-children-in-a-foreach-loop accessed 7/8/23
        //change material for all elements to InactiveMaterial
        foreach (Transform child in objectToChange.transform)
        {
            //https://gamedev.stackexchange.com/questions/84160/how-do-i-change-the-material-of-an-object-with-script-in-unity accessed 7/8/23
            MeshRenderer my_renderer = child.GetComponent<MeshRenderer>();
            if (my_renderer != null)
            {
                my_renderer.material = inactiveObjectMaterial;
            }
            else
            {
                changeMaterial(child);
            }
        }
    }
}
