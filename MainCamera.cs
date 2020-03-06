using UnityEngine;

public class MainCamera : MonoBehaviour
{
    SpeedSlider slider;

    int AreaMin;
    int AreaMax;
    int AreaWidth;

    const float rotateSpeed = 4.0f;
    const float panSpeed = 2.5f;

    Ray ray;
    Plane XZplane;
    float enter;


    Vector3 zoomTarget;
    Vector3 rotateTarget;      //Center of the structure?
    

    void Start()
    {
        // CAMERA START POSITION
        AreaMin = _3DSelfAssembly.AreaMin;
        AreaMax = _3DSelfAssembly.AreaMax;
        AreaWidth = Mathf.Abs(AreaMax - AreaMin);
        this.transform.position = new Vector3(AreaMin + AreaWidth / 2, AreaWidth, AreaMin + AreaWidth / 2);



        slider = FindObjectOfType(typeof(SpeedSlider)) as SpeedSlider;

        XZplane = new Plane(Vector3.up, new Vector3(AreaMin, AreaMin, AreaMin));   //Center of plane = Center of the structure?
        //rotateTarget = new Vector3(AreaMin + AreaWidth / 2, AreaMin + AreaWidth / 2, AreaMin + AreaWidth / 2);  // CORRIGIR
        rotateTarget = new Vector3(AreaMin + AreaWidth / 2, 0, AreaMin + AreaWidth / 2);
    }

    void Update()
    {
        ///////////////////////////////////////

        // PAN

        bool pan = Input.GetMouseButton(0) && slider.isBeingDragged == false;

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
