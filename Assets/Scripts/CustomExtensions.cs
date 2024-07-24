using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Interaction
{
    // �ƹ��͵� ����, �ǹ�����, �Ѱ����, ����, ������, �ݱ�, ��ǰ, ���� ���
    None, Build, OnOff, Repair, Dump, Pick, Deliver, takeRope
}

public interface IInteraction
{
    // ���;׼� enum �����ؾ���. 
    public Interaction InteractionStart( Player player);

    public bool InteractionUpdate(float deltaTime, Interaction interaction);

    public bool InteractionEnd();

    public Vector3 GetPosition();
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
