using UnityEngine;

public class LoaderSpinner : MonoBehaviour
{
    // Speed of rotation in degrees per second
    public float rotationSpeed = 200f;

    // Reference to the Image component
    private RectTransform rectTransform;

    void Start()
    {
        // Get the RectTransform component of the Image
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        // Rotate the spinner smoothly
        rectTransform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);
    }
}