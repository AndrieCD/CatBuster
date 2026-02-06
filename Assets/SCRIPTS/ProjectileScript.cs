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
        Destroy(gameObject, 1f); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (IsDead) return;

        GameObject fenceWall = collision.gameObject;
        if (fenceWall.CompareTag("Fence") )
        {
            Destroy(gameObject);
        } else if (fenceWall.CompareTag("Crate"))
        {
            Destroy(gameObject);
            Destroy(fenceWall);
        }
    }
}
