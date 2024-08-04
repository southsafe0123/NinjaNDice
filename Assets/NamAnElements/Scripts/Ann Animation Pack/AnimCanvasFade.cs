using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class AnimCanvasFade : AnimBase
{
    public float targetValue;
    public float defaultValue;
    private CanvasGroup canvasGroup;
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    [ContextMenu("OpenAnim")]
    public override void OpenAnim()
    {
        canvasGroup.DOKill();
        canvasGroup.alpha = defaultValue;
        canvasGroup.DOFade(targetValue, dotSpeed);
    }
    [ContextMenu("CloseAnim")]
    public override void CloseAnim()
    {
        canvasGroup.DOKill();
        canvasGroup.alpha = targetValue;
        canvasGroup.DOFade(defaultValue,dotSpeed).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
