using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public InputAction MoveAction;
    public InputAction UnfreezeAction;

    public float walkSpeed = 1.0f;
    public float turnSpeed = 20f;
    public bool shield;

    public float freezeIntervalMin = 5f;
    public float freezeIntervalMax = 12f;
    public int requiredPresses = 5;

    private bool isFrozen = false;
    private int unfreezePresses = 0;
    private float nextFreezeTime;

    public TextMeshProUGUI freezeText;   

    Animator m_Animator;
    Rigidbody m_Rigidbody;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;
    
    public GameObject shieldPrefab;
    public GameObject ghostPrefab;


    void Start()
{
    m_Animator = GetComponent<Animator>();
    m_Rigidbody = GetComponent<Rigidbody>();

    MoveAction.Enable();

    //  Binding unfreeze key
    UnfreezeAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/e");
    UnfreezeAction.Enable();

    shield = false;

    nextFreezeTime = Time.time + Random.Range(freezeIntervalMin, freezeIntervalMax);

    if (freezeText != null)
        freezeText.gameObject.SetActive(false);

    sprintIconActive.SetActive(false);
    sprintIconOff.SetActive(true);
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
    
        //Vector3 desiredForward = Vector3.RotateTowards (transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
        m_Rotation = Quaternion.LookRotation (desiredForward);
        
        m_Rigidbody.MoveRotation (m_Rotation);
        m_Rigidbody.MovePosition (m_Rigidbody.position + m_Movement * walkSpeed * Time.deltaTime); 
    
    }
    void Update()
    {
        Sprint();
    }
        

        
    private void OnTriggerEnter(Collider whatDidIHit)
    {
        if(whatDidIHit.tag == "Powerup")
        {
            Destroy(whatDidIHit.gameObject);
            if (shield == false)
            {
                shield = true;
                shieldPrefab = Instantiate(shieldPrefab);
                shieldPrefab.transform.parent = transform;
                shieldPrefab.transform.position = new Vector3 (transform.position.x - 1.633f, transform.position.y - 1.248f, transform.position.z - 3.143f);
            } else
            {
                
            }
        }
        if(whatDidIHit.tag == "Enemy")
        {
            if (shield == true)
            {
                Destroy(whatDidIHit.gameObject);
                shield = false;
                Destroy(shieldPrefab);
            }
        }
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




