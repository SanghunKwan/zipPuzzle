using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILoginInteractive : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _id;
    [SerializeField] TextMeshProUGUI _name;
    [SerializeField] TextMeshProUGUI _beforeRecord;
    [SerializeField] RawImage _picture;

    GameObject _logInObject;
    GameObject _logOutObject;


    private void Awake()
    {
        InitInteractive();
    }

    public void InitInteractive()
    {
        _logOutObject = transform.GetChild(0).gameObject;
        _logInObject = transform.GetChild(1).gameObject;
    }
    public void SetLogInOut(bool isOn)
    {
        _logOutObject.SetActive(!isOn);
        _logInObject.SetActive(isOn);
    }

    public void SetId(in string id)
    {
        _id.text = id;
    }
    public void SetName(in string name)
    {
        _name.text = name;
    }
    public void SetImage(Texture2D texture)
    {
        _picture.texture = texture;
    }
    public void SetRecord(in string record)
    {
        _beforeRecord.text = record;
    }
}
