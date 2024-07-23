using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SkinItem : MonoBehaviour
{

    [SerializeField] private SkinInfo skinInfo;
    [SerializeField] private GameObject skinImage;
    // Start is called before the first frame update


    public void SetData(SkinInfo skinInfo, Sprite sprite)
    {
        this.skinInfo = skinInfo;
        skinImage.GetComponent<Image>().sprite = sprite;
        gameObject.name = skinInfo.name;
    }


}
