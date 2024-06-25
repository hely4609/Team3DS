using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ResourceManager : Manager
{
    static Dictionary<ResourceEnum.Prefab, GameObject> prefabDictionary;

    public static int resourceAmount = 0;
    public static int resourceLoadCompleted = 0;
    public override IEnumerator Initiate()
    {
        if (prefabDictionary != null) yield break;

        prefabDictionary = new Dictionary<ResourceEnum.Prefab, GameObject> ();

        resourceAmount = 0;
        resourceLoadCompleted = 0;

        resourceAmount += prefabDictionary.Count;

        yield return Load<ResourceEnum.Prefab, GameObject>(prefabDictionary, ResourcesPath.prefabPathArray, "prefabs");
    }


    // resourceType : �ε��Ҷ� �����ֱ��
    IEnumerator Load<key, value>(Dictionary<key, value> dictionary, string[] pathArray, string resourceType) where key : Enum where value : UnityEngine.Object
    {
        for(int i =0; i < pathArray.Length; i++)
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
            //              fileName�̶�� �̸��� key�� ������ fileKey ��ȯ
            if (Enum.TryParse(typeof(key), fileName, out object fileKey))
            {
                // path��ġ�� ������ ã�Ƽ� value�� �ְ� �� value�� fileKey�� ���� ��ųʸ��� �߰�
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

}
