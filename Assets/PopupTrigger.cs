using System.Collections;
using System.Collections.Generic;
using Platformer.Core;
using Platformer.Model;
using Platformer.Mechanics;
using UnityEngine;

public class PopupTrigger : MonoBehaviour
{

    public GameObject popup;
    bool triggered;
    
    void OnTriggerEnter2D(Collider2D collision) {
        if (!triggered) {
            var player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null) {
                if (player.currentPopup != null) {
                    player.currentPopup.SetActive(false);
                }
                player.currentPopup = popup;
                popup.SetActive(true);
                Invoke("DisablePopup", 8f);
                triggered = true;
            }
        }
    }

    void DisablePopup() {
        popup.SetActive(false);
        var player = Simulation.GetModel<PlatformerModel>().player;
        if (player.currentPopup == popup) {
            player.currentPopup = null;
        }
    }
}
