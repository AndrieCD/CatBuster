using UnityEngine;

public class DropRotation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Continuously rotate slowly
        transform.Rotate(0f, 0f, 20f * Time.deltaTime);
    }
}
