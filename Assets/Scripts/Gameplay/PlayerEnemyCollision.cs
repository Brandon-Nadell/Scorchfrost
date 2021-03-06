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
                if (player.feetPower == PlayerController.Power.Ice) {
                    if (player.Bounds.min.y + .1f >= enemy.Bounds.max.y) {
                        float y = player.transform.position.y;

                        // force teleport player only if at least 2 pixels into ice
                        // if (player.GetComponent<Collider2D>().bounds.min.y <= top + .05f) {
                            // player.transform.position = new Vector3(player.transform.position.x, top + player.GetComponent<Collider2D>().bounds.size.y/2 + .111f, player.transform.position.z);
                        // }
                        float top = enemy.CreateIce(player);
                        player.transform.position = new Vector3(player.transform.position.x, Mathf.Max(y, top + player.GetComponent<SpriteRenderer>().bounds.size.y/2) + .015f, player.transform.position.z);
                        // player.transform.position = new Vector3(player.transform.position.x, y, player.transform.position.z);
                        player.velocity.y *= 0;

                        enemy.path = null;
                        enemy.mover = null;
                        enemy._collider.enabled = false;
                        enemy.control.immobile = true;
                        // enemy.control.enabled = false;
                        enemy.GetComponent<AnimationController>().flying = true;
                        enemy.GetComponent<AnimationController>().invulnerable = true;
                        enemy.GetComponent<Animator>().Play("idle", -1, 0f);
                        enemy.GetComponent<Animator>().enabled = false;
                    }
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
            } else if (player.headPower == PlayerController.Power.Fire && !enemy.immuneToFire && player.Bounds.max.y <= enemy.Bounds.center.y && player.velocity.y >= 0) {
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