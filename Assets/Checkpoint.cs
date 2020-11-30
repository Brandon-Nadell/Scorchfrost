using System.Collections;
using System.Collections.Generic;
using Platformer.Model;
using Platformer.Mechanics;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    
    void OnTriggerEnter2D(Collider2D collision) {
        var player = collision.gameObject.GetComponent<PlayerController>();
        player.SetCheckpoint(this);
        GetComponent<Animator>().SetBool("collected", true);
    }
}
