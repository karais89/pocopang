// 싱글톤 클래스
// 전체 게임 플로우 관리.
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public const int kBoardX = 7;
    public const int kBoardY = 6;
    public const int kTotalCoinType = 4;
    public const int kTotalAdjCoin = 6;

    public int[,] _adjCoin = new int[kBoardX * kBoardY, kTotalAdjCoin] {
        { -1, -1,  1,  7,  6, -1},   // 0, line 1
        { -1, -1,  2,  8,  7,  0},
        { -1, -1,  3,  9,  8,  1},
        { -1, -1,  4, 10,  9,  2},
        { -1, -1,  5, 11, 10,  3},
        { -1, -1, -1, -1, 11,  4},
        { -1,  0,  7, 12, -1, -1},   // 6, line 2
        {  0,  1,  8, 13, 12,  6},
        {  1,  2,  9, 14, 13,  7},
        {  2,  3, 10, 15, 14,  8},
        {  3,  4, 11, 16, 15,  9},
        {  4,  5, -1, 17, 16, 10},
        {  6,  7, 13, 19, 18, -1},   // 12, line 3
        {  7,  8, 14, 20, 19, 12},
        {  8,  9, 15, 21, 20, 13},
        {  9, 10, 16, 22, 21, 14},
        { 10, 11, 17, 23, 22, 15},
        { 11, -1, -1, -1, 23, 16},
        { -1, 12, 19, 24, -1, -1},   // 18, line 4
        { 12, 13, 20, 25, 24, 18},
        { 13, 14, 21, 26, 25, 19},
        { 14, 15, 22, 27, 26, 20},
        { 15, 16, 23, 28, 27, 21},
        { 16, 17, -1, 29, 28, 22},
        { 18, 19, 25, 31, 30, -1},   // 24, line 5
        { 19, 20, 26, 32, 31, 24},
        { 20, 21, 27, 33, 32, 25},
        { 21, 22, 28, 34, 33, 26},
        { 22, 23, 29, 35, 34, 27},
        { 23, -1, -1, -1, 35, 28},
        { -1, 24, 31, 36, -1, -1},   // 30, line 6
        { 24, 25, 32, 37, 36, 30},
        { 25, 26, 33, 38, 37, 31},
        { 26, 27, 34, 39, 38, 32},
        { 27, 28, 35, 40, 39, 33},
        { 28, 29, -1, 41, 40, 34},
        { 30, 31, 37, -1, -1, -1},   // 36, line 7
        { 31, 32, 38, -1, -1, 36},
        { 32, 33, 39, -1, -1, 37},
        { 33, 34, 40, -1, -1, 38},
        { 34, 35, 41, -1, -1, 39},
        { 35, -1, -1, -1, -1, 40}
    };

    public GameObject GameCoinPrefab;
    public GameObject SelectMaskPrefab;

    private List<GameCoin> _gameCoins;
    private List<GameObject> _selectMasks;
    private List<GameCoin> _selectCoins;
    private TouchSystem _touchSystem;

    void Awake()
    {
        _touchSystem = GetComponent<TouchSystem>();
        _touchSystem.GameManager = this;

        InitGameCoin();
    }

    private void InitGameCoin()
    {
        float coinXPos = 0.0f;
        float coinYPos = 0.0f;
        float diffX = 1.04f;
        float diffY = 1.20f;
        float initCoinXPos = -3.10f;
        float initCoinYPos = 1.38f;

        _gameCoins = new List<GameCoin>();
        _selectMasks = new List<GameObject>();
        _selectCoins = new List<GameCoin>();

        for(int xIndex = 0; xIndex < kBoardX; xIndex++)
        {
            coinXPos = initCoinXPos + (xIndex * diffX);
            coinYPos = initCoinYPos;
            if (xIndex % 2 == 0)
            { 
                coinYPos -= diffY * 0.5f;
            }

            for (int yIndex = 0; yIndex < kBoardY; yIndex++)
            {

                GameCoin gameCoin = 
                    CreateGameCoin(
                    new Vector2(coinXPos, coinYPos - (yIndex * diffY)), 
                    Random.Range(0, kTotalCoinType), 
                    (int)GameCoin.GameState.Live);
                _gameCoins.Add(gameCoin);
            }
        }

        // selectmask
        for (int xIndex = 0; xIndex < kBoardX; xIndex++)
        {
            coinXPos = initCoinXPos + (xIndex * diffX);
            coinYPos = initCoinYPos;
            if (xIndex % 2 == 0)
                coinYPos -= diffY * 0.5f;
        
            for (int yIndex = 0; yIndex < kBoardY; yIndex++)
            {
                GameObject gMask = Instantiate(SelectMaskPrefab,
                    new Vector3(coinXPos, coinYPos - (yIndex * diffY), 0),
                    Quaternion.identity) as GameObject;

                gMask.SetActive(false);
                gMask.transform.parent = transform;
                //gMask.transform.localScale = Vector2.one * 0.4f;
                _selectMasks.Add(gMask);
            }
        }


    }

    private GameCoin CreateGameCoin(Vector2 pos, int type, int state)
    {
        GameObject g = Instantiate(GameCoinPrefab, pos, Quaternion.identity) as GameObject;
        g.transform.parent = transform;

        GameCoin gameCoin = g.GetComponent<GameCoin>();
        gameCoin.Type = type;
        gameCoin.State = (GameCoin.GameState)state;
        
        return gameCoin;
    }

    public int CheckPushGameCoin()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider)
        {
            //Debug.Log("name : "+ hit.collider.name);
            GameCoin gameCoin = hit.transform.GetComponent<GameCoin>();
            //if (gameCoin.IsVisible)
            //    gameCoin.IsVisible = false;
            //else
            //    gameCoin.IsVisible = true;

            // 굳이 gamecoin 인덱스를 넘겨줘야 되나?
            for(int i = 0; i < _gameCoins.Count; i++)
            {
                if(_gameCoins[i] == gameCoin)
                {
                    Debug.Log("index : " + i);
                    return i;
                }
            }
        }

        return -1;
    }

    public int CheckAdjacentCoin(int lastCoin)
    {
        if (lastCoin < 0)
            return -1;

        RaycastHit2D hit = Physics2D.Raycast(
            Camera.main.ScreenToWorldPoint(Input.mousePosition),
            Vector2.zero);

        if(hit.collider)
        { 
            for (int index = 0; index < _adjCoin.GetLength(1); index++)
            {
                if (_adjCoin[lastCoin ,index] == -1)
                    continue;

                GameCoin tmpCoin = _gameCoins[_adjCoin[lastCoin, index]];

                if (hit.collider.gameObject == tmpCoin.gameObject)
                    return _adjCoin[lastCoin, index];
            }
        }

        return -1;
    }

    public bool InLastCoin(int lastCoin)
    {
        if (lastCoin == -1)
            return false;

        GameCoin gameCoin = _gameCoins[lastCoin];
        RaycastHit2D hit = Physics2D.Raycast(
            Camera.main.ScreenToWorldPoint(Input.mousePosition), 
            Vector2.zero);

        if (hit.collider)
            return hit.collider.gameObject == gameCoin.gameObject;
        
        return false;
    }

    public int AddSelectCoins(int index)
    {
        if (index < 0)
            return -1;

        GameCoin tmpCoin = _gameCoins[index];
        if(tmpCoin.State != GameCoin.GameState.Select)
        {
            tmpCoin.State = GameCoin.GameState.Select;
            _selectCoins.Add(tmpCoin);

            GameObject selectMask = _selectMasks[index];
            selectMask.SetActive(true);
        }

        return 0;
    }

    public bool CompareCoinType(int index1, int index2)
    {
        if (index1 < 0 || index2 < 0)
            return false;
        
        return _gameCoins[index1].Type == _gameCoins[index2].Type;
    }
}
