using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostFader : MonoBehaviour
{
    int timer = 0;


    void Start()
    {
        InvokeRepeating("Timer", 0f, .0166f);
    }

    void Timer() {
        if (GetComponent<Animator>().enabled) {
            if (timer >= 30 && timer <= 60) {
                Color c = GetComponent<SpriteRenderer>().color;
                c.a = (timer - 30)/30f;
                GetComponent<SpriteRenderer>().color = c;
                if (timer == 45) {
                    GetComponent<Collider2D>().isTrigger = true;
                    GetComponent<Collider2D>().enabled = true;
                } else if (timer == 50) {
                    GetComponent<Collider2D>().isTrigger = false;
                }
            } else if (timer >= 90 && timer <= 120) {
                Color c = GetComponent<SpriteRenderer>().color;
                c.a = 1f - (timer - 90)/30f;
                GetComponent<SpriteRenderer>().color = c;
                if (timer == 105) {
                    GetComponent<Collider2D>().enabled = false;
                }
            }

            timer++;
            if (timer > 120)
                timer = 0;
        }
    }
}
