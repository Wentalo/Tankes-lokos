using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask;    //Layer mask
    public ParticleSystem m_ExplosionParticles;       
    public AudioSource m_ExplosionAudio;              
    public float m_MaxDamage = 100f;    //Maximo daño si consigues hit perfecto, pero no lo vas a conseguir debido al collider , close but not cigar             
    public float m_ExplosionForce = 1000f;            
    public float m_MaxLifeTime = 2f;                  
    public float m_ExplosionRadius = 5f;              


    private void Start()
    {
        Destroy(gameObject, m_MaxLifeTime);  //Si sigue vivo despues de 2 segs, destruyelo
    }

    [System.Obsolete]
    private void OnTriggerEnter(Collider other)
    {
        // Find all the tanks in an area around the shell and damage them.
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);  //Similar a un raycast, pero en forma de esfera, desde la posicion, un radio y una layer

        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody targetRigibody = colliders[i].GetComponent<Rigidbody>();

            if (!targetRigibody) continue;

            targetRigibody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius); //una fuerza con un valor (maginutd), radio (alcance) y una posicion (inicio)

            TankHealth targetHealth = targetRigibody.GetComponent<TankHealth>(); 
            /*
             * Puedes usar una referncia a un componente de un objeto 
             * para referirte a otro componente del mismo
             */

            if (!targetHealth) continue;

            float damage = CalculateDamage(targetRigibody.position);

            targetHealth.TakeDamage(damage);  //Se puede acceder ya que es publica
        }

        m_ExplosionParticles.transform.parent = null; //Dejarlo sin padre

        m_ExplosionParticles.Play();

        m_ExplosionAudio.Play();

        //Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.duration); //Obsoleto

        Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.duration); //Delay de destruccion
        Destroy(gameObject);
    }


    private float CalculateDamage(Vector3 targetPosition)
    {
        // Calculate the amount of damage a target should take based on it's position.
        Vector3 explosionToTarget = targetPosition - transform.position;

        float explosionDistance = explosionToTarget.magnitude; //No vector

        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;  // 1 si esta en el centro y 0 en el borde

        float damage = relativeDistance * m_MaxDamage;

        damage = Mathf.Max(0f, damage);

        return damage;
    }
}