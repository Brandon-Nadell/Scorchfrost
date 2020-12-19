using Platformer.Gameplay;
using Platformer.Core;
using Platformer.Mechanics;
using Platformer.Model;
using UnityEngine;
using static Platformer.Core.Simulation;

namespace Platformer.Mechanics
{
    /// <summary>
    /// Marks a trigger as a VictoryZone, usually used to end the current game level.
    /// </summary>
    public class VictoryZone : MonoBehaviour
    {

        public SpriteRenderer fadeoutLight;
        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        void OnTriggerEnter2D(Collider2D collider)
        {
            var p = collider.gameObject.GetComponent<PlayerController>();
            if (p != null) {
                Invoke("Lighten", .025f);
            }
        }

        void Lighten() {
            fadeoutLight.color = new Color(fadeoutLight.color.r, fadeoutLight.color.g, fadeoutLight.color.g, Mathf.Min(fadeoutLight.color.a + .01f, 1));
            if (fadeoutLight.color.a == 1) {
                model.player.victory = true;
            }
            GameObject.Find("GameController").GetComponent<AudioSource>().volume -= .01f;
            Invoke("Lighten", .025f);
        }
    }
}