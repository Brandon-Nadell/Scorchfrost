using System.Collections;
using System.Collections.Generic;
using Platformer.Gameplay;
using Platformer.Mechanics;
using UnityEngine;

public class IceCeiling : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision) {
        var player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null && player.headPower == PlayerController.Power.Ice) {
            player.flying = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision) {
        var player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null) {
            player.flying = false;
        }
    }
}
