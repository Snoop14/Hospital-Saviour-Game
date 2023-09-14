using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;


public class GameManager : MonoBehaviour
{
    public bool playStartingAnimations = false;
    [SerializeField] NavMeshSurface surface;

    GameObject player1;
    GameObject player2;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject player2Prefab;

    [Header("Prefabs")]
    [SerializeField]
    GameObject bedPrefab;
    [SerializeField]
    GameObject coveredBedPrefab;

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
    [SerializeField]
    GameObject tutorialObject;
    [SerializeField]
    GameObject EndLevelButtonObject;

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
    List<GameObject> patients;
    List<Vector3> queuePositions;
    List<float> spawnTimes;
    [SerializeField]
    Transform EnterTransform;
    [SerializeField]
    Transform ExitTransform;

    public List<GameObject> objectList;

    [Header("Level Controls")]
    public int scoreAim = 250;
    private int currScore;
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

    private Animator animator;

    private int timeForLevel;
    private bool angryNotAllowed;

    [SerializeField] public CustomTimer inGameTimer;

    private void Awake()
    {
        GetLevelData();

        //might need to change this to different function later when we setup levels for resetting
        patientQueue = new List<GameObject>();
        queuePositions = new List<Vector3>();
        objectList = new List<GameObject>();
        patients = new List<GameObject>();
        spawnTimes = new List<float>();
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

        if (playStartingAnimations)
        {
            //trigger start animations
            animator = GameObject.Find("Scene").GetComponent<Animator>();
            animator.SetInteger("Level", levelNo);
        }


        if (timeForLevel > 0)
        {
            if(PlayerPrefs.GetInt("PlayerNum") == 2)
            {
                //Increase timer for 2 player mode
                timeForLevel += 30;
            }
            //Activate the timer related objects
            inGameTimer.gameObject.SetActive(true);
            inGameTimer.SetTimeFromSeconds(timeForLevel);
            inGameTimer.StartTimer();
            StartCoroutine(WaitForTimerRunOut());
        }

        //tutorialObject.GetComponent<tutorial>().objectList = objectList;
        tutorialObject.GetComponent<tutorial>().setupTutorial(currentLevel);

    }

    /// <summary>
    /// Initalize the data from the current levels data into variables
    /// </summary>
    private void InitializeLevelData()
    {
        patientCount = currentLevel.patientCount;
        if(PlayerPrefs.GetInt("PlayerNum") == 2)
        {
            patientCount += patientCount;
        }

        spawnTimes = currentLevel.spawnTimes;
        if (PlayerPrefs.GetInt("PlayerNum") == 2)
        {
            List<float> tempList = new List<float>();
            tempList.Add(spawnTimes[0]);
            for(int i = 1; i < spawnTimes.Count; i++)
            {
                float temp = (spawnTimes[i] + spawnTimes[i - 1]) / 2;
                tempList.Add(temp);
                tempList.Add(spawnTimes[i]);
            }
            tempList.Add(spawnTimes[spawnTimes.Count - 1] + 1f);
            spawnTimes = tempList;
        }

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

        timeForLevel = currentLevel.timer;

        angryNotAllowed = currentLevel.angryNotAllowed;
    }

    /// <summary>
    /// Generate the active and inactive objects for the current level
    /// based on the data collected from InitializeLevelData()
    /// </summary>
    void generateObjects()
    {
        player1 = Instantiate(playerPrefab, GameObject.Find("Player1SpawnSite").transform);
        player1.GetComponent<Player>().gameManager = gameObject;
        player1.GetComponent<Player>().tutorial = tutorialObject.GetComponent<tutorial>();
        //player1.transform.position += new Vector3(-7, 0, -2);
        if(PlayerPrefs.GetInt("PlayerNum") == 2)
        {
            player2 = Instantiate(player2Prefab, GameObject.Find("Player2SpawnSite").transform);
            player2.GetComponent<Player>().gameManager = gameObject;
            player2.GetComponent<Player>().tutorial = tutorialObject.GetComponent<tutorial>();
        }
        else
        {
            GameObject.Find("PlayerIcon").SetActive(false);
        }

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
            //objectList.Add(newBed);

        }

        //inActiveBedParent.localPosition = new Vector3(1.8f, -0.5f, 0.3f);

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

        //Build nav mesh
        surface.BuildNavMesh();

        //Instatiate Patients and folders
        StartCoroutine(CreatePatient());

