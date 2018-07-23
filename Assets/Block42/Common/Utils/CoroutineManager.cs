using System.Collections;
using UnityEngine;

namespace Block42
{

	// A class for running coroutine, so static class can run corouine without a instance in game.
    public class CoroutineManager : Singleton<CoroutineManager>
    {

        public static void Start(IEnumerator enumerator)
        {
            Instance?.StartCoroutine(enumerator);
        }

    }

}