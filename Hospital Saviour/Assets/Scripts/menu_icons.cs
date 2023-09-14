using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class menu_icons : MonoBehaviour
{
    [SerializeField]
    List<Transform> icons;
    List<Vector3> directions;
    // Start is called before the first frame update
    void Start()
    {
        directions = new List<Vector3>();
        for (int i = 0; i < icons.Count; i++)
        {
            icons[i].GetComponent<Image>().color = new Color(1,1,1,Random.Range(0.15f,0.85f));
            icons[i].position = new Vector3(Random.Range(10,Screen.width-10), Random.Range(10, Screen.height - 10), 0);
            float randomScale = Random.Range(1.0f,1.8f);
            icons[i].localScale = new Vector3(randomScale, randomScale, 1);
            directions.Add(new Vector3(Random.Range(-1.0f, 1.0f)*2, Random.Range(-1.0f, 1.0f)*2, 0));
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        for(int i = 0; i < icons.Count; i++)
        {
            if (icons[i].position.x > Screen.width || icons[i].position.x < 0)
                directions[i] = new Vector3(-directions[i].x, directions[i].y, 0);
            if (icons[i].position.y > Screen.height || icons[i].position.y < 0)
                directions[i] = new Vector3(directions[i].x, -directions[i].y, 0);
            icons[i].position += directions[i];
        }
    }
}
