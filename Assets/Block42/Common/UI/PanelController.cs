using System.Collections;
using UnityEngine;

namespace Block42
{

	public class PanelController : MonoBehaviour
	{

		private void Awake()
		{
			Panel[] panels = GetComponentsInChildren<Panel>(true);
			foreach (Panel p in panels)
				p.Init();
		}

	}

}