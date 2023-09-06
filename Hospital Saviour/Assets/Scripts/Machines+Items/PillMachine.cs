using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillMachine : MonoBehaviour
{
    [SerializeField] GameObject pillPrefab;

    public bool isInteractable = true;
    [SerializeField]
    Material inactiveObjectMaterial;
    [SerializeField]
    GameObject coveredObject;

    public GameObject currentPill { get; private set; } = null;

    // Start is called before the first frame update
    void Start()
    {
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
                                       1.2f,
                                       -0.95f);
        currentPill = Instantiate(pillPrefab, transform);
        currentPill.transform.localPosition = spawnLoc;
    }

    public void disableSelf()
    {
        //disable the interactable variable
        isInteractable = false;

        coverObject();
        //changeMaterial(transform);
    }

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
