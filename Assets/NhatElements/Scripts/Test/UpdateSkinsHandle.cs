using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Unity.VisualScripting;
using UnityEngine.AddressableAssets.ResourceLocators;
using System;






public class UpdateSkinsHandle : MonoBehaviour
{
    public static UpdateSkinsHandle instance;
    // [SerializeField] private List<skin> skinsInfo;
    [SerializeField] private Dictionary<string, GameObject> itemSkins;

    [SerializeField] private Transform content;
    [SerializeField] private AssetLabelReference assetLabelReference;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // Addressables.CheckForCatalogUpdates().Completed += OnCheckForCatalogUpdatesComplete;
    }

    // get name all folder in path
    public void RenewData()
    {
        // xoa het cac skin cu nam trong content
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

    }

    // public skin GetInfoSkin(string id)
    // {
    //     skin S = new skin();
    //     string path = skinsFolderPaths.FirstOrDefault(x => x.Contains(id));
    //     if (System.IO.Directory.Exists(path))
    //     {
    //         string jsonPath = path + "/SkinInfo.json";
    //         if (System.IO.File.Exists(jsonPath))
    //         {
    //             string json = System.IO.File.ReadAllText(jsonPath);
    //             S = JsonUtility.FromJson<skin>(json);

    //             Debug.Log("Get info skin success: " + S.name);
    //         }
    //         else
    //         {
    //             Debug.Log("Info file not found");
    //             return null;
    //         }
    //     }

    //     return S;
    // }

    // public void UpdateSkins()
    // {
    //     if (skinsInfo.Length > 0)
    //     {
    //         foreach (Transform child in content)
    //         {
    //             Destroy(child.gameObject);
    //         }

    //         for (int i = 0; i < skinsInfo.Length; i++)
    //         {
    //             //Assets/NhatElements/Prefabs/Character/Skins/001/ItemShop.prefab
    //             Addressables.LoadAssetsAsync<GameObject>(assetLabelReference, (gameObject) =>
    //             {
    //                 if (gameObject.name == "ItemShop")
    //                 {
    //                     Instantiate(gameObject, content);
    //                 }

    //                 // skin.GetComponent<SkinItem>().SetData(skinsInfo[i], skin.GetComponentInChildren<SpriteRenderer>().sprite);
    //             });
    //         }
    //     }
    // }

    // Tải một asset từ remote
    public void LoadRemoteAsset()
    {
        //Skins/66bc57c5ea0484839ae5c2c1/ItemShop.prefab
        LoadPrefab(ApiHandle.Instance.skins);
        // Kiểm tra cập nhật catalog
        // Addressables.CheckForCatalogUpdates().Completed += OnCheckForCatalogUpdatesComplete;

    }

    public void updateSkins()
    {
        StartCoroutine(ApiHandle.Instance.getAllSkins());
    }

    private void OnCheckForCatalogUpdatesComplete(AsyncOperationHandle<List<string>> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result.Count > 0)
        {
            Debug.Log("Catalog updates found, downloading...");

            // Tải và áp dụng catalog mới
            Addressables.UpdateCatalogs(handle.Result).Completed += OnUpdateCatalogsComplete;
        }
        else
        {
            Debug.Log("No catalog updates found.");
            LoadPrefab(ApiHandle.Instance.skins);
        }
    }

    private void OnUpdateCatalogsComplete(AsyncOperationHandle<List<IResourceLocator>> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Catalogs updated successfully.");
            // Tiếp tục với các bước load tài nguyên sau khi cập nhật catalog
            LoadPrefab(ApiHandle.Instance.skins);
        }
        else
        {
            Debug.LogError("Failed to update catalogs.");
        }
    }

    [ContextMenu("test")]
    public void LoadPrefab(List<skin> skins)
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
        foreach (skin s in skins)
        {
            //Skins/66bc57c5ea0484839ae5c2c1/ItemShop.prefab
            string path = "Skins/" + s._id + "/ItemShop.prefab";
            //load tu addressable
            Addressables.LoadAssetAsync<GameObject>(path).Completed += (obj) =>
            {
                if (obj.Status == AsyncOperationStatus.Succeeded)
                {
                    GameObject prefab = obj.Result;
                    GameObject item = Instantiate(prefab, content);
                    item.GetComponent<ItemShop>().UpdateData(s, s._id);
                    SkinPool.instance.CallUpdateSkinPool();
                }
                else
                {
                    Debug.LogError("Failed to load prefab");
                }
            };
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
