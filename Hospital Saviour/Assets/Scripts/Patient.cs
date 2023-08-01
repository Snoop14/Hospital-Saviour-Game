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

    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();

        targetPosition = queuePosition;
        icon = Instantiate(iconPrefab, FindObjectOfType<Canvas>().transform,true);

        sicknessIconBackground = icon.transform.GetChild(0).GetComponent<Image>();
        sicknessIconObject = icon.transform.GetChild(1).gameObject;
        sicknessIcon = sicknessIconObject.GetComponent<Image>();
        sicknessIcon.SetNativeSize();
        healingIconObject = icon.transform.GetChild(2).gameObject;
        healingIcon = healingIconObject.GetComponent<Image>();
        healingOrderIcons = sickness.healingOrderIcons;
        sicknessIconBackground.sprite = sickness.sicknessIconBackGround;
        sicknessIcon.sprite = sickness.sicknessIcon;
        healingIcon.sprite = healingOrderIcons[currHeal];
        healingIcon.SetNativeSize();
        healingIconObject.transform.localScale = new Vector3(0.3f, 0.3f, 1);
    }
    void Update()
    {
        Vector3 offset = new Vector3(0, 2.5f, 0);
        Vector2 positionOnScreen = Camera.main.WorldToScreenPoint(transform.position + offset);
        icon.transform.position = positionOnScreen;
    }
    void FixedUpdate()
    {
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
        //folder = null; //is this okay to be null, this way we still know exactly which folder is actually the patients
    }

    public void folderPlaced(GameObject place)
    {
        assignedPlacement = place;
        isInteractable = false;
        targetPosition = assignedPlacement.transform.position - new Vector3(0,0,1.0f);
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
        if(other.gameObject.TryGetComponent(out Bed b))
        {
            if (b.currentFolder == folder)
            {
                b.NPCInteract(gameObject);
                changePosToBed();
                isInteractable = true;
                sicknessIconObject.gameObject.SetActive(false);
                healingIconObject.gameObject.SetActive(true);
            }
        }
    }

    public void changePosToBed()
    {
        Debug.Log("Patient: Switched to bed");
        agent.enabled = false;//Disable AI movement
        transform.parent = assignedPlacement.transform; //changes the parent of folder to the transfered object
        transform.localPosition = new Vector3(0f, 1f, 0f);
        transform.localRotation = new Quaternion(0f, 0f, 0f, 0f); //resets rotation
        transform.localRotation = Quaternion.Euler(-90f, 90f, 180f); //resets rotation
    }

    //Called when an object is given to the patient while they are on the bed
    public void healOnBed(string item)
    {
        if(item == healingIcon.sprite.name) 
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
        }
    }


    IEnumerator leaveBed(GameObject bed)
    {
        yield return new WaitForSeconds(1.5f);
        Bed b = bed.GetComponent<Bed>();
        b.FolderPickUp();
        b.NPCLeaves();
        folder.GetComponent<Folder>().destroySelf();
        leaveHospital();
    }
    //called when patient is to leave the hospital
    private void leaveHospital()
    {
        agent.enabled = true;
        icon.gameObject.SetActive(false);
        agent.SetDestination(ExitTransform.position);
    }
}
