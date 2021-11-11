using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//无人机导航器
public class Navigation : MonoBehaviour {

    private bool pauseFlag = true;//停止标志
    private bool finishFlag = true;//完成任务点标志
    private bool finishAllFlag = true;//完成任务标志
    public int index; //任务点
    private List<Vector3> listPos = new List<Vector3>();//路径轨迹
    public Vector3 nextMovePos;//下一个目标位置
    public Vector3 startPos;//开始时的位置

    void Start()
    {
        index = 0;
        startPos = transform.position;
        listPos.Add(transform.position);
        nextMovePos = transform.position;
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        if (!pauseFlag)
        {
            if (Vector3.Distance(nextMovePos, transform.position) < 0.1f)
            {
                finishFlag = true;
            }
        }
    }
    //停止标志修改
    public void setPauseFalg(bool f)
    {
        pauseFlag = f;
    }

    public bool getPauseFalg()
    {
        return pauseFlag;
    }

    public bool getFinishFalg()
    {
        return finishFlag;
    }

    public void nextPos()
    {
        if (!finishFlag) return;
        //最后一个任务点了
        if (listPos.Count != index + 1)
        {
            nextMovePos = listPos[++index];
            finishFlag = false;
        }
        else
        {
            finishAllFlag = true;
        }
    }

    public Vector3 getPoint(int index)
    {
        if (index < listPos.Count && index >= 0)
        {
            return listPos[index];
        }
        return listPos[listPos.Count - 1];
    }

    //重置导航
    private void resetState()
    {
        pauseFlag = true;//停止标志
        listPos.Clear();
        index = 0;
        listPos.Add(transform.position);
        nextMovePos = transform.position;
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
        if (a == b) return;
        float f = -(a.z - b.z)/ Mathf.Abs(a.z - b.z);
        for (int i = 1; i <= Mathf.Abs(a.z - b.z); i++)
        {
            listPos.Add(new Vector3(a.x, a.y, a.z + i * f));
        }
        f = -(a.x - b.x) / Mathf.Abs(a.x - b.x);
        for (int i = 1; i <= Mathf.Abs(a.x - b.x); i++)
        {
            listPos.Add(new Vector3(a.x + i * f, a.y, b.z));
        }
    }

    //逐个起飞，绕圈飞行
    public void plan1()
    {
        if (finishAllFlag == false)
        {
            Debug.Log("正在执行任务,不可载入");
            return;
        }
        resetState();//重置
        Debug.Log("载入任务成功");
        startFly();
        insertPos(new Vector3(3f, 1.5f, 2f));
        insertPos(new Vector3(8f, 1.5f, 2f));
        insertPos(new Vector3(8f, 1.5f, 8f));
        insertPos(new Vector3(2f, 1.5f, 8f));
        insertPos(new Vector3(2f, 1.5f, startPos.z));
        finishFly();
        finishAllFlag = false;
    }
    Vector3[] ans = new Vector3[8] {
        new Vector3(6f, 1.5f, 4f),
        new Vector3(7f, 1.5f, 5f),
        new Vector3(7f, 1.5f, 6f),
        new Vector3(6f, 1.5f, 7f),
        new Vector3(5f, 1.5f, 4f),
        new Vector3(4f, 1.5f, 5f),
        new Vector3(4f, 1.5f, 6f),
        new Vector3(5f, 1.5f, 7f),
    };
    public void plan2(int a)
    {
        if (finishAllFlag == false)
        {
            Debug.Log("正在执行任务,不可载入");
            return;
        }
        resetState();//重置
        Debug.Log("载入任务成功");
        startFly();
        addPos(new Vector3(startPos.x + 3f, 1.5f, startPos.z + 2f));
        Vector3 last = listPos[listPos.Count - 1];
        addPos(ans[a]);
        roate();
        addPos(last);
        finishFly();
        finishAllFlag = false;
    }

    Vector3[] roates = new Vector3[8] {
        new Vector3(6f, 1.5f, 4f),
        new Vector3(5f, 1.5f, 4f),
        new Vector3(4f, 1.5f, 5f),
        new Vector3(4f, 1.5f, 6f),
        new Vector3(5f, 1.5f, 7f),
        new Vector3(6f, 1.5f, 7f),
        new Vector3(7f, 1.5f, 6f),
        new Vector3(7f, 1.5f, 5f),
    };

    private void roate()
    {
        Vector3 last = listPos[listPos.Count - 1];
        int t = 0;
        for (int i = 0; i < roates.Length; i++)
        {
            if (roates[i] == last) {
                t = i;
                break;
            }
        }
        for (int i = t; i < roates.Length; i++)
        {
            addPos(roates[i]);
        }
        for (int i = 0; i <= t; i++)
        {
            addPos(roates[i]);
        }
    }
}
