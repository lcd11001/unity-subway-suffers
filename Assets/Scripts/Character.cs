using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum SIDE { Left = -2, Mid = 0, Right = 2 }

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class Character : MonoBehaviour
{
    public float ForwardSpeed = 5.0f;
    public float SpeedDodge = 10.0f;
    public float JumpPower = 7f;
    public float RollDuration = 0.5f;
    public float StumbleTolerance = 10f;

    public Collider CharacterCollider;

    public SIDE Side = SIDE.Mid;
    public SIDE LastSide = SIDE.Mid;

    [SerializeField]
    bool canInput = true;

    [Space(10)]
    [Header("Animation Config")]
    public string AnimLeft = "dodgeLeft";
    public string AnimRight = "dodgeRight";
    public string AnimJump = "jump";
    public string AnimFalling = "falling";
    public string AnimLanding = "landing";
    public string AnimRoll = "roll";

    public string AnimDeathDown = "death_lower";
    public string AnimDeathUp = "death_upper";
    public string AnimDeathBounce = "death_bounce";
    public string AnimDeathTrain = "death_movingTrain";

    public string AnimStumbleSideRight = "stumbleSideRight";
    public string AnimStumbleSideLeft = "stumbleSideLeft";
    public string AnimStumbleCornerRight = "stumbleCornerRight";
    public string AnimStumbleCornerLeft = "stumbleCornerLeft";
    public string AnimStumbleLeft = "stumbleOffLeft";
    public string AnimStumbleRight = "stumbleOffRight";
    public string AnimStumbleLow = "stumble_low";




    bool SwipeLeft = false;
    bool SwipeRight = false;
    bool SwipeUp = false;
    bool SwipeDown = false;

    float x = 0.0f;
    float y = 0.0f;
    bool isJumping = false;
    bool isRolling = false;
    float rollTimer = 0.0f;
    float stumbleTimer = 0.0f;

    float colliderHeight = 0.0f;
    float colliderCenterY = 0.0f;

    CharacterController m_controller;
    Animator m_animator;

    bool stopAllState = false;



    void Start()
    {
        m_controller = GetComponent<CharacterController>();
        m_controller.detectCollisions = false;
        colliderHeight = m_controller.height;
        colliderCenterY = m_controller.center.y;

        m_animator = GetComponent<Animator>();

        transform.position = Vector3.zero;
        stumbleTimer = StumbleTolerance;
    }


    void Update()
    {
        CharacterCollider.isTrigger = !canInput;
        if (!canInput)
        {
            m_controller.Move(Vector3.down * 10f * Time.deltaTime);
            return;
        }

        SwipeLeft = (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) && canInput;
        SwipeRight = (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) && canInput;
        SwipeUp = (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && canInput;
        SwipeDown = (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && canInput;

        if (m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            DisableStumbleLayer();
            stopAllState = false;
        }
        stumbleTimer = Mathf.MoveTowards(stumbleTimer, StumbleTolerance, Time.deltaTime);

        Swipe();
        Jump();
        Roll();

        Vector3 moveVector = new Vector3(x - transform.position.x, y * Time.deltaTime, ForwardSpeed * Time.deltaTime);
        m_controller.Move(moveVector);
    }

    private void Swipe()
    {
        if (SwipeLeft && !isRolling)
        {
            if (Side == SIDE.Mid)
            {
                LastSide = Side;
                Side = SIDE.Left;
                PlayAnim(AnimLeft);
            }
            else if (Side == SIDE.Right)
            {
                LastSide = Side;
                Side = SIDE.Mid;
                PlayAnim(AnimLeft);
            }
            else if (Side != LastSide)
            {
                LastSide = Side;
                PlayAnim(AnimStumbleLeft);
            }

        }
        else if (SwipeRight && !isRolling)
        {
            if (Side == SIDE.Mid)
            {
                LastSide = Side;
                Side = SIDE.Right;
                PlayAnim(AnimRight);
            }
            else if (Side == SIDE.Left)
            {
                LastSide = Side;
                Side = SIDE.Mid;
                PlayAnim(AnimRight);
            }
            else if (Side != LastSide)
            {
                LastSide = Side;
                PlayAnim(AnimStumbleRight);
            }
        }

        x = Mathf.Lerp(x, (int)Side, SpeedDodge * Time.deltaTime);
    }

    private void Jump()
    {
        // Debug.Log("isGrounded: " + m_controller.isGrounded);
        // Debug.Log("Position: " + transform.position);
        // Debug.Log("Velocity: " + m_controller.velocity);

        if (m_controller.isGrounded)
        {
            if (m_animator.GetCurrentAnimatorStateInfo(0).IsName(AnimFalling))
            {
                PlayAnim(AnimLanding);
                isJumping = false;
                y = 0.0f;
            }
            if (SwipeUp)
            {
                y = JumpPower;
                m_animator.CrossFadeInFixedTime(AnimJump, 0.1f);
                isJumping = true;
            }
        }
        else
        {
            y -= JumpPower * 2 * Time.deltaTime;
            if (m_controller.velocity.y < -0.1f)
            {
                PlayAnim(AnimFalling);
            }
        }
    }

    private void Roll()
    {
        if (isRolling)
        {
            rollTimer -= Time.deltaTime;

            if (rollTimer <= 0)
            {
                rollTimer = 0;
                isRolling = false;

                // reset collider to original size
                m_controller.height = colliderHeight;
                m_controller.center = new Vector3(m_controller.center.x, colliderCenterY, m_controller.center.z);
            }
        }
        else if (SwipeDown)
        {
            if (m_controller.isGrounded)
            {
                //y -= 10f;
                rollTimer = RollDuration;
                m_animator.CrossFadeInFixedTime(AnimRoll, 0.1f);
                isRolling = true;
                isJumping = false;

                // reduce collider size
                m_controller.height = colliderHeight / 2;
                m_controller.center = new Vector3(m_controller.center.x, colliderCenterY / 2, m_controller.center.z);
            }
        }
    }

    public bool OnDeath(HIT_X hitX, HIT_Y hitY, HIT_Z hitZ, string tag)
    {
        if (hitZ == HIT_Z.Front && hitX == HIT_X.Mid)
        {
            if (hitY == HIT_Y.Up)
            {
                PlayStumbleAnim(AnimStumbleLow);
                return true;
            }
            else if (hitY == HIT_Y.Mid)
            {
                if (tag == "MovingTrain")
                {
                    StartCoroutine(PlayDeathAnim(AnimDeathTrain));
                    return true;
                }
                else if (tag != "Ramp")
                {
                    StartCoroutine(PlayDeathAnim(AnimDeathDown));
                    return true;
                }
            }
            else if (hitY == HIT_Y.Down)
            {
                StartCoroutine(PlayDeathAnim(AnimDeathBounce));
                return true;
            }
            else if (hitY == HIT_Y.Low && !isRolling)
            {
                StartCoroutine(PlayDeathAnim(AnimDeathUp));
                return true;
            }
        }
        else if (hitZ == HIT_Z.Mid)
        {
            if (hitX == HIT_X.Right)
            {
                // swipe back to last side
                Side = LastSide;
                PlayStumbleAnim(AnimStumbleSideLeft);
                return true;
            }
            else if (hitX == HIT_X.Left)
            {
                // swipe back to last side
                Side = LastSide;
                PlayStumbleAnim(AnimStumbleSideRight);
                return true;
            }
        }
        else
        {
            if (hitX == HIT_X.Right)
            {
                EnableStumbleLayer();
                PlayStumbleAnim(AnimStumbleCornerLeft);
                return true;
            }
            else if (hitX == HIT_X.Left)
            {
                EnableStumbleLayer();
                PlayStumbleAnim(AnimStumbleCornerRight);
                return true;
            }
        }

        return false;
    }

    private void PlayAnim(string anim)
    {
        if (stopAllState)
        {
            return;
        }

        Debug.Log("Play: " + anim);
        m_animator.Play(anim);
    }

    private IEnumerator PlayDeathAnim(string anim)
    {
        Debug.Log("Death: " + anim);
        DisableStumbleLayer();

        stopAllState = true;
        m_animator.Play(anim);
        yield return new WaitForSeconds(0.2f);
        canInput = false;
    }

    private void PlayStumbleAnim(string anim)
    {
        Debug.Log("Stumble: " + anim);
        // Deprecated
        //m_animator.ForceStateNormalizedTime(0);
        //PlayAnim(anim);

        // Replace with:
        m_animator.Play(anim, -1, 0);
        stopAllState = true;

        if (stumbleTimer < StumbleTolerance / 2)
        {
            // stumble 2 times in a row
            StartCoroutine(PlayDeathAnim(AnimStumbleLow));
            return;
        }

        stumbleTimer -= 0.6f * StumbleTolerance;
    }

    private void EnableStumbleLayer(float weight = 1.0f)
    {
        m_animator.SetLayerWeight(1, weight);
    }

    private void DisableStumbleLayer()
    {
        m_animator.SetLayerWeight(1, 0);
    }
}
