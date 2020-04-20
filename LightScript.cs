using UnityEngine;

public class LightScript : MonoBehaviour
{
    
    void Update()
    {
        // Makes sure Light is always constant in relation to Camera
        Vector3 CameraPosition = Camera.main.transform.position;
        Quaternion CameraRotation = Camera.main.transform.rotation;
        this.transform.position = CameraPosition + new Vector3(-100, 100, -100);
        this.transform.rotation = CameraRotation;
    }
}
