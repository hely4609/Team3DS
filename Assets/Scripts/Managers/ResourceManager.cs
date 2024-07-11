using ResourceEnum;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ResourceManager : Manager
{
    static Dictionary<ResourceEnum.Prefab, GameObject> prefabDictionary;
    static Dictionary<ResourceEnum.Material, UnityEngine.Material> materialDictionary;

    public static int resourceAmount = 0;
    public static int resourceLoadCompleted = 0;
    public override IEnumerator Initiate()
    {
        if (prefabDictionary != null) yield break;
        if (materialDictionary != null) yield break;

        prefabDictionary = new Dictionary<ResourceEnum.Prefab, GameObject>();
        materialDictionary = new Dictionary<ResourceEnum.Material, UnityEngine.Material>();

        resourceAmount = 0;
        resourceLoadCompleted = 0;

        resourceAmount += prefabDictionary.Count;
        resourceAmount += materialDictionary.Count;

        yield return Load<ResourceEnum.Prefab, GameObject>(prefabDictionary, ResourcesPath.prefabPathArray, "prefabs");
        yield return Load<ResourceEnum.Material, UnityEngine.Material>(materialDictionary, ResourcesPath.MaterialPathArray, "materials");
    }


    // resourceType : 로딩할때 보여주기용
    IEnumerator Load<key, value>(Dictionary<key, value> dictionary, string[] pathArray, string resourceType) where key : Enum where value : UnityEngine.Object
    {
        for (int i = 0; i < pathArray.Length; i++)
        {
            if (Load(pathArray[i], dictionary))
            {
                GameManager.ClaimLoadInfo(resourceType);
                resourceLoadCompleted++;
            }
            else
            {
                yield return null;
            }
        }
    }

    bool Load<key, value>(string path, Dictionary<key, value> dictionary) where key : Enum where value : UnityEngine.Object
    {
        try
        {
            //                     Extensions
            string fileName = path.GetFileName();
            //              fileName이라는 이름의 key가 있으면 fileKey 반환
            if (Enum.TryParse(typeof(key), fileName, out object fileKey))
            {
                // path위치의 파일을 찾아서 value에 넣고 그 value를 fileKey와 묶어 딕셔너리에 추가
                value loadedData = Load<value>(path);
                if (loadedData == null) return false;
                dictionary.Add((key)fileKey, loadedData);
                return true;
            }
            else
            {
                Debug.LogWarning($"There is no key to match with file name '{fileName}'");
                Exception currentException = new Exception("Enum mismatch");
                throw currentException;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"{e} : Fail to load '{path}'");
            return false;
        }
    }

    value Load<value>(string path) where value : UnityEngine.Object
    {
        try
        {
            value loadedData = Resources.Load<value>(path);
            if (loadedData == null) throw new Exception("File not found");
            return loadedData;

        }
        catch (Exception e)
        {
            Debug.LogWarning($"{e} : Fail to load '{path}'");
            return null;
        }
    }

    public static GameObject Get(ResourceEnum.Prefab prefab)
    {
        if (prefabDictionary.TryGetValue(prefab, out GameObject result))
        {
            return result;
        }
        return null;
    }

    public static UnityEngine.Material Get(ResourceEnum.Material material)
    {
        if (materialDictionary.TryGetValue(material, out UnityEngine.Material result))
        {
            return result;
        }
        return null;
    }
}