using System;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// use to call event (or can use in animcombo for delay time etc...)
/// </summary>
public class AnimEvent : AnimBase
{
    [Header("Only use animEvent, other variable wont work")]
    public UnityEvent animEvent;
    private void Awake()
    {
        useOnEnable = false;
    }
    
    /// <summary>
    /// this function is not available
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public override void CloseAnim()
    {
    }


    public override void OpenAnim()
    {
        InvokeEvent();
    }

    public void InvokeEvent()
    {
        animEvent?.Invoke();
    }
}
