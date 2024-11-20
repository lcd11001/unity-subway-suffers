using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum SIDE { Left, Mid, Right }

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class Character : MonoBehaviour
{
    public float XValue = 2.0f;
    public float SpeedDodge = 10.0f;
    public float JumpPower = 7f;

    [Space(10)]
    [Header("Animation Config")]
    public string AnimLeft = "dodgeLeft";
    public string AnimRight = "dodgeRight";
    public string AnimJump = "jump";
    public string AnimFalling = "falling";
    public string AnimLanding = "landing";

    SIDE Side = SIDE.Mid;
    bool SwipeLeft = false;
    bool SwipeRight = false;
    bool SwipeUp = false;
    bool SwipeDown = false;

    float newXPos = 0.0f;
    float newYPos = 0.0f;
    float x = 0.0f;
    float y = 0.0f;
    bool isJumping = false;
    bool isScrolling = false;

    CharacterController m_controller;
    Animator m_animator;

    void Start()
    {
        m_controller = GetComponent<CharacterController>();
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

        Vector3 moveVector = new Vector3(x - transform.position.x, y, 0);
        m_controller.Move(moveVector);
    }

    private void Swipe()
    {
        if (SwipeLeft)
        {
            if (Side == SIDE.Mid)
            {
                Side = SIDE.Left;
                newXPos = -XValue;
            }
            else if (Side == SIDE.Right)
            {
                Side = SIDE.Mid;
                newXPos = 0.0f;
            }
            m_animator.Play(AnimLeft);
        }
        else if (SwipeRight)
        {
            if (Side == SIDE.Mid)
            {
                Side = SIDE.Right;
                newXPos = XValue;
            }
            else if (Side == SIDE.Left)
            {
                Side = SIDE.Mid;
                newXPos = 0.0f;
            }
            m_animator.Play(AnimRight);
        }

        x = Mathf.Lerp(x, newXPos, SpeedDodge * Time.deltaTime);
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
                newYPos = 0.0f;
            }
            if (SwipeUp)
            {
                newYPos = JumpPower;
                m_animator.CrossFadeInFixedTime(AnimJump, 0.1f);
                isJumping = true;
            }
        }
        else
        {
            newYPos -= JumpPower * 2 * Time.deltaTime;
            if (m_controller.velocity.y < -0.1f)
            {
                m_animator.Play(AnimFalling);
            }
        }

        y = newYPos * Time.deltaTime;
    }
}
