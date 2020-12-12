using System.Collections;
using System.Collections.Generic;
using Platformer.Model;
using Platformer.Mechanics;
using UnityEngine;

public class Heart : MonoBehaviour
{
    public AudioClip audioSource;

    void OnTriggerEnter2D(Collider2D collision) {
        var player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null) {
            player.AddLives(1);
            player.GetComponent<AudioSource>().PlayOneShot(audioSource, 1f);
            Destroy(gameObject);
        }
    }
}
