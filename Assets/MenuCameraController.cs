using UnityEngine;
using UnityEngine.Rendering.Universal;
// Ensure you have the correct using directive for Light2D

public class MenuCameraController : MonoBehaviour
{
    public Light2D light2D; // Reference to the Light2D component
    public Camera mainCamera; // Reference to the main camera
    public float followSpeed = 5f; // Speed of the light following the mouse

    private bool isZoomingIn = false;

    void Start()
    {
        

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        
    }

    void Update()
    {
        FollowMouse();
        
    }

    void FollowMouse()
    {
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; // Ensure the light stays on the same Z plane
        light2D.transform.position = Vector3.Lerp(light2D.transform.position, mousePosition, followSpeed * Time.deltaTime);
    }


}
