using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointMove : MonoBehaviour {
	public GameObject line;
	public Text X;
	public Text Y;
	private Transform[] wayPoints;
	private float speed = 0.05f;
	private int index = 1;
	private bool flag = true;
	private void FixedUpdate()
    {
		X.text = "X:" + transform.position.x;
		Y.text = "Y:" + transform.position.z;
		if (index == wayPoints.Length) flag=false;
		if (!flag) return;
		if (Vector3.Distance(transform.position, wayPoints[index].position) > 0.01f)
        {
			Vector3 temp = Vector3.MoveTowards(transform.position, wayPoints[index].position, speed);
			transform.position = temp;
		}
        else
        {
			index++;
        }
    }
	private void OnGUI()
	{
		if (GUILayout.Button("重新启动"))
		{
			startflag();
			
		}
	}

	public void startflag()
    {
		if (!flag)
        {
			flag = true;
			index = 1;
		}
		else
			print("正在飞行");
    }
	void Start () {
		wayPoints = line.GetComponentsInChildren<Transform>();
		//foreach (Transform i in line.transform) print(i.position.y);
	}
	
	void Update () {
		
	}
}
