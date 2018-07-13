using System.Collections;
using UnityEngine;

namespace Block42
{

    public class CoroutineManager : Singleton<CoroutineManager>
    {

        public static void Start(IEnumerator enumerator)
        {
            s_Instance?.StartCoroutine(enumerator);
        }
    }

}