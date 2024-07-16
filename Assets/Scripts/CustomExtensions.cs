using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Interaction
{
    // �ǹ�����, �Ѱ����, ����, ������, �ݱ�, ��ǰ, 
    Build, OnOff, Repair, Dump, Pick, Deliver, 
}
public interface IInteraction
{
    // ���;׼� enum �����ؾ���. 
    public bool InteractionStart(Interaction interactionType);

    public bool Interaction(float deltaTime);

    public bool InteractionEnd();
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
