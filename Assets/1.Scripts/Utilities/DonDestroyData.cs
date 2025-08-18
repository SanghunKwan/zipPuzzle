using UnityEngine;


public class DonDestroyData : MonoBehaviour
{

    public DataLoader.RecordDataList _recordData { get; private set; }
    public int _currentPlayerIndex { get; private set; }

    public void InitData(DataLoader.RecordDataList record, int index)
    {
        _recordData = record;
        _currentPlayerIndex = index;
    }

}
