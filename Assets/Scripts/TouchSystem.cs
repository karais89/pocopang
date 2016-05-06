using UnityEngine;
using System.Collections;
using System;

public class TouchSystem : MonoBehaviour
{
    private GameManager _gameManager;
    public GameManager GameManager
    {
        get
        {
            return _gameManager;
        }
        set
        {
            _gameManager = value;
        }
    }

	// Update is called once per frame
	void Update ()
    {
        if(Input.GetMouseButtonDown(0))
        {
            TouchBegan();
        }
        else if(Input.GetMouseButtonUp(0))
        {
            TouchEnded();
        }
        else if(Input.GetMouseButton(0))
        {
            TouchMoved();
        }
	}

    private void TouchBegan()
    {
        //Debug.Log("TouchBegan");]
        //CheckPushGameCoin();
        _gameManager.CheckPushGameCoin();
    }

    private void TouchMoved()
    {
        //Debug.Log("TouchMoved");
    }

    private void TouchEnded()
    {
        //Debug.Log("TouchEnded");
    }
 }
