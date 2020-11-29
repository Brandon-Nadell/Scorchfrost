using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using Platformer.Model;
using Platformer.Core;

namespace Platformer.Mechanics
{
    /// <summary>
    /// This is the main class used to implement control of the player.
    /// It is a superset of the AnimationController class, but is inlined to allow for any kind of customisation.
    /// </summary>
    public class PlayerController : KinematicObject
    {
        public AudioClip jumpAudio;
        public AudioClip respawnAudio;
        public AudioClip ouchAudio;

        /// <summary>
        /// Max horizontal speed of the player.
        /// </summary>
        public float maxSpeed = 7;
        /// <summary>
        /// Initial jump velocity at the start of a jump.
        /// </summary>
        public float jumpTakeOffSpeed = 7;

        public JumpState jumpState = JumpState.Grounded;
        private bool stopJump;
        /*internal new*/ public Collider2D collider2d;
        /*internal new*/ public AudioSource audioSource;
        public Health health;
        public bool controlEnabled = true;

        bool jump;
        /// <summary>
        /// Used to indicated desired direction of travel.
        /// </summary>
        public Vector2 move;
        bool movedLeftLast;
        SpriteRenderer spriteRenderer;
        internal Animator animator;
        readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public Bounds Bounds => collider2d.bounds;


        bool flipX;
        public SpriteRenderer head;
        public GameObject feet;

        public Power headPower;
        public Power feetPower;
        List<Power> powers;


        public SpriteRenderer fireHead;
        public GameObject fireFeet;
        public SpriteRenderer iceHead;
        public GameObject iceFeet;
        public SpriteRenderer earthHead;
        public GameObject earthFeet;
        public SpriteRenderer airHead;
        public GameObject airFeet;


        public enum Power {
            Fire,
            Ice,
            Earth,
            Air,
            None
        }

        void SetPowers() {
            switch (headPower) {
                case Power.Fire:
                    head = fireHead;
                    break;
                case Power.Ice:
                    head = iceHead;
                    break;
                case Power.Earth:
                    head = earthHead;
                    break;
                case Power.Air:
                    head = airHead;
                    break;
                case Power.None:
                    head = null;
                    break;
            }
            switch (feetPower) {
                case Power.Fire:
                    feet = fireFeet;
                    break;
                case Power.Ice:
                    feet = iceFeet;
                    break;
                case Power.Earth:
                    feet = earthFeet;
                    break;
                case Power.Air:
                    feet = airFeet;
                    break;
                case Power.None:
                    feet = null;
                    break;
            }
        }

        void Awake()
        {
            health = GetComponent<Health>();
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();

            powers = new List<Power>();
        }

        protected override void Update()
        {
            if (controlEnabled)
            {
                if (Input.GetKey("a")) {
                    movedLeftLast = true;
                } else if (Input.GetKey("d")) {
                    movedLeftLast = false;
                }
                if (Input.GetKey("a") || Input.GetKey("d")) {
                    accel = Mathf.Min(accel + .1f, 1f);
                } else {
                    accel = Mathf.Max(accel - .1f, 0f);
                }
                move.x = movedLeftLast ? -accel : accel;

                if (jumpState == JumpState.Grounded && Input.GetButton("Jump"))
                    jumpState = JumpState.PrepareToJump;
                else if (Input.GetButtonUp("Jump"))
                {
                    stopJump = true;
                    Schedule<PlayerStopJump>().player = this;
                }

                if (Input.GetButtonDown("Swap")) {
                    if (head != null)
                        head.gameObject.SetActive(false);
                    if (feet != null)
                        feet.SetActive(false);

                    Power temp = headPower;
                    headPower = feetPower;
                    feetPower = temp;
                    SetPowers();

                    if (head != null)
                        head.gameObject.SetActive(true);
                    if (feet != null)
                        feet.SetActive(true);

                    SetFlips();
                }

                if (headPower != Power.Ice) {
                    flying = false;
                }
            }
            else
            {
                move.x = 0;
            }
            UpdateJumpState();
            base.Update();
        }

        void UpdateJumpState()
        {
            jump = false;
            switch (jumpState)
            {
                case JumpState.PrepareToJump:
                    jumpState = JumpState.Jumping;
                    jump = true;
                    stopJump = false;
                    break;
                case JumpState.Jumping:
                    if (!IsGrounded)
                    {
                        Schedule<PlayerJumped>().player = this;
                        jumpState = JumpState.InFlight;
                    }
                    break;
                case JumpState.InFlight:
                    if (IsGrounded)
                    {
                        Schedule<PlayerLanded>().player = this;
                        jumpState = JumpState.Landed;
                    }
                    break;
                case JumpState.Landed:
                    jumpState = JumpState.Grounded;
                    break;
            }
        }

        protected override void ComputeVelocity()
        {
            if (jump && IsGrounded)
            {
                velocity.y = jumpTakeOffSpeed;
                jump = false;
            }
            else if (stopJump)
            {
                
                stopJump = false;
                // if (velocity.y > 0)
                // {
                //     velocity.y = velocity.y * model.jumpDeceleration;
                // }
            }

            if (move.x > .0f)
                flipX = false;
            else if (move.x < -.01)
                flipX = true;
            SetFlips();

            animator.SetBool("grounded", IsGrounded);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

            targetVelocity = move * maxSpeed;
        }

        public void SetFlips() {
            spriteRenderer.flipX = flipX;
            if (head != null)
                head.flipX = flipX;
            if (feet != null) {
                feet.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().flipX = flipX;
                feet.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().flipX = flipX;
                feet.transform.localPosition = new Vector3(flipX ? .02f : 0, 0, 0);
            }
            // if (move.x > 0.01f) {
            //     spriteRenderer.flipX = false;
            //     if (head != null)
            //         head.flipX = false;
            //     if (feet != null) {
            //         feet.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().flipX = false;
            //         feet.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().flipX = false;
            //         feet.transform.localPosition = new Vector3(0, 0, 0);
            //     }
            // } else if (move.x < -0.01f) {
            //     spriteRenderer.flipX = true;
            //     if (head != null)
            //         head.flipX = true;
            //    if (feet != null) {
            //         feet.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().flipX = true;
            //         feet.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().flipX = true;
            //         feet.transform.localPosition = new Vector3(.02f, 0, 0);
            //     }
            // }
        }

        public void AddPower(Power power) {
            powers.Add(power);
            if (headPower == Power.None) {
                headPower = power;
                SetPowers();
                head.gameObject.SetActive(true);
                SetFlips();
            } else if (feetPower == Power.None) {
                feetPower = power;
                SetPowers();
                feet.SetActive(true);
                SetFlips();
            }
        }

        public enum JumpState
        {
            Grounded,
            PrepareToJump,
            Jumping,
            InFlight,
            Landed
        }
    }
}