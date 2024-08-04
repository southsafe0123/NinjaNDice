using DG.Tweening;
using UnityEngine;

public class AnimPopup : AnimBase
{
    public Vector2 targetVector;
    public Vector2 defaultVector;
    public Ease openMoveType;
    public Ease closeMoveType;

    [ContextMenu("OpenAnim")]
    public override void OpenAnim()
    {
        transform.DOKill();
        transform.localScale = defaultVector;
        transform.DOScale(targetVector, dotSpeed).SetEase(openMoveType);
    }

    [ContextMenu("CloseAnim")]
    public override void CloseAnim()
    {
        transform.DOKill();
        transform.localScale = targetVector;
        transform.DOScale(defaultVector, dotSpeed).SetEase(closeMoveType).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
