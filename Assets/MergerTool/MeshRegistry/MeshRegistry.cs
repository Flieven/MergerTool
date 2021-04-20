using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("MergerTool/Required Components/Mesh Registry")]
public class MeshRegistry : MonoBehaviour
{
    Dictionary<string, Dictionary<int, KDTree>> posDictionary = new Dictionary<string, Dictionary<int, KDTree>>();
    bool fastSearch = true;

    [SerializeField] private bool generateDebugLogs = false;

    private int vertexLimit = 65536;

    public void MergeToRoot(GameObject obj, string ID, int prefabIndex, float maxDistance)
    {
        if(!posDictionary.ContainsKey(ID))
        {
            if (generateDebugLogs) { Debug.Log("===== New Upper Dictionary Entry With ID: '" + ID + "' Created ====="); }
            posDictionary.Add(ID, new Dictionary<int, KDTree>());
        }

        if (!posDictionary[ID].ContainsKey(prefabIndex))
        {
            if (generateDebugLogs) { Debug.Log("===== New Lower Dictionary Entry With prefabIndex: '" + prefabIndex + "' Created In Upper Registry: '" + ID + "' ====="); }
            posDictionary[ID].Add(prefabIndex, new KDTree());
        }

        if(null == posDictionary[ID][prefabIndex].getRoot)
        {
            if (generateDebugLogs) { Debug.Log("===== Created New Root In Lower Dictionary '" + prefabIndex + "' In Upper Registry: '" + ID + "' Using Object: '" + obj.name + "' ====="); }
            posDictionary[ID][prefabIndex].AddNewNode(null, obj);
            CombineMeshes(obj, obj.GetComponent<MergerTool_Component>().IsStatic, obj.GetComponent<MergerTool_Component>().CustomMaterial);
            return;
        }

        Node nearestFound = posDictionary[ID][prefabIndex].Nearest(posDictionary[ID][prefabIndex].getRoot, obj.transform.position, null, 0, fastSearch);

        if(Vector3.Distance(nearestFound.pos, obj.transform.position) <= maxDistance)
        {
            //Debug.Log("===== Found Nearest: '" + nearestFound.obj.name + "' Within minimumDistance To '" + obj.name + "' =====");

            // PARENT/MERGE THE OBJECTS HERE
            posDictionary[ID][prefabIndex].CheckSubGroups(nearestFound, obj);
            //obj.transform.SetParent(nearestFound.parentObj.transform);
            //nearestFound.parentObj.transform.GetChild(0).GetComponent<MergerTool_Component>().CombineMeshes();
            CombineMeshes(obj, obj.GetComponent<MergerTool_Component>().IsStatic, obj.GetComponent<MergerTool_Component>().CustomMaterial);
        }
        else 
        {
            //Debug.Log("===== Nearest: '" + nearestFound.obj.name + "' Not Near Enough To: '" + obj.name + "' Using It To Create New Root =====");
            posDictionary[ID][prefabIndex].AddNewNode(nearestFound, obj); 
        }
    }

    private bool CheckCanCombine(GameObject obj)
    {
        int currentTotalVertexCount = obj.transform.parent.GetComponent<MeshFilter>().mesh.vertexCount;
        int currentObjVertexCount = obj.GetComponent<MeshFilter>().mesh.vertexCount;

        //Debug.Log("Total Vertex of '" + obj.name + "'(" + currentObjVertexCount +") in '"+ obj.transform.parent.name +"'("+currentTotalVertexCount+") should be: " + (currentObjVertexCount + currentTotalVertexCount));

        if(currentObjVertexCount + currentTotalVertexCount < vertexLimit) { return true; }
        else { return false; }

    }

    private void CombineMeshes(GameObject obj, bool isStatic, Material customMaterial)
    {
        if(!CheckCanCombine(obj)) { return; }

        MeshFilter parentFilter = obj.transform.parent.GetComponent<MeshFilter>();

        Vector3 originalPos = obj.transform.parent.position;
        obj.transform.parent.position = Vector3.zero;

        obj.transform.parent.GetComponent<MeshRenderer>().material = customMaterial;

        MeshFilter[] meshFilters = obj.transform.parent.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;

        while (i < meshFilters.Length)
        {
            if(meshFilters[i].gameObject == obj.transform.parent) { }
            else
            {
                if (!isStatic) { parentFilter.mesh.Clear(); }
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                if (isStatic) { meshFilters[i].gameObject.SetActive(false); }
                else if (!isStatic && i > 0) { meshFilters[i].GetComponent<MeshRenderer>().enabled = false; }
            }
            i++;
        }

        parentFilter.mesh = new Mesh();
        parentFilter.mesh.CombineMeshes(combine, true);

        if (parentFilter.mesh.vertexCount > vertexLimit)
        { throw new System.Exception("!!! ERROR: object '" + obj.transform.parent.name + "' has exceeded vertex limit on combined mesh, it's going to look really weird !!!"); }

        if (isStatic) { parentFilter.sharedMesh = parentFilter.mesh; }

        obj.transform.parent.gameObject.SetActive(true);

        obj.transform.parent.position = originalPos;
    }

