using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubGroup
{
    public GameObject subGroupParent = null;
    public int totalVerteciesInSubGroup = 0;

    public SubGroup(GameObject parentObj, int vertecies)
    {
        subGroupParent = parentObj;
        totalVerteciesInSubGroup = vertecies;
    }
}

public class Node
{
    public int depth = 0;
    public Vector3 pos = Vector3.zero;
    public GameObject parentObj = null;
    public List<SubGroup> subGroups = new List<SubGroup>();
    public Node lesser, greater;

    public int numSubGroups = 0;
    public int currentAvailableSubGroup = 0;

    public Node(GameObject objRef, int currentDepth, int num)
    {
        parentObj = new GameObject();

        parentObj.name = "RootObject " + objRef.GetComponent<MeshFilter>().sharedMesh.name + " " + num;

        pos = objRef.transform.position;
        parentObj.transform.position = pos;

        depth = currentDepth;

        parentObj.isStatic = objRef.isStatic;
    }
}

public class KDTree
{
    int kDepth = 3;
    Node root = null;
    int num = 0;

    private List<Node> proximityNodes = new List<Node>();

    private int vertexLimit = 65536;

    public void AddNewSubGroup(Node current, GameObject obj)
    {
        GameObject newSubgroupObject = new GameObject();
        newSubgroupObject.name = "SubGroup " + current.numSubGroups;
        newSubgroupObject.transform.position = current.pos;
        newSubgroupObject.transform.SetParent(current.parentObj.transform);
        newSubgroupObject.AddComponent<MeshFilter>();
        newSubgroupObject.AddComponent<MeshRenderer>();

        if (obj.isStatic)
        {
            newSubgroupObject.isStatic = obj.isStatic;
            newSubgroupObject.AddComponent<MeshCollider>(); 
        }

        newSubgroupObject.GetComponent<MeshFilter>().mesh = new Mesh();
        current.numSubGroups++;

        current.subGroups.Add(new SubGroup(newSubgroupObject,0));
        CheckSubGroups(current, obj);
    }

    public void CheckSubGroups(Node current, GameObject obj)
    {
        if(current.subGroups[current.currentAvailableSubGroup].totalVerteciesInSubGroup + obj.GetComponent<MeshFilter>().mesh.vertexCount < vertexLimit)
        {
            obj.transform.SetParent(current.subGroups[current.currentAvailableSubGroup].subGroupParent.transform);
            current.subGroups[current.currentAvailableSubGroup].totalVerteciesInSubGroup += obj.GetComponent<MeshFilter>().mesh.vertexCount;
            Debug.Log("Adding '" + obj.name + "' to subGroup of Node '" + current.parentObj.name + "' vertex count (" + current.subGroups[current.currentAvailableSubGroup].totalVerteciesInSubGroup + ")");
        }
        else 
        {
            current.currentAvailableSubGroup++;
            AddNewSubGroup(current, obj); 
        }
    }

    public void AddNewNode(Node nearest, GameObject obj)
    {
        Node newNode = null;
        if (null == root) 
        {
            newNode = new Node(obj, 0, num);
            num++;
            root = newNode;
            AddNewSubGroup(root, obj);
            return;
        }

        if(null != nearest)
        {
            newNode = new Node(obj, nearest.depth++, num);
            num++;
            AddNewSubGroup(newNode, obj);
            //obj.transform.SetParent(newNode.parentObj.transform);

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
