using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using Platformer.Model;
using Platformer.Core;
using UnityEngine.UI;
using TMPro;
using static System.Array;
using System;
using System.Linq;

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
        public AudioClip freezeAudio;
        public AudioClip fireAudio;
        public AudioClip powerstationAudio;
        public AudioClip swapAudio;

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
        public Checkpoint checkpoint;
        public TextMeshProUGUI livesText;
        public int lives;
        public int livesMax = 5;
        public bool touchingPowerStation;
        int powerstationindex;
        public GameObject currentPopup;
        public bool victory;

        public Bounds Bounds => collider2d.bounds;
        // public List<Pusher> pushers;
        public Pusher pusherL;
        public Pusher pusherR;
        public Pusher pusherB;


        bool flipX;
        public SpriteRenderer head;
        public GameObject feet;

        public Power headPower;
        public Power feetPower;
        public List<Power> powers;


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

        public void SetPowers() {
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

            ResetLives();
        }

        protected override void FixedUpdate() {
            base.FixedUpdate();
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
                    accel = Mathf.Max(accel - (IsGrounded ? .1f: .05f), 0f);
                }
                move.x = movedLeftLast ? -accel : accel;

            }
        }

        protected override void Update()
        {
            if (controlEnabled)
            {
                if (jumpState == JumpState.Grounded && Input.GetButton("Jump"))
                    jumpState = JumpState.PrepareToJump;
                else if (Input.GetButtonUp("Jump"))
                {
                    stopJump = true;
                    Schedule<PlayerStopJump>().player = this;
                }

                if (Input.GetKey("left alt") || Input.GetKey("right alt")) {
                    int index = -1;
                    if (Input.GetKeyDown("1")) {
                        index = 0;
                    } else if (Input.GetKeyDown("2")) {
                        index = 1;
                    } else if (Input.GetKeyDown("3")) {
                        index = 2;
                    } else if (Input.GetKeyDown("4")) {
                        index = 3;
                    } else if (Input.GetKeyDown("r")) {
                        Application.LoadLevel(Application.loadedLevel);
                        return;
                    }
                    if (index != -1) {
                        List<Checkpoint> checkpoints = FindObjectsOfType<Checkpoint>().OrderBy(cp => cp.name).ToList();
                        transform.position = new Vector3(checkpoints[index].transform.position.x, checkpoints[index].transform.position.y, -1);
                    }
                }

                if (Input.GetButtonDown("Swap")) {
                    if (!touchingPowerStation) {
                        if (head != null || feet != null) {
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
                            GetComponent<AudioSource>().PlayOneShot(swapAudio, 1f);
                        }
                    } else {
                        if (powers.Count >= 3) {
                            PlayerController.Power newpower;
                            do {
                                powerstationindex++;
                                newpower = powers[powerstationindex % powers.Count];
                            } while (newpower == feetPower || newpower == headPower);
                            // Debug.Log(newpower);

                            if (head != null)
                                head.gameObject.SetActive(false);
                            headPower = newpower;
                            SetPowers();
                            if (head != null)
                                head.gameObject.SetActive(true);
                            SetFlips();
                            GetComponent<AudioSource>().PlayOneShot(powerstationAudio, 1f);
                        }
                    }
                } else {
                    // touchingPowerStation = false;
                }

                if (headPower != Power.Ice) {
                    flying = false;
                }

                if (feetPower == Power.Air) {
                    gravityModifier = .1f;
                } else {
                    gravityModifier = 1.5f;
                }

                if (headPower == Power.Air) {
                    // pusher.gameObject.SetActive(true);
                    pusherL.gameObject.SetActive(velocity.x > .01);
                    pusherR.gameObject.SetActive(velocity.x < -.01);
                    pusherB.gameObject.SetActive(true);
                    // pusherL.gameObject.transform.position = new Vector3(transform.position.x - .36f, transform.position.y + .78f, -1);
                    // pusherR.gameObject.transform.position = new Vector3(transform.position.x + .36f, transform.position.y + .78f, -1);
                    // pusherB.gameObject.transform.position = new Vector3(transform.position.x, transform.position.y + .55f, -1);
                } else {
                    // pusher.gameObject.SetActive(false);
                    pusherL.gameObject.SetActive(false);
                    pusherR.gameObject.SetActive(false);
                    pusherB.gameObject.SetActive(false);
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
        }

        public void AddPower(Power power) {
            if (!powers.Contains(power)) {
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
        }

        public enum JumpState
        {
            Grounded,
            PrepareToJump,
            Jumping,
            InFlight,
            Landed
        }

        public void SetCheckpoint(Checkpoint checkpoint) {
            this.checkpoint = checkpoint;
        }

        public void ResetLives() {
            lives = Mathf.Max(lives, livesMax);
            livesText.text = "Lives: " + lives;
        }

        public void AddLives(int amt) {
            lives += amt;
            livesText.text = "Lives: " + lives;
        }
    }
}