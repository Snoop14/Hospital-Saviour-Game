using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] NavMeshSurface surface;

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

        GameObject tempObj = GameObject.Find("SoupMachine");
        objectList.Add(tempObj);

        tempObj = GameObject.Find("PillDispenser");
        objectList.Add(tempObj);

        tempObj = GameObject.Find("Medkit");
        objectList.Add(tempObj);
    }

    IEnumerator CreatePatient()
    {
        for (int i = 0; i < patientCount; i++)
        {
            yield return new WaitForSeconds(currentLevel.spawnTimes[i]);
            GameObject newPatient = Instantiate(patientPrefab, patientParent, false);
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
            p.sickness = currentLevel.sicknessType[i];
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
