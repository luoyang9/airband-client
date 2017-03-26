using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public class PhysicalInstrument: MonoBehaviour
	{
		//public Main main;

		protected Main getMain(){
			return GameObject.Find ("Main").GetComponent<Main> ();
		}
	}
}

