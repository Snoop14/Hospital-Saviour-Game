using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Patient : MonoBehaviour
{
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
    //int to hold current position in icons
    private int currHeal = 0;

    private Animator animator;
    private int toBedHash;
    private int fromBedHash;

    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();

        targetPosition = queuePosition;
        icon = Instantiate(iconPrefab, FindObjectOfType<Canvas>().transform, true);

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
        
        //animator = GetComponent<Animator>();
        animator = gameObject.GetComponentInChildren<Animator>();
        toBedHash = Animator.StringToHash("ToBed");
        fromBedHash = Animator.StringToHash("FromBed");
    }
    void Update()
    {
        Vector3 offset = new Vector3(0, 2.5f, 0);
        Vector2 positionOnScreen = Camera.main.WorldToScreenPoint(transform.position + offset);
        icon.transform.position = positionOnScreen;
    }

    void FixedUpdate()
    {
        if ((targetPosition - transform.position).sqrMagnitude > 1f)
        {
            if (isInQueue)
            {
                move();
            }
        }
        else
        {
            //interact with bed is here now
            if (assignedPlacement.TryGetComponent(out Bed b))
            {
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
        //folder = null; //is this okay to be null, this way we still know exactly which folder is actually the patients
    }

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

    public void changePosToBed()
    {
        Debug.Log("Patient: Switched to bed");
        agent.enabled = false;//Disable AI movement
        transform.parent = assignedPlacement.transform; //changes the parent of folder to the transfered object
        transform.localRotation = new Quaternion(0f, 0f, 0f, 0f); //resets rotation
        animator.applyRootMotion = false; // true breaks animation, but false breaks spawning of patients
        //animator.SetTrigger(toBedHash);
        animator.SetTrigger("ToBed");
    }

    //Called when an object is given to the patient while they are on the bed
    public void healOnBed(string item)
    {
        //animator.ResetTrigger(toBedHash);
        //if the sickness icon is active
        if (sicknessIconObject.gameObject.activeSelf)
        {
            //end the function
            return;
        }

        if (item == healingIcon.sprite.name)
        {
            currHeal++;
        }

        if (currHeal == healingOrderIcons.Count)
        {
            StartCoroutine(leaveBed(assignedPlacement));
        }
        else
        {
            healingIcon.sprite = healingOrderIcons[currHeal];
            healingIcon.SetNativeSize();
            healingIcon.transform.localScale = new Vector3(0.3f, 0.3f, 1);

            //turn off sickness icon
            icon.transform.GetChild(1).gameObject.SetActive(false);
            //turn on healing icon
            icon.transform.GetChild(2).gameObject.SetActive(true);

            //icon = healingIcon;
        }
    }

    IEnumerator leaveBed(GameObject bed)
    {
        targetPosition = ExitTransform.position;
        yield return new WaitForSeconds(1.5f);
        //animator.SetTrigger(fromBedHash);
        animator.SetTrigger("FromBed");
        Debug.Log("leaving bed");
        Bed b = bed.GetComponent<Bed>();
        b.FolderPickUp();
        b.NPCLeaves();
        folder.GetComponent<Folder>().destroySelf();
        leaveHospital();
    }

    //called when patient is to leave the hospital
    private void leaveHospital()
    {
        targetPosition = ExitTransform.position;
        agent.enabled = true;
        icon.gameObject.SetActive(false);
        agent.SetDestination(targetPosition);
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
            healingIcon.sprite = healingOrderIcons[currHeal];
            healingIcon.SetNativeSize();
            healingIcon.transform.localScale = new Vector3(0.3f, 0.3f, 1);

            //turn off sickness icon
            icon.transform.GetChild(1).gameObject.SetActive(false);
            //turn on healing icon
            icon.transform.GetChild(2).gameObject.SetActive(true);
        }

    }
}
