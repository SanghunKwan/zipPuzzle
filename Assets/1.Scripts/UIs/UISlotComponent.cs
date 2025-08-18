using TMPro;
using UnityEngine;
using DefineEnums;
using UnityEngine.UI;
using DefineReadonly;

public class UISlotComponent : MonoBehaviour
{
    int _patternCount;
    int _stackCount;
    PatternType _patternType;
    PatternColor _patternColor;


    Animator _anim;
    TextMeshProUGUI _stackCountText;

    public bool _IsMovable { get; set; }
    public int _StackCount => _stackCount;


    public void InitSlot(int patternCount, PatternType patternType, PatternColor patternColor, int stackCount = 1)
    {
        _stackCountText = transform.Find("StackCountText").GetComponent<TextMeshProUGUI>();
        _anim = GetComponent<Animator>();

        _patternCount = patternCount;
        _patternType = patternType;
        _stackCount = stackCount;
        _patternColor = patternColor;

        if (_stackCount == 1)
            _stackCountText.enabled = false;

        SetHighlight();
    }
    public void AddStack(int Count)
    {
        _stackCountText.enabled = true;

        _stackCount += Count;
        _stackCountText.text = "x" + _stackCount.ToString();
    }
    public void SetHighlight()
    {
        _anim.SetTrigger(ReadOnly._animHash_Highlight);
    }
    public void SetFailed()
    {
        _anim.SetTrigger(ReadOnly._animHash_Failed);
    }

    public bool IsMovable(UISlotComponent targetComponent)
    {
        if (targetComponent == null) return false;

        if (_patternColor == targetComponent._patternColor
            || _patternType == targetComponent._patternType
            || _patternCount == targetComponent._patternCount)
            return true;

        return false;
    }
}
