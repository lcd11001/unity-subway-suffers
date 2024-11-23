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

    public SIDE Side = SIDE.Mid;
    public SIDE LastSide = SIDE.Mid;

    [Space(10)]
    [Header("Animation Config")]
    public string AnimLeft = "dodgeLeft";
    public string AnimRight = "dodgeRight";
    public string AnimJump = "jump";
    public string AnimFalling = "falling";
    public string AnimLanding = "landing";
    public string AnimRoll = "roll";

    public string AnimDeathDown = "death_lower";
    public string AnimDeathLow = "stumble_low";
    public string AnimDeathUp = "death_upper";
    public string AnimDeathBounce = "death_bounce";
    public string AnimDeathTrain = "death_movingTrain";

    public string AnimStumbleSideRight = "stumbleSideRight";
    public string AnimStumbleSideLeft = "stumbleSideLeft";
    public string AnimStumbleCornerRight = "stumbleCornerRight";
    public string AnimStumbleCornerLeft = "stumbleCornerLeft";
    public string AnimStumbleLeft = "stumbleOffLeft";
    public string AnimStumbleRight = "stumbleOffRight";




    bool SwipeLeft = false;
    bool SwipeRight = false;
    bool SwipeUp = false;
    bool SwipeDown = false;

    float x = 0.0f;
    float y = 0.0f;
    bool isJumping = false;
    bool isRolling = false;
    float rollTimer = 0.0f;

    float colliderHeight = 0.0f;
    float colliderCenterY = 0.0f;

    CharacterController m_controller;
    Animator m_animator;

    void Start()
    {
        m_controller = GetComponent<CharacterController>();
        m_controller.detectCollisions = false;
        colliderHeight = m_controller.height;
        colliderCenterY = m_controller.center.y;

        m_animator = GetComponent<Animator>();

        transform.position = Vector3.zero;
    }


    void Update()
    {
        SwipeLeft = Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A);
        SwipeRight = Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D);
        SwipeUp = Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);
        SwipeDown = Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S);

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
                m_animator.Play(AnimLeft);
            }
            else if (Side == SIDE.Right)
            {
                LastSide = Side;
                Side = SIDE.Mid;
                m_animator.Play(AnimLeft);
            }
            else
            {
                LastSide = Side;
                m_animator.Play(AnimStumbleLeft);
            }

        }
        else if (SwipeRight && !isRolling)
        {
            if (Side == SIDE.Mid)
            {
                LastSide = Side;
                Side = SIDE.Right;
                m_animator.Play(AnimRight);
            }
            else if (Side == SIDE.Left)
            {
                LastSide = Side;
                Side = SIDE.Mid;
                m_animator.Play(AnimRight);
            }
            else
            {
                LastSide = Side;
                m_animator.Play(AnimStumbleRight);
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
                m_animator.Play(AnimLanding);
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
                m_animator.Play(AnimFalling);
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

    public void OnDeath(HIT_X hitX, HIT_Y hitY, HIT_Z hitZ, string tag)
    {
        if (hitZ == HIT_Z.Front && hitX == HIT_X.Mid)
        {
            if (hitY == HIT_Y.Up && !isRolling)
            {
                Debug.Log("Dead: " + AnimDeathLow);
                m_animator.Play(AnimDeathLow);
            }
            else if (hitY == HIT_Y.Mid)
            {
                if (tag == "MovingTrain")
                {
                    Debug.Log("Dead: " + AnimDeathTrain);
                    m_animator.Play(AnimDeathTrain);
                }
                else if (tag != "Ramp")
                {
                    Debug.Log("Dead: " + AnimDeathDown);
                    m_animator.Play(AnimDeathDown);
                }
            }
            else if (hitY == HIT_Y.Down)
            {
                Debug.Log("Death: " + AnimDeathBounce);
                m_animator.Play(AnimDeathBounce);
            }
            else if (hitY == HIT_Y.Low)
            {
                Debug.Log("Death: " + AnimDeathUp);
                m_animator.Play(AnimDeathUp);
            }
        }
        else if (hitZ == HIT_Z.Mid)
        {
            if (hitX == HIT_X.Right)
            {
                // swipe back to last side
                Side = LastSide;
                Debug.Log("Death: " + AnimStumbleSideLeft);
                m_animator.Play(AnimStumbleSideLeft);
            }
            else if (hitX == HIT_X.Left)
            {
                // swipe back to last side
                Side = LastSide;
                Debug.Log("Death: " + AnimStumbleSideRight);
                m_animator.Play(AnimStumbleSideRight);
            }
        }
        else
        {
            if (hitX == HIT_X.Right)
            {
                Debug.Log("Death: " + AnimStumbleCornerLeft);
                m_animator.Play(AnimStumbleCornerLeft);
            }
            else if (hitX == HIT_X.Left)
            {
                Debug.Log("Death: " + AnimStumbleCornerRight);
                m_animator.Play(AnimStumbleCornerRight);
            }
        }
    }
}
