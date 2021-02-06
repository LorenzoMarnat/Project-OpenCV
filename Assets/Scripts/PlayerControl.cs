using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float moveSpeed = 5;
    public float maxTravelSpeed = 20;
    public float rotationSpeed = 5;
    public float cameraRotationSpeed = 2;

    private float travelSpeed;
    private bool gripped;
    private Vector3 target;
    private float distanceToTarget;

    private Rigidbody rb;
    private LineRenderer lr;

    private Vector3[] lnPoints;
    // Start is called before the first frame update
    void Start()
    {
        gripped = false;
        rb = GetComponent<Rigidbody>();
        lr = GetComponent<LineRenderer>();
        travelSpeed = moveSpeed;

        lnPoints = new Vector3[2];
    }

    // Update is called once per frame
    void Update()
    {
        // If left click, use grapple on hit point
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            // If an object with correct layer is hitted, move oject at hit point
            if (Physics.Raycast(ray, out hit, 1000, LayerMask.GetMask("Terrain")))
            {
                target = hit.point + new Vector3(0, transform.localScale.y, 0);
                distanceToTarget = hit.distance;
                gripped = true;
                rb.useGravity = false;
                rb.angularVelocity = new Vector3(0, 0, 0);

                lnPoints[0] = transform.position;
                lnPoints[1] = target;
                lr.SetPositions(lnPoints);
            }
        }

        // If right click, rotate pov camera with mouse
        if (Input.GetMouseButton(1))
        {
            Camera.main.transform.Rotate(new Vector3(Input.GetAxis("Mouse Y") * cameraRotationSpeed, -Input.GetAxis("Mouse X") * cameraRotationSpeed, 0));
            float X = Camera.main.transform.rotation.eulerAngles.x;
            float Y = Camera.main.transform.rotation.eulerAngles.y;
            Camera.main.transform.rotation = Quaternion.Euler(X, Y, 0);
        }

        // Reset camera rotation
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Camera.main.transform.rotation = Camera.main.transform.parent.rotation;
        }

        // Move
        if (!gripped)
        {
            // Rotate using horizontal input
            transform.Rotate(0, Input.GetAxis("Horizontal") * rotationSpeed, 0);

            // Move using vertical input
            float input = Input.GetAxis("Vertical");
            Vector3 move = transform.rotation * Vector3.forward * input;
            rb.MovePosition(transform.position + (move * moveSpeed * Time.deltaTime));

            if(input == 0)
                rb.angularVelocity = new Vector3(0, 0, 0);
        }

        // Move toward target
        if(gripped)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, travelSpeed * Time.deltaTime);

            lr.SetPosition(0, transform.position);

            if (travelSpeed < maxTravelSpeed)
                travelSpeed += Time.deltaTime * 10;

            if (transform.position == target)
            {
                gripped = false;
                rb.useGravity = true;
                travelSpeed = moveSpeed;
            }
        }
    }
}
