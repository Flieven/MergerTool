using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public int depth = 0;
    public Vector3 pos = Vector3.zero;
    public GameObject parentObj = null;
    public Node lesser, greater;

    public Node(GameObject objRef, int currentDepth, int num)
    {
        parentObj = new GameObject();
        parentObj.AddComponent<MeshFilter>();
        parentObj.AddComponent<MeshRenderer>();

        if (objRef.GetComponent<MergerTool_Component>().IsStatic)
        { parentObj.AddComponent<MeshCollider>(); }

        parentObj.GetComponent<MeshFilter>().mesh = new Mesh();

        parentObj.name = "RootObject " + objRef.GetComponent<MeshFilter>().sharedMesh.name + num;

        pos = objRef.transform.position;
        parentObj.transform.position = pos;

        depth = currentDepth;
    }
}

public class KDTree
{
    int kDepth = 3;
    Node root = null;
    int num = 0;

    private List<Node> proximityNodes = new List<Node>();

    public void AddNewNode(Node nearest, GameObject obj)
    {
        Node newNode = null;
        if (null == root) 
        {
            newNode = new Node(obj, 0, num);
            num++;
            root = newNode;
            obj.transform.SetParent(root.parentObj.transform);
            return;
        }

        if(null != nearest)
        {
            newNode = new Node(obj, nearest.depth++, num);
            num++;
            obj.transform.SetParent(newNode.parentObj.transform);

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

    public Node Nearest(Node current, Vector3 goal, Node currentBest, int depth, bool fastSearch)
    {
        //if(null != current) { Debug.Log("Called Nearest with root: " + current.parentObj.name); }

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

        currentBest = Nearest(goodSide, goal, currentBest, depth++, fastSearch);

        //Having this will check the entire Tree BUT it will be slower because of it!
        if(!fastSearch)
        { currentBest = Nearest(badSide, goal, currentBest, depth++, fastSearch); }

        return currentBest;

    }

    public List<Node> AllNodesInProximity(Node current, Vector3 goal, int depth, bool fastSearch, float maxDistance)
    {
        Node goodSide;
        Node badSide;

        int currentDepth = depth;

        if (null != current) { Debug.Log("Called Proximity with root: " + current.parentObj.name); }

        if (current == root) { proximityNodes.Clear(); }

        if(Vector3.Distance(current.pos, goal) <= maxDistance) 
        { 
            proximityNodes.Add(current);
            Debug.Log("Added " + current.parentObj.name + " to list of proximity nodes!");
        }

        if (currentDepth % kDepth == 0)
        {
            if (goal.x < current.pos.x)
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
        else if (currentDepth % kDepth == 1)
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
        else if (currentDepth % kDepth == 2)
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

        if(null != goodSide)
        { AllNodesInProximity(goodSide, goal, depth++, fastSearch, maxDistance); }

        //Having this will check the entire Tree BUT it will be slower because of it!
        if (!fastSearch && null != badSide)
        { AllNodesInProximity(badSide, goal, depth++, fastSearch, maxDistance); }

        return proximityNodes;
    }

    public Node getRoot { get { return root; } }
}
