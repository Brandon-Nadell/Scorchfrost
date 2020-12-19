﻿using System.Collections;
using System.Collections.Generic;
using Platformer.Core;
using Platformer.Model;
using UnityEngine;

namespace Platformer.Mechanics
{
    /// <summary>
    /// Implements game physics for some in game entity.
    /// </summary>
    public class KinematicObject : MonoBehaviour
    {
        /// <summary>
        /// The minimum normal (dot product) considered suitable for the entity sit on.
        /// </summary>
        public float minGroundNormalY = .65f;

        /// <summary>
        /// A custom gravity coefficient applied to this entity.
        /// </summary>
        public float gravityModifier = 1f;
        public bool flying;
        public bool invulnerable;

        /// <summary>
        /// The current velocity of the entity.
        /// </summary>
        public Vector2 velocity;
        protected float accel;
        public bool immobile;
        public Vector2 freeMovement = new Vector2(1, 1);
        public bool lookat;

        /// <summary>
        /// Is the entity currently sitting on a surface?
        /// </summary>
        /// <value></value>
        public bool IsGrounded { get; private set; }

        public Vector2 targetVelocity;
        protected Vector2 groundNormal;
        protected Rigidbody2D body;
        protected ContactFilter2D contactFilter;
        protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];

        protected const float minMoveDistance = 0.001f;
        protected const float shellRadius = 0.01f;

        /// <summary>
        /// Bounce the object's vertical velocity.
        /// </summary>
        /// <param name="value"></param>
        public void Bounce(float value)
        {
            velocity.y = value;
        }

        /// <summary>
        /// Bounce the objects velocity in a direction.
        /// </summary>
        /// <param name="dir"></param>
        public void Bounce(Vector2 dir)
        {
            velocity.y = dir.y;
            velocity.x = dir.x;
        }

        /// <summary>
        /// Teleport to some position.
        /// </summary>
        /// <param name="position"></param>
        public void Teleport(Vector3 position)
        {
            body.position = position;
            velocity *= 0;
            body.velocity *= 0;
        }

        protected virtual void OnEnable()
        {
            body = GetComponent<Rigidbody2D>();
            // body.isKinematic = true;
            body.isKinematic = Simulation.GetModel<PlatformerModel>().player == this;
        }

        protected virtual void OnDisable()
        {
            body.isKinematic = false;
        }

        protected virtual void Start()
        {
            contactFilter.useTriggers = false;
            contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
            contactFilter.useLayerMask = true;
        }

        protected virtual void Update()
        {
            targetVelocity = Vector2.zero;
            ComputeVelocity();
        }

        protected virtual void ComputeVelocity()
        {

        }

        protected virtual void FixedUpdate()
        {
            if (!immobile) {
                //if already falling, fall faster than the jump speed, otherwise use normal gravity.
                // if (velocity.y < 0)
                //     velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
                // else if (!flying)
                //     velocity += Physics2D.gravity * Time.deltaTime;
                if (velocity.y < 0) {
                    velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
                    if (Simulation.GetModel<PlatformerModel>().player == this && Simulation.GetModel<PlatformerModel>().player.GetComponent<PlayerController>().feetPower == PlayerController.Power.Air) {
                        if (velocity.y < -1) {
                            velocity = new Vector2(velocity.x, -1);
                        }
                    }
                } else {
                    // velocity = new Vector2(velocity.x + Physics2D.gravity.x * Time.deltaTime, velocity.y + (!flying ? (Physics2D.gravity.y * Time.deltaTime) : 0));
                    if (!flying) {
                        velocity += Physics2D.gravity * Time.deltaTime;
                    }
                }


                velocity.x = targetVelocity.x;
                if (flying)
                    velocity.y = targetVelocity.y;

                IsGrounded = false;

                var deltaPosition = velocity * Time.deltaTime;

                var moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);

                if (moveAlongGround.x == 0 && moveAlongGround.y == 0) {
                    moveAlongGround = new Vector2(freeMovement.x, freeMovement.y);
                }

                var move = moveAlongGround * deltaPosition.x;

                PerformMovement(move, false);

                move = Vector2.up * deltaPosition.y;

                PerformMovement(move, true);
            }

        }

        void PerformMovement(Vector2 move, bool yMovement)
        {
            var distance = move.magnitude;

            if (distance > minMoveDistance)
            {
                //check if we hit anything in current direction of travel
                var count = body.Cast(move, contactFilter, hitBuffer, distance + shellRadius);
                for (var i = 0; i < count; i++)
                {
                    var currentNormal = hitBuffer[i].normal;

                    //is this surface flat enough to land on?
                    if (currentNormal.y > minGroundNormalY)
                    {
                    IsGrounded = true;
                        // if moving up, change the groundNormal to new surface normal.
                        if (yMovement)
                        {
                            groundNormal = currentNormal;
                            currentNormal.x = 0;
                        }
                    } else if (currentNormal.y != -1) {
                        accel = 0f;
                    }
                    if (true)
                    {
                        //how much of our velocity aligns with surface normal?
                        var projection = Vector2.Dot(velocity, currentNormal);
                        if (projection < 0)
                        {
                            //slower velocity if moving against the normal (up a hill).
                            velocity = velocity - projection * currentNormal;
                        }
                    }
                    // else
                    // {
                    //     //We are airborne, but hit something, so cancel vertical up and horizontal velocity.
                    //     move.x *= 0;
                    //     velocity.x *= 0;
                    //     velocity.y = Mathf.Min(velocity.y, 0);
                    // }
                    //remove shellDistance from actual move distance.
                    var modifiedDistance = hitBuffer[i].distance - shellRadius;
                    distance = modifiedDistance < distance ? modifiedDistance : distance;
                }
            }
            body.position = body.position + move.normalized * distance;
        }

    }
}