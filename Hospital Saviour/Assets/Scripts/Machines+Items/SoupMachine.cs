using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoupMachine : MonoBehaviour
{
    [SerializeField] GameObject soupPrefab;

    public bool isInteractable = true;
    [SerializeField]
    Material inactiveObjectMaterial;

    public GameObject currentSoup { get; private set; } = null;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(generateSoup());
    }

    /// <summary>
    /// Called when player picks up the soup
    /// </summary>
    public void soupPickUp()
    {
        currentSoup = null;
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
                                       0.275f);
        currentSoup = Instantiate(soupPrefab, transform);
        currentSoup.transform.localPosition = spawnLoc;
    }

    public void disableSelf()
    {
        //disable the interactable variable
        isInteractable = false;

        changeMaterial(transform);
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
