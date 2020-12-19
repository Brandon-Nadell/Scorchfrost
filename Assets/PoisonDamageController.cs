using System.Collections;
using System.Collections.Generic;
using Platformer.Gameplay;
using Platformer.Mechanics;
using UnityEngine;
using static Platformer.Core.Simulation;

public class PoisonDamageController : MonoBehaviour
{
    void OnCollisionStay2D(Collision2D collision) {
        var player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null) {
            Schedule<Damager>();
        }
    }
}
