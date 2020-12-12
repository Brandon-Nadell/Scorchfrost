using System.Collections;
using System.Collections.Generic;
using Platformer.Gameplay;
using UnityEngine;
using static Platformer.Core.Simulation;

namespace Platformer.Mechanics
{
    /// <summary>
    /// DeathZone components mark a collider which will schedule a
    /// PlayerEnteredDeathZone event when the player enters the trigger.
    /// </summary>
    public class DeathZone : MonoBehaviour
    {

        public AudioClip clip;

        void OnTriggerEnter2D(Collider2D collider)
        {
            var p = collider.gameObject.GetComponent<PlayerController>();
            if (p != null)
            {
                p.GetComponent<AudioSource>().PlayOneShot(clip, 1f);
                var ev = Schedule<PlayerEnteredDeathZone>();
                ev.deathzone = this;
            }
            var e = collider.gameObject.GetComponent<EnemyController>();
            if (e != null) {
                // e.shouldReset = true;
                e.gameObject.SetActive(false);
            }
        }
    }
}