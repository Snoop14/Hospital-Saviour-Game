using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    Transform target;
    // Start is called before the first frame update
    public void assignObject(Transform t)
    {
        target = t;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 offset = new Vector3(0, 3f, 3.5f);
        Vector2 positionOnScreen = Camera.main.WorldToScreenPoint(target.position + offset);
        transform.position = positionOnScreen;
    }
}
