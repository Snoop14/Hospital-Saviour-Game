using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using TMPro;

public class Patient : MonoBehaviour
{
    private GameManager manager;

    private NavMeshAgent agent;

    public bool isInteractable { get; private set; } = true;
    //bool to hold if carrying folder
    public bool isHoldingFolder { get; private set; } = true;
    public GameObject folder = null;
    //for bed or machines
    GameObject assignedPlacement = null;

    public Vector3 queuePosition;
    public bool isInQueue = true;

    Vector3 targetPosition;
    public Transform ExitTransform;

    public Image iconPrefab; // Assign this in the Inspector
    Image icon;
    public Sickness sickness;
    Image sicknessIconBackground;
    GameObject sicknessIconObject;
    Image sicknessIcon;
    GameObject healingIconObject;
    Image healingIcon;
    List<Sprite> healingOrderIcons;
    GameObject FillObject;
    Image fillImage;
    GameObject EmojiHappy;
    GameObject EmojiAngry;
    //int to hold current position in icons
    private int currHeal;
    bool inAction = false;

    public float happinessLvl { get; private set; } = 100f;
    private float happinessDrop;

    private Animator animator;
    private int toBedHash;
    private int fromBedHash;

    public tutorial tutorial;

    void Start()
    {
        manager = GameObject.Find("Manager").GetComponent<GameManager>();
        agent = gameObject.GetComponent<NavMeshAgent>();

        targetPosition = queuePosition;
        
        //icon = Instantiate(iconPrefab, FindObjectOfType<Canvas>().transform, true);
        icon = Instantiate(iconPrefab, GameObject.Find("IconCanvas").transform, true);

        sicknessIconBackground = icon.transform.GetChild(0).GetComponent<Image>();
        sicknessIconBackground.SetNativeSize();
        sicknessIconBackground.transform.localScale = new Vector3(0.8f, 0.8f, 1);
        sicknessIconObject = icon.transform.GetChild(1).gameObject;
        sicknessIcon = sicknessIconObject.GetComponent<Image>();
        sicknessIcon.SetNativeSize();
        healingIconObject = icon.transform.GetChild(2).gameObject;
        healingIcon = healingIconObject.GetComponent<Image>();
        healingOrderIcons = sickness.healingOrderIcons;
        sicknessIconBackground.sprite = sickness.sicknessBase.sicknessIconBackGround;
        sicknessIcon.sprite = sickness.sicknessBase.sicknessIcon;
        healingIcon.sprite = healingOrderIcons[currHeal];
        healingIcon.SetNativeSize();
        healingIconObject.transform.localScale = new Vector3(0.3f, 0.3f, 1);
        EmojiHappy = icon.transform.GetChild(4).gameObject;
        EmojiAngry = icon.transform.GetChild(5).gameObject;

        FillObject = icon.transform.GetChild(3).GetChild(0).GetChild(0).gameObject;
        fillImage = FillObject.GetComponent<Image>();
        fillImage.color = Color.green;
        happinessDrop = sickness.happinessDropLevel;

        //if level is higher than 2, run the dropp happiness code
        if (manager.levelNo > 2)
        {
            //function starts after 10s and repeats every 5s
            InvokeRepeating("dropHappinessLvl", 10, 5);
        }
        //otherwise, don't display the bar
        else
        {
            GameObject.Find("HappinessBackground").SetActive(false);
        }


        //animator = GetComponent<Animator>();
        animator = gameObject.GetComponentInChildren<Animator>();
        toBedHash = Animator.StringToHash("ToBed");
        fromBedHash = Animator.StringToHash("FromBed");
    }

    /// <summary>
    /// Drop happiness level based on sickness type
    /// https://docs.unity3d.com/ScriptReference/Color.Lerp.html
    /// </summary>
    private void dropHappinessLvl()
    {
        //drop happiness level based on sickness
        happinessLvl -= happinessDrop;
        float startPos = -5.5f;
        float endPos = -40f;
        float percentage = 1 - (happinessLvl / 100f);
        FillObject.transform.localPosition = new Vector3((endPos-startPos)* percentage + startPos, 0,0);
        if(percentage < 0.5f)
            fillImage.color = Color.Lerp(Color.green,Color.yellow,percentage*2);
        else
            fillImage.color = Color.Lerp(Color.yellow, Color.red, (percentage - 0.5f)*2f);

        if (happinessLvl <= 0)
        {
            CancelInvoke();
            MadPatient();
        }
    }

    void Update()
    {
        Vector3 offset = new Vector3(0, 2.5f, 1.5f);
        Vector2 positionOnScreen = Camera.main.WorldToScreenPoint(transform.position + offset);
        icon.transform.position = positionOnScreen;
    }

