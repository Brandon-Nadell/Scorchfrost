using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resetter : MonoBehaviour
{

    public GameObject prefab;
    public Vector3 position;

    public void Awake() {
    }

    public void Reset() {
        if (!gameObject.GetComponent<Animator>().enabled) {
            gameObject.SetActive(false);
            Instantiate(prefab, position, prefab.transform.rotation);
        }
    }
}
