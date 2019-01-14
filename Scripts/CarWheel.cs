using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarWheel : MonoBehaviour {

	public WheelCollider targetWheel;
	
	private Vector3 wheelPosition=new Vector3();
	private Quaternion wheelRotation=new Quaternion();												//Quaternion refers to the rotation of objects
	
	// Update is called once per frame
	void Update () {
		targetWheel.GetWorldPose(out wheelPosition, out wheelRotation);								//Stores the wheel position and rotation in GetworldPose
		transform.position=wheelPosition;
		transform.rotation=wheelRotation;
	}
}
