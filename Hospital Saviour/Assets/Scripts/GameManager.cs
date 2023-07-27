using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField]
    GameObject bedPrefab;
    [SerializeField]
    GameObject patientPrefab;
    [SerializeField]
    GameObject folderPrefab;

    [SerializeField]
    Transform patientParent;
    [SerializeField]
    Transform inActiveBedParent;
    [SerializeField]
    Transform activeBedParent;

    [SerializeField]
    Material inActiveMat;

    //this later needs to be determined within the level data
    public int activeBedCount = 2;
    public int inActiveBedCount = 4;
    // bedSeperation 3 looks correct
    [Range(0, 5)]
    public float bedSeperation = 3;
    //temperary for testing
    public int patientCount = 4;
    [Range(0, 10)]
    public float patientSeperation = 5;



    public List<GameObject> objectList;
    private void Awake()
    {
        objectList = new List<GameObject>();
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

    void generateObjects()
    {
        //Instatiate Inactive beds
        for (int i = 0; i < inActiveBedCount; i++)
        {
            GameObject newBed = Instantiate(bedPrefab, inActiveBedParent, false);
            newBed.transform.position += new Vector3(0, 0, -bedSeperation * i);
            var bedRenderers = newBed.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer renderer in bedRenderers)
            {
                renderer.material = inActiveMat;
            }
            //Place bed into object list
            objectList.Add(newBed);

        }

        //Instatiate Active beds
        for (int i = 0; i < activeBedCount; i++)
        {
            float bedStartingPos = inActiveBedCount * -bedSeperation;
            GameObject newBed = Instantiate(bedPrefab, activeBedParent, false);
            newBed.transform.position += new Vector3(0, 0, bedStartingPos - (bedSeperation * i));
            //Place bed into object list
            objectList.Add(newBed);
            Bed b = newBed.GetComponent<Bed>();
            b.isActive = true;
            b.isInteractable = true;
        }

        //Instatiate Patients and folders
        for (int i = 0; i < patientCount; i++)
        {
            GameObject newPatient = Instantiate(patientPrefab, patientParent, false);
            newPatient.transform.position += new Vector3(0, 0, patientSeperation * i);

            GameObject newFolder = Instantiate(folderPrefab, newPatient.transform, false);
            newFolder.GetComponent<Folder>().changePosToPlayer();

            //Place patient into object list
            objectList.Add(newPatient);
            Patient p = newPatient.GetComponent<Patient>();
            p.isInteractable = true;
            p.isHoldingFolder = true;
            p.folder = newFolder;
        }
    }
}
