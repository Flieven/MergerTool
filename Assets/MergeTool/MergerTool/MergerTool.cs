using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MergeTool
{
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

        [SerializeField] private List<DataPacket> dataSets;
        /*[SerializeField]*/ private MaterialMaker matMaker = null;
        /*[SerializeField]*/ private MeshRegistry meshRegistry = null;

        public static MergeTool.MergerTool main;

        private void Awake()
        {
            if(null != main) { GameObject.Destroy(main); }
            else 
            { 
                main = this;
                DontDestroyOnLoad(this);
            }

            matMaker = GetComponent<MaterialMaker>();
            meshRegistry = GetComponent<MeshRegistry>();
            meshRegistry.FastSearch = fastTreeSearch;

            if(dataSets.Count > 0)
            {
                for (int i = 0; i < dataSets.Count; i++)
                {
                    PopulateDataSet(i);
                }
            }

            AddComponentToPrefabs(false);
        }

        private void OnDestroy()
        {
            for (int i = 0; i < dataSets.Count; i++)
            {
                for (int ii = 0; ii < dataSets[i].prefabs.Length; ii++)
                {
                    if (null != dataSets[i].prefabs[ii].prefab.GetComponent<MergerTool_Component>())
                    {
                        if (dataSets[i].prefabs[ii].prefab.GetComponent<MergerTool_Component>().wasAddedManually == false)
                        { dataSets[i].prefabs[ii].prefab.GetComponent<MergerTool_Component>().DestroyComponent(); }
                    }
                }
            }
        }

        public void AddComponentToPrefabs(bool manuallyAdded)
        {
            for (int i = 0; i < dataSets.Count; i++)
            {
                for (int ii = 0; ii < dataSets[i].prefabs.Length; ii++)
                {
                    if (null == dataSets[i].prefabs[ii].prefab.GetComponent<MergerTool_Component>())
                    {
                        dataSets[i].prefabs[ii].prefab.AddComponent<MergerTool_Component>().wasAddedManually = manuallyAdded;
                        dataSets[i].prefabs[ii].prefab.GetComponent<MergerTool_Component>().ID = dataSets[i].ID;
                    }
                }
            }
        }

        private void PopulateDataSet(int index)
        {
            dataSets[index].textureRegistry.registrySize = dataSets[index].prefabs.Length;
            dataSets[index].mergedMaterial = matMaker.Run(dataSets[index]);
        }

        public DataPacket getData(string ID, MergerTool_Component component)
        {
            component.meshRegistry = this.meshRegistry;

            for (int i = 0; i < dataSets.Count; i++)
            {
                if(dataSets[i].ID == ID) { return dataSets[i]; }
            }
            throw new System.Exception("!!! ERROR: No Datapacket with ID: '" + ID + "' Found !!!");
        }

        #region AddFunctions

        public void Add_DataPacket(string ID, PrefabStruct[] prefabStruct)
        {
            for (int i = 0; i < dataSets.Count; i++)
            { if (dataSets[i].ID == ID) { throw new System.Exception("!!! ERROR: MergeTool Already Contains DataPacket With ID: '" + ID + "' !!!"); } }
            
            dataSets.Add(new DataPacket(ID, prefabStruct));
            PopulateDataSet(dataSets.Count - 1);
        }

        public void Add_DataPacket(DataPacket newPacket)
        {
            if (null == dataSets) { dataSets = new List<DataPacket>(); Debug.Log("DataSets was Null"); }

            for (int i = 0; i < dataSets.Count; i++)
            { if (dataSets[i].ID == newPacket.ID) { throw new System.Exception("!!! ERROR: MergeTool Already Contains DataPacket With ID: '" + newPacket.ID + "' !!!"); } }

            dataSets.Add(newPacket);
            PopulateDataSet(dataSets.Count - 1);
        }

        #endregion

        #region RemoveFunctions
        public void Remove_DataPacket(string ID)
        {
            for (int i = 0; i < dataSets.Count; i++)
            {
                if (dataSets[i].ID == ID) { dataSets.RemoveAt(i); }
                else { throw new System.Exception("!!! ERROR: MergeTool Does Not Contain DataPacket With ID: '" + ID + "' !!!"); }
            }
        }

        public void Remove_DataPacket(DataPacket packet)
        {
            if(!dataSets.Contains(packet)) { throw new System.Exception("!!! ERROR: MergeTool Does Not Contain DataPacket: '" + packet.ID + "' !!!"); }
            else { dataSets.Remove(packet); }
        }

        #endregion

        #region ClearFunctions
        public void Clear_DataPackets() { dataSets.Clear(); }
        
        #endregion
    }
}
