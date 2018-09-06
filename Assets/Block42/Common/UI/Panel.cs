using System.Collections;
using UnityEngine;

namespace Block42
{

	public class Panel : MonoBehaviour
	{

		public virtual void Init()
		{
		}

		protected virtual void Toggle(bool toggle)
		{
			gameObject.SetActive(toggle);
		}

		public void OnBackClick()
		{
			Toggle(false);
		}

	}

}