using UnityEngine;
using DefineReadonly;
using DefineEnums;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class SlotManager : MonoBehaviour
{
    [Header("data")]
    [SerializeField] GameObject[] _slotPatterns;
    [SerializeField] GameObject _slot;

    [SerializeField] float _spawnDelay = 0.1f;
    [SerializeField] float _slotMoveSpeed = 10f;

    [SerializeField] Color[] _patternColors;
    [SerializeField] Sprite[] _patternImages;
    [SerializeField] Color[] _backgroundColor;


    [Header("SlotParent")]
    [SerializeField] RectTransform _slotParent;

    int _movableCount;

    Animator _anim;
    UISlotComponent[,] _slots;
    Image _clickTile;

    Queue<Action> _orderQueue;

    UISlotComponent _movingSlot;
    UISlotComponent _targetSlot;
    (int row, int column) _movingSlotIndex;
    (int row, int column) _targetSlotIndex;

    Vector3 _beforeMovingSlotPosition;


    private void Update()
    {
        if (_movingSlot == null) return;



        if (_targetSlot != null)
        {
            _movingSlot.transform.position = Vector2.Lerp(_movingSlot.transform.position, _targetSlot.transform.position, _slotMoveSpeed * Time.deltaTime);
            if (Vector2.Distance(_movingSlot.transform.position, _targetSlot.transform.position) < 0.01f)
            {
                //목적지 도착.

                //이동할 수 있는 조건이 안 맞으면
                if (!_movingSlot._IsMovable || !_movingSlot.IsMovable(_targetSlot))
                {
                    _movingSlot.SetFailed();

                    _orderQueue.Clear();
                    _orderQueue.Enqueue(() => _orderQueue.Clear());
                    _targetSlot = null;

                }
                else
                {
                    _slots[_movingSlotIndex.row, _movingSlotIndex.column] = null;
                    _movingSlot.AddStack(_targetSlot._StackCount);
                    InGameManager._Instance.AddScore((int)((Mathf.Pow(2, _movingSlot._StackCount) * 0.5f) - Mathf.Pow(2, _targetSlot._StackCount)));

                    if (_targetSlot._IsMovable)
                        _movableCount--;

                    _slots[_targetSlotIndex.row, _targetSlotIndex.column] = _movingSlot;
                    //기존 슬롯 index를 변경.
                    //기존 슬롯 stack 증가.
                    //타겟 슬롯 삭제.

                    //이동으로 바뀐 Movable 슬롯 체크.
                    CheckWhatUserCanMove(_movingSlotIndex.row, _movingSlotIndex.column);
                    CheckWhatUserCanMove(_targetSlotIndex.row, _targetSlotIndex.column);

                    _movingSlotIndex = (0, 0);
                    _targetSlotIndex = (0, 0);

                    Destroy(_targetSlot.gameObject);
                    _movingSlot = null;
                    _beforeMovingSlotPosition = Vector3.zero;

                    if (_movableCount <= 0)
                    {
                        InGameManager._Instance.OnPuzzleClear();
                    }


                    if (_orderQueue.Count > 0)
                    {
                        _orderQueue.Dequeue()();
                    }
                }
            }
        }
        else
        {
            _movingSlot.transform.position = Vector2.Lerp(_movingSlot.transform.position, _beforeMovingSlotPosition, _slotMoveSpeed * Time.deltaTime);
            if (Vector2.Distance(_movingSlot.transform.position, _beforeMovingSlotPosition) < 0.01f)
            {
                //복귀
                _movingSlot = null;
                _beforeMovingSlotPosition = Vector3.zero;

                if (_orderQueue.Count > 0)
                    _orderQueue.Dequeue()();
            }
        }
    }


    public void InitManager(int row, int collumn)
    {
        _anim = GetComponent<Animator>();
        _anim.SetBool(ReadOnly._animHash_isShow, true);
        _orderQueue = new Queue<Action>();

        _slots = new UISlotComponent[row, collumn];

        _clickTile = transform.Find("BG").GetComponent<Image>();
    }

    public void ScatterSlots()
    {
        StartCoroutine(Scatter());
    }
    IEnumerator Scatter()
    {
        int length = _slots.GetLength(0);
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j <= i; j++)
            {
                MakeSlot(i - j, j);
            }
            yield return new WaitForSeconds(_spawnDelay);
        }
        length = _slots.GetLength(1);
        for (int i = 1; i < length; i++)
        {
            for (int j = 0; j + i < length; j++)
            {
                MakeSlot(i + j, 9 - j);
            }
            yield return new WaitForSeconds(_spawnDelay);
        }

        for (int i = 0; i < _slots.GetLength(0); i++)
        {
            for (int j = 0; j < _slots.GetLength(1); j++)
            {
                CheckWhatUserCanMove(i, j);
            }
        }
        Debug.Log(_movableCount + " slots are movable.");
    }


    void MakeSlot(int row, int column)
    {
        Vector2 spawnPosition = (Vector2)_slotParent.position - _slotParent.sizeDelta / 2;
        UISlotComponent newSlot = Instantiate(_slot, spawnPosition + new Vector2(row * 135, column * 135), Quaternion.identity, _slotParent).GetComponent<UISlotComponent>();
        _slots[row, column] = newSlot;

        int patternIndex = Random.Range(0, _slotPatterns.Length);
        PatternType patternType = (PatternType)Random.Range(0, (int)PatternType.Max);
        PatternColor patternColor = (PatternColor)Random.Range(0, (int)PatternColor.Max);
        GameObject tempObj = Instantiate(_slotPatterns[patternIndex], newSlot.transform);

        foreach (var item in tempObj.GetComponentsInChildren<Image>())
        {
            item.color = _patternColors[(int)patternColor];
            item.sprite = _patternImages[(int)patternType];
        }

        newSlot.InitSlot(patternIndex + 1,
                         patternType,
                         patternColor);

    }

    public void SetPuzzleEnable(bool isOn)
    {
        _clickTile.raycastTarget = isOn;
        enabled = isOn;
    }

    public void OnInputSlide(int rowIndex, int columnIndex, int rowValue, int columnValue)
    {
        if (_movingSlot != null)
        {
            _orderQueue.Enqueue(() => SlideSlot(rowIndex, columnIndex, rowValue, columnValue));
        }
        else
            SlideSlot(rowIndex, columnIndex, rowValue, columnValue);

    }

    void SlideSlot(int rowIndex, int columnIndex, int rowValue, int columnValue)
    {
        _movingSlot = _slots[rowIndex, columnIndex];
        if (_movingSlot == null)
            return;

        _movingSlotIndex = (rowIndex, columnIndex);
        _movingSlot.SetHighlight();
        int newRowIndex = rowIndex + rowValue;
        int newColumnIndex = columnIndex + columnValue;

        if (newRowIndex >= 0 && newRowIndex < _slots.GetLength(0)
            && newColumnIndex >= 0 && newColumnIndex < _slots.GetLength(1))
        {
            _targetSlot = _slots[newRowIndex, newColumnIndex];
            _targetSlotIndex = (newRowIndex, newColumnIndex);
            _beforeMovingSlotPosition = _movingSlot.transform.position;
            _movingSlot.transform.SetSiblingIndex(_movingSlot.transform.parent.childCount - 1);
        }
    }

    void CheckWhatUserCanMove(int rowIndex, int columnIndex)
    {
        if (rowIndex > 0)
            CheckSlot(rowIndex - 1, columnIndex);
        if (rowIndex < (_slots.GetLength(0) - 1))
            CheckSlot(rowIndex + 1, columnIndex);
        if (columnIndex > 0)
            CheckSlot(rowIndex, columnIndex - 1);
        if (columnIndex < (_slots.GetLength(1) - 1))
            CheckSlot(rowIndex, columnIndex + 1);
    }

    void CheckSlot(int rowIndex, int columnIndex)
    {
        UISlotComponent currentSlot = _slots[rowIndex, columnIndex];

        if (currentSlot == null) return;


        bool isMovable = false;

        if (rowIndex > 0 && currentSlot.IsMovable(_slots[rowIndex - 1, columnIndex]))
            isMovable = true;
        else if (rowIndex < (_slots.GetLength(0) - 1) && currentSlot.IsMovable(_slots[rowIndex + 1, columnIndex]))
            isMovable = true;
        else if (columnIndex > 0 && currentSlot.IsMovable(_slots[rowIndex, columnIndex - 1]))
            isMovable = true;
        else if (columnIndex < (_slots.GetLength(1) - 1) && currentSlot.IsMovable(_slots[rowIndex, columnIndex + 1]))
            isMovable = true;

        if (currentSlot._IsMovable != isMovable)
        {
            currentSlot._IsMovable = isMovable;
            _movableCount += isMovable ? 1 : -1;
        }

    }
}
