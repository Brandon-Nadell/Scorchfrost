using Platformer.Core;
using Platformer.Mechanics;
using Platformer.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Fired when the player is spawned after dying.
    /// </summary>
    public class PlayerSpawn : Simulation.Event<PlayerSpawn>
    {
        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public override void Execute()
        {
            var player = model.player;
            player.AddLives(-1);
            if (player.lives == 0) {
                player.lives = player.livesMax;
                player.checkpoint = null;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                return;
            }
            player.GetComponent<Collider2D>().enabled = true;
            player.controlEnabled = false;
            player.flying = false;
            player.immobile = false;
            player.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, player.GetComponent<SpriteRenderer>().color.a);
            foreach (SpriteRenderer sr in player.GetComponentsInChildren<SpriteRenderer>()) {
                sr.color = new Color(1f, 1f, 1f, sr.color.a);
            }
            if (player.audioSource && player.respawnAudio)
                player.audioSource.PlayOneShot(player.respawnAudio);
            player.health.Increment();
            if (player.checkpoint != null) {
                player.Teleport(player.checkpoint.transform.position);
            } else {
                player.Teleport(model.spawnPoint.transform.position);
            }
            player.jumpState = PlayerController.JumpState.Grounded;
            player.animator.SetBool("dead", false);
            model.virtualCamera.m_Follow = player.transform;
            model.virtualCamera.m_LookAt = player.transform;
            Simulation.Schedule<EnablePlayerInput>(0f);
        }
    }
}