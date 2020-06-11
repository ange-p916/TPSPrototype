using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public Transform gameCamera;

	public float mouseSensitivity = 10;
	public float dstFromTarget = 2;
	public Vector2 pitchMinMax = new Vector2(-40, 85);
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 inputdir = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

		transform.Translate(inputdir);

		gameCamera.Rotate(new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));

	}
}
