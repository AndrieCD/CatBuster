using UnityEngine;

public class HealDropScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // On collision with player, heal the player and destroy the heal drop
    private void OnCollisionEnter(Collision collision)
    {
        GameObject player = collision.gameObject;
        if (player.CompareTag("Player"))
        {
            PlayerController playerController = player.GetComponent<PlayerController>( );
            playerController.Heal(40);
            playerController.RestoreEnergy(50);
            // Destroy the heal drop after healing
            Destroy(gameObject);
        }
    }
}
