using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(Texture2DRegistry))]
public class MaterialMaker : MonoBehaviour
{
    [SerializeField] private Shader mergeShader;

    [SerializeField] private Dictionary<string, Material> materialDictionary;

    Texture2DRegistry textureRegistry = null;

    public void Run(DataPacket[] packet)
    {

        string filePath = System.IO.Path.Combine("Assets", "Code", "TextureRegistry", "MergerShader.shader");
        mergeShader = AssetDatabase.LoadAssetAtPath<Shader>(filePath);
        //ClearDirectory();

        textureRegistry = GetComponent<Texture2DRegistry>();

       if(null != textureRegistry) 
       {
            materialDictionary = new Dictionary<string, Material>();
            for (int i = 0; i < packet.Length; i++)
            {
                textureRegistry.Run(packet[i]);
                CreateCustomMaterials(packet[i]);
            }
       }
       else { throw new System.Exception("!!! ERROR: No TextureRegistry !!!"); }
    }

    //void ClearDirectory()
    //{
    //    string filePath = System.IO.Path.Combine("Assets", "Code", "TextureRegistry", "TextureArrays");

    //    if (Directory.Exists(filePath)) { AssetDatabase.DeleteAsset(filePath); }
    //    Directory.CreateDirectory(filePath);
    //    AssetDatabase.Refresh();
    //}

    void CreateCustomMaterials(DataPacket packet)
    {
        Material newMat = new Material(mergeShader);
        ApplyTextureData(newMat, packet);
        if (!materialDictionary.ContainsKey(packet.ID)) 
        {
            Debug.Log("===== New Material '" + newMat.name + "' Registered To Material Dictionary =====");
            materialDictionary.Add(packet.ID, newMat); 
        }
    }

    void ApplyTextureData(Material mat, DataPacket packet)
    {
        //Set the diffuse texture
        mat.SetTexture("_MainTex", textureRegistry.MakeTexture2DArray(packet.textureRegistry.diffuse, packet.ID));

        //Set the normal texture
        mat.SetTexture("_BumpMap", textureRegistry.MakeTexture2DArray(packet.textureRegistry.normal, packet.ID));

        mat.name = "MergedMaterial_" + packet.ID;
    }

    public Material getMaterial(string ID)
    {
        if(materialDictionary.ContainsKey(ID)) { return materialDictionary[ID]; }
        else { throw new System.Exception("!!! ERROR: No Material In Dictionary Found With ID: '" + ID + "' !!!"); }
    }

}
