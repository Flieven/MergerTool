using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Texture2DArrays
{
    public Texture2D[] array;
    public Texture2DArray texture2DArray;
}

[System.Serializable]
public struct Texture2DStruct
{
    [SerializeField] private string setID;
    [SerializeField] internal GameObject[] prefabs;
    public Texture2DArrays diffuse;
}

[System.Serializable]
public class Texture2DRegistry
{
    public Texture2DStruct[] mergeSets;

    public void Run()
    {
        for (int i = 0; i < mergeSets.Length; i++)
        {
            // Set the Array size for each texture map
            mergeSets[i].diffuse.array = new Texture2D[mergeSets[i].prefabs.Length];

            //Extract textures to their associated texture2D arrays
            ExtractTextures(mergeSets[i]);
            
            //Make the now extracted textures into a Texture2DArray
            MakeTexture2DArray(mergeSets[i].diffuse);
        }
    }

    void ExtractTextures(Texture2DStruct currentStruct)
    {
        for (int i = 0; i < currentStruct.prefabs.Length; i++)
        {
            if(null != currentStruct.prefabs[i].GetComponent<Renderer>())
            {
                Material currentMat = currentStruct.prefabs[i].GetComponent<Renderer>().sharedMaterial;

                if(null != currentMat.mainTexture) { currentStruct.diffuse.array[i] = currentMat.mainTexture as Texture2D; }
                else { Debug.LogWarning(currentMat.name + " Does not contain a mainTexture"); }

            }
            
            else { throw new System.Exception("ERROR: No Renderer component found on prefab object: " + currentStruct.prefabs[i].name); }
        }
    }

    void MakeTexture2DArray(Texture2DArrays currentMap)
    {
        currentMap.texture2DArray = new Texture2DArray(1024, 1024, currentMap.array.Length, TextureFormat.ARGB32, false);
        
        for (int i = 0; i < currentMap.array.Length; i++)
        {
            //TODO: Figure out why this is such a strange size? It's coming out at 262144 which is not 1024*1024

            Color[] newColors = currentMap.array[i].GetPixels(0);
            Debug.Log("Size " + i + " : " + newColors.Length);
            currentMap.texture2DArray.SetPixels(newColors, i, 0);
        }

        currentMap.texture2DArray.Apply();

    }
}
