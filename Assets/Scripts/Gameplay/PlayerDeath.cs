using System.Collections;
using System.Collections.Generic;
using Platformer.Core;
using Platformer.Model;
using UnityEngine;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Fired when the player has died.
    /// </summary>
    /// <typeparam name="PlayerDeath"></typeparam>
    public class PlayerDeath : Simulation.Event<PlayerDeath>
    {
        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public override void Execute()
        {
            var player = model.player;
            if (player.health.IsAlive && !player.animator.GetBool("dead")) {
                player.health.Die();
                model.virtualCamera.m_Follow = null;
                model.virtualCamera.m_LookAt = null;
                
                player.GetComponent<Collider2D>().enabled = false;
                if (!player.pitDeath) {
                    player.velocity *= 0;
                    player.flying = true;
                    player.immobile = true;
                    player.GetComponent<SpriteRenderer>().color = new Color(1f, .5f, .5f, player.GetComponent<SpriteRenderer>().color.a);
                    foreach (SpriteRenderer sr in player.GetComponentsInChildren<SpriteRenderer>()) {
                        sr.color = new Color(1f, .5f, .5f, sr.color.a);
                    }
                }
                player.pitDeath = false;

                player.controlEnabled = false;


                if (player.audioSource && player.ouchAudio)
                    player.audioSource.PlayOneShot(player.ouchAudio);
                player.animator.SetTrigger("hurt");
                player.animator.SetBool("dead", true);
                Simulation.Schedule<PlayerSpawn>(1);
            }
        }
    }
}