using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class MagNode
{
    public float magnitude;
    public GameObject objectRef;
    public MagNode larger;
    public MagNode smaller;

    public MagNode(GameObject obj, float mag)
    {
        objectRef = obj;
        magnitude = mag;
    }
}

public class MagTree
{
    private MagNode root;

    public void AddNewRoot(GameObject obj)
    {
        float newMagnitude = obj.transform.position.magnitude;

        MagNode newNode = new MagNode(obj, newMagnitude);

        if(null == root) { root = newNode; return; }

    }
}
