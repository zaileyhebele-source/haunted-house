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
    private float sprintCooldown = 2.0f;
    private float sprintDuration = 5.0f;
    public GameObject sprintIconActive;
    public GameObject sprintIconOff;
private void Sprint(){

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isSprintOnCooldown){
            StartCoroutine(SprintRoutine());
        }
    }
    //sprint coroutine
    private IEnumerator SprintRoutine(){
        isSprinting = true;
        walkSpeed *= 2;
        turnSpeed *= 2;

    yield return new WaitForSeconds(sprintDuration);

        walkSpeed /= 2;
        turnSpeed /= 2;
        isSprinting = false;
        isSprintOnCooldown = true;
        
        //show sprint ui
        sprintIconActive.SetActive(true);
        sprintIconOff.SetActive(false);

    yield return new WaitForSeconds(sprintCooldown);
        isSprintOnCooldown = false;
        sprintIconActive.SetActive(false);
        sprintIconOff.SetActive(true);
    }

}

