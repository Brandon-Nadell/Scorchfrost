using Platformer.Core;
using Platformer.Mechanics;
using Platformer.Model;
using UnityEngine;
using static Platformer.Core.Simulation;

namespace Platformer.Gameplay
{

    /// <summary>
    /// Fired when a Player collides with an Enemy.
    /// </summary>
    /// <typeparam name="EnemyCollision"></typeparam>
    public class PlayerEnemyCollision : Simulation.Event<PlayerEnemyCollision>
    {
        public EnemyController enemy;
        public PlayerController player;

        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public override void Execute()
        {
            var willHurtEnemy = player.Bounds.center.y >= enemy.Bounds.max.y;
            if (willHurtEnemy) {
                if (player.feetPower == PlayerController.Power.Ice && player.Bounds.min.y + .1f >= enemy.Bounds.max.y) {
                    enemy.CreateIce(player);
                    // player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + diff, player.transform.position.z);
                    enemy.path = null;
                    enemy.mover = null;
                    enemy._collider.enabled = false;
                    enemy.control.immobile = true;
                    // enemy.control.enabled = false;
                    enemy.GetComponent<AnimationController>().flying = true;
                    enemy.GetComponent<AnimationController>().invulnerable = true;
                    enemy.GetComponent<Animator>().Play("idle", -1, 0f);
                    enemy.GetComponent<Animator>().enabled = false;
                } else {
                    var enemyHealth = enemy.GetComponent<Health>();
                    if (enemyHealth != null)
                    {
                        enemyHealth.Decrement();
                        if (!enemyHealth.IsAlive && !enemy.GetComponent<AnimationController>().invulnerable) {
                            Schedule<EnemyDeath>().enemy = enemy;
                            player.Bounce(5.5f);
                            player.GetComponent<AudioSource>().PlayOneShot(player.jumpAudio, 1f);
                        } else {
                            player.Bounce(5.5f);
                            player.GetComponent<AudioSource>().PlayOneShot(player.jumpAudio, 1f);
                        }
                    } else {
                        player.Bounce(5.5f);
                        if (!enemy.GetComponent<AnimationController>().invulnerable) {
                            Schedule<EnemyDeath>().enemy = enemy;
                        } else {
                            player.GetComponent<AudioSource>().PlayOneShot(player.jumpAudio, 1f);
                        }
                    }
                }
            } else if (player.headPower == PlayerController.Power.Fire && !enemy.immuneToFire && player.Bounds.max.y <= enemy.Bounds.center.y && Mathf.Abs(player.Bounds.center.x - enemy.Bounds.center.x) < 2) {
                var enemyHealth = enemy.GetComponent<Health>();
                if (enemyHealth != null) {
                    enemyHealth.Decrement();
                    if (!enemyHealth.IsAlive)
                    {
                        Schedule<EnemyDeath>().enemy = enemy;
                        player.GetComponent<AudioSource>().PlayOneShot(player.fireAudio, 1f);
                    }
                } else {
                    Schedule<EnemyDeath>().enemy = enemy;
                    player.GetComponent<AudioSource>().PlayOneShot(player.fireAudio, 1f);
                }
            } else {
                Schedule<PlayerDeath>();
            }

           
        }
    }
}