using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    [SerializeField]
    GameObject bedPrefab;
    [SerializeField]
    GameObject inActiveBedParent;
    [SerializeField]
    GameObject activeBedParent;

    [SerializeField]
    Material inActiveMat;

    public int activeBedCount = 2;
    public int inActiveBedCount = 3;

    [Range(0,5)]
    public float bedSeperation = 4;

    List<GameObject> beds;
    // Start is called before the first frame update
    void Start()
    {
        //Instatiate inactive beds
        for(int i = 0; i < inActiveBedCount; i++)
        {
            GameObject newBed = Instantiate(bedPrefab,inActiveBedParent.transform,false);
            newBed.transform.position += new Vector3(0,0,-bedSeperation * i);
            var bedRenderers = newBed.GetComponentsInChildren<MeshRenderer>();
            foreach(MeshRenderer renderer in bedRenderers)
            {
                renderer.material = inActiveMat;
            }
        }

        //Instatiate Active beds
        for (int i = inActiveBedCount; i < activeBedCount + inActiveBedCount; i++)
        {
            GameObject newBed = Instantiate(bedPrefab, inActiveBedParent.transform, false);
            newBed.transform.position += new Vector3(0, 0, -bedSeperation * i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
