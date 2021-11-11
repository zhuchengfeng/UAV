using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateLine : MonoBehaviour {

    private LineRenderer lineRenderer;//储存 gameobject 的 LineRenderer 组件
    private Navigation nav;

	void Start () {
        lineRenderer = GetComponent<LineRenderer>();
        nav = GetComponent<Navigation>();
    }
	
	// Update is called once per frame
	void Update () {
        Create();
    }

    void Create()
    {
        Vector3 path = nav.nextMovePos;//储存路径
        lineRenderer.positionCount = 2;//设置线段数
        lineRenderer.SetPosition(0, transform.position);//设置线段顶点坐标
        lineRenderer.SetPosition(1, path);//设置线段顶点坐标
    }
}
