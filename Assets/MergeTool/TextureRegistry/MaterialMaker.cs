using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[AddComponentMenu("MergerTool/Required Components/Material Constructor")]
[RequireComponent(typeof(Texture2DRegistry))]
public class MaterialMaker : MonoBehaviour
{
    [ReadOnly] [SerializeField] private Shader mergeShader;

    Texture2DRegistry textureRegistry = null;

    public Material Run(DataPacket packet)
    {

        string filePath = System.IO.Path.Combine("Assets", "MergeTool", "TextureRegistry", "MergerShader.shader");
        mergeShader = AssetDatabase.LoadAssetAtPath<Shader>(filePath);

        textureRegistry = GetComponent<Texture2DRegistry>();

       if(null != textureRegistry) 
       {
            textureRegistry.Run(packet);
            return CreateCustomMaterials(packet);
       }

       else { throw new System.Exception("!!! ERROR: No TextureRegistry !!!"); }
       throw new System.Exception("!!! ERROR: Both If/Else ignored in MaterialMaker.Run() !!!");
    }

    Material CreateCustomMaterials(DataPacket packet)
    {
        Material newMat = new Material(mergeShader);
        ApplyTextureData(newMat, packet);
        return newMat;
    }

    void ApplyTextureData(Material mat, DataPacket packet)
    {
        //Set the diffuse texture
        mat.SetTexture("_MainTex", textureRegistry.MakeTexture2DArray(packet.textureRegistry.diffuse, packet.ID, packet.textureSize));

        //Set the normal texture
        mat.SetTexture("_BumpMap", textureRegistry.MakeTexture2DArray(packet.textureRegistry.normal, packet.ID, packet.textureSize));

        //NOT YET IMPLEMENTED IN SHADER

        ////Set the height texture
        //mat.SetTexture("_ParallaxMap", textureRegistry.MakeTexture2DArray(packet.textureRegistry.height, packet.ID, packet.textureSize));
        
        ////Set the occlusion texture
        //mat.SetTexture("_OcclusionMap", textureRegistry.MakeTexture2DArray(packet.textureRegistry.occlusion, packet.ID, packet.textureSize));
       
        ////Set the detailMask texture
        //mat.SetTexture("_DetailMask", textureRegistry.MakeTexture2DArray(packet.textureRegistry.detailMask, packet.ID, packet.textureSize));

        mat.name = "MergedMaterial_" + packet.ID;
    }
}
