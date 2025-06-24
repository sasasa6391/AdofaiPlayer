using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static Dictionary<string, Sprite> resourceCache = new Dictionary<string, Sprite>();

    public static Sprite LoadResourceSprite(string path)
    {
        if (resourceCache.ContainsKey(path))
        {
            return resourceCache[path];
        }
        else
        {
            Sprite resource = Resources.Load<Sprite>(path);
            if (resource != null)
            {
                resourceCache[path] = resource;
            }
            return resource;
        }
    }
}
