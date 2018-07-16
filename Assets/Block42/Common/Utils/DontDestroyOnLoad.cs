using UnityEngine;

namespace Block42
{

	public class DontDestroyOnLoad : MonoBehaviour
	{

		private void Awake()
		{
			DontDestroyOnLoad(gameObject);
		}

	}

}