    void FixedUpdate()
    {
        //I am unsure if we need fixedUpdate for movement because i think the AI handles it?
        if ((targetPosition - transform.position).sqrMagnitude > 1f)
        {
            if (isInQueue)
            {
                move();
            }
        }
        else if(assignedPlacement)
        {
            if (assignedPlacement.TryGetComponent(out Bed b))
            {
                //check if patient should get on this bed
                if (b.currentFolder == folder && b.currentPatient == null)
                {
                    b.NPCInteract(gameObject);
                    changePosToBed();
                    isInteractable = true;
                }
            }
        }
    }

    public void move()
    {
        agent.SetDestination(targetPosition);
    }
    /// <summary>
    /// this function releases folder
    /// </summary>
    public void releaseFolder()
    {
        isHoldingFolder = false;
    }

    public void retakeFolder()
    {
        isHoldingFolder = true;
    }

    /// <summary>
    /// called when folder is placed somewhere else i.e. bed
    /// </summary>
    /// <param name="place"></param>
    public void folderPlaced(GameObject place)
    {
        assignedPlacement = place;
        isInteractable = false;
        targetPosition = assignedPlacement.transform.position - new Vector3(0, 0, 1.0f);
        agent.SetDestination(targetPosition);
    }

    public void moveInQueue(Vector3 pos)
    {
        queuePosition = pos;
        targetPosition = queuePosition;
        agent.SetDestination(targetPosition);
    }

    private void OnCollisionEnter(Collision other)
    {
        //if the other objecy is a bed
        if (other.gameObject.TryGetComponent(out Bed b))
        {
            //if the bed has this patients folder
            if (b.currentFolder == folder)
            {
                b.NPCInteract(gameObject);
                changePosToBed();
                isInteractable = true;
            }
        }
    }

    /// <summary>
    /// changes position of player to lay down on bed
    /// </summary>
    public void changePosToBed()
    {
        agent.enabled = false;//Disable AI movement
        gameObject.GetComponent<Collider>().enabled = false;
        transform.parent = assignedPlacement.transform; //changes the parent of folder to the transfered object
        transform.localRotation = new Quaternion(0f, 0f, 0f, 0f); //resets rotation
        
        transform.localPosition = new Vector3(-0.5f, 0.55f, -1.5f);
        animator.applyRootMotion = false; // true breaks animation, but false breaks spawning of patients
        //animator.SetTrigger(toBedHash);
        animator.SetTrigger("ToBed");
    }

    /// <summary>
    /// Called when an object is given to the patient while they are on the bed
    /// </summary>
    /// <param name="item"></param>
    public void healOnBed(string item)
    {
        //animator.ResetTrigger(toBedHash);
        //if the sickness icon is active
        if (sicknessIconObject.gameObject.activeSelf)
        {
            //end the function
            return;
        }

        //if given item is same as healing icon
        if (item == healingIcon.sprite.name)
        {
            if (healingIcon.sprite.name == "Soup")
            {
                StartCoroutine(triggerAction("eat soup"));
            }
            if (healingIcon.sprite.name == "Pill")
            {
                StartCoroutine(triggerAction("eat pill"));
            }
            if (healingIcon.sprite.name == "Bandage")
            {
                StartCoroutine(triggerAction("bandage head"));
            }
            currHeal++; //increase current heal state
        }

        //if currHeal is greater than or equal to length of healingIcons
        if (currHeal >= healingOrderIcons.Count)
        {
            CancelInvoke();
            //should be leaving hospital but changes to functions will need to be made
            StartCoroutine(triggerAction("leave placement"));
            StartCoroutine(leaveHospital());
        }
        else
        {
            //change healing icon to next one
            healingIcon.sprite = healingOrderIcons[currHeal];
            healingIcon.SetNativeSize();
            healingIcon.transform.localScale = new Vector3(0.3f, 0.3f, 1);

            //These lines of code seem uneccesary
            //turn off sickness icon
            //icon.transform.GetChild(1).gameObject.SetActive(false);
            //turn on healing icon
            //icon.transform.GetChild(2).gameObject.SetActive(true);

            //icon = healingIcon;
        }
    }

    /// <summary>
    /// Makes the patient do something based on the parameter
    /// action
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    IEnumerator triggerAction(string action)
    {
        while (inAction)
        {
            yield return null;
        }
        //Makes the patient leave the bed
        if (action == "leave placement")
        {
            //Makes the patient leave the bed
            //animator.SetTrigger(fromBedHash);
            inAction = true;
            animator.SetTrigger("FromBed");
            yield return new WaitForSeconds(1.5f);
            inAction = false;
            //Debug.Log("leaving bed");
            Bed b = assignedPlacement.GetComponent<Bed>();
            b.FolderPickUp();
            b.NPCLeaves();
            assignedPlacement = null;
        }
        else if(action == "eat soup")
        {
            //Animate patient drinking soup
            inAction = true;
            animator.SetTrigger("EatSoup");
            yield return new WaitForSeconds(1.5f);
            inAction = false;
        }
        else if (action == "eat pill")
        {
            //Animate patient eating pill
            inAction = true;
            animator.SetTrigger("EatPill");
            yield return new WaitForSeconds(1.5f);
            inAction = false;
        }
        else if (action == "bandage head")
        {
            inAction = true;
            StartCoroutine("applyBandages");
            inAction = false;
        }
    }

