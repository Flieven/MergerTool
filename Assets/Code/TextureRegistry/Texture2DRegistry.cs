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
    public int registrySize;
    public Texture2DArrays diffuse;
    public Texture2DArrays normal;
}

[System.Serializable]
public class Texture2DRegistry : MonoBehaviour
{
    public void Run(DataPacket packet)
    {
        for (int i = 0; i < packet.textureRegistry.registrySize; i++)
        {
            // Set the Array size for each texture map
            packet.textureRegistry.diffuse.array = new Texture2D[packet.textureRegistry.registrySize];
            packet.textureRegistry.normal.array = new Texture2D[packet.textureRegistry.registrySize];

            //Extract textures to their associated texture2D arrays
            ExtractTextures(packet);

        }
    }

    void ExtractTextures(DataPacket packet)
    {
        for (int i = 0; i < packet.textureRegistry.registrySize; i++)
        {
            if(null != packet.prefabs[i].prefab.GetComponent<Renderer>())
            {
                Material currentMat = packet.prefabs[i].prefab.GetComponent<Renderer>().sharedMaterial;

                if(null != currentMat.mainTexture) { packet.textureRegistry.diffuse.array[i] = currentMat.mainTexture as Texture2D; }
                else { Debug.LogWarning("<<< '" + currentMat.name + "' Does not contain a mainTexture >>>"); }
                
                if (null != currentMat.GetTexture("_BumpMap")) { packet.textureRegistry.normal.array[i] = currentMat.GetTexture("_BumpMap") as Texture2D; }
                else { Debug.LogWarning("<<< '" + currentMat.name + "' Does not contain a BumpMap >>>"); }

            }
            
            else { throw new System.Exception("!!! ERROR: No Renderer component found on prefab object: '" + packet.prefabs[i].prefab.name + "' !!!"); }
        }
    }

    public Texture2DArray MakeTexture2DArray(Texture2DArrays currentArray, string ID)
    {
        Texture2DArray newT2DArray = new Texture2DArray(512, 512, currentArray.array.Length, TextureFormat.ARGB32, false);

        for (int i = 0; i < currentArray.array.Length; i++)
        {
            //TODO: Currently only works with sizes = or > than the given (as unity can size things down but not up to match on import). Figure out if can use smaller size as well, or if that needs special functionality.
            Graphics.CopyTexture(currentArray.array[i], 0, 0, newT2DArray, i,0);
        }

        return newT2DArray;
    }
}
