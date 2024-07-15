using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class NicknameCanvas : MonoBehaviour
{
    [SerializeField] TMP_InputField inputNickname;
    [SerializeField] Button closeBtn;

    public void UpdateNickname()
    {
        // #Important
        if (string.IsNullOrEmpty(inputNickname.text))
        {
            GameManager.Instance.UIManager.ClaimError("", "Nickname Can't set null or empty", "OK");
            return;
        }
        NetworkManager.ClaimUpdateNickname(inputNickname.text);
    }

    public void Close()
    {
        GameManager.Instance.UIManager.Close(UIEnum.SetNicknameCanvas);
    }

    private void Update()
    {
        if(GameManager.Instance.NetworkManager.MyNickname == null) closeBtn.interactable = false;
        else closeBtn.interactable = true;
    }
}
