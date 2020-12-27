using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CarController : MonoBehaviour
{
    /// <summary>
    /// Maximim Speed of the car
    /// </summary>
    public float m_MaxSpeed;
    /// <summary>
    /// Force applies to the car when moving
    /// </summary>
    public float m_ForceValue;
    /// <summary>
    /// Multiplier used to calculate effectiveness of the parking brake
    /// </summary>
    public float m_ParkingBrakeForceMultiplier;
    /// <summary>
    /// Camera the view is switched to when the car is destroyed
    /// </summary>
    public GameObject deathCamera;
    /// <summary>
    /// Child camera of the car
    /// </summary>
    public GameObject CarCamera;
    /// <summary>
    /// Value used to calculate friction
    /// </summary>
    public float CoefficientOfFriction;
    /// <summary>
    /// How fast the car can turn
    /// </summary>
    public float m_TurnSpeed;
    /// <summary>
    /// How fast the car can go
    /// </summary>
    public float m_Speed;
    /// <summary>
    /// 
    /// </summary>
    public Text m_SpeedText, m_ParkingBrakeText;
    /// <summary>
    /// Is the parking brake on?
    /// </summary>
    public bool m_ParkingBrake;
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
    void Start ()
    {
        deathCamera.SetActive(false);
        //------------------------------------------------------------------------------------------------------------------//
        //Sets the parking brake to off;
        m_ParkingBrake = false;
        //------------------------------------------------------------------------------------------------------------------//
        //Sets the UI text components to defualt text
        m_SpeedText = GameObject.FindGameObjectWithTag("UI_SPEED").GetComponent<Text>();
        m_ParkingBrakeText = GameObject.FindGameObjectWithTag("UI_PARKING_BRAKE").GetComponent<Text>();
        m_SpeedText.text = ("Speed: ");
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
        //m_AccelerationOfGravity = 9.81f;
        //------------------------------------------------------------------------------------------------------------------//
        //Gets the car's mass
        m_Mass = m_Rb.mass;

        m_Speed = 0;
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
            //Rotates Character
            Rotation();
            //------------------------------------------------------------------------------------------------------------------//
            //Moves Character
            Movement();
            //Checks to see if Car is still in vertical boundary
            BoundaryCheck(-10, 20);
        }
        else if(StaticVariables.isPaused)
        {
            m_Rb.isKinematic = true;
            m_Rb.velocity.Set(0, 0, 0);
        }
	}

    void Update()
    {
        //------------------------------------------------------------------------------------------------------------------//
        //Toggles parking brake when KeyCode.P is pressed
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (m_ParkingBrake == false)
            {
                m_ParkingBrake = true;
            }
            else if (m_ParkingBrake == true)
            {
                m_ParkingBrake = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            LReset();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(1);
        }
        //------------------------------------------------------------------------------------------------------------------//
        //Shows user condition of parking brake
        if (m_ParkingBrakeText != null)
        {
            if (m_ParkingBrake)
            {
                m_ParkingBrakeText.text = ("Parking Brake: ON");
            }
            else if (!m_ParkingBrake)
            {
                m_ParkingBrakeText.text = ("Parking Brake: OFF");
            }
        }
    }
    //------------------------------------------------------------------------------------------------------------------//
    //Function that resets Car
    public void LReset()
    {
        StaticVariables.LReset();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.transform.CompareTag("Sidewalk"))
        {
            m_Speed = 0;
        }
    }

    void BoundaryCheck(float yMin, float yMax)
    {
        if(m_Tr.position.y < yMin || m_Tr.position.y > yMax)
        {
            CarCamera.SetActive(false);
            deathCamera.SetActive(true);
            Destroy(gameObject);
        }
    }

    void Movement()
    {
        //------------------------------------------------------------------------------------------------------------------//
        //Gets input from the user from up/down arrow keys and w/s keys: up and w result in return value of 1 and down and s keys result in return value of -1
        VerticalInput = Input.GetAxis("Vertical");

        float force = VerticalInput * m_ForceValue;

        if(m_ParkingBrake)
        {
            // Modify force
            force = -m_Speed * m_ParkingBrakeForceMultiplier / 10; // Multiply by some factor...
        }

        //------------------------------------------------------------------------------------------------------------------//
        //Applies friction to car
        if (m_Speed > 0)
        {
            m_Speed = m_Speed - CoefficientOfFriction;
            if (m_Speed < 0)
            {
                m_Speed = 0;
            }
        }
        else if (m_Speed < 0)
        {
            m_Speed = m_Speed + CoefficientOfFriction;
            if(m_Speed > 0)
            {
                m_Speed = 0;
            }
        }
        //------------------------------------------------------------------------------------------------------------------//
        //Calculates speed
        float accel = force / m_Mass;
        m_Speed = m_Speed + accel * Time.deltaTime;

        //------------------------------------------------------------------------------------------------------------------//
        //Constrains m_Speed to m_MaxSpeed
        m_Speed = Mathf.Clamp(m_Speed, -m_MaxSpeed, m_MaxSpeed);

        //------------------------------------------------------------------------------------------------------------------//
        //Raises car when space is held down
        if (Input.GetKey(KeyCode.Space))
        {
            m_Rb.velocity.Set(m_Rb.velocity.x, 5, m_Rb.velocity.z);
        }

        //------------------------------------------------------------------------------------------------------------------//
        m_Rb.velocity = new Vector3(m_Tr.forward.x * m_Speed, m_Rb.velocity.y, m_Tr.forward.z * m_Speed);
        Velocity = m_Rb.velocity;
        //------------------------------------------------------------------------------------------------------------------//
        //Converts speed to int value
        int intString = (int)m_Speed;
        //Displays speed on screen
        if (m_SpeedText != null)
        {
            m_SpeedText.text = ("Speed: ") + intString.ToString() + ("Mph");
        }
        //------------------------------------------------------------------------------------------------------------------//
        //Draws the car's velocity ray in unity editor
        Debug.DrawRay(m_Tr.position, m_Rb.velocity, Color.green);
    }
    void Rotation()
    {
        //------------------------------------------------------------------------------------------------------------------//
        //Gets input from the user from left/right arrow keys and a/d keys: left and a result in return value of 1 and right and d keys result in return value of -1
        HorizontalInput = Input.GetAxis("Horizontal");
        //Rotates the car at the rotation speed 
        m_Rotation = new Vector3(0, m_Rotation.y + m_TurnSpeed * HorizontalInput, 0);
        //Applies rotation to car
        m_Tr.rotation = Quaternion.Euler(m_Rotation);
        //Gets rotation for reference later
        m_RotationVector = m_Tr.rotation;
    }

}


