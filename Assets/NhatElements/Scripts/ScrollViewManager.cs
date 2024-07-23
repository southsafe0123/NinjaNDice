using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewManager : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform contentPanel;
    public ObjectPool objectPool;

    public GameObject shopManager;

    private List<GameObject> activeObjects = new List<GameObject>();



    [SerializeField] private int totalItems = 100; // Tổng số mục trong ScrollView
    [SerializeField] private int visibleItemsCount = 35; // Số lượng mục có thể hiển thị cùng một lúc

    void Start()
    {
        if (scrollRect == null)
        {
            Debug.LogError("ScrollRect is not assigned in ScrollViewManager.");
            return;
        }

        if (contentPanel == null)
        {
            Debug.LogError("ContentPanel is not assigned in ScrollViewManager.");
            return;
        }

        if (objectPool == null)
        {
            Debug.LogError("ObjectPool is not assigned in ScrollViewManager.");
            return;
        }
        // visibleItemsCount = Mathf.CeilToInt(scrollRect.GetComponent<RectTransform>().rect.height / (objectPool.prefab.GetComponent<RectTransform>().rect.height));
        PopulateVisibleItems();
        scrollRect.onValueChanged.AddListener(OnScrollChanged);
    }

    void PopulateVisibleItems()
    {
        for (int i = 0; i < visibleItemsCount + 1; i++)
        {
            GameObject obj = objectPool.GetObject();
            obj.transform.SetParent(contentPanel, false);
            activeObjects.Add(obj);
        }
        UpdateVisibleItems();
    }

    void OnScrollChanged(Vector2 scrollPosition)
    {
        UpdateVisibleItems();
    }

    void UpdateVisibleItems()
    {
        float itemHeight = objectPool.prefab.GetComponent<RectTransform>().rect.height;
        float contentHeight = contentPanel.rect.height;
        float scrollPosition = scrollRect.verticalNormalizedPosition;
        float totalVisibleHeight = scrollRect.GetComponent<RectTransform>().rect.height;

        int startIndex = Mathf.FloorToInt((1 - scrollPosition) * (contentHeight - totalVisibleHeight) / itemHeight);
        startIndex = Mathf.Max(startIndex, 0);

        for (int i = 0; i < activeObjects.Count; i++)
        {
            int itemIndex = startIndex + i;
            if (itemIndex < totalItems)
            {
                GameObject obj = activeObjects[i];
                obj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -itemHeight * itemIndex);
                // Set the data for this item here
                // obj.GetComponentInChildren<Text>().text = "Item " + itemIndex;
                // shopManager.GetComponent<UpdateSkinsHandle>().UpdateSkins(obj);
            }
            else
            {
                activeObjects[i].SetActive(false);
            }
        }
    }
}
