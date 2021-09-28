using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//点击地板移动
public class Move : MonoBehaviour {

    private Vector3 pos;
    private bool isOver = true;
    private float speed=4;
    void Start () {
        
	}
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo = new RaycastHit();
            if(Physics.Raycast(ray,out hitInfo)){ 
                if(hitInfo.collider.name=="Plane"){
                    pos=hitInfo.point;
                    pos.y = 0.5f;
                    isOver=false;
                }

            } 
        }
        MoveTo(pos);
	}
    private void MoveTo(Vector3 tar)
    {
        if (!isOver)
        {
            Vector3 v1 = tar - transform.position;
            transform.position += v1.normalized * speed * Time.deltaTime;
            if (Vector3.Distance(tar, transform.position) <= 0.1f)
            {
                isOver = true;
                transform.position = tar;
            }
        }
    }
}
