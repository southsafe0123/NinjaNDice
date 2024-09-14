using DG.Tweening;
using UnityEngine;

public abstract class AnimBase : MonoBehaviour
{
    [Header("The smaller dotSpeed, the speeder")]
    public float dotSpeed;
    public bool useOnEnable = true;
    private void OnEnable()
    {
        if (!useOnEnable) return;
        OpenAnim();
    }
    /// <summary>
    /// Note: OpenAnim() already run in onEnable of the gameobject, but you can change it using bool useOnEnable
    /// </summary>
    public abstract void OpenAnim();
    public abstract void CloseAnim();
}
