
using UnityEngine;

[CreateAssetMenu(fileName = "SkinName", menuName = "NhatElements/SkinScriptable", order = 1)]
public class SkinScriptable : ScriptableObject
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
