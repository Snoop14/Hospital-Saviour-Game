using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class State
{
    public bool interactable = false;
}
public class GameManager : MonoBehaviour
{

    [SerializeField]
    GameObject bedPrefab;
    [SerializeField]
    GameObject inActiveBedParent;
    [SerializeField]
    GameObject activeBedParent;

    [SerializeField]
    Material inActiveMat;

    //this later needs to be determined within the level data
    public int activeBedCount = 2;
    public int inActiveBedCount = 4;

    // bedSeperation 3 looks correct
    [Range(0, 5)]
    public float bedSeperation = 3;

    Dictionary<GameObject, State> objectStates;
    private void Awake()
    {
        objectStates = new Dictionary<GameObject, State>();
    }
    // Start is called before the first frame update
    void Start()
    {
        /**
         * for now call generateObjects in start() function, later we need to change this to be 
         * called when user clicks on a level, then pass the level data into 
         * generateObjects function as a parameter
        **/
        generateObjects();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void generateObjects()
    {
        //Instatiate inactive beds
        for (int i = 0; i < inActiveBedCount; i++)
        {
            GameObject newBed = Instantiate(bedPrefab, inActiveBedParent.transform, false);
            newBed.transform.position += new Vector3(0, 0, -bedSeperation * i);
            var bedRenderers = newBed.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer renderer in bedRenderers)
            {
                renderer.material = inActiveMat;
            }
            //Place bed into object list
            objectStates[newBed] = new State();

        }

        //Instatiate Active beds
        for (int i = inActiveBedCount; i < activeBedCount + inActiveBedCount; i++)
        {
            GameObject newBed = Instantiate(bedPrefab, inActiveBedParent.transform, false);
            newBed.transform.position += new Vector3(0, 0, -bedSeperation * i);
            //Place bed into object list
            objectStates[newBed] = new State();
        }
    }
}
