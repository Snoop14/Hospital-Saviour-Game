using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField]
    GameObject bedPrefab;
    [SerializeField]
    GameObject patientPrefab;
    [SerializeField]
    GameObject folderPrefab;

    [Header("Parents")]
    [SerializeField]
    Transform patientParent;
    [SerializeField]
    Transform inActiveBedParent;
    [SerializeField]
    Transform activeBedParent;

    [Header("")]
    [SerializeField]
    Material inActiveMat;

    [Header("Bed Controls")]
    //this later needs to be determined within the level data
    public int activeBedCount = 2;
    public int inActiveBedCount = 4;
    // bedSeperation 3 looks correct
    [Range(0, 5)]
    public float bedSeperation = 3;

    [Header("Patient Controls")]
    //temperary for testing
    public int patientCount = 4;
    [Range(0, 10)]
    public float patientSeperation = 5;


    List<GameObject> patientQueue;
    List<Vector3> queuePositions;
    [SerializeField]
    Vector3 initialPosition = new Vector3(-12, 0, -5);

    public List<Sickness> sicknessList;

    public List<GameObject> objectList;
    private void Awake()
    {
        //might need to change this to different function later when we setup levels for resetting
        patientQueue = new List<GameObject>();
        queuePositions = new List<Vector3>();
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
        assignPatientPositions();
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
            Folder f = newFolder.GetComponent<Folder>();
            f.changePosToPlayer();
            f.patientOwner = newPatient;

            //Place patient into object list
            objectList.Add(newPatient);
            Patient p = newPatient.GetComponent<Patient>();
            p.isInteractable = true;
            p.isHoldingFolder = true;
            p.folder = newFolder;
            p.sickness = sicknessList[0];

            patientQueue.Add(newPatient);
        }

        GameObject tempObj = GameObject.Find("SoupMachine");

        objectList.Add(tempObj);
    }

    void assignPatientPositions()
    {
        for (int i = 0; i < patientCount; i++)
        {
            queuePositions.Add(initialPosition + new Vector3(0,0,patientSeperation*i));
            patientQueue[i].GetComponent<Patient>().queuePosition = queuePositions[i];
        }
    }

    public void removeFromQueue(GameObject p)
    {
        patientQueue.Remove(p);
        for(int i = 0; i < patientQueue.Count; i++)
        {
            patientQueue[i].GetComponent<Patient>().moveInQueue(queuePositions[i]);
        }
    }
}
