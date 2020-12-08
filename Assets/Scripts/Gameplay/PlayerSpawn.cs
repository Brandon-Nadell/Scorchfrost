using Platformer.Core;
using Platformer.Mechanics;
using Platformer.Model;
using UnityEngine;

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
            player.lives--;
            if (player.lives == 0) {
                player.lives = player.livesMax;
                player.checkpoint = null;
                //reset checkpoints, enemies, and player powers
                // foreach (Checkpoint cp : checkpoints) {
                //     cp.GetComponent<Animator>().SetBool("collected", false);
                // }
                // foreach (EnemyController e : enemies) {
                //     e.Reset();
                //     -> {
                //         e.state = original = state;
                //     }
                // }
            }
            player.livesText.text = "Lives: " + player.lives;
            player.collider2d.enabled = true;
            player.controlEnabled = false;
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