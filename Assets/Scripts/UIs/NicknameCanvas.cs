using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NicknameCanvas : MonoBehaviour
{
    [SerializeField] TMP_InputField inputNickname;

    public void UpdateNickname()
    {
        NetworkManager.UpdateNickname(inputNickname.text);
    }

    public void Close()
    {
        GameManager.Instance.UIManager.Close(UIEnum.SetNicknameCanvas);
    }
}
