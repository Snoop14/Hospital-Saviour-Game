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

    //goal variables
    private int goalPatients = 0;

    public void setupTutorial(Levels levelData)
    {
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
        
    }

    void FixTextSize()
    {
        if (prevHeight != Screen.height || prevWidth != Screen.width)
        {
            int s = Mathf.Min(Screen.height, Screen.width);
            print(texts[0].fontSize);
            foreach (Text t in texts)
            {
                t.fontSize = (int)(s * fontScale);
                print(t.fontSize);
        }
            texts[0].fontSize = (int)(s * fontScale * 1.7f);
            prevHeight = Screen.height;
            prevWidth = Screen.width;
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

    public void interactedPatient()
    {
        if (currentStep == 1)
            changeStep(1);
        else if (currentStep == 2)
            changeStep(-1);
    }
    public void interactedBedFolder()
    {
        if (currentStep == 2)
            changeStep(1);
    }
    public void interactedBed()
    {
        if (currentStep == 4)
            changeStep(1);
        else if (currentStep == 6)
            changeStep(1);
    }
    public void interactedBin()
    {
        if (currentStep == 6)
            changeStep(-1);
    }
    public void interactedSoup()
    {
        if (currentStep == 5)
            changeStep(1);
    }
    public void PatientInteractWithBed()
    {
        if (currentStep == 3)
            changeStep(1);
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
