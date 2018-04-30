using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScenesCoordinator : MonoBehaviour
{
    public static ScenesCoordinator Shared;

    private Dictionary<string, object> scenesParameters;

    // Use this for initialization
    void Awake()
    {
        if (Shared == null)
        {
            Shared = this;
            Shared.scenesParameters = new Dictionary<string, object>();
        }
        DontDestroyOnLoad(gameObject);
    }

    public object FecthParameters(string sceneName)
    {
        object parameters;
        if (scenesParameters.ContainsKey(sceneName))
        {
            parameters = scenesParameters[sceneName];
            scenesParameters.Remove(sceneName);
            return parameters;
        }
        return null;
    }

    public void PushParameters(string sceneName, object parameters)
    {
        scenesParameters.Add(sceneName, parameters);
    }


}
