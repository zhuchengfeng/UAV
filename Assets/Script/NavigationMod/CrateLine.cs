using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateLine : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //_lineRenderer = line.GetComponent<LineRenderer>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public Transform line;//就是我们刚才添加的 gameobject
    private LineRenderer _lineRenderer;//储存 gameobject 的 LineRenderer 组件

    void Create()
    {
        //Vector3[] _path = Nav.path.corners;//储存路径
        //_lineRenderer.SetVertexCount(_path.Length);//设置线段数
        //for (int i = 0; i < _path.Length; i++)
        //{
        //    _lineRenderer.SetPosition(i, _path[i]);//设置线段顶点坐标
        //}
    }
}
