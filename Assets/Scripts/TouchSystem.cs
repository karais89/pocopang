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

    private int _lastCoin = -1;

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
        _lastCoin = _gameManager.CheckPushGameCoin();
        if (_lastCoin >= 0)
            GameManager.AddSelectCoins(_lastCoin);
    }

    private void TouchMoved()
    {
        //Debug.Log("TouchMoved");
        int newCoin = -1;

        if(!GameManager.InLastCoin(_lastCoin))
        {
            newCoin = GameManager.CheckAdjacentCoin(_lastCoin);

            if(GameManager.CompareCoinType(_lastCoin, newCoin))
            {
                _lastCoin = newCoin;
                _gameManager.AddSelectCoins(_lastCoin);
            }
        }
    }

    private void TouchEnded()
    {
        //Debug.Log("TouchEnded");
    }
 }
