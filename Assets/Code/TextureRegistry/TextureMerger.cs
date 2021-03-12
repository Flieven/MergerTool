using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureMerger : MonoBehaviour
{
    [SerializeField] private Shader mergeShader;

    [SerializeField] private Texture2DRegistry textureRegistry;

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
    }

    void CreateCustomMaterials()
    {
        for (int i = 0; i < materialArray.Length; i++)
        {
            Material newMat = new Material(mergeShader);
            ApplyTextureData(newMat, i);
            materialArray[i] = newMat;
        }
    }

    void ApplyTextureData(Material mat, int currentIndex)
    {
        mat.SetTexture("_TexArr", textureRegistry.mergeSets[currentIndex].diffuse.texture2DArray);
    }

}
