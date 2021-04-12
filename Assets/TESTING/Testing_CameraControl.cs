using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing_CameraControl : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5.0f;
    [SerializeField] private float cameraSensitivity = 0.25f;

    [SerializeField] private GameObject heldObject = null;
    [SerializeField] private GameObject heldObjectNode = null;

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

        if(Input.GetMouseButtonDown(0))
        {
            Debug.Log("Moust button pressed!");
            if(null == heldObject) { LineTraceGrab(); }
            else if (null != heldObject) 
            { 
                if(heldObject.GetComponent<MergerTool_Component>())
                { heldObject.GetComponent<MergerTool_Component>().MergeMesh(); }
                heldObject = null; 
            }
        }

        if(null != heldObject)
        { heldObject.transform.position = heldObjectNode.transform.position; }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Camera.main.transform.position, 2f);
    }

    private void LineTraceGrab()
    {
        RaycastHit hit;

        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 100f))
        {
            Debug.Log("Hit: " + hit.transform.name);
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * hit.distance, Color.cyan, 10f);
            if(hit.transform.GetComponent<MergerTool_Component>())
            { 
                hit.transform.GetComponent<MergerTool_Component>().ReleaseMergedMesh(); 
                if(null == hit.transform.parent) { heldObject = hit.transform.gameObject; }
            }
        }
        else 
        {
            Debug.Log("Miss");
            Debug.DrawRay(transform.position, Camera.main.transform.forward * 100f, Color.red, 10f); 
        }

    }

}
