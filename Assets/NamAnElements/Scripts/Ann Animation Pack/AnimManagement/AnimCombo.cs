using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimCombo : MonoBehaviour
{
    public List<AnimComboSetupData> animComboSetupList = new List<AnimComboSetupData>();

    private List<AnimBase> animClassList = new List<AnimBase>();
    private Coroutine currentCoroutine;

    [ContextMenu("Import All Anim From Children")]
    public void SetAnimList()
    {
        animClassList.Clear();
        animClassList = GetComponentsInChildren<AnimBase>(true).ToList();
        if (animClassList.Count == 0)
        {
            Debug.Log("There are no children that have anim class in this component!");
            return;
        }

        CreateComboList();
    }

    private void CreateComboList()
    {
        animComboSetupList.Clear();
        foreach (AnimBase anim in animClassList)
        {
            animComboSetupList.Add(new AnimComboSetupData(anim, 0.5f));
        }
        TurnOffGameobjectInComboList();
    }
    [ContextMenu("Turn off gameobject in combolist")]
    private void TurnOffGameobjectInComboList()
    {
        foreach (AnimComboSetupData animCombo in animComboSetupList)
        {
            animCombo.animClass.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Note: OpenAnim() Only affect the anim class that have the gameobject is unActive.
    /// Note2: OpenAnim() run when gameobject is unActive because the Class Anim have OnEnable() function.
    /// </summary>
    [ContextMenu("TestOpenAnim")]
    public void OpenAnim()
    {
        currentCoroutine = StartCoroutine(OpenAnimCoroutine());
    }

    private IEnumerator OpenAnimCoroutine()
    {
        yield return null;
        foreach (AnimComboSetupData anim in animComboSetupList)
        {
            yield return new WaitForSeconds(anim.delayTime);
            anim.animClass.gameObject.SetActive(true);
            if (anim.animClass.useOnEnable == false) anim.animClass.OpenAnim();
        }
    }

    /// <summary>
    /// kinda buggy, test later
    /// </summary>
    [ContextMenu("TestCloseAnim")]
    public void CloseAnim()
    {
        StartCoroutine(CloseAnimCoroutine());
    }

    private IEnumerator CloseAnimCoroutine()
    {
        yield return null;
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        };

        for (int i = animComboSetupList.Count-1; i >= 0; i--)
        {
            animComboSetupList[i].animClass.CloseAnim();
        }
    }

    [System.Serializable]
    public class AnimComboSetupData
    {
        public float delayTime;
        public AnimBase animClass;

        public AnimComboSetupData(AnimBase animClass, float delayTime)
        {
            this.animClass = animClass;
            this.delayTime = delayTime;
        }
    }
}
