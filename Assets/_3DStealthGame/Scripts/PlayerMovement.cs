using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public InputAction MoveAction;

    public float walkSpeed = 1.0f;
    public float turnSpeed = 20f;

    Animator m_Animator;
    Rigidbody m_Rigidbody;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;

    void Start ()
    {
        m_Animator = GetComponent<Animator> ();
        m_Rigidbody = GetComponent<Rigidbody> ();
        MoveAction.Enable();
        sprintIconActive.SetActive(false);
        sprintIconOff.SetActive(true);
    }

    void FixedUpdate ()
    {
        var pos = MoveAction.ReadValue<Vector2>();
        
        float horizontal = pos.x;
        float vertical = pos.y;
        
        m_Movement.Set(horizontal, 0f, vertical);
        m_Movement.Normalize ();
        bool hasHorizontalInput = !Mathf.Approximately (horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately (vertical, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;
        m_Animator.SetBool ("IsWalking", isWalking);

        Vector3 desiredForward = Vector3.RotateTowards (transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
        m_Rotation = Quaternion.LookRotation (desiredForward);
        
        m_Rigidbody.MoveRotation (m_Rotation);
        m_Rigidbody.MovePosition (m_Rigidbody.position + m_Movement * walkSpeed * Time.deltaTime);

        Sprint();


        
    }
 //Sprint Function
    private bool isSprintOnCooldown = false;
    private bool isSprinting = false;
    private float sprintStartTime;
    private float sprintCooldown = 2.0f;
    private float sprintMaxDuration = 3.0f;
    public GameObject sprintIconActive;
    public GameObject sprintIconOff;
    public GameObject sprintIconReady;
private void Sprint()
{
    // Start sprint
    if (Input.GetKeyDown(KeyCode.LeftShift) && !isSprinting && !isSprintOnCooldown)
    {
        isSprinting = true;
        sprintStartTime = Time.time;
        walkSpeed *= 2;
        turnSpeed *= 2;

        sprintIconActive.SetActive(true);
        sprintIconReady.SetActive(false);
        sprintIconOff.SetActive(false);
    }

    // Stop sprint either when key released OR max duration reached
    if ((isSprinting && !Input.GetKey(KeyCode.LeftShift)) || (isSprinting && Time.time - sprintStartTime >= sprintMaxDuration))
    {
        isSprinting = false;
        walkSpeed /= 2;
        turnSpeed /= 2;

        isSprintOnCooldown = true;
        sprintIconActive.SetActive(false);
        sprintIconOff.SetActive(true);

        StartCoroutine(SprintCooldownRoutine());
    }
}
    //sprint Cooldown
    private IEnumerator SprintCooldownRoutine()
{
    yield return new WaitForSeconds(sprintCooldown);
    isSprintOnCooldown = false;

    sprintIconOff.SetActive(false);
    sprintIconReady.SetActive(true);
}
}




