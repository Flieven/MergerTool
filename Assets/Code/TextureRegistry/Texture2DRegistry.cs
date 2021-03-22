using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public struct Texture2DArrays
{
    public Texture2D[] array;
}

[System.Serializable]
public struct Texture2DStruct
{
    [SerializeField] internal string setID;
    [SerializeField] internal GameObject[] prefabs;
    public Texture2DArrays diffuse;
    public Texture2DArrays normal;
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
            mergeSets[i].normal.array = new Texture2D[mergeSets[i].prefabs.Length];

            //Extract textures to their associated texture2D arrays
            ExtractTextures(mergeSets[i]);

            //Make the now extracted textures into a Texture2DArray
            //Debug.Log("Merge Set: " + i);
            MakeTexture2DArray(mergeSets[i].diffuse, mergeSets[i].setID);
            MakeNormal2DArray(mergeSets[i].normal, mergeSets[i].setID);
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
                
                if (null != currentMat.GetTexture("_BumpMap")) { currentStruct.normal.array[i] = currentMat.GetTexture("_BumpMap") as Texture2D; }
                else { Debug.LogWarning(currentMat.name + " Does not contain a BumpMap"); }

            }
            
            else { throw new System.Exception("ERROR: No Renderer component found on prefab object: " + currentStruct.prefabs[i].name); }
        }
<<<<<<< HEAD
    }

    void MakeTexture2DArray(Texture2DArrays currentArray, string ID)
    {
        Texture2DArray newT2DArray = new Texture2DArray(512, 512, currentArray.array.Length, TextureFormat.ARGB32, false);

        for (int i = 0; i < currentArray.array.Length; i++)
        {
            //TODO: Currently only works with sizes = or > than the given (as unity can size things down but not up to match on import). Figure out if can use smaller size as well, or if that needs special functionality.

            Color[] newColors = currentArray.array[i].GetPixels(0);
            //Debug.Log("Texture2DArray set at " + i + " with size: " + newColors.Length + "\nWants: " + (512 * 512));
            newT2DArray.SetPixels(newColors, i, 0);
        }

        newT2DArray.Apply();

        string newT2DArrayPath = System.IO.Path.Combine("Assets", "Code", "TextureRegistry", "TextureArrays", ID + "_diffuse" + ".asset");
        AssetDatabase.CreateAsset(newT2DArray, newT2DArrayPath);
=======
>>>>>>> parent of 759b0e0 (Mesh Merging 0.1)
    }

    void MakeNormal2DArray(Texture2DArrays currentArray, string ID)
    {
        Texture2DArray newT2DArray = new Texture2DArray(512, 512, currentArray.array.Length, TextureFormat.ARGB32, true, true);

        for (int i = 0; i < currentArray.array.Length; i++)
        {
            //TODO: Currently only works with sizes = or > than the given (as unity can size things down but not up to match on import). Figure out if can use smaller size as well, or if that needs special functionality.

            Color[] newColors = currentArray.array[i].GetPixels(0);
            //Debug.Log("Texture2DArray set at " + i + " with size: " + newColors.Length + "\nWants: " + (512 * 512));
            newT2DArray.SetPixels(newColors, i, 0);
        }

        newT2DArray.Apply();

        string newT2DArrayPath = System.IO.Path.Combine("Assets", "Code", "TextureRegistry", "TextureArrays", ID + "_normal" + ".asset");
        AssetDatabase.CreateAsset(newT2DArray, newT2DArrayPath);
    }
}
