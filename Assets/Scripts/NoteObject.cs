using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public class NoteObject: MonoBehaviour
	{
		void OnTriggerEnter(Collider col){
			Debug.Log (col);
			if (col.gameObject.name == "End") {
				Destroy (gameObject);
			}
		}
	}
}

