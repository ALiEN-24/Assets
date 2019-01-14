using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTimer : MonoBehaviour {
	public float timer = 0.0f;
	public bool start_timer = false;

	// Use this for initialization
	void Start () {
		float time = Time.fixedDeltaTime;
	}
	
	// Update is called once per frame
	void Update () {
		if (start_timer != false)
			timer += Time.deltaTime;
	}
}
