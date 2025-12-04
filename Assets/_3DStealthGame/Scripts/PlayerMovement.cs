using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public InputAction MoveAction;

    public float walkSpeed = 1.0f;
    public float turnSpeed = 20f;
    public bool shield;

    Animator m_Animator;
    Rigidbody m_Rigidbody;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;
    
    public GameObject shieldPrefab;
    public GameObject ghostPrefab;


    void Start ()
    {
        m_Animator = GetComponent<Animator> ();
        m_Rigidbody = GetComponent<Rigidbody> ();
        MoveAction.Enable();
        shield = false;
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
}