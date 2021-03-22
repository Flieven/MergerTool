using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MaterialMaker : MonoBehaviour
{
    [SerializeField] private Shader mergeShader;

    [SerializeField] private Texture2DRegistry textureRegistry;
<<<<<<< HEAD

    [SerializeField] private Material[] materialArray;

    private void Awake()
    {
       if(null != textureRegistry) 
       {
            materialArray = new Material[textureRegistry.mergeSets.Length];
            textureRegistry.Run(); 
       }
       else { throw new System.Exception("ERROR: No TextureRegistry"); }
       
        CreateCustomMaterials();
=======

    [SerializeField] private Dictionary<string, Material> materialDictionary;

    private void Awake()
    {
        ClearDirectory();

       if(null != textureRegistry) 
       {
            materialDictionary = new Dictionary<string, Material>();
            textureRegistry.Run(); 
       }
       else { throw new System.Exception("ERROR: No TextureRegistry"); }

       CreateCustomMaterials();

        for (int i = 0; i < textureRegistry.mergeSets.Length; i++)
        {
            for (int ii = 0; ii < textureRegistry.mergeSets[i].prefabs.Length; ii++)
            {
                if (!textureRegistry.mergeSets[i].prefabs[ii].GetComponent<MergerTool_Component>())
                {
                    textureRegistry.mergeSets[i].prefabs[ii].AddComponent<MergerTool_Component>();
                }
                textureRegistry.mergeSets[i].prefabs[ii].GetComponent<MergerTool_Component>().ConstructComponent(this, textureRegistry.mergeSets[i].setID);
            }
        }
    }

    void ClearDirectory()
    {
        string filePath = System.IO.Path.Combine("Assets", "Code", "TextureRegistry", "TextureArrays");

        if (Directory.Exists(filePath)) { AssetDatabase.DeleteAsset(filePath); }
        Directory.CreateDirectory(filePath);
        AssetDatabase.Refresh();
    }

    private void OnDestroy()
    {
        for (int i = 0; i < textureRegistry.mergeSets.Length; i++)
        {
            for (int ii = 0; ii < textureRegistry.mergeSets[i].prefabs.Length; ii++)
            {
                textureRegistry.mergeSets[i].prefabs[ii].GetComponent<Renderer>().sharedMaterial = textureRegistry.mergeSets[i].prefabs[ii].GetComponent<MergerTool_Component>().getOriginalMat;
                textureRegistry.mergeSets[i].prefabs[ii].GetComponent<MergerTool_Component>().DestroyComponent();
            }
        }

        ClearDirectory();
>>>>>>> parent of 759b0e0 (Mesh Merging 0.1)
    }

    void CreateCustomMaterials()
    {
        for (int i = 0; i < materialArray.Length; i++)
        {
            Material newMat = new Material(mergeShader);
            ApplyTextureData(newMat, textureRegistry.mergeSets[i].setID);
<<<<<<< HEAD
            materialArray[i] = newMat;
            ApplyMaterial(i);
=======
            materialDictionary.Add(textureRegistry.mergeSets[i].setID, newMat);
            
            if(materialDictionary.ContainsKey(textureRegistry.mergeSets[i].setID)) { Debug.Log("Key: " + textureRegistry.mergeSets[i].setID + " exists with value: " + materialDictionary[textureRegistry.mergeSets[i].setID].name); }
            else { Debug.Log("No key of type: " + textureRegistry.mergeSets[i].setID + " was created!"); }
            
            //ApplyMaterial(i);
>>>>>>> parent of 759b0e0 (Mesh Merging 0.1)
        }
    }

    void ApplyTextureData(Material mat, string ID)
    {
        //Set the diffuse texture
        string newT2DArrayPath = System.IO.Path.Combine("Assets", "Code", "TextureRegistry", "TextureArrays", ID + "_diffuse" + ".asset");
        mat.SetTexture("_MainTex", (Texture2DArray)AssetDatabase.LoadAssetAtPath(newT2DArrayPath, typeof(Texture2DArray)));

        //Set the normal texture
        newT2DArrayPath = System.IO.Path.Combine("Assets", "Code", "TextureRegistry", "TextureArrays", ID + "_normal" + ".asset");
        mat.SetTexture("_BumpMap", (Texture2DArray)AssetDatabase.LoadAssetAtPath(newT2DArrayPath, typeof(Texture2DArray)));

        mat.name = "MergedMaterial_" + ID;
    }

    void ApplyMaterial(int index)
    {
        for (int i = 0; i < textureRegistry.mergeSets[index].prefabs.Length; i++)
        {
            textureRegistry.mergeSets[index].prefabs[i].GetComponent<MeshRenderer>().sharedMaterial = materialArray[index];
            List<Vector3> uvList = new List<Vector3>();

            for (int ii = 0; ii < textureRegistry.mergeSets[index].prefabs[i].GetComponent<MeshFilter>().sharedMesh.uv.Length; ii++)
            {
                uvList.Add(new Vector3(textureRegistry.mergeSets[index].prefabs[i].GetComponent<MeshFilter>().sharedMesh.uv[ii].x,
                    textureRegistry.mergeSets[index].prefabs[i].GetComponent<MeshFilter>().sharedMesh.uv[ii].y, i));
            }

            /*Debug.Log("Vert array size: " + textureRegistry.mergeSets[index].prefabs[i].GetComponent<MeshFilter>().sharedMesh.vertices.Length + "\n uvs: " +
               textureRegistry.mergeSets[index].prefabs[i].GetComponent<MeshFilter>().sharedMesh.uv.Length);*/

            textureRegistry.mergeSets[index].prefabs[i].GetComponent<MeshFilter>().sharedMesh.SetUVs(0, uvList);
        }
    }

}
