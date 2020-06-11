using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountdownScript {

    public bool isDoingAction = false; //is doing action

    public float CDToAction; //count down to action
    public float newCdToAction; //new cd for action

    public float timeIsDoingAction; //time is doing action
    public float newTimeIsDoingAction; //new doing action timer

    public bool isDeciding = false;

    //must be called in start
    public CountdownScript(float _cd, float _newcd, float _timedoing, float _newtimedoing)
    {
        CDToAction = _cd;
        newCdToAction = _newcd;
        timeIsDoingAction = _timedoing;
        newTimeIsDoingAction = _newtimedoing;
    }
    //must be called in update
    public void DoAction( Action action) 
    {
        CDToAction -= Time.deltaTime;
        if (CDToAction <= 0)
        {
            isDoingAction = true;
            timeIsDoingAction -= Time.deltaTime;
            if (isDoingAction)
            {
                action();
            }
            if (timeIsDoingAction <= 0)
            {
                isDoingAction = false;
                CDToAction = newCdToAction;
            }
            if (!isDoingAction && CDToAction >= 0)
            {
                timeIsDoingAction = newTimeIsDoingAction;
            }
        }
    }

    public void DoActionTwo(Action action)
    {
        CDToAction -= Time.deltaTime;
        if (CDToAction <= 0)
        {
            isDoingAction = true;
            timeIsDoingAction -= Time.deltaTime;
            if (isDoingAction)
            {
                action();
            }
            if (timeIsDoingAction <= 0)
            {
                isDoingAction = false;
                CDToAction = newCdToAction;
            }
            if (!isDoingAction && CDToAction >= 0)
            {
                timeIsDoingAction = newTimeIsDoingAction;
            }
        }
    }

    public void ResetTimer()
    {
        CDToAction = newCdToAction;
        isDoingAction = false;
    }

}