        //for each interactable object, check if it's interactable and add to list if it is, and
        //if not, make if not interactable and disable it
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

    /// <summary>
    /// Spawn the patients in for the level
    /// </summary>
    /// <returns></returns>
    IEnumerator CreatePatient()
    {
        for (int i = 0; i < patientCount; i++)
        {
            yield return new WaitForSeconds(spawnTimes[i]);
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
            queuePositions.Add(EnterTransform.position + new Vector3(25 - patientSeperation * i, 0, 0));
            p.queuePosition = queuePositions[patientQueue.Count - 1];
            p.tutorial = tutorialObject.GetComponent<tutorial>();
            patients.Add(newPatient);
        }
    }

    /// <summary>
    /// Removes the specific patient from the list of patients
    /// </summary>
    /// <param name="_patient"></param>
    public void RemovePatientFromList(GameObject _patient)
    {
        patients.Remove(_patient);
    }

    /// <summary>
    /// Removes the patient from the initial queue of patients
    /// </summary>
    /// <param name="p"></param>
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
    public void EndGame(bool angryPatient = false, bool patientGoalMet = true)
    {
        
        player1.GetComponent<Player>().speed = 0;
        
        if(PlayerPrefs.GetInt("PlayerNum") == 2)
        {
            player2.GetComponent<Player>().speed = 0;
        }
        

        GameObject endDetails = HUD.transform.Find("EndDetails").gameObject;
        EndLevelButtonObject.SetActive(false);
        endDetails.transform.Find("ScoreText").GetComponent<TMP_Text>().text = "Score: " + currScore.ToString();
        //disaply failed to complete if an angry patient in relevant level
        if (angryPatient)
        {
            endDetails.transform.Find("ErrorText").GetComponent<TMP_Text>().text = "Sorry, you had an angry patient, you did not complete this level, try again";
        }

        //disaply failed to complete if an not enough patients treated in relevant level
        if (!patientGoalMet)
        {
            endDetails.transform.Find("ErrorText").GetComponent<TMP_Text>().text = "Sorry, you didn't treat enough patients, you did not complete this level, try again";
        }

        endDetails.SetActive(true);

        //Update current high score for level
        string levelName = currentLevel.levelName;
        if(PlayerPrefs.GetInt("PlayerNum") == 1)
        {
            levelName += "_1p";
        }
        else if(PlayerPrefs.GetInt("PlayerNum") == 2)
        {
            levelName += "_2p";
        }

        if (PlayerPrefs.GetInt(levelName) < currScore)
        {
            PlayerPrefs.SetInt(levelName, currScore);
        }


        if (PlayerPrefs.GetInt("PlayerNum") == 1 && PlayerPrefs.GetInt("Highest_Level_Complete_1p") < levelNo)
        {
            //only move on max level if successfully completed with no angry patients
            if (!angryPatient)
            {
                PlayerPrefs.SetInt("Highest_Level_Complete_1p", levelNo);
            }
        }
        else if (PlayerPrefs.GetInt("PlayerNum") == 2 && PlayerPrefs.GetInt("Highest_Level_Complete_2p") < levelNo)
        {
            //only move on max level if successfully completed with no angry patients and  patient goal met
            if (!angryPatient && patientGoalMet)
            {
                PlayerPrefs.SetInt("Highest_Level_Complete_2p", levelNo);
            }

        }
    }


    /// <summary>
    /// This function returns the users to the menu.
    /// It is called by a button when endDetails becomes active
    /// </summary>
    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    /// <summary>
    /// If the patient gets mad, check to end the level early
    /// </summary>
    public void MadPatient()
    {
        //int levelNum = PlayerPrefs.GetInt("LevelNum");
        if (angryNotAllowed)
        {
            StopGameplay();
            EndGame(true);
        }
    }

    /// <summary>
    /// Disable the patients that are still on screen
    /// </summary>
    private void StopGameplay()
    {

        for(int i = 0; i < patients.Count; i++)
        {
            try
            {
                patients[i].GetComponent<NavMeshAgent>().enabled = false;
                patients[i].GetComponent<Patient>().CancelInvoke();
                patients[i].GetComponent<Patient>().enabled = false;
            }
            catch(MissingReferenceException e)
            {
                throw e;
            }
        }
    }
    
    IEnumerator WaitForTimerRunOut()
    {
        yield return new WaitForSeconds(timeForLevel);
        StopGameplay();
        EndGame(false, tutorialObject.GetComponent<tutorial>().checkPatientGoal());
    }
}
