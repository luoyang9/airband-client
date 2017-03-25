using UnityEngine;
using System.Collections;
using Leap;

public class GetLeapFingers : MonoBehaviour 
{
	HandModel hand_model;
	Hand leap_hand;

	void Start() 
	{
		hand_model = gameObject.GetComponent<HandController>().rightGraphicsModel;
		leap_hand = hand_model.GetLeapHand();
		if (leap_hand == null) Debug.LogError("No leap_hand founded");
	}

	void Update() 
	{
		for (int i = 0; i < HandModel.NUM_FINGERS;i++)
		{
			FingerModel finger = hand_model.fingers[i];
			// draw ray from finger tips (enable Gizmos in Game window to see)
			Debug.DrawRay(finger.GetTipPosition(), finger.GetRay().direction, Color.red); 
		}
	}
}