using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteraction
{
    public bool Interaction();
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
