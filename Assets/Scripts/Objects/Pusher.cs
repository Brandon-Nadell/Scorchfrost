using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Mechanics;
using Platformer.Model;
using Platformer.Core;

public class Pusher : MonoBehaviour
{

    PlatformerModel model = Simulation.GetModel<PlatformerModel>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void nOnCollisionStay2D(Collision2D collision)
    {
        var player = model.player;
        var enemy = collision.gameObject.GetComponent<EnemyController>();
        if (enemy != null)
        {
            Debug.Log(player.velocity.x*Time.deltaTime);
            if (gameObject.name == "pusherL" && player.velocity.x > .01) {
                enemy.transform.position = new Vector3(enemy.transform.position.x + player.velocity.x*Time.deltaTime, enemy.transform.position.y, enemy.transform.position.z);
            } else if (gameObject.name == "pusherR" && player.velocity.x < -.01) {
                enemy.transform.position = new Vector3(enemy.transform.position.x + player.velocity.x*Time.deltaTime, enemy.transform.position.y, enemy.transform.position.z);
            } else if (gameObject.name == "pusherB" && player.velocity.y > .01) {
                enemy.transform.position = new Vector3(enemy.transform.position.x, enemy.transform.position.y + player.velocity.y*Time.deltaTime, enemy.transform.position.z);
            }
        }
    }

    void nOnTriggerStay2D(Collider2D collision)  {
        // var player = gameObject.GetComponent<PlayerController>();
        var player = model.player;
        var enemy = collision.gameObject.GetComponent<EnemyController>();
        if (enemy != null)
        {
            // Bounds bounds = enemy.GetComponent<Collider2D>().bounds;
            // if (player.velocity.x > .01) {
            //     // enemy.GetComponent<Collider2D>().bounds.min = new Vector3(player.GetComponent<Collider2D>().bounds.max.x, bounds.min.y, bounds.min.z);
            // } else if (player.velocity.x < -.01) {
            //     // enemy.GetComponent<Collider2D>().bounds.max = new Vector3(player.GetComponent<Collider2D>().bounds.min.x, bounds.max.y, bounds.max.z);
            // }

            // float move = player.velocity.x*Time.deltaTime;
            // enemy.transform.position = new Vector3(enemy.transform.position.x + (player.velocity.x > .01 ? 1 : player.velocity.x < -.01 ? -1 : 0)*move, enemy.transform.position.y + (player.velocity.y > .01 ? 1 : player.velocity.y < -.01 ? -1 : 0)*move, enemy.transform.position.z);
        
            // enemy.transform.position = new Vector3(enemy.transform.position.x + 2*player.velocity.x*Time.deltaTime, enemy.transform.position.y + 2*player.velocity.y*Time.deltaTime, enemy.transform.position.z);
        
            // enemy.GetComponent<AnimationController>().velocity = new Vector2(enemy.GetComponent<AnimationController>().velocity.x + player.velocity.x, enemy.GetComponent<AnimationController>().velocity.y + player.velocity.y);
        
            Debug.Log(player.velocity.x*Time.deltaTime);
            if (gameObject.name == "pusherL" && player.velocity.x > .01) {
                enemy.transform.position = new Vector3(enemy.transform.position.x + player.velocity.x*Time.deltaTime, enemy.transform.position.y, enemy.transform.position.z);
            } else if (gameObject.name == "pusherR" && player.velocity.x < -.01) {
                enemy.transform.position = new Vector3(enemy.transform.position.x + player.velocity.x*Time.deltaTime, enemy.transform.position.y, enemy.transform.position.z);
            } else if (gameObject.name == "pusherB" && player.velocity.y > .01) {
                enemy.transform.position = new Vector3(enemy.transform.position.x, enemy.transform.position.y + player.velocity.y*Time.deltaTime, enemy.transform.position.z);
            }
        }
    }
}
