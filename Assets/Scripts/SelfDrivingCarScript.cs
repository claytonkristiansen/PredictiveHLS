using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelfDrivingCarScript : MonoBehaviour
{
    /// <summary>
    /// Multiplier used to calculate effectiveness of the parking brake
    /// </summary>
    public float m_Speed = 0;
    /// <summary>
    /// The car's current rotation
    /// </summary>
    private Vector3 m_Rotation;
    /// <summary>
    /// 
    /// </summary>
    private Ray m_Ray;
    /// <summary>
    /// 
    /// </summary>
    private Ray m_HeightRay;
    /// <summary>
    /// Reference to the car's transform compenent
    /// </summary>
    private Transform m_Tr;
    /// <summary>
    /// Reference to the car's rigidbody compenent
    /// </summary>
    private Rigidbody m_Rb;
    /// <summary>
    /// Reference to the car's position
    /// </summary>
    private Vector3 m_Pos;
    /// <summary>
    /// Reference to the car's position
    /// </summary>
    public Quaternion m_RotationVector;
    /// <summary>
    /// 0 if no input and
    /// 1 if input
    /// </summary>
    private float VerticalInput, HorizontalInput;
    /// <summary>
    /// Minimun height the car can be at
    /// </summary>
    private float m_CarHeightMin;
    /// <summary>
    /// Is the car on the ground?
    /// </summary>
    public bool m_Grounded;
    /// <summary>
    /// Reference to the car's mass
    /// </summary>
    private float m_Mass;
    /// <summary>
    /// Reference to the car's velocity 
    /// </summary>
    public Vector3 m_CarVelocity;
    /// <summary>
    /// Car's original position
    /// </summary>
    private Vector3 m_OriginalPos;
    /// <summary>
    /// Car's original position
    /// </summary>
    private Quaternion m_OriginalRot;
    /// <summary>
    /// Reference to the car's velocity
    /// </summary>
    public Vector3 Velocity;

    // Use this for initialization
    void Start()
    {
        //------------------------------------------------------------------------------------------------------------------//
        //Sets the minimum height of the car above the ground
        m_CarHeightMin = 1;
        //------------------------------------------------------------------------------------------------------------------//
        //Initializing the rigidbody and transform variables to be used in the script
        m_Rb = GetComponent<Rigidbody>();
        m_Tr = GetComponent<Transform>();
        m_OriginalPos = m_Tr.position;
        m_OriginalRot = m_Tr.rotation;
        //------------------------------------------------------------------------------------------------------------------//
        //Sets a local Ray variable to point up from car's initial spawn point
        m_Ray = new Ray(m_Tr.position, m_Tr.up);
        //------------------------------------------------------------------------------------------------------------------//
        //Sets a local Ray variable to point forward from car's initial spawn point
        m_HeightRay = new Ray(m_Tr.position, m_Tr.forward);
        //------------------------------------------------------------------------------------------------------------------//
        //Sets a variable m_Pos to the Car's position
        m_Pos = m_Tr.position;
        //------------------------------------------------------------------------------------------------------------------//
        //Sets a variable m_Rotation tp the Car's rotation
        m_Rotation = m_Tr.rotation.eulerAngles;
        //------------------------------------------------------------------------------------------------------------------//
        //Sets the acceleration of gravity
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!StaticVariables.isPaused)
        {
            m_Rb.isKinematic = false;
            //------------------------------------------------------------------------------------------------------------------//
            //Checks to see if character is grounded
            if (Physics.Raycast(m_Tr.position, m_Tr.right, m_CarHeightMin))
            {
                m_Grounded = true;
            }
            else
            {
                m_Grounded = false;
            }
            //------------------------------------------------------------------------------------------------------------------//
            Rotation();
            //Moves Character
            SelfDrivingMovement();
            
            //Checks to see if Car is still in vertical boundary
            BoundaryCheck(-10, 20);
        }
        else if (StaticVariables.isPaused)
        {
            m_Rb.isKinematic = true;
            m_Rb.velocity.Set(0, 0, 0);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.transform.CompareTag("Sidewalk"))
        {
            m_Speed = 0;
        }
    }

    void BoundaryCheck(float yMin, float yMax)
    {
        if (m_Tr.position.y < yMin || m_Tr.position.y > yMax)
        {
            Destroy(gameObject);
        }
    }

    void SelfDrivingMovement()
    {
        //------------------------------------------------------------------------------------------------------------------//
        m_Rb.velocity = new Vector3(m_Tr.forward.x * m_Speed, m_Rb.velocity.y, m_Tr.forward.z * m_Speed);
        Velocity = m_Rb.velocity;
        //------------------------------------------------------------------------------------------------------------------//
        //Draws the car's velocity ray in unity editor
        Debug.DrawRay(m_Tr.position, m_Rb.velocity, Color.green);
    }
    void Rotation()
    {
        //------------------------------------------------------------------------------------------------------------------//
        //Applies rotation to car
        m_Tr.rotation = Quaternion.Euler(m_Rotation);
    }

}


