using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Sprite))]
public class AnimSpriteColor : AnimBase
{
    public Color defaultColor;
    public Color targetColor;

    private SpriteRenderer spriteRenderer;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    [ContextMenu("OpenAnim")]
    public override void OpenAnim()
    {
        spriteRenderer.DOKill();
        spriteRenderer.color = defaultColor;
        spriteRenderer.DOColor(targetColor, dotSpeed);
    }

    [ContextMenu("CloseAnim")]
    public override void CloseAnim()
    {
        spriteRenderer.DOKill();
        spriteRenderer.color = targetColor;
        spriteRenderer.DOColor(defaultColor, dotSpeed).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }

}
