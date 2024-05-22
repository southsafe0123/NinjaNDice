using System.Collections.Generic;
using UnityEngine;

public class SimpleObjectPool : MonoBehaviour
{
    public GameObject prefabObj;
    public Dictionary<string, List<GameObject>> dicPool = new Dictionary<string, List<GameObject>>();

    public void GetFromPool(string gameObjectTag)
    {
        if (!dicPool.ContainsKey(gameObjectTag)) dicPool[gameObjectTag] = new List<GameObject>();

        List<GameObject> objsInPool = dicPool[gameObjectTag];

        if (objsInPool.Count < 1 || IsAllObjectInPoolActive(objsInPool))
        {
            GameObject newObj = Instantiate(prefabObj, transform);
            newObj.tag = gameObjectTag;
            newObj.SetActive(false);
            dicPool[gameObjectTag].Add(newObj);
        }
        
        GetUnActiveObjInPool(objsInPool)?.SetActive(true);
    }

    private GameObject GetUnActiveObjInPool(List<GameObject> objsInPool)
    {
        foreach (GameObject obj in objsInPool)
        {
            if (!obj.activeSelf)
            {
                return obj;
            }
        }
        return null;
    }

    private bool IsAllObjectInPoolActive(List<GameObject> objsInPool)
    {
        foreach (GameObject obj in objsInPool)
        {
            if (!obj.activeSelf) return false;
        }

        return true;
    }
}