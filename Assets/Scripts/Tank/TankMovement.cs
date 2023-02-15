using UnityEngine;
using UnityEngine.AI;

public class TankMovement : MonoBehaviour
{
    public int m_PlayerNumber = 1;         
    public float m_Speed = 12f;            
    public float m_TurnSpeed = 180f;       
    public AudioSource m_MovementAudio;   //how many degrees is gonna turn over time 
    public AudioClip m_EngineIdling;       
    public AudioClip m_EngineDriving;      
    public float m_PitchRange = 0.2f;  //tono de audio?

    
    private string m_MovementAxisName;     //vertical
    private string m_TurnAxisName;         //horizontal
    private Rigidbody m_Rigidbody;         
    private float m_MovementInputValue;    
    private float m_TurnInputValue;        
    private float m_OriginalPitch;


    //Cambiado

    public NavMeshAgent m_navMeshAgent;
    public GameObject[] m_TanksGO;
    public Transform m_wpPlayer1;
    public Transform m_wpPlayer2;
    //public TankShooting m_shoot;
    //public int m_CurrentWaypointIndex;



    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }


    private void OnEnable ()  //When the script is turned on, before update
    {
        m_Rigidbody.isKinematic = false;  
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }


    private void OnDisable ()
    {
        m_Rigidbody.isKinematic = true; //no physics applied //if dying tuning them off, kinematic movement
    }


    private void Start()
    {
        m_TanksGO = GameObject.FindGameObjectsWithTag("Tank");
        m_wpPlayer1 = m_TanksGO[0].transform;
        m_wpPlayer2 = m_TanksGO[1].transform;

        if (m_PlayerNumber <= 2)
        {
            m_MovementAxisName = "Vertical" + m_PlayerNumber;
            m_TurnAxisName = "Horizontal" + m_PlayerNumber;
        }
        

        m_OriginalPitch = m_MovementAudio.pitch; //propiedad del pitch
    }
    

    private void Update()
    {

        if (m_PlayerNumber <= 2)
        {
            // Store the value of both input axes.
            m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
            m_TurnInputValue = Input.GetAxis(m_TurnAxisName);
        }
        else
        {
            float d0 = (m_TanksGO[m_PlayerNumber - 1].transform.position - m_TanksGO[0].transform.position).sqrMagnitude;
            float d1 = (m_TanksGO[m_PlayerNumber - 1].transform.position - m_TanksGO[1].transform.position).sqrMagnitude;

            if (d0 < d1)
            {
                m_navMeshAgent.SetDestination(m_TanksGO[0].transform.position);
                //m_shoot.Fire();

            }
            else
            {
                m_navMeshAgent.SetDestination(m_TanksGO[1].transform.position);
                //m_shoot.Fire();
            }
        }
        EngineAudio();
    }


    private void EngineAudio()
    {
        // Play the correct audio clip based on whether or not the tank is moving and what audio is currently playing.
        if(Mathf.Abs(m_MovementInputValue) < 0.1f && Mathf.Abs(m_TurnInputValue) < 0.1f) //Si no te estas moviendo
        {
            if(m_MovementAudio.clip == m_EngineDriving) //Si se esta reproduciendo
            {
                m_MovementAudio.clip = m_EngineIdling;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
        else
        {
            if(m_MovementAudio.clip == m_EngineIdling) //Si se esta reproduciendo
            {
                m_MovementAudio.clip = m_EngineDriving;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
    }


    private void FixedUpdate()
    {
        // Move and turn the tank.
        Move();
        Turn();
    }


    private void Move()
    {
        // Adjust the position of the tank based on the player's input.
        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;

        m_Rigidbody.MovePosition(m_Rigidbody.position + movement); //Mueve el rigidbody a una posicon absoluta
    }


    private void Turn()
    {
        // Adjust the rotation of the tank based on the player's input.
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;

        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f); //Gira solo alrededor del eje y

        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }
}