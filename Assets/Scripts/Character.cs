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
    public SIDE Side = SIDE.Mid;
    public bool SwipeLeft = false;
    public bool SwipeRight = false;
    public float XValue = 2.0f;
    public string AnimLeft = "dodgeLeft";
    public string AnimRight = "dodgeRight";
    public float SpeedDodge = 10.0f;

    float newXPos = 0.0f;
    float x;
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
        m_controller.Move(new Vector3(x - transform.position.x, 0, 0));
    }
}
