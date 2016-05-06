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

    //private int _lastCoin = -1;

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
        _gameManager.LastCoin = _gameManager.CheckPushGameCoin();
        if (_gameManager.LastCoin >= 0)
            GameManager.AddSelectCoins(_gameManager.LastCoin);
    }

    private void TouchMoved()
    {
        //Debug.Log("TouchMoved");
        int newCoin = -1;

        if(!GameManager.InLastCoin())
        {
            newCoin = GameManager.CheckAdjacentCoin();

            if(GameManager.CompareCoinType(_gameManager.LastCoin, newCoin))
            {
                _gameManager.LastCoin = newCoin;
                _gameManager.AddSelectCoins(_gameManager.LastCoin);
            }
        }
    }

    private void TouchEnded()
    {
        //Debug.Log("TouchEnded");
        _gameManager.ClearSelectCoin();
        _gameManager.MoveUpDeadCoin();
    }
 }
