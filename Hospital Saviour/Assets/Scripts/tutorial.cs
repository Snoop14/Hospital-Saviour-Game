using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tutorial : MonoBehaviour
{
    List<string> steps;
    List<Text> texts;
    [SerializeField]
    GameObject textPrefab;
    int currentStep = 0;

    public List<GameObject> patients = null;
    public GameObject bed1 = null;
    public GameObject bed2 = null;
    public GameObject soupMachine = null;
    public GameObject bin = null;

    public GameObject goals;

    [SerializeField, Range(0.0005f, 0.05f)]
    float fontScale = 0.01f;

    float prevHeight = 0;
    float prevWidth = 0;

    int patientsCured = 0;

    Levels level;

    //goal variables
    private int goalPatients = 0;

    Dictionary<GameObject, GameObject> arrows = new Dictionary<GameObject, GameObject>();
    [SerializeField]
    GameObject arrowPrefab;
    public GameObject iconCanvas;
    public List<GameObject> objectList;


    public void setupTutorial(Levels levelData)
    {
        level = levelData;
        texts = new List<Text>();
        Text header = transform.GetChild(0).GetComponent<Text>();
        header.text = levelData.levelName;
        texts.Add(header);

        steps = levelData.tutorialSteps;
        for(int i = 0; i < steps.Count; i++)
        {
            GameObject textObject = Instantiate(textPrefab, transform);
            texts.Add(textObject.GetComponent<Text>());
            textObject.SetActive(false);
            texts[i + 1].GetComponent<RectTransform>().anchorMax = new Vector2(0.9f, 0.8f - (0.1f * i));
            texts[i + 1].GetComponent<RectTransform>().anchorMin = new Vector2(0.1f, 0.7f - (0.1f * i));
            texts[i + 1].text = steps[i];
        }
        InvokeRepeating("FixTextSize",0,0.5f);

        if (levelData.patientsToBeTreated > 0)
        {
            goalPatients = levelData.patientsToBeTreated;
        }
        startGoals();

    }

    void FixTextSize()
    {
        if (prevHeight != Screen.height || prevWidth != Screen.width)
        {
            int s = Mathf.Min(Screen.height, Screen.width);
            foreach (Text t in texts)
            {
                t.fontSize = (int)(s * fontScale);
            }
            texts[0].fontSize = (int)(s * fontScale * 1.7f);
            prevHeight = Screen.height;
            prevWidth = Screen.width;
        }
    }

    public void startGoals()
    {
        if (goalPatients > 0)
        {
            goals.GetComponent<Text>().text = patientsCured.ToString() + "/" + goalPatients.ToString() + " Patients Cured";
        }
    }

    public void updateGoal()
    {
        patientsCured += 1;
        if (goalPatients > 0)
        {
            goals.GetComponent<Text>().text = patientsCured.ToString() + "/" + goalPatients.ToString() + " Patients Cured";
        }
    }

    void setupArrows()
    {
        foreach (var o in objectList)
        {
            GameObject a = Instantiate(arrowPrefab,iconCanvas.transform, true);
            a.SetActive(false);
            a.GetComponent<Arrow>().assignObject(o.transform);
            a.GetComponent<Image>().SetNativeSize();
            a.transform.localScale = new Vector3(0.15f, 0.1f, 1);
            arrows.Add(o,a);
        }
    }

    void resetArrows()
    {
        foreach(var k_v in arrows)
        {
            k_v.Value.SetActive(false);
        }
    }
    void displayArrows<T>() where T : Component
    {
        foreach (var k_v in arrows)
        {
            if (k_v.Key.GetComponent<T>())
            {
                k_v.Value.SetActive(true);
            }
        }
    }

    public void PatientsAdded()
    {
        if (level.levelName == "Level 1")
        {
            if (patients.Count == level.patientCount && currentStep == 0)
            {
                changeStep(1);
                setupArrows();
                displayArrows<Patient>();

                //turn on spotlight
                foreach (GameObject patient in patients)
                {
                    patient.GetComponent<Patient>().ActivateLight();
                }
            }
        }
    }
    public void interactedPatient()
    {
        if (level.levelName == "Level 1")
        {
            if (currentStep == 1)
            {
                changeStep(1);
                resetArrows();
                displayArrows<Patient>();
                displayArrows<Bed>();
            }
            else if (currentStep == 2)
            {
                changeStep(-1);
                resetArrows();
                displayArrows<Patient>();
            }
        }
    }
    public void interactedBedFolder()
    {
        if (level.levelName == "Level 1")
        {
            if (currentStep == 2)
            {
                changeStep(1);
                resetArrows();
            }

        }
    }
    public void PatientInteractWithBed()
    {
        if (level.levelName == "Level 1")
        {
            if (currentStep == 3)
            {
                changeStep(1);
                displayArrows<Bed>();
            }
        }
    }
    public void interactedBed()
    {
        if (level.levelName == "Level 1")
        {
            if (currentStep == 4)
            {
                changeStep(1);
                resetArrows();
                displayArrows<SoupMachine>();
            }
            else if (currentStep == 6) 
            { 
                changeStep(1);
                resetArrows();
            }
        }
        else if (level.levelName == "Level 2")
        {
            if (currentStep == 0)
            {
                changeStep(1);
            }
        }
    }
    public void interactedBin()
    {
        if (level.levelName == "Level 1")
        {
            if (currentStep == 6)
            {
                changeStep(-1);
                resetArrows();
                displayArrows<SoupMachine>();
            }
        }
        if (level.levelName == "Level 2")
        {
            if (currentStep == 2)
            {
                changeStep(-1);
            }
        }
    }
    public void interactedPill()
    {
        if (level.levelName == "Level 2")
        {
            if (currentStep == 1)
            {
                changeStep(1);
            }
        }
    }
    public void interactedSoup()
    {
        if (level.levelName == "Level 1")
        {
            if (currentStep == 5)
            {
                changeStep(1);
                resetArrows();
                displayArrows<Bed>();
                displayArrows<Bin>();
            }
        }
    }
    public void changeStep(int s)
    {
        currentStep += s;
        for (int i = 1; i < texts.Count; i++)
        {
            texts[i].gameObject.SetActive(false);
            texts[i].GetComponent<Text>().fontStyle = FontStyle.Normal;
        }
        for (int i = 1; i <= currentStep; i++)
        {
            if ( i == currentStep && currentStep == texts.Count)
            {
                break;
            }
            texts[i].gameObject.SetActive(true);
            if (i == currentStep)
            {
                texts[i].GetComponent<Text>().fontStyle = FontStyle.Bold;
            }
        }
    }
}
