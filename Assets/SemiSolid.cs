using System.Collections;
using System.Collections.Generic;
using Platformer.Model;
using Platformer.Mechanics;
using UnityEngine;
using Platformer.Core;

public class SemiSolid : MonoBehaviour
{
    PlatformerModel model = Simulation.GetModel<PlatformerModel>();

    void FixedUpdate()
    {
        if (model.player.velocity.y >= 0 || model.player.GetComponent<Collider2D>().bounds.min.y < GetComponent<Collider2D>().bounds.max.y) {
            GetComponent<Collider2D>().enabled = false;
        } else {
            GetComponent<Collider2D>().enabled = true;
        }
    }
}
