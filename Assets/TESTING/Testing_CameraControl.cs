using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing_CameraControl : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5.0f;
    [SerializeField] private float cameraSensitivity = 0.25f;

    private Vector3 lastMouse = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Translate(new Vector3(Input.GetAxis("Horizontal") * movementSpeed,
                                                    0, Input.GetAxis("Vertical") * movementSpeed));

        if (Input.GetMouseButtonDown(1)) { lastMouse = Input.mousePosition; }

        if(Input.GetMouseButton(1))
        {
            lastMouse = Input.mousePosition - lastMouse;
            lastMouse = new Vector3(-lastMouse.y * cameraSensitivity, lastMouse.x * cameraSensitivity, 0);
            lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x, transform.eulerAngles.y + lastMouse.y, 0);
            transform.eulerAngles = lastMouse;
            lastMouse = Input.mousePosition;
        }
    }
}
