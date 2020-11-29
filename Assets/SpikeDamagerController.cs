using System.Collections;
using System.Collections.Generic;
using Platformer.Gameplay;
using Platformer.Mechanics;
using UnityEngine;
using static Platformer.Core.Simulation;

public class SpikeDamagerController : MonoBehaviour
{
    void OnCollisionStay2D(Collision2D collision) {
        Schedule<Damager>();
    }
    void OnTriggerStay2D(Collider2D collision) {
        Schedule<Damager>();
    }
}
