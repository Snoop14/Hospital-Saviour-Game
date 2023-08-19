﻿using System;
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

    GameObject player1;
    [SerializeField] GameObject playerPrefab;

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
    public int levelNo;

    private GameObject HUD;
    private GameObject displayScore;
    [SerializeField]
    Levels currentLevel;

    //bools to hold interactable status of machines
    private bool soupMachine;
    private bool pharmacy;
    private bool bandageDispenser;

    private bool ScalpelDispenser;
    private bool TweezerDispenser;
    private bool Surgery;
    private bool XRayMachine;
    private bool ECGMachine;

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
        //string[] guids = AssetDatabase.FindAssets(levelName);

        //https://docs.unity3d.com/2020.1/Documentation/ScriptReference/Resources.Load.html accessed 14/8/23
        var levelToLoad = Resources.Load<Levels>("Level Data/" + levelName);
        //Debug.Log(levelToLoad.activeBedCount);

        //string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
        //https://docs.unity3d.com/ScriptReference/AssetDatabase.LoadAssetAtPath.html
        //currentLevel = (Levels)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Levels));

        currentLevel = levelToLoad;
        levelNo = levelNum;
    }

    // Start is called before the first frame update
    void Start()
    {
        /**
         * for now call generateObjects in start() function, later we need to change this to be 
         * called when user clicks on a level, then pass the level data into 
         * generateObjects function as a parameter
        **/
        //This actually works fine.

        InitializeLevelData();
        
        generateObjects();
        GenerateHUD();
    }

    /// <summary>
    /// Initalize the data from the current levels data into variables
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
        ScalpelDispenser = currentLevel.ScalpelDispenser; 
        TweezerDispenser = currentLevel.TweezerDispenser;
        Surgery = currentLevel.Surgery;
        XRayMachine = currentLevel.XRayMachine;
        ECGMachine = currentLevel.ECGMachine;
    }

    void generateObjects()
    {
        player1 = Instantiate(playerPrefab);
        player1.GetComponent<Player1>().gameManager = gameObject;
        player1.transform.position += new Vector3(-7, 0, -2);

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
        
        if (ScalpelDispenser)
        {
            GameObject tempObj = GameObject.Find("ScalpelDispenser");
            objectList.Add(tempObj);
        }
        else
        {
            GameObject tempObj = GameObject.Find("ScalpelDispenser");
            tempObj.GetComponent<ScalpelMachine>().disableSelf();
        }

        if (TweezerDispenser)
        {
            GameObject tempObj = GameObject.Find("TweezerDispenser");
            objectList.Add(tempObj);
        }
        else
        {
            GameObject tempObj = GameObject.Find("TweezerDispenser");
            tempObj.GetComponent<TweezerMachine>().disableSelf();
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
        if (Surgery)
        {
            GameObject tempObj = GameObject.Find("Surgery");
            objectList.Add(tempObj);
        }
        else
        {
            GameObject tempObj = GameObject.Find("Surgery");
            tempObj.GetComponent<SurgeryMachine>().disableSelf();
        }

        if (XRayMachine)
        {
            GameObject tempObj = GameObject.Find("X-Ray");
            objectList.Add(tempObj);
        }
        else
        {
            GameObject tempObj = GameObject.Find("X-Ray");
            tempObj.GetComponent<XRayMachine>().disableSelf();
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

        if (ECGMachine)
        {
            GameObject tempObj = GameObject.Find("ECG");
            objectList.Add(tempObj);
        }
        else
        {
            GameObject tempObj = GameObject.Find("ECG");
            tempObj.GetComponent<ECGMachine>().disableSelf();
        }


        GameObject tempBin = GameObject.Find("Bin");
        objectList.Add(tempBin);
    }

//}

    IEnumerator CreatePatient()
    {
        
        for (int i = 0; i < patientCount; i++)
        {
            yield return new WaitForSeconds(currentLevel.spawnTimes[i]);
            int prefabType = Random.Range(0, patientPrefabs.Length); 
            GameObject newPatient = Instantiate(patientPrefabs[prefabType].patientPrefab, EnterTransform.position, EnterTransform.rotation, patientParent);
            //newPatient.transform.position = EnterTransform.position;
            //newPatient.transform.rotation = EnterTransform.rotation;

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
            queuePositions.Add(EnterTransform.position + new Vector3(18 - patientSeperation * i, 0, 0));
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

    /// <summary>
    /// Generates the HUD at start of game
    /// </summary>
    private void GenerateHUD()
    {
        HUD = GameObject.Find("HUDCanvas");
        displayScore = HUD.transform.Find("DisplayScore").gameObject;
        UpdateScore(0);
        displayScore.GetComponent<Text>().text = currScore.ToString();
    }

    /// <summary>
    /// Updates the score in displayed in the HUD
    /// </summary>
    /// <param name="score"></param>
    public void UpdateScore(float score)
    {
        currScore += (int)score;
        displayScore.GetComponent<Text>().text = currScore.ToString();
    }

    /// <summary>
    /// This function ends the gameplay and
    /// should then display the other end game info
    /// e.g. Score and target completion
    /// </summary>
    public void EndGame()
    {
        player1.GetComponent<Player1>().enabled = false;
        GameObject endDetails = HUD.transform.Find("EndDetails").gameObject;
        endDetails.transform.Find("ScoreText").GetComponent<Text>().text = "Score: " + currScore.ToString();
        endDetails.SetActive(true);
    }

    /// <summary>
    /// This function returns the users to the menu.
    /// It is called by a button when endDetails becomes active
    /// </summary>
    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
