using System.Collections;
using System.Collections.Generic;
using Platformer.Model;
using Platformer.Mechanics;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    public AudioClip clip;
    
    void OnTriggerEnter2D(Collider2D collision) {
        if (!GetComponent<Animator>().GetBool("collected")) {
            var player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null) {
                player.SetCheckpoint(this);
                player.ResetLives();
                GetComponent<Animator>().SetBool("collected", true);
                player.GetComponent<AudioSource>().PlayOneShot(clip, 1f);
            }
        }
    }
}