    /// <summary>
    /// Enables the bandages on the patients head one by one
    /// </summary>
    /// <returns></returns>
    IEnumerator applyBandages()
    {
        Transform bandagesParent = transform.GetChild(0).Find("BaseCharacter/Specifics/Bandages");
        Transform hatObject = transform.GetChild(0).Find("BaseCharacter/Specifics/Hat");

        if (hatObject != null)
        {
            hatObject.gameObject.SetActive(false);
        }
        for (int i = 0; i < 10; i++)
        {
            bandagesParent.GetChild(i).gameObject.SetActive(true);
            yield return new WaitForSeconds(0.15f);
        }
    }

    /// <summary>
    /// called when patient is to leave the hospital
    /// </summary>
    IEnumerator leaveHospital()
    {
        while (assignedPlacement != null)
        {
            yield return null;
        }
        targetPosition = ExitTransform.position;
        gameObject.GetComponent<Collider>().enabled = true;
        agent.enabled = true; //re-enable navmesh ageny
        if (happinessLvl <= 0)
        {
            StartCoroutine(DisplayMad());
        }
        else 
        { 
            StartCoroutine(DisplayHappy());// show happy icon for a few seconds
            StartCoroutine(ScorePopup(happinessLvl));
        }
        agent.SetDestination(ExitTransform.position); //patient heads to exit loc
        manager.UpdateScore(happinessLvl); // increase score

        folder.GetComponent<Folder>().destroySelf();
        isInteractable = false;
    }

    /// <summary>
    /// Called when patient is aided and happy
    /// </summary>
    /// <returns></returns>
    IEnumerator DisplayHappy()
    {
        sicknessIconBackground.gameObject.SetActive(false);
        healingIconObject.SetActive(false);
        icon.transform.GetChild(3).gameObject.SetActive(false);
        //FillObject.SetActive(false);
        EmojiHappy.SetActive(true); //enable happy icon
        yield return new WaitForSeconds(2f);
        icon.gameObject.SetActive(false); //disable icons above head

        tutorial.updateGoal();
    }

    [SerializeField]
    GameObject popupText;
    [SerializeField]
    GameObject iconCanvas;
    IEnumerator ScorePopup(float score)
    {
        iconCanvas = GameObject.Find("IconCanvas");
        GameObject popup = Instantiate(popupText,iconCanvas.transform);
        RectTransform rect = popup.GetComponent<RectTransform>();
        TMP_Text popupT = popup.GetComponent<TMP_Text>();
        Vector2 newP = Camera.main.WorldToScreenPoint(transform.position);
        rect.position = new Vector2(newP.x,newP.y+60);
        popupT.text += score;
        while (popupT.color.a > 0.1f)
        {
            Color n = popupT.color;
            popupT.color = new Color(n.r,n.g,n.b,n.a-0.05f);
            rect.position = new Vector2(rect.position.x, rect.position.y+8);
            yield return new WaitForSeconds(0.1f);
        }
        Destroy(popup);
    }

    /// <summary>
    /// Called when patient is not aided in time and unhappy
    /// </summary>
    /// <returns></returns>
    IEnumerator DisplayMad()
    {
        sicknessIconBackground.gameObject.SetActive(false);
        healingIconObject.SetActive(false);
        icon.transform.GetChild(3).gameObject.SetActive(false);
        //FillObject.SetActive(false);
        EmojiAngry.SetActive(true); //enable happy icon
        yield return new WaitForSeconds(2f);
        icon.gameObject.SetActive(false); //disable icons above head
    }


    //managed interactions with player whilst on bed
    public void interactionOnBed(bool isCarrying, GameObject player)
    {
        //change icon
        iterateIcons(isCarrying, player);
    }

    private void iterateIcons(bool isCarrying, GameObject player)
    {
        if (icon.sprite == sickness.sicknessBase.sicknessIcon  && !isCarrying)
        {
            icon = healingIcon;
        }
        else if (currHeal == 0)
        {
            currHeal = 0;
            healingIcon.sprite = healingOrderIcons[currHeal];
            healingIcon.SetNativeSize();
            healingIcon.transform.localScale = new Vector3(0.3f, 0.3f, 1);

            //turn off sickness icon
            icon.transform.GetChild(1).gameObject.SetActive(false);
            //turn on healing icon
            icon.transform.GetChild(2).gameObject.SetActive(true);
        }
        player.GetComponent<Player>().IconChange(healingIcon.sprite);
    }

    /// <summary>
    /// Called when patient isn't healed in time
    /// </summary>
    private void MadPatient()
    {
        if (assignedPlacement)
        {
            StartCoroutine(triggerAction("leave placement"));
        }
        StartCoroutine(leaveHospital());
        
        manager.MadPatient();
    }

    /// <summary>
    /// Destroys the entire patient
    /// </summary>
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
