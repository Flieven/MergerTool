using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public int depth = 0;
    public Vector3 pos = Vector3.zero;
    public GameObject obj = null;
    public Node lesser, greater;

    public Node(GameObject objRef, int currentDepth)
    {
        obj = objRef;
        pos = obj.transform.position;
        depth = currentDepth;
    }
}

public class KDTree
{
    int kDepth = 3;
    Node root = null;

    public void AddNewNode(Node nearest, GameObject obj)
    {
        Node newNode = null;
        if (null == root) 
        {
            newNode = new Node(obj, 0);
            root = newNode;
            return;
        }

        if(null != nearest)
        {
            newNode = new Node(obj, nearest.depth++);

            if (nearest.depth % kDepth == 0)
            {
                if(obj.transform.position.x < nearest.pos.x) { nearest.lesser = newNode; }
                else { nearest.greater = newNode; }
            }
            else if (nearest.depth % kDepth == 1)
            {
                if (obj.transform.position.y < nearest.pos.y) { nearest.lesser = newNode; }
                else { nearest.greater = newNode; }
            }
            else if (nearest.depth % kDepth == 2)
            {
                if (obj.transform.position.z < nearest.pos.z) { nearest.lesser = newNode; }
                else { nearest.greater = newNode; }
            }
        }
        else
        { throw new System.Exception("!!! ERROR: Attempting To Create New KDTree Node Using Null As Nearest With Object: '" + obj.name + "' !!!"); }

    }

    public Node Nearest(Node current, Vector3 goal, Node currentBest, int depth)
    {

        Node goodSide;
        Node badSide;

        int currentDepth = depth;

        if(null == current) { return currentBest; }

        if(null != currentBest)
        {
            if (Vector3.Distance(current.pos, goal) < Vector3.Distance(currentBest.pos, goal))
            { currentBest = current; }
        }
        else { currentBest = current; }

        if(currentDepth%kDepth == 0)
        {
            if(goal.x < current.pos.x)
            {
                goodSide = current.lesser;
                badSide = current.greater;
            }
            else
            {
                goodSide = current.greater;
                badSide = current.lesser;
            }
        }
        else if(currentDepth % kDepth == 1)
        {
            if (goal.y < current.pos.y)
            {
                goodSide = current.lesser;
                badSide = current.greater;
            }
            else
            {
                goodSide = current.greater;
                badSide = current.lesser;
            }
        }
        else if(currentDepth % kDepth == 2)
        {
            if (goal.z < current.pos.z)
            {
                goodSide = current.lesser;
                badSide = current.greater;
            }
            else
            {
                goodSide = current.greater;
                badSide = current.lesser;
            }
        }
        else
        { throw new System.Exception("!!! ERROR: currentDepth % kDepth Gave Unexpected Return: '" + currentDepth % kDepth + "' !!!"); }

        currentBest = Nearest(goodSide, goal, currentBest, depth++);

        //Having this will check the entire Tree BUT it will be slower because of it!
        currentBest = Nearest(badSide, goal, currentBest, depth++);

        return currentBest;

    }

    public Node getRoot { get { return root; } }
}
