using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public struct PrefabStruct
{
    [SerializeField] public GameObject prefab;
    [SerializeField] public float maximumDistanceToRoot;
    [SerializeField] public bool isStatic;

    public PrefabStruct(GameObject prefab, float maxDistanceToRoot, bool isStatic)
    {
        this.prefab = prefab;
        maximumDistanceToRoot = maxDistanceToRoot;
        this.isStatic = isStatic;
    }
}

[System.Serializable]
public class DataPacket
{
    [SerializeField] public string ID;
    [ReadOnly] [SerializeField] public Vector2 textureSize = Vector2.zero;
    [SerializeField] public PrefabStruct[] prefabs;
    [SerializeField] public Texture2DStruct textureRegistry;
    [ReadOnly] [SerializeField] public Material mergedMaterial;

    public DataPacket(string ID, PrefabStruct[] prefabs)
    {
        this.ID = ID;
        this.prefabs = prefabs;
    }
}

[AddComponentMenu("MergerTool/MergerTool")]
[System.Serializable]
[RequireComponent(typeof(MaterialMaker), typeof(MeshRegistry))]
public class MergerTool : MonoBehaviour
{
    [Tooltip("Optimize Mesh Merge Root search for speed, less accurate.")]
    [SerializeField] private bool fastTreeSearch = true;

    [SerializeField] private List<DataPacket> dataPackets;
    /*[SerializeField]*/
    private MaterialMaker matMaker = null;
    /*[SerializeField]*/
    private MeshRegistry meshRegistry = null;

    public static MergerTool main;

    public static event Action<string> packetObserver;

    private void Awake()
    {
        if (null != main) { GameObject.Destroy(main); }
        else
        {
            main = this;
            DontDestroyOnLoad(this);
        }

        matMaker = GetComponent<MaterialMaker>();
        meshRegistry = GetComponent<MeshRegistry>();
        meshRegistry.FastSearch = fastTreeSearch;

        if (dataPackets.Count > 0)
        {
            for (int i = 0; i < dataPackets.Count; i++)
            {
                PopulateDataSet(i);
            }
        }

        AddComponentToPrefabs(false);
    }

    private void OnDestroy()
    {
        for (int i = 0; i < dataPackets.Count; i++)
        {
            for (int ii = 0; ii < dataPackets[i].prefabs.Length; ii++)
            {
                if (null != dataPackets[i].prefabs[ii].prefab && null != dataPackets[i].prefabs[ii].prefab.GetComponent<MergerTool_Component>())
                {
                    if (dataPackets[i].prefabs[ii].prefab.GetComponent<MergerTool_Component>().wasAddedManually == false)
                    { dataPackets[i].prefabs[ii].prefab.GetComponent<MergerTool_Component>().DestroyComponent(); }
                }
            }
        }
    }

    public void AddComponentToPrefabs(bool manuallyAdded)
    {
        for (int i = 0; i < dataPackets.Count; i++)
        {
            for (int ii = 0; ii < dataPackets[i].prefabs.Length; ii++)
            {
                if (null == dataPackets[i].prefabs[ii].prefab.GetComponent<MergerTool_Component>())
                {
                    dataPackets[i].prefabs[ii].prefab.AddComponent<MergerTool_Component>().wasAddedManually = manuallyAdded;
                    dataPackets[i].prefabs[ii].prefab.GetComponent<MergerTool_Component>().ID = dataPackets[i].ID;
                }
            }
        }
    }

    private void PopulateDataSet(int index)
    {
        dataPackets[index].textureSize.x = dataPackets[index].prefabs[0].prefab.GetComponent<Renderer>().sharedMaterial.mainTexture.width;
        dataPackets[index].textureSize.y = dataPackets[index].prefabs[0].prefab.GetComponent<Renderer>().sharedMaterial.mainTexture.height;
        dataPackets[index].textureRegistry.registrySize = dataPackets[index].prefabs.Length;
        dataPackets[index].mergedMaterial = matMaker.Run(dataPackets[index]);

        if(null != packetObserver) { packetObserver(dataPackets[index].ID); }
        else { Debug.Log("packetObserver was null"); }
    }

    public DataPacket getData(string ID, MergerTool_Component component)
    {
        component.meshRegistry = this.meshRegistry;

        for (int i = 0; i < dataPackets.Count; i++)
        {
            if (dataPackets[i].ID == ID) { return dataPackets[i]; }
        }
        throw new System.Exception("!!! ERROR: No Datapacket with ID: '" + ID + "' Found !!!");
    }

    public bool hasData(string ID)
    {
        for (int i = 0; i < dataPackets.Count; i++)
        {
            if (dataPackets[i].ID == ID) { return true; }
        }
        return false;
    }

    #region AddFunctions

    public void Add_DataPacket(string ID, PrefabStruct[] prefabStruct)
    {
        for (int i = 0; i < dataPackets.Count; i++)
        { if (dataPackets[i].ID == ID) { throw new System.Exception("!!! ERROR: MergeTool Already Contains DataPacket With ID: '" + ID + "' !!!"); } }

        dataPackets.Add(new DataPacket(ID, prefabStruct));
        PopulateDataSet(dataPackets.Count - 1);
    }

    public void Add_MergeToolComponent(GameObject obj, string ID)
    {
        if(obj.GetComponent<MergerTool_Component>()) { obj.GetComponent<MergerTool_Component>().ID = ID; }
        else { obj.AddComponent<MergerTool_Component>().ID = ID; }
    }
    #endregion

    #region RemoveFunctions
    public void Remove_DataPacket(string ID)
    {
        for (int i = 0; i < dataPackets.Count; i++)
        {
            if (dataPackets[i].ID == ID) { dataPackets.RemoveAt(i); break; }
            else { }
            if (i > dataPackets.Count) 
            { throw new System.Exception("!!! ERROR: MergeTool Does Not Contain DataPacket With ID: '" + ID + "' !!!"); }
        }
    }

    public void Remove_DataPacket(DataPacket packet)
    {
        if (!dataPackets.Contains(packet)) { throw new System.Exception("!!! ERROR: MergeTool Does Not Contain DataPacket: '" + packet.ID + "' !!!"); }
        else { dataPackets.Remove(packet); }
    }

    #endregion

    #region ClearFunctions
    public void Clear_DataPackets() { dataPackets.Clear(); }

    #endregion
}
