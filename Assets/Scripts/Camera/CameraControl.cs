using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float m_DampTime = 0.2f;      //The approximate time that we want to take the camero  to move to the position that is required (delay)        
    public float m_ScreenEdgeBuffer = 4f;         //Number that we add to sides (tank) to make sure the cameras aren't at the edge of the screen
    public float m_MinSize = 6.5f;   //Minimo de zoom               
    [HideInInspector] public Transform[] m_Targets; 


    private Camera m_Camera;                        
    private float m_ZoomSpeed;                      
    private Vector3 m_MoveVelocity;                 
    private Vector3 m_DesiredPosition;              


    private void Awake()
    {
        m_Camera = GetComponentInChildren<Camera>();
    }


    private void FixedUpdate()
    {
        Move();
        Zoom();
    }


    private void Move()
    {
        FindAveragePosition();

        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);  //Mover de la posicion actual a la deseada en un tiempo y velocidad especificos
    }


    private void FindAveragePosition()
    {
        Vector3 averagePos = new Vector3();
        int numTargets = 0;

        for (int i = 0; i < m_Targets.Length; i++)  //Recorre los objetivos
        {
            if (!m_Targets[i].gameObject.activeSelf) //Ver si un tanque esta activo (No muerto)
                continue;                            //Siguiente iteracion

            averagePos += m_Targets[i].position;
            numTargets++;
        }

        if (numTargets > 0)
            averagePos /= numTargets;   //Posicion media entre todos los tanques

        averagePos.y = transform.position.y;  //????  //porque es ortografico entiendo

        m_DesiredPosition = averagePos;  //Just in case
    }


    private void Zoom()
    {
        float requiredSize = FindRequiredSize();
        m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);  //De zoom actual al necesario en tiempo determinado y velocidad
    }


    private float FindRequiredSize()
    {
        Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition); //Queremos encontrar el desired position of camera in the camera's rig local space

        float size = 0f;

        for (int i = 0; i < m_Targets.Length; i++)
        {
            if (!m_Targets[i].gameObject.activeSelf)
                continue;

            Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position);  //encontrar ese target en en local position del rig

            //?????

            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

            //Ver si es mas grande el actual o el calculado

            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.y));  //Distancia abs (tmbn hacia abajo) 

            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.x) / m_Camera.aspect);
        }
        
        size += m_ScreenEdgeBuffer;  //Extra distance para que los tanques quepan dentro de los borde de la pantalla

        size = Mathf.Max(size, m_MinSize);  //Controlar que no este la camera demasiado cerca

        return size;
    }


    public void SetStartPositionAndSize() //Reseteo de camera
    {
        FindAveragePosition();

        transform.position = m_DesiredPosition;

        m_Camera.orthographicSize = FindRequiredSize();
    }
}