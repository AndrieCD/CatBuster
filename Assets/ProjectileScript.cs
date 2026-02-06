using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>( );
        rb.isKinematic = false;
        rb.AddRelativeForce(Vector3.forward * 500f);
        Destroy(gameObject, 5f); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
