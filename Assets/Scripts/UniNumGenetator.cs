using UnityEngine;
using System.Collections;

public class UniNumGenetator : MonoBehaviour
{
    static long id;

    static public long NewId()
    {
        return ++id;
    }
}
