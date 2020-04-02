using UnityEngine;

public class MainCamera : MonoBehaviour
{  
    const float rotateSpeed = 4.0f;
    const float panSpeed = 2.5f;

    Ray ray;
    Plane XZplane;
    float enter;
     
    static Vector3 CameraPosition;
    static Quaternion CameraRotation;

    Vector3 zoomTarget;
    Vector3 rotateTarget;     
    

    void Start()
    {
        XZplane = new Plane(Vector3.up, Vector3.zero);
        rotateTarget = Vector3.zero;                     // FIX(?): rotateTarget in center of the structure?  
    }




    // CAMERA START POSITION
    public void SetCameraPosition(int AreaMin, int AreaMax)
    {
        int AreaWidth = Mathf.Abs(AreaMax - AreaMin);
        float AreaMiddle = AreaMin + AreaWidth * 0.5f;

        if (GUI.Reset != true)
        {
            CameraPosition = new Vector3(AreaMiddle, AreaMax, AreaMiddle);
            transform.position = CameraPosition;
        }
        else
        {
            transform.position = CameraPosition;
            transform.rotation = CameraRotation;
        }
    }





    void Update()
    {
        CameraPosition = this.transform.position;
        CameraRotation = this.transform.rotation;


        ///////////////////////////////////////

        // PAN

        bool pan = Input.GetMouseButton(0) && UIElements.isBeingDragged == false;

        if (pan)
        {
            float horizontal = -Input.GetAxis("Mouse X") * panSpeed;
            float vertical = -Input.GetAxis("Mouse Y") * panSpeed;

            Vector3 vector = new Vector3(horizontal, vertical, 0);
            transform.Translate(vector);
        }




        ///////////////////////////////////////

        // ROTATE

        bool rotate = Input.GetMouseButton(1);

        if (rotate)
        {
            float yaw = Input.GetAxis("Mouse X") * rotateSpeed;
            float pitch = -Input.GetAxis("Mouse Y") * rotateSpeed;

            transform.RotateAround(rotateTarget, Vector3.up, yaw);
            transform.RotateAround(rotateTarget, transform.rotation * Vector3.right, pitch);
        }




        ///////////////////////////////////////
        
        // ZOOM

        float zoom = Input.GetAxis("Mouse ScrollWheel");

        if (zoom != 0)
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (XZplane.Raycast(ray, out enter))
                zoomTarget = ray.GetPoint(enter);

            Vector3 direction = zoomTarget - transform.position;
            float distance = direction.magnitude * zoom;
            transform.Translate(direction.normalized * distance, Space.World);
        }
    }


}
