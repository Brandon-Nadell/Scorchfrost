using System.Collections;
using System.Collections.Generic;
using Platformer.Model;
using Platformer.Mechanics;
using UnityEngine;

public class PowerUp : MonoBehaviour
{

    public PlayerController.Power power;

    void OnTriggerEnter2D(Collider2D collision) {
        var player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null) {
            player.AddPower(power);
            Destroy(gameObject);
        }
    }
}