    public void DetachFromRoot(GameObject obj, string ID, int prefabIndex, float maxDistance)
    {

        GameObject nearestFound = null;
        Node nearestNode = null;
        List<Node> proximityNodes = posDictionary[ID][prefabIndex].AllNodesInProximity(posDictionary[ID][prefabIndex].getRoot, obj.transform.position, 0, false, maxDistance);
        foreach(Node node in proximityNodes)
        {
            for (int i = 0; i < node.subGroups.Count; i++)
            {
                if(node.subGroups[i].subGroupParent.transform.Find(obj.name)) 
                { 
                    nearestFound = node.subGroups[i].subGroupParent;
                    nearestNode = node;
                }
            }
            //if(node.parentObj.transform.Find(obj.name)) { nearestFound = node; }
        }

        if(null == nearestFound)
        {
            Debug.Log("!!! ERROR: Could not find object: '" + obj.name + "' in '" + nearestFound.transform.parent.name + "' in KD tree !!!");
            Debug.Log("List of objects searched: ");
            foreach (Transform child in nearestFound.transform.parent.transform)
            { Debug.Log(child.gameObject.name); }
        }

        Debug.Log("<<< Found object: '" + obj.name + "' in '" + nearestFound.transform.parent.name + "' in KD tree >>>");
        obj.transform.parent = null;
        obj.GetComponent<MeshRenderer>().enabled = true;

        nearestFound.transform.GetComponent<MeshFilter>().mesh.Clear();
        nearestFound.transform.GetComponent<MeshFilter>().mesh = null;

        if(nearestFound.transform.childCount > 0)
        {
            Transform childTransform = nearestFound.transform.GetChild(0);

            CombineMeshes(childTransform.gameObject,
                          childTransform.GetComponent<MergerTool_Component>().IsStatic,
                          childTransform.GetComponent<MergerTool_Component>().CustomMaterial);
        }
        else
        {
            Debug.Log("Less than or equal to 0 children found in node '" +nearestNode.parentObj.name+"'");
            posDictionary[ID][prefabIndex].RemoveSubGroup(nearestNode, nearestFound);
            Destroy(nearestFound);
        }
        //nearestFound.parentObj.transform.GetChild(0).GetComponent<MergerTool_Component>().CombineMeshes();

        //if (nearestFound.parentObj.gameObject.transform.Find(obj.name))
        //{ 
        //    Debug.Log("<<< Found object: '" + obj.name + "' in '" + nearestFound.parentObj.name + "' in KD tree >>>");
        //    obj.transform.parent = null;
        //    obj.GetComponent<MeshRenderer>().enabled = true;

        //    nearestFound.parentObj.GetComponent<MeshFilter>().mesh.Clear();
        //    nearestFound.parentObj.GetComponent<MeshFilter>().mesh = null;

        //    CombineMeshes(nearestFound.parentObj.transform.GetChild(0).gameObject,
        //        nearestFound.parentObj.transform.GetChild(0).GetComponent<MergerTool_Component>().IsStatic,
        //        nearestFound.parentObj.transform.GetChild(0).GetComponent<MergerTool_Component>().CustomMaterial);
        //    //nearestFound.parentObj.transform.GetChild(0).GetComponent<MergerTool_Component>().CombineMeshes();
        //}
    }

    public Node getNearest(GameObject obj, string ID, int prefabIndex, float maxDistance)
    {
        Node nearestFound = posDictionary[ID][prefabIndex].Nearest(posDictionary[ID][prefabIndex].getRoot, obj.transform.position, null, 0, fastSearch);

        if (Vector3.Distance(nearestFound.pos, obj.transform.position) <= maxDistance)
        {
            return nearestFound;
        }
        else
        {
            //Debug.Log("===== Nearest: '" + nearestFound.obj.name + "' Not Near Enough To: '" + obj.name + "' Using It To Create New Root =====");
            posDictionary[ID][prefabIndex].AddNewNode(nearestFound, obj);
        }
        return null;
    }

    public bool FastSearch 
    {
        get { return fastSearch; }
        set { fastSearch = value; }
    }
}
