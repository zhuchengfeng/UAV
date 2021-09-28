using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTo : MonoBehaviour
{
    private bool pauseFlag;
    private Queue<Vector3> queuePos = new Queue<Vector3>();
    private Vector3 startPos;
    private Vector3 nextMovePos;
    private float speed=1.0f;

    void Start()
    {
        startPos = transform.position;
        pauseFlag = false;
        nextMovePos = startPos;
        queuePos.Enqueue(new Vector3(1, 1, 1));
        queuePos.Enqueue(new Vector3(1, 1, 5));
        queuePos.Enqueue(new Vector3(5, 1, 1));
        queuePos.Enqueue(new Vector3(1, 1, 1));
    }

    void Update()
    {
        if(!pauseFlag)
            Move();
    }

    private void Move() {
        transform.position = Vector3.MoveTowards(transform.position, nextMovePos,speed*Time.deltaTime);
        if(Vector3.Distance(nextMovePos,transform.position)< 0.00001f)
        {
            if (queuePos.Count == 0) return;
            nextMovePos = queuePos.Dequeue();
        }
    }

    public void changePauseFalg()
    {
        pauseFlag = !pauseFlag;
    }

    public Vector3 getStartPos()
    {
        return startPos;
    }

    public void addPos(Vector3 pos)
    {
        queuePos.Enqueue(pos);
    }

    public float getSpeed()
    {
        return speed;
    }

    public void setSpeed(float s)
    {
        speed = s;
    }

}
