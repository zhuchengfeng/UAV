using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationView : MonoBehaviour {

	public Transform CenterPoint;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.A))
        {
			transform.RotateAround(CenterPoint.position, Vector3.up, 1);
			CenterPoint.Rotate(Vector3.up, 1);
		}
		if (Input.GetKey(KeyCode.D))
		{
			transform.RotateAround(CenterPoint.position, Vector3.down, 1);
			CenterPoint.Rotate(Vector3.down, 1);
		}
		if (Input.GetKey(KeyCode.W))
		{
			transform.RotateAround(CenterPoint.position, new Vector3(Mathf.Sin(CenterPoint.rotation.eulerAngles.y * 2 * Mathf.PI / 360), 0, Mathf.Cos(CenterPoint.rotation.eulerAngles.y * 2 * Mathf.PI / 360)), -1);
		}
		if (Input.GetKey(KeyCode.S))
		{
			transform.RotateAround(CenterPoint.position, new Vector3(Mathf.Sin(CenterPoint.rotation.eulerAngles.y * 2 * Mathf.PI / 360), 0, Mathf.Cos(CenterPoint.rotation.eulerAngles.y * 2 * Mathf.PI / 360)), 1);

		}
	}
}
