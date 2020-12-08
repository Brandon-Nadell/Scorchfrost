using System.Collections;
using System.Collections.Generic;
using Platformer.Model;
using Platformer.Mechanics;
using UnityEngine;

public class PowerStation : MonoBehaviour
{

    int index;

    void OnTriggerStay2D(Collider2D collision) {
        var player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null) {
            player.touchingPowerStation = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision) {
        var player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null) {
            player.touchingPowerStation = false;
        }
    }
}
