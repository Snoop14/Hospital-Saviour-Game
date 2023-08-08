using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;


public class GameManager : MonoBehaviour
{
    [SerializeField] NavMeshSurface surface;

    [Header("Prefabs")]
    [SerializeField]
    GameObject bedPrefab;

    [Serializable]
    public struct PatientsPrefab
    {
        public GameObject patientPrefab;
    }

    public PatientsPrefab[] patientPrefabs;
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
    public int activeBedCount;
    public int inActiveBedCount;
    // bedSeperation 3 looks correct
    [Range(0, 5)]
    public float bedSeperation = 3;

    [Header("Patient Controls")]
    //temperary for testing
    public int patientCount;
    [Range(0, 10)]
    public float patientSeperation = 5;

    List<GameObject> patientQueue;
    List<Vector3> queuePositions;
    [SerializeField]
    Transform EnterTransform;
    [SerializeField]
    Transform ExitTransform;

    public List<GameObject> objectList;

    [Header("Level Controls")]
    public int scoreAim = 250;
    private float currScore;

    private GameObject HUD;
    private GameObject displayScore;
    [SerializeField]
    Levels currentLevel;

    //bools to hold interactable status of machines
    private bool soupMachine;
    private bool pharmacy;
    private bool bandageDispenser;

    private void Awake()
    {
        GetLevelData();

        //might need to change this to different function later when we setup levels for resetting
        patientQueue = new List<GameObject>();
        queuePositions = new List<Vector3>();
        objectList = new List<GameObject>();
    }

    /// <summary>
    /// Gets the data of the current level through playerPrefs and finding assets
    /// </summary>
    private void GetLevelData()
    {
        int levelNum = PlayerPrefs.GetInt("LevelNum");
        string levelName = "Level" + levelNum + "Data";
        //https://docs.unity3d.com/ScriptReference/AssetDatabase.FindAssets.html
        string[] guids = AssetDatabase.FindAssets(levelName);
        string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
        //https://docs.unity3d.com/ScriptReference/AssetDatabase.LoadAssetAtPath.html
        currentLevel = (Levels)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Levels));
    }

    // Start is called before the first frame update
    void Start()
    {
        /**
         * for now call generateObjects in start() function, later we need to change this to be 
         * called when user clicks on a level, then pass the level data into 
         * generateObjects function as a parameter
        **/
        InitializeLevelData();
        
        generateObjects();
        GenerateHUD();
    }

    /// <summary>
    /// Initalize the data from the current levels data into varaiables
    /// </summary>
    private void InitializeLevelData()
    {
        patientCount = currentLevel.patientCount;
        activeBedCount = currentLevel.activeBedCount;
        inActiveBedCount = currentLevel.inActiveBedCount;
        //May need to initialize more here later


        //bools to hold interactable status of machines
        soupMachine = currentLevel.soupMachine;
        pharmacy = currentLevel.pharmacy;
        bandageDispenser = currentLevel.bandageDispenser;
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

        surface.BuildNavMesh();

        //Instatiate Patients and folders
        StartCoroutine(CreatePatient());

        //for each interactable object, check if it's interactable and add to lit if it is, and
        //if not, make if not interactable and change material
        if (soupMachine)
        {
            GameObject tempObj = GameObject.Find("SoupMachine");
            objectList.Add(tempObj);
        }
        else
        {
            GameObject tempObj = GameObject.Find("SoupMachine");
            tempObj.GetComponent<SoupMachine>().disableSelf();
            //Debug.Log("soup" + tempObj.GetComponent<SoupMachine>().isInteractable);
        }

        
        if (pharmacy)
        {
            GameObject tempObj = GameObject.Find("PillDispenser");
            objectList.Add(tempObj);
        }
        else
        {
            GameObject tempObj = GameObject.Find("PillDispenser");
            tempObj.GetComponent<PillMachine>().disableSelf();
            //Debug.Log("pharmacy" + tempObj.GetComponent<PillMachine>().isInteractable);

        }

        if (bandageDispenser)
        {
            GameObject tempObj = GameObject.Find("Medkit");
            objectList.Add(tempObj);
        }
        else
        {
            GameObject tempObj = GameObject.Find("Medkit");
            tempObj.GetComponent<BandageMachine>().disableSelf();
            //Debug.Log("bandage dispenser" + tempObj.GetComponent<BandageMachine>().isInteractable);

        }

    }

    IEnumerator CreatePatient()
    {
        
        for (int i = 0; i < patientCount; i++)
        {
            yield return new WaitForSeconds(currentLevel.spawnTimes[i]);
            int prefabType = Random.Range(0, patientPrefabs.Length); 
            GameObject newPatient = Instantiate(patientPrefabs[prefabType].patientPrefab, patientParent, false);
            newPatient.transform.position = EnterTransform.position;
            newPatient.transform.rotation = EnterTransform.rotation;

            GameObject newFolder = Instantiate(folderPrefab, newPatient.transform, false);
            Folder f = newFolder.GetComponent<Folder>();
            f.changePosToPlayer();
            f.patientOwner = newPatient;

            //Place patient into object list
            objectList.Add(newPatient);
            Patient p = newPatient.GetComponent<Patient>();
            p.folder = newFolder;

            //get random value from sickness list for level
            //https://docs.unity3d.com/ScriptReference/Random.Range.html accessed 7/8/23
            int sickInt = Random.Range(0, currentLevel.sicknessType.Count);
            //p.sickness = currentLevel.sicknessType[i];
            p.sickness = currentLevel.sicknessType[sickInt];
            p.ExitTransform = ExitTransform;

            patientQueue.Add(newPatient);
            queuePositions.Add(EnterTransform.position + new Vector3(patientSeperation * (patientCount - i + 1), 0, 0));
            p.queuePosition = queuePositions[patientQueue.Count - 1];
        }
    }

    public void removeFromQueue(GameObject p)
    {
        patientQueue.Remove(p);
        p.GetComponent<Patient>().isInQueue = false;
        for(int i = 0; i < patientQueue.Count; i++)
        {
            patientQueue[i].GetComponent<Patient>().moveInQueue(queuePositions[i]);
        }
    }

    private void GenerateHUD()
    {
        HUD = GameObject.Find("HUDCanvas");
        displayScore = HUD.transform.Find("DisplayScore").gameObject;
        UpdateScore(0);
        HUD.transform.Find("DisplayScore").GetComponent<Text>().text = currScore.ToString();
    }

    public void UpdateScore(float score)
    {
        currScore += score;
        int displayVal = (int)currScore;
        displayScore.GetComponent<Text>().text = displayVal.ToString();
    }

    public void EndGame()
    {
        Debug.Log("End game now");
    }
}
