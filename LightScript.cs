using UnityEngine;

public class LightScript : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 CameraPosition = Camera.main.transform.position;
        Quaternion CameraRotation = Camera.main.transform.rotation;
        this.transform.position = CameraPosition + new Vector3(-100, 100, -100);
        this.transform.rotation = CameraRotation;
    }
}
