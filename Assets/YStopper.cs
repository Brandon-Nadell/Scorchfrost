using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YStopper : MonoBehaviour
{

    public float yMax;

    void Update()
    {
        if (gameObject.GetComponent<SpriteRenderer>().bounds.max.y > yMax) {
            gameObject.transform.position += new Vector3(0, yMax - gameObject.GetComponent<SpriteRenderer>().bounds.max.y, 0);
        }
    }
}
