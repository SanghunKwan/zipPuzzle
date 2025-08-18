using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInputTile : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] int _tileSize = 120;
    [SerializeField] int _gap = 15;

    [SerializeField] SlotManager _slotManager;

    [SerializeField] RectTransform _rectTransform;

    int _rowIndex;
    int _columnIndex;

    public void OnBeginDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, eventData.pressPosition, eventData.pressEventCamera, out Vector2 localPoint);

        _rowIndex = Mathf.FloorToInt(localPoint.x) / (_tileSize + _gap);
        _columnIndex = Mathf.FloorToInt(localPoint.y) / (_tileSize + _gap);
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);

        Debug.Log(localPoint);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector2 offset = eventData.position - eventData.pressPosition;
        if (Vector2.SqrMagnitude(offset) > 5000)
        {
            Debug.Log("ÀÌµ¿ : " + _rowIndex + " : " + _columnIndex);

            if (Mathf.Abs(offset.x) > Mathf.Abs(offset.y))
            {
                _slotManager.OnInputSlide(_rowIndex, _columnIndex, (offset.x > 0) ? 1 : -1, 0);
            }
            else
            {
                _slotManager.OnInputSlide(_rowIndex, _columnIndex, 0, (offset.y > 0) ? 1 : -1);
            }
        }
    }
}
