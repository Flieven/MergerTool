using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("MergerTool/Required Components/Mesh Registry")]
public class MeshRegistry : MonoBehaviour
{
    Dictionary<string, Dictionary<int, KDTree>> posDictionary = new Dictionary<string, Dictionary<int, KDTree>>();
    bool fastSearch = true;
    public void MergeToRoot(GameObject obj, string ID, int prefabIndex, float maxDistance)
    {
        if(!posDictionary.ContainsKey(ID))
        {
            Debug.Log("===== New Upper Dictionary Entry With ID: '" + ID + "' Created =====");
            posDictionary.Add(ID, new Dictionary<int, KDTree>());
        }

        if (!posDictionary[ID].ContainsKey(prefabIndex))
        {
            Debug.Log("===== New Lower Dictionary Entry With prefabIndex: '" + prefabIndex + "' Created In Upper Registry: '" + ID + "' =====");
            posDictionary[ID].Add(prefabIndex, new KDTree());
        }

        if(null == posDictionary[ID][prefabIndex].getRoot)
        {
            Debug.Log("===== Created New Root In Lower Dictionary '" + prefabIndex + "' In Upper Registry: '" + ID + "' Using Object: '" + obj.name + "' =====");
            posDictionary[ID][prefabIndex].AddNewNode(null, obj);
            return;
        }

        Node nearestFound = posDictionary[ID][prefabIndex].Nearest(posDictionary[ID][prefabIndex].getRoot, obj.transform.position, null, 0, fastSearch);

        if(Vector3.Distance(nearestFound.pos, obj.transform.position) <= maxDistance)
        { 
            //Debug.Log("===== Found Nearest: '" + nearestFound.obj.name + "' Within minimumDistance To '" + obj.name + "' =====");
            
            // PARENT/MERGE THE OBJECTS HERE
            obj.transform.SetParent(nearestFound.obj.transform);
            nearestFound.obj.GetComponent<MergerTool_Component>().MergeMesh();
        }
        else 
        {
            //Debug.Log("===== Nearest: '" + nearestFound.obj.name + "' Not Near Enough To: '" + obj.name + "' Using It To Create New Root =====");
            posDictionary[ID][prefabIndex].AddNewNode(nearestFound, obj); 
        }
    }

    public bool FastSearch 
    {
        get { return fastSearch; }
        set { fastSearch = value; }
    }
}
