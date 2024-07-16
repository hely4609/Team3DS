using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NicknameCanvas : MonoBehaviour
{
    [SerializeField] TMP_InputField inputNickname;
    [SerializeField] Button closeBtn;

    public void UpdateNickname()
    {
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
