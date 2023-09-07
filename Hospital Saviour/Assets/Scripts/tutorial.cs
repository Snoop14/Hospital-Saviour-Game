using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

enum messageType
{
    Image, Text
}
class TutorialMessage
{
    public messageType type;
    public Transform obj;
}
public class tutorial : MonoBehaviour
{
    List<TutorialMessage> steps;
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
    private bool angryNotAllowed;

    Dictionary<GameObject, GameObject> arrows = new Dictionary<GameObject, GameObject>();
    [SerializeField]
    GameObject arrowPrefab;
    public GameObject iconCanvas;
    public List<GameObject> objectList;
    public Transform headerobj;

    public void setupTutorial(Levels levelData)
    {
        level = levelData;

        TMP_Text header = headerobj.GetComponent<TMP_Text>();
        header.text = level.levelName + " Tutorials";

        steps = new List<TutorialMessage>();
        GameObject page = transform.GetChild(1).gameObject;

        for (int i = 0; i < page.transform.childCount; i++)
        {
            Transform child = page.transform.GetChild(i);
            TMP_Text _t;
            TutorialMessage tm = new TutorialMessage();
            child.TryGetComponent<TMP_Text>(out _t);
            if (_t)
            {
                tm.type = messageType.Text;
            }
            else
            {
                tm.type = messageType.Image;
            }
            tm.obj = child;
            steps.Add(tm);
        }

        StartCoroutine(textOverTime());
        //InvokeRepeating("FixTextSize",0,0.5f);

        if (levelData.patientsToBeTreated > 0)
        {
            goalPatients = levelData.patientsToBeTreated;
            if(PlayerPrefs.GetInt("PlayerNum") == 2)
            {
                goalPatients *= 2;
            }
        }
        angryNotAllowed = levelData.angryNotAllowed;

        startGoals();

    }

    int currentStepIndex = 0;
    int currentTextInstruction = 0;
    int currentCharIndex = 0;
    public float typingSpeed = 0.005f;
    public float imageDisplayDuration = 0.4f;
    public GameObject textAudio;

    IEnumerator textOverTime()
    {
        textAudio.GetComponent<AudioSource>().Play();
        while (currentStepIndex < steps.Count)
        {
            TutorialMessage currentStep = steps[currentStepIndex];
            print(currentStep.type + " " + currentStep.obj.name);
            if (currentStep.type == messageType.Text)
            {
                TMP_Text textComponent = currentStep.obj.GetComponent<TMP_Text>();
                string currentString = textComponent.text;
                currentTextInstruction++;
                textComponent.text = "";
                currentStep.obj.gameObject.SetActive(true);
                while (currentCharIndex < currentString.Length)
                {
                    textComponent.text += currentString[currentCharIndex];
                    currentCharIndex++;
                    yield return new WaitForSeconds(typingSpeed);
                }
            }
            else if (currentStep.type == messageType.Image)
            {
                // Assuming the image is a child of the text and it's initially set to inactive
                currentStep.obj.gameObject.SetActive(true);
                yield return new WaitForSeconds(imageDisplayDuration); // Pause for some time to show image
            }
            currentStepIndex++;
            currentCharIndex = 0;
        }
        textAudio.GetComponent<AudioSource>().Stop();
    }

    /*void FixTextSize()
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
    }*/

    public void startGoals()
    {
        if (goalPatients > 0)
        {
            goals.GetComponent<Text>().text = patientsCured.ToString() + "/" + goalPatients.ToString() + " Patients Cured";
        }
        
        //no anry patients allowed this level
        if (angryNotAllowed)
        {
            //other conntent
            if (goals.GetComponent<Text>().text.Length > 0)
            {
                goals.GetComponent<Text>().text = goals.GetComponent<Text>().text + "\n" + "Don't let any patients get angry and leave without treatment";

            }
            //no other content
            else
            {
                goals.GetComponent<Text>().text = "Don't let any patients get angry and leave without treatment";
            }
        }
    }

    public void updateGoal()
    {
        patientsCured += 1;
        if (goalPatients > 0)
        {
            goals.GetComponent<Text>().text = patientsCured.ToString() + "/" + goalPatients.ToString() + " Patients Cured";
        }

        //no anry patients allowed this level
        if (angryNotAllowed)
        {
       
            //content  is only angry wording
            if(goals.GetComponent<Text>().text == "Don't let any patients get angry and leave without treatment")
            {
                goals.GetComponent<Text>().text = "Don't let any patients get angry and leave without treatment";
            }
            //other conntent
            else if (goals.GetComponent<Text>().text.Length > 0)
            {
                goals.GetComponent<Text>().text = goals.GetComponent<Text>().text + "\n" + "Don't let any patients get angry and leave without treatment";

            }
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
            changeStep(1);
            setupArrows();
            displayArrows<Patient>();
        }

        if (level.levelName == "Level 3" || level.levelName == "Level 4")
        {
            if (currentStep == 0)
            {
                changeStep(1);
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
        /*currentStep += s;
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
        }*/
    }

    public bool checkPatientGoal()
    {
        if (goalPatients <= patientsCured)
        {
            return (true);
        }
        else
        {
            return (false);
        }
    }
}
