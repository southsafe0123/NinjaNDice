using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;



public class SkinInfo
{
    //     {
    //     "id": "001",
    //     "name": "Ninja Mage Black",
    //     "version": "1.0",
    //     "description": "Ninja Mage Black Skin",
    //     "author": "Nhat Nguyen",
    //     "updateDate": "2019-07-01",
    //     "price": 100,
    //     "currency": "USD"
    //      }

    public string id;
    public string name;
    public string version;
    public string description;
    public string author;
    public string updateDate;
    public int price;
    public string currency;

}


public class UpdateSkinsHandle : MonoBehaviour
{
    [SerializeField] private string folderPath;
    [SerializeField] private string[] skinsFolderPaths;
    [SerializeField] private SkinInfo[] skinsInfo;
    // [SerializeField] private GameObject skinPrefab;

    [SerializeField] private Transform content;
    [SerializeField] private AssetLabelReference assetLabelReference;


    // get name all folder in path
    public void RenewData()
    {
        Addressables.LoadAssetsAsync<GameObject>(assetLabelReference, (gameObject) =>
        {
            if (gameObject.name == "ItemShop")
            {
                Instantiate(gameObject, content);
            }
        });
    }

    public SkinInfo GetInfoSkin(string id)
    {
        SkinInfo S = new SkinInfo();
        string path = skinsFolderPaths.FirstOrDefault(x => x.Contains(id));
        if (System.IO.Directory.Exists(path))
        {
            string jsonPath = path + "/SkinInfo.json";
            if (System.IO.File.Exists(jsonPath))
            {
                string json = System.IO.File.ReadAllText(jsonPath);
                S = JsonUtility.FromJson<SkinInfo>(json);

                Debug.Log("Get info skin success: " + S.name);
            }
            else
            {
                Debug.Log("Info file not found");
                return null;
            }
        }

        return S;
    }

    public void UpdateSkins()
    {
        if (skinsInfo.Length > 0)
        {
            foreach (Transform child in content)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < skinsInfo.Length; i++)
            {
                //Assets/NhatElements/Prefabs/Character/Skins/001/ItemShop.prefab
                Addressables.LoadAssetsAsync<GameObject>(assetLabelReference, (gameObject) =>
                {
                    if (gameObject.name == "ItemShop")
                    {
                        Instantiate(gameObject, content);
                    }

                    // skin.GetComponent<SkinItem>().SetData(skinsInfo[i], skin.GetComponentInChildren<SpriteRenderer>().sprite);
                });
            }
        }
    }


    // private void OnPrefabLoaded(AsyncOperationHandle<GameObject> obj)
    // {
    //     if (obj.Status == AsyncOperationStatus.Succeeded)
    //     {
    //         GameObject prefab = obj.Result;
    //         Instantiate(prefab, content);

    //     }
    //     else
    //     {
    //         Debug.LogError("Failed to load prefab");
    //     }
    // }




}
