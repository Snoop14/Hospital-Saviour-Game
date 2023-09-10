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

    public GameObject goals;
    public GameObject bg;

    int patientsCured = 0;

    Levels level;

    //goal variables
    private int goalPatients = 0;
    private bool angryNotAllowed;
    public GameObject iconCanvas;
    public GameObject timeCanvas;

    public void setupTutorial(Levels levelData)
    {
        level = levelData;

        steps = new List<TutorialMessage>();
        pages = new List<GameObject>();

        int skipToPage = 0;
        int addPage = 7;
        if (level.levelName == "Level 2")
        {
            skipToPage = 7;
            addPage += 1;
        }
        if (level.levelName == "Level 3")
        {
            skipToPage = 8;
            addPage += 2;
        }
        if (level.levelName == "Level 4")
        {
            skipToPage = 9;
            addPage += 3;
        }
        if (level.levelName == "Level 5")
        {
            skipToPage = 10;
            addPage += 4;
        }
        for (int i = 1; i <= addPage; i++)
        {
            pages.Add(transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < skipToPage; i++)
        {
            setPageActive(pages[i]);
            pages[i].SetActive(false);
            currentPage++;
            maxPage++;
        }

        if (level.levelName != "Level 1")
        {
            pages[maxPage - 1].SetActive(true);
            backForthCheck();
        }

        nextPage();

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
    int currentCharIndex = 0;
    public float typingSpeed = 0.05f;
    public float imageDisplayDuration = 0.3f;
    public GameObject textAudio;

    bool ongoing = false;
    bool rush = false;
    List<GameObject> pages;
    int maxPage = 0;
    int currentPage = 0;
    public Image prev;
    public Image next;

    public void nextPage()
    {
        if (currentPage != maxPage)
        {
            pages[currentPage - 1].SetActive(false);
            pages[currentPage].SetActive(true);
            currentPage++;
            backForthCheck();
            return;
        }
        if (maxPage >= pages.Count)
            return;

        if (ongoing)
        {
            rush = true;
            return;
        }

        steps.Clear();
        if(maxPage!= 0)
            pages[maxPage-1].SetActive(false);
        pages[maxPage].SetActive(true);
        for (int i = 0; i < pages[maxPage].transform.childCount; i++)
        {
            Transform child = pages[maxPage].transform.GetChild(i);
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
    }
    public void prevPage()
    {
        if (currentPage <= 1)
            return;
        if (ongoing)
        {
            rush = true;
            return;
        }
        pages[currentPage-1].SetActive(false);
        pages[currentPage-2].SetActive(true);
        currentPage--;
        backForthCheck();

    }
    void backForthCheck()
    {
        if (currentPage <= 1)
            prev.color = new Color(1, 1, 1, 0.5f);
        else
            prev.color = new Color(1, 1, 1, 1);

        if (currentPage < maxPage || maxPage < pages.Count)
            next.color = new Color(1, 1, 1, 1);
        else
            next.color = new Color(1, 1, 1, 0.5f);
    }
    IEnumerator textOverTime()
    {
        maxPage++;
        currentPage++;
        backForthCheck();
        ongoing = true;
        textAudio.GetComponent<AudioSource>().Play();

        while (currentStepIndex < steps.Count)
        {
            TutorialMessage currentStep = steps[currentStepIndex];
            if (currentStep.type == messageType.Text)
            {
                TMP_Text textComponent = currentStep.obj.GetComponent<TMP_Text>();
                string currentString = textComponent.text;
                textComponent.text = "";
                currentStep.obj.gameObject.SetActive(true);
                while (currentCharIndex < currentString.Length)
                {
                    textComponent.text += currentString[currentCharIndex];
                    currentCharIndex++;
                    if(!rush)
                        yield return new WaitForSeconds(typingSpeed);
                }
            }
            else if (currentStep.type == messageType.Image)
            {
                // Assuming the image is a child of the text and it's initially set to inactive
                currentStep.obj.gameObject.SetActive(true);
                if(!rush)
                    yield return new WaitForSeconds(imageDisplayDuration); // Pause for some time to show image
            }
            currentStepIndex++;
            currentCharIndex = 0;
        }
        textAudio.GetComponent<AudioSource>().Stop();
        ongoing = false;
        rush = false;
        currentStepIndex = 0;
    }

    void setPageActive(GameObject item)
    {
        Transform parent = item.transform;
        foreach(Transform child in parent)
        {
            setPageActive(child.gameObject);
            child.gameObject.SetActive(true);
        }
    }

    public void changeActive()
    {
        if (ongoing)
        {
            rush = true;
        }
        else if (gameObject.activeSelf)
        {
            bg.SetActive(false);
            gameObject.SetActive(false);
        }
        else
        {
            backForthCheck();
            bg.SetActive(true);
            gameObject.SetActive(true);
        }
    }

    public void startGoals()
    {
        if (goalPatients > 0)
        {
            goals.GetComponent<TMP_Text>().text = patientsCured.ToString() + "/" + goalPatients.ToString() + " Patients Cured";
        }
        
        //no anry patients allowed this level
        if (angryNotAllowed)
        {
            //other conntent
            if (goals.GetComponent<TMP_Text>().text.Length > 0)
            {
                goals.GetComponent<TMP_Text>().text = goals.GetComponent<TMP_Text>().text + "\n" + "Don't let any patients get angry and leave without treatment";

            }
            //no other content
            else
            {
                goals.GetComponent<TMP_Text>().text = "Don't let any patients get angry and leave without treatment";
            }
        }
    }

    public void updateGoal()
    {
        patientsCured += 1;
        if (goalPatients > 0)
        {
            goals.GetComponent<TMP_Text>().text = patientsCured.ToString() + "/" + goalPatients.ToString() + " Patients Cured";
        }

        //no anry patients allowed this level
        if (angryNotAllowed)
        {
       
            //content  is only angry wording
            if(goals.GetComponent<TMP_Text>().text == "Don't let any patients get angry and leave without treatment")
            {
                goals.GetComponent<TMP_Text>().text = "Don't let any patients get angry and leave without treatment";
            }
            //other conntent
            else if (goals.GetComponent<TMP_Text>().text.Length > 0)
            {
                goals.GetComponent<TMP_Text>().text = goals.GetComponent<TMP_Text>().text + "\n" + "Don't let any patients get angry and leave without treatment";

            }
        }
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
