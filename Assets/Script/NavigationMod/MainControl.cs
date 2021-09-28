using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainControl : MonoBehaviour {

    private Transform[] drone;
    private Navigation[] nav;

    private int index;

    // Use this for initialization
    void Start () {
        drone = new Transform[8];
        nav = new Navigation[8];
        index = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            drone[i] = transform.GetChild(i);
            nav[i] = drone[i].GetComponent<Navigation>();
        }
        nav[index].setPauseFalg(false);
    }
	
	// Update is called once per frame
	void Update () {
        oneByone();
    }

    private void oneByone()
    {
        if(nav[0].index == 2)
        {
            nav[1].setPauseFalg(false);
        }
        if (nav[1].index == 2)
        {
            nav[2].setPauseFalg(false);
        }
        if (nav[2].index == 2)
        {
            nav[3].setPauseFalg(false);
        }

        if (nav[3].index == 5)
        {
            nav[4].setPauseFalg(false);
        }
        if (nav[4].index == 2)
        {
            nav[5].setPauseFalg(false);
        }
        if (nav[5].index == 2)
        {
            nav[6].setPauseFalg(false);
        }
        if (nav[6].index == 2)
        {
            nav[7].setPauseFalg(false);
        }
    }

    private void AllFly()
    {
        for (int i = 0; i < nav.Length; i++)
        {
            nav[i].setPauseFalg(false);
        }
    }

    void plan1()
    {
        
    }
}
