using TMPro;
using UnityEngine;

public class AnouncementManager : MonoBehaviour
{
    public static AnouncementManager instance;
    public Animator animator;
    public TextMeshProUGUI txtAnnouncement;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void DisplayAnouncement(string log)
    {
        txtAnnouncement.text = log;
        animator.Play("Anounce_Play");
    }
}