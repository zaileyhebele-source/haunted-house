using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;   

public class PlayerMovement : MonoBehaviour
{
    public InputAction MoveAction;
    public InputAction UnfreezeAction;

    public float walkSpeed = 1.0f;
    public float turnSpeed = 20f;

    public float freezeIntervalMin = 5f;
    public float freezeIntervalMax = 12f;
    public int requiredPresses = 5;

    private bool isFrozen = false;
    private int unfreezePresses = 0;
    private float nextFreezeTime;

    public Text freezeText;   

    Animator m_Animator;
    Rigidbody m_Rigidbody;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;

    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();

        MoveAction.Enable();
        UnfreezeAction.Enable();

        nextFreezeTime = Time.time + Random.Range(freezeIntervalMin, freezeIntervalMax);

        if (freezeText != null)
            freezeText.gameObject.SetActive(false);
    }

    void FixedUpdate()
    {
        //  Trigger random freeze
        if (!isFrozen && Time.time >= nextFreezeTime)
        {
            isFrozen = true;
            unfreezePresses = 0;

            m_Animator.SetBool("IsWalking", false);

            if (freezeText != null)
            {
                freezeText.gameObject.SetActive(true);
                freezeText.text = $"Mash E to overcome fear! (0 / {requiredPresses})";
            }

            return;
        }

        //  While frozen
        if (isFrozen)
        {
            if (UnfreezeAction.WasPerformedThisFrame())
            {
                unfreezePresses++;

                if (freezeText != null)
                    freezeText.text = $"Mash E to overcome fear! ({unfreezePresses} / {requiredPresses})";

                if (unfreezePresses >= requiredPresses)
                {
                    isFrozen = false;
                    nextFreezeTime = Time.time + Random.Range(freezeIntervalMin, freezeIntervalMax);

                    if (freezeText != null)
                        freezeText.gameObject.SetActive(false);
                }
            }

            return; 
        }

        //  Normal movement
        var pos = MoveAction.ReadValue<Vector2>();

        float horizontal = pos.x;
        float vertical = pos.y;

        m_Movement.Set(horizontal, 0f, vertical);
        m_Movement.Normalize();

        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;

        m_Animator.SetBool("IsWalking", isWalking);

        Vector3 desiredForward = Vector3.RotateTowards(
            transform.forward,
            m_Movement,
            turnSpeed * Time.deltaTime,
            0f
        );

        m_Rotation = Quaternion.LookRotation(desiredForward);

        m_Rigidbody.MoveRotation(m_Rotation);
        m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * walkSpeed * Time.deltaTime);
    }
}
