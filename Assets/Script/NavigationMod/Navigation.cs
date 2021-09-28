using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigation : MonoBehaviour {

    private bool pauseFlag = true;//停止标志
    private List<Vector3> listPos = new List<Vector3>();//路径轨迹
    public int index; //任务点
    public Vector3 nextMovePos;//下一个目标位置
    public Vector3 startPos;//开始时的位置

    void Start()
    {
        index = 0;
        startPos = transform.position;
        listPos.Add(transform.position);
        //listPos[index] = transform.position;
        nextMovePos = transform.position;
        plan1();
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        if (!pauseFlag)
        {
            if (Vector3.Distance(nextMovePos, transform.position) < 0.3f)
            {
                if (listPos.Count != index + 1)
                {
                    nextMovePos = listPos[++index];
                }
            }
        }
    }

    public void setPauseFalg(bool f)
    {
        pauseFlag = f;
    }

    public Vector3 getPoint(int index)
    {
        if (index < listPos.Count && index >= 0)
        {
            return listPos[index];
        }
        return listPos[listPos.Count - 1];
    }

    public void addPos(Vector3 pos)
    {
        
        listPos.Add(pos);
    }

    public void startFly()
    {
        listPos.Add(new Vector3(startPos.x, 1.5f, startPos.z));
    }
    public void finishFly()
    {
        listPos.Add(new Vector3(startPos.x, 1.5f, startPos.z));
    }
    /// <summary>
    /// 插入x,z轴平面的整点，需要坐标值为整数
    /// </summary>
    /// <param name="a">起始点（不包含）</param>
    /// <param name="b">终点（不包含）</param>
    private void insertPos(Vector3 b)
    {

        Vector3 a = listPos[listPos.Count - 1];
        float f = -(a.z - b.z)/ Mathf.Abs(a.z - b.z);
        for (int i = 1; i < Mathf.Abs(a.z - b.z); i++)
        {
            listPos.Add(new Vector3(a.x, a.y, a.z + i * f));
        }
        f = -(a.x - b.x) / Mathf.Abs(a.x - b.x);
        for (int i = 0; i < Mathf.Abs(a.x - b.x); i++)
        {
            listPos.Add(new Vector3(a.x + i * f, a.y, b.z));
        }
        listPos.Add(b);
    }

    private void plan1()
    {
        startFly();
        insertPos(new Vector3(3f, 1.5f, 2f));
        insertPos(new Vector3(8f, 1.5f, 2f));
        insertPos(new Vector3(8f, 1.5f, 8f));
        insertPos(new Vector3(2f, 1.5f, 8f));
        insertPos(new Vector3(2f, 1.5f, startPos.z));
        finishFly();
    }
}
