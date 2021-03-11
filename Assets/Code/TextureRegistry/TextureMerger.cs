using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureMerger : MonoBehaviour
{
    [SerializeField] private Texture2DRegistry textureRegistry;

    [SerializeField] private Material[] materialArray;

    private void Awake()
    {
       if(null != textureRegistry) 
        {
            materialArray = new Material[textureRegistry.mergeSets.Length];
            textureRegistry.Run(); 
        }
    }

    void CreateCustomMaterials(Texture2DStruct currentStruct)
    {

    }

}
