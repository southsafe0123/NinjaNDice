using DG.Tweening;
using UnityEngine;

public class AnimMove : AnimBase
{
    public Vector2 defaultVector;
    public Vector2 targetVector;
    public Ease openMoveType;
    public Ease closeMoveType;

    [ContextMenu("OpenAnim")]
    public override void OpenAnim()
    {
        transform.DOKill();
        transform.position = defaultVector;
        transform.DOMove(targetVector, dotSpeed).SetEase(openMoveType);
    }

    [ContextMenu("CloseAnim")]
    public override void CloseAnim()
    {
        transform.DOKill();
        transform.position = targetVector;
        transform.DOMove(defaultVector, dotSpeed).SetEase(closeMoveType).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
