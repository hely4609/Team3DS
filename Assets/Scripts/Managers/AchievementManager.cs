using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class AchievementManager : MonoBehaviour
{
    public void SetAchievement(string achievementApiName)
    {
        if (SteamManager.Initialized)
        {
            Steamworks.SteamUserStats.GetAchievement(achievementApiName, out bool isAchieved);

            if (!isAchieved)
            {
                SteamUserStats.SetAchievement(achievementApiName);
                SteamUserStats.StoreStats();
            }
        }
    }

}
