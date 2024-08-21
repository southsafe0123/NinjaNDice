using UnityEngine;

public class FreezeEffect : MonoBehaviour
{
    Player player;
    public Animator anim;
    private void OnEnable()
    {
        gameObject.name = "FreezeEffect";
        foreach (Player player in PlayerList.Instance.playerDic.Values)
        {
            if(player.transform.position == gameObject.transform.position && player.isPlayerFrozen.Value)
            {
                this.player = player;
                break;
            }
        }
    }
    
    public void UnFreezeAnimation()
    {
        if (player == null) return;
        if (!player.isPlayerFrozen.Value)
        {
            anim.Play("Freeze_End");
        }
    }
}