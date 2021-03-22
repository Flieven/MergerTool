using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataPacket
{
    public string ID;
    public GameObject[] prefabs;
    public Texture2DStruct textureRegistry;
}

[RequireComponent(typeof(MaterialMaker), typeof(MeshRegistry))]
public class MergerTool_Main : MonoBehaviour
{
    [SerializeField] private DataPacket[] dataSets;

    [SerializeField] private MaterialMaker matMaker = null;
    [SerializeField] private MeshRegistry meshRegistry = null;

    private void Awake()
    {
        matMaker = GetComponent<MaterialMaker>();
        meshRegistry = GetComponent<MeshRegistry>();

        for (int i = 0; i < dataSets.Length; i++)
        {
            dataSets[i].textureRegistry.registrySize = dataSets[i].prefabs.Length;
        }

        matMaker.Run(dataSets);

        for (int i = 0; i < dataSets.Length; i++)
        {
            for (int ii = 0; ii < dataSets[i].prefabs.Length; ii++)
            {
                if (null == dataSets[i].prefabs[ii].GetComponent<MergerTool_Component>())
                {
                    dataSets[i].prefabs[ii].AddComponent<MergerTool_Component>();
                }
                dataSets[i].prefabs[ii].GetComponent<MergerTool_Component>().ConstructComponent(matMaker, meshRegistry, dataSets[i].ID, ii);
            }
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < dataSets.Length; i++)
        {
            for (int ii = 0; ii < dataSets[i].prefabs.Length; ii++)
            {
                dataSets[i].prefabs[ii].GetComponent<MergerTool_Component>().DestroyComponent();
            }
        }

        //ClearDirectory();
    }
}
