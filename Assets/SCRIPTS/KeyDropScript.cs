using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class KeyDropScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log(collision.gameObject.name);
        GameObject player = collision.gameObject;
        if (player.CompareTag("Player"))
        {
            // destroy "Gate" object
            GameObject gate = GameObject.Find("Gate");

            Destroy(gate);
            Destroy(gameObject);
        }
    }
}
