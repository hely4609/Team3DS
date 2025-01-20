using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class AchivementManager : MonoBehaviour
{
    public void SetAchivement(string achivementApiName)
    {
        if (SteamManager.Initialized)
        {
            Steamworks.SteamUserStats.GetAchievement(achivementApiName, out bool isAchieved);

            if (!isAchieved)
            {
                SteamUserStats.SetAchievement(achivementApiName);
                SteamUserStats.StoreStats();
            }
        }
    }

}
