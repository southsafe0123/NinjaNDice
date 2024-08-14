using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class AnimCanvasMove : AnimBase
{
    public Vector2 defaultVector;
    public Vector2 targetVector;
    public Ease openMoveType;
    public Ease closeMoveType;

    private RectTransform rectTransform;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    [ContextMenu("OpenAnim")]
    public override void OpenAnim()
    {
        rectTransform.DOKill();
        rectTransform.anchoredPosition = defaultVector;
        rectTransform.DOAnchorPos(targetVector, dotSpeed).SetEase(openMoveType);
    }
    [ContextMenu("CloseAnim")]
    public override void CloseAnim()
    {
        rectTransform.DOKill();
        rectTransform.anchoredPosition = targetVector;
        rectTransform.DOAnchorPos(defaultVector, dotSpeed).SetEase(closeMoveType).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
