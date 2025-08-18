using DefineReadonly;
using TMPro;
using UnityEngine;
using DefineCoroutine;
using System;

public class UIMessageBox : MonoBehaviour
{
    readonly int _animHash_Popup = Animator.StringToHash("PopUp");
    readonly int _animHash_Slide = Animator.StringToHash("Slide");

    TextMeshProUGUI _messageText;
    Animator _anim;

    public event Action AnimationEndCallback;

    public enum ShowType
    {
        PopUp,
        Slide
    }

    public void InitBox()
    {
        _messageText = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        _anim = GetComponent<Animator>();
    }

    public void SetText(in string message)
    {
        _messageText.text = message;
    }

    public void ShowBox(ShowType fadeInType, ShowType fadeOutType, string changedText = null)
    {
        _anim.SetBool(ReadOnly._animHash_isShow, true);

        int inHash = GetHash(fadeInType);
        int outHash = GetHash(fadeOutType);

        _anim.SetTrigger(inHash);

        StartCoroutine(CoroutineUtility.DelayAction(2.5f, () =>
        {
            _anim.SetTrigger(outHash);
            _anim.SetBool(ReadOnly._animHash_isShow, false);


        }));

        if (!string.IsNullOrEmpty(changedText))
        {
            StartCoroutine(CoroutineUtility.DelayAction(1.5f, () =>
            {
                SetText(changedText);
            }));
        }

    }
    int GetHash(ShowType type)
    {
        return type switch
        {
            ShowType.PopUp => _animHash_Popup,
            ShowType.Slide => _animHash_Slide,
            _ => _animHash_Popup
        };
    }

    public void OnAnimationEnd()
    {
        AnimationEndCallback?.Invoke();
        AnimationEndCallback = null;
    }
}
