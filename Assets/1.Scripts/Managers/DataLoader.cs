using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataLoader : MonoBehaviour
{
    public static readonly string _dataPath = Path.Combine(Application.streamingAssetsPath, "save.json");

    int _currentIndex;

    [Serializable]
    public class RecordData
    {
        public string _id;
        public int _records;

    }
    [Serializable]
    public class RecordDataList
    {
        public List<RecordData> recordDataList;
    }


    RecordDataList _dataList;

    public void InitLoad()
    {
        FileStream fs = new FileStream(_dataPath, FileMode.OpenOrCreate, FileAccess.Read);

        using (StreamReader sr = new StreamReader(fs))
        {
            string jsonData = sr.ReadToEnd();

            _dataList = JsonUtility.FromJson<RecordDataList>(jsonData);
        }
        fs.Dispose();

        if (_dataList == null)
        {
            Debug.Log("세이브 데이터 없음");
        }
    }
    public bool GetData(in string idStr, out int record)
    {
        record = -1;
        if (_dataList == null)
        {
            Debug.LogError("데이터가 없습니다.");


            _dataList.recordDataList = new List<RecordData>();
            _dataList.recordDataList.Add(new RecordData { _id = idStr, _records = 0 });


            return false;
        }

        for (int i = 0; i < _dataList.recordDataList.Count; i++)
        {
            if (_dataList.recordDataList[i]._id == idStr)
            {
                _currentIndex = i;
                record = _dataList.recordDataList[i]._records;
                Debug.Log($"ID: {idStr}, Record: {record}");
                return true;
            }
        }
        _dataList.recordDataList.Add(new RecordData { _id = idStr, _records = 0 });
        Debug.LogError("데이터가 없습니다.");
        return false;
    }
    public void HandOverData()
    {
        GameObject tempObj = new GameObject("idDataHolder");
        tempObj.AddComponent<DonDestroyData>().InitData(_dataList, _currentIndex);
        tempObj.tag = "DontDestroyData";
        DontDestroyOnLoad(tempObj);
    }
}
