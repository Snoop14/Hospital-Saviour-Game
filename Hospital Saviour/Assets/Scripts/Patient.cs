using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

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
    //int to hold current position in icons
    private int currHeal;

    public float happinessLvl { get; private set; } = 100f;
    private float happinessDrop;

    void Start()
    {
        manager = GameObject.Find("Manager").GetComponent<GameManager>();
        agent = gameObject.GetComponent<NavMeshAgent>();

        targetPosition = queuePosition;
        
        //icon = Instantiate(iconPrefab, FindObjectOfType<Canvas>().transform, true);
        icon = Instantiate(iconPrefab, GameObject.Find("IconCanvas").transform, true);

        sicknessIconBackground = icon.transform.GetChild(0).GetComponent<Image>();
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

        FillObject = icon.transform.GetChild(3).GetChild(0).GetChild(0).gameObject;
        fillImage = FillObject.GetComponent<Image>();
        fillImage.color = Color.green;
        happinessDrop = sickness.happinessDropLevel;

        //function starts after 10s and repeats every 5s
        InvokeRepeating("dropHappinessLvl", 10, 1);
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

    }

    void Update()
    {
        Vector3 offset = new Vector3(0, 2.5f, 0);
        Vector2 positionOnScreen = Camera.main.WorldToScreenPoint(transform.position + offset);
        icon.transform.position = positionOnScreen;
    }
    void FixedUpdate()
    {
        //I am unsure if we need fixedUpdate for movement because i think the AI handles it?
        if ((targetPosition - transform.position).sqrMagnitude > 1f && isInQueue)
        {
            move();
        }
    }

    public void move()
    {
        agent.SetDestination(targetPosition);
    }

    public void releaseFolder()
    {
        isHoldingFolder = false;
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
        
        //patients transform in my opinion does not need to be changed
        //transform.parent = assignedPlacement.transform; //changes the parent of folder to the transfered object
        
        Vector3 pos = assignedPlacement.transform.position;
        transform.position = new Vector3(pos.x, pos.y + 1f, pos.z);
        transform.localRotation = new Quaternion(0f, 0f, 0f, 0f); //resets rotation
        transform.localRotation = Quaternion.Euler(-90f, 90f, 180f); //resets rotation
    }

    /// <summary>
    /// Called when an object is given to the patient while they are on the bed
    /// </summary>
    /// <param name="item"></param>
    public void healOnBed(string item)
    {
        //iif the sickenss icon is active
        if (sicknessIconObject.gameObject.activeSelf)
        {
            //end the function
            return;
        }

        //if given item is same as healing icon
        if (item == healingIcon.sprite.name)
        {
            currHeal++; //increase current heal state
        }

        //if currHeal is greater than or equal to length of healingIcons
        if (currHeal >= healingOrderIcons.Count)
        {
            //should be leaving hospital but changes to functions will need to be made
            StartCoroutine(leaveBed(assignedPlacement));
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
    /// called when patient leaves the bed
    /// </summary>
    /// <param name="bed"></param>
    /// <returns></returns>
    IEnumerator leaveBed(GameObject bed)
    {
        yield return new WaitForSeconds(1.5f);
        Bed b = bed.GetComponent<Bed>();
        b.FolderPickUp();
        b.NPCLeaves();
        folder.GetComponent<Folder>().destroySelf();
        
        //may not necessarily leave the hospital after leaving bed
        leaveHospital();
    }

    /// <summary>
    /// called when patient is to leave the hospital
    /// </summary>
    private void leaveHospital()
    {
        agent.enabled = true; //re-enable navmesh ageny
        StartCoroutine(DisplayHappy());// show happy icon for a few seconds
        agent.SetDestination(ExitTransform.position); //patient heads to exit loc
        manager.UpdateScore(happinessLvl); // increase score
    }

    IEnumerator DisplayHappy()
    {
        sicknessIconBackground.gameObject.SetActive(false);
        healingIconObject.SetActive(false);
        FillObject.SetActive(false);
        icon.transform.GetChild(4).gameObject.SetActive(true); //enable happy icon
        yield return new WaitForSeconds(2f);
        icon.gameObject.SetActive(false); //disable icons above head
    }
    

    //managed interactions with player whilst on bed
    public void interactionOnBed(bool isCarrying)
    {

        //change icon
        iterateIcons(isCarrying);

    }

    private void iterateIcons(bool isCarrying)
    {
        if (icon.sprite == sickness.sicknessBase.sicknessIcon  && !isCarrying)
        {
            icon = healingIcon;
        }
        else
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

    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
