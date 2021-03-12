using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public struct Texture2DArrays
{
    public Texture2D[] array;
    public Texture2DArray texture2DArray;
}

[System.Serializable]
public struct Texture2DStruct
{
    [SerializeField] internal string setID;
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
            Debug.Log("Merge Set: " + i);
            MakeTexture2DArray(mergeSets[i]);
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

    void MakeTexture2DArray(Texture2DStruct currentStruct)
    {
        currentStruct.diffuse.texture2DArray = new Texture2DArray(512, 512, currentStruct.diffuse.array.Length, TextureFormat.ARGB32, false);

        for (int i = 0; i < currentStruct.diffuse.array.Length; i++)
        {
            //TODO: Currently only works with sizes = or > than the given (as unity can size things down but not up to match on import). Figure out if can use smaller size as well, or if that needs special functionality.

            Color[] newColors = currentStruct.diffuse.array[i].GetPixels(0);
            Debug.Log("Texture2DArray set at " + i + " with size: " + newColors.Length + "\nWants: " + (512 * 512));
            currentStruct.diffuse.texture2DArray.SetPixels(newColors, i, 0);
        }

        currentStruct.diffuse.texture2DArray.Apply();
        AssetDatabase.CreateAsset(currentStruct.diffuse.texture2DArray, "Assets/Code/TextureRegistry/TextureArrays/" + currentStruct.setID + ".tarr");


        if (null == currentStruct.diffuse.texture2DArray)
        { throw new System.Exception("ERROR: Something went wrong, no Texture2DArray created for " + currentStruct.setID); }
    }
}
