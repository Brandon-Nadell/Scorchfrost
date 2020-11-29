using System.Collections;
using System.Collections.Generic;
using Platformer.Core;
using Platformer.Mechanics;
using Platformer.Model;
using Platformer.Gameplay;
using UnityEngine;
using static Platformer.Core.Simulation;

public class Damager : Simulation.Event<Damager>
{

    public PlayerController player;
    PlatformerModel model = Simulation.GetModel<PlatformerModel>();

    public override void Execute()
    {
        if (!model.player.animator.GetBool("dead")) {
            Schedule<PlayerDeath>();
        }
    }
}
