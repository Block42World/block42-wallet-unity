using System.Collections;
using UnityEngine;

namespace Block42
{

	// A class for running coroutine, so static class can run corouine without a instance in game.
    public class CoroutineManager : Singleton<CoroutineManager>
    {

		public static Coroutine Start(IEnumerator enumerator)
        {
            return Instance.StartCoroutine(enumerator);
        }

		public static void Stop(IEnumerator enumerator)
		{
			Instance.StopCoroutine(enumerator);
		}

    }

}