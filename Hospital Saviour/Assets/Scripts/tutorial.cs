using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tutorial : MonoBehaviour
{
    List<string> steps;
    List<GameObject> texts;
    [SerializeField]
    GameObject textPrefab;
    int currentStep = 0;

    public List<GameObject> patients = null;
    public GameObject bed1 = null;
    public GameObject bed2 = null;
    public GameObject soupMachine = null;
    public GameObject bin = null;

    private void Start()
    {
        texts = new List<GameObject>();
        texts.Add(null);
    }
    public void setupTutorial(Levels levelData)
    {
        transform.GetChild(0).GetComponent<Text>().text = levelData.levelName;
        steps = levelData.tutorialSteps;
        for(int i = 0; i < steps.Count; i++)
        {
            texts.Add(Instantiate(textPrefab, transform));
            texts[i + 1].SetActive(false);
            texts[i + 1].GetComponent<RectTransform>().anchorMax = new Vector2(0.9f, 0.8f - (0.15f * i));
            texts[i + 1].GetComponent<RectTransform>().anchorMin = new Vector2(0.1f, 0.65f - (0.15f * i));
            texts[i + 1].GetComponent<Text>().text = steps[i];
        }
        
    }
    public void interactedPatient()
    {
        if (currentStep == 1)
            changeStep(1);
        else if (currentStep == 2)
            changeStep(-1);
    }
    public void interactedBed()
    {
        if (currentStep == 2)
            changeStep(1);
        else if (currentStep == 3)
            changeStep(1);
        else if (currentStep == 5)
            changeStep(1);
    }
    public void interactedBin()
    {
        if (currentStep == 5)
            changeStep(-1);
    }
    public void interactedSoup()
    {
        if (currentStep == 4)
            changeStep(1);
    }
    public void changeStep(int s)
    {
        currentStep += s;
        for (int i = 1; i < texts.Count; i++)
        {
            texts[i].SetActive(false);
            texts[i].GetComponent<Text>().fontStyle = FontStyle.Normal;
        }
        for (int i = 1; i <= currentStep; i++)
        {
            if ( i == currentStep && currentStep == texts.Count)
            {
                break;
            }
            texts[i].SetActive(true);
            if (i == currentStep)
            {
                texts[i].GetComponent<Text>().fontStyle = FontStyle.Bold;
            }
        }
    }
}
