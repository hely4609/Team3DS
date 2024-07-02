using BackEnd;
using BackEnd.Tcp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class NetworkManager : Manager
{
    void RegistCallBackFunction()
    {
        Backend.Match.OnJoinMatchMakingServer = (args) =>
        {
            if(args.ErrInfo != ErrorInfo.Success)
            {
                GameManager.Instance.UIManager.ClaimError(args.ErrInfo.Category.ToString(), args.ErrInfo.Reason.ToString(), "OK");
            }
            
        };
    }

}
