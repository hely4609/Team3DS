using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Interaction
{
    // 아무것도 안함, 건물짓기, 켜고끄기, 수리, 버리기, 줍기, 납품, 전선 잡기
    None, Build, OnOff, Repair, Dump, Pick, Deliver, takeRope, AttachRope, Upgrade, Demolish
}

public interface IInteraction
{
    // 인터액션 enum 제작해야함. 
    public Interaction InteractionStart(Player player, Interaction interaction);

    public float InteractionUpdate(float deltaTime, Interaction interaction);

    public bool InteractionEnd();

    public Bounds GetInteractionBounds();
    public abstract string GetName();
    public List<Interaction> GetInteractions(Player player);
    public Collider[] GetInteractionColliders();

}


public static class CustomExtensions
{
    public static string GetFileName(this string path)
    {
        if (path == null || path.Length == 0) return "";
        string newPath = "";
        bool findSpace = false;
        for (int i = 0; i < path.Length; i++)
        {
            if (path[i] != ' ')
            {
                if (findSpace)
                {
                    newPath += char.ToUpper(path[i]);
                    findSpace = false;
                }
                else
                {
                    newPath += path[i];

                }

            }
            else
            {
                findSpace = true;

            }
        }


        return newPath[(path.LastIndexOf("/") + 1)..];

    }

}
