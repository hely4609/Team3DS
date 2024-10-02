using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SignInCanvas : MonoBehaviour
{
    [SerializeField] TMP_InputField SignInId;
    [SerializeField] TMP_InputField SignInPassword;
    [SerializeField] TMP_InputField SignUpId;
    [SerializeField] TMP_InputField SignUpPassword;
    [SerializeField] TMP_InputField SignUpConfirmPassword;
    [SerializeField] GameObject SignUpWindow;

    public void SignIn()
    {
        NetworkManager.ClaimSignIn(SignInId.text, SignInPassword.text);
    }

    public void SignUp()
    {
        if(string.Compare(SignUpPassword.text, SignUpConfirmPassword.text) != 0)
        {
            GameManager.Instance.UIManager.ClaimError("Error", "비밀번호가 틀렸습니다.", "확인");
        }
        else
        {
            NetworkManager.ClaimSignUp(SignUpId.text, SignUpPassword.text);

        }
    }

    public void ClearSignUp()
    {
        SignUpId.text = "";
        SignUpPassword.text = "";
        SignUpConfirmPassword.text = "";

    }

    public void CloseSignUp()
    {
        ClearSignUp();
        SignUpWindow.SetActive(false);
    }

}
