using TMPro;
using UnityEngine;

public class SearchPanel : MonoBehaviour
{
    public TMP_InputField txtNameSearch;
    public GameObject searchContent;
    public GameObject searchPrefab;
    private void OnEnable()
    {
        UI_Controller.Instance.searchContent = searchContent;
        UI_Controller.Instance.searchPrefab = searchPrefab;
    }
    public void SearchName()
    {
        ApiHandle.Instance.SearchFriendButton(txtNameSearch);
    }
}