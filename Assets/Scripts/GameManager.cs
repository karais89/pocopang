// 싱글톤 클래스
// 전체 게임 플로우 관리.
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

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
    private int _selectCoinCount;
    private int _lastCoin = -1;

    // 배열에 각 라인의 사라진 코인수 저장.
    private int[] _lineDeadCoin = new int[kBoardX];

    public int LastCoin
    {
        get { return _lastCoin; }
        set { _lastCoin = value; }
    }

    void Awake()
    {
        _touchSystem = GetComponent<TouchSystem>();
        _touchSystem.GameManager = this;

        InitGameCoin();

        DOTween.Init(false, true, LogBehaviour.ErrorsOnly);
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
        _selectCoinCount = 0;
        
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

    public int CheckAdjacentCoin()
    {
        if (_lastCoin < 0)
            return -1;

        RaycastHit2D hit = Physics2D.Raycast(
            Camera.main.ScreenToWorldPoint(Input.mousePosition),
            Vector2.zero);

        if(hit.collider)
        { 
            for (int index = 0; index < _adjCoin.GetLength(1); index++)
            {
                if (_adjCoin[_lastCoin ,index] == -1)
                    continue;

                GameCoin tmpCoin = _gameCoins[_adjCoin[_lastCoin, index]];

                if (hit.collider.gameObject == tmpCoin.gameObject)
                    return _adjCoin[_lastCoin, index];
            }
        }

        return -1;
    }

    public bool InLastCoin()
    {
        if (_lastCoin == -1)
            return false;

        GameCoin gameCoin = _gameCoins[_lastCoin];
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

        _selectCoinCount += 1;

        return 0;
    }

    public bool CompareCoinType(int index1, int index2)
    {
        if (index1 < 0 || index2 < 0)
            return false;
        
        return _gameCoins[index1].Type == _gameCoins[index2].Type;
    }

    public void ClearSelectCoin()
    {
        if(_selectCoinCount >= 3)
        {
            for(int index = 0; index < _selectCoins.Count; index++)
            {
                GameCoin tmpCoin = _selectCoins[index];
                tmpCoin.State = GameCoin.GameState.Dead;
                tmpCoin.IsVisible = false;
            }
        }
        else
        {
            for (int index = 0; index < _selectCoins.Count; index++)
            {
                GameCoin tmpCoin = _selectCoins[index];
                tmpCoin.State = GameCoin.GameState.Live;
            }
        }

        ResetSelectMask();
        ResetGameInfo();
    }

    public void ResetSelectMask()
    {
        for (int i = 0; i < _selectMasks.Count; i++)
        {
            GameObject selectMask = _selectMasks[i];
            selectMask.SetActive(false);
        }
    }

    public void ResetGameInfo()
    {
        _selectCoinCount = 0;
        
        _selectCoins.Clear();
    }
    /*
        (선택된 코인 = 없어질 코인)
        1. 맨밑부터 시작
        2. 현재 coin이 선택된 것이면 아무것도 안함
        3. 선택되지 않았다면
         - 자신의 아래에 선택된 coin 갯수를 파악
         - 갯수만큼 아래의 coin과 위치 변경
         (예) 내 밑에 두개면 두개 밑이랑 나랑 교체
    */
    public void MoveUpDeadCoin()
    {
        for(int x = 0; x < kBoardX; x++)
        {
            // 현재 라인의 제일 밑의 Position - 첫 번째 줄의 bottomPos = ((0 + 1) * 6) - 1 = 5 -두 번째 줄의 bottomPos = ((1 + 1) * 6) - 1 = 11
            int bottomPos = ((x + 1) * kBoardY) - 1;

            // 내 밑의 dead 상태의 코인 수
            int deadCoinNum = 0;

            // 현재 코인이 DEAD 상태이면 deadCountNum을 '1' 증가시키고 끝! 다음꺼.
            // 내 밑에 DEAD 상태의 코인이 1개 이상이면, 나와 그 위치의 코인을 changeCoin()으로 변경
            for (int y = bottomPos; y > bottomPos - kBoardY; y--)
            {
                GameCoin coin = _gameCoins[y];
                if (coin.State == GameCoin.GameState.Dead)
                {
                    deadCoinNum++;
                    continue;
                }

                if (deadCoinNum > 0)
                    ChangeCoin(y, y + deadCoinNum);
            }
        }     
    }

    public void ChangeCoin(int index1, int index2)
    {
        GameCoin tmpCoin1 = _gameCoins[index1];
        GameCoin tmpCoin2 = _gameCoins[index2];

        // pos swap
        Vector2 tmpPos = tmpCoin2.transform.position;
        tmpCoin2.transform.position = tmpCoin1.transform.position;
        //tmpCoin1.transform.position = tmpPos;

        MoveCoin(tmpCoin1, tmpPos);

        // array swap
        GameCoin tmpCoin = _gameCoins[index1];
        _gameCoins[index1] = _gameCoins[index2];
        _gameCoins[index2] = tmpCoin;
    }

    public void MoveCoin(GameCoin coin, Vector2 pos)
    {
        // (시간 = 거리 / 속력) 이기 때문에 우리가 사용하는 duration 또한 
        // Vector2.Distance(prevPos, pos) / coinSpeed 로 결정합니다.
        // 거리는 변경하지 못하므로 coinSpeed 값을 변경해서 떨어지는 속력이 어떻게 달라지는지 확인할 수 있습니다.

        float duration = 0.0f;
        float coinSpeed = 10.0f;

        Vector2 prevPos = coin.transform.position;
        coin.State = GameCoin.GameState.Move;
        duration = Vector2.Distance(prevPos, pos) / coinSpeed;

        coin.transform.DOMove(
            pos, duration).OnComplete(coin.CoinMoveDone);
    }

    public void AddNewCoin()
    {
        for (int i = 0; i < kBoardX; i++)
            _lineDeadCoin[i] = 0;

        for(int i = 0; i < _gameCoins.Count; i++)
        {
            GameCoin coin = _gameCoins[i];
            if(coin.State == GameCoin.GameState.Dead)
            {
                int line = (int)(i / kBoardY);
                _lineDeadCoin[line] += 1;

                SetNewCoin(coin);
            }
        }

        AddNewCoinAction();
    }

    public void SetNewCoin(GameCoin coin)
    {
        int coinType = Random.Range(0, kTotalCoinType);
        coin.Type = coinType;
        coin.IsVisible = true;
        coin.State = GameCoin.GameState.Live;
    }

    public void AddNewCoinAction()
    {
        float diffY = 1.20f;

        for(int i = 0; i < kBoardX; i++)
        {
            if(_lineDeadCoin[i] > 0)
            {
                int startIndex = i * kBoardY;

                for(int j = startIndex; j < startIndex + _lineDeadCoin[i]; j++)
                {
                    GameCoin coin = _gameCoins[j];
                    Vector2 pos = coin.transform.position;
                    coin.transform.position = new Vector2(pos.x, pos.y + (_lineDeadCoin[i] * diffY));

                    MoveCoin(coin, pos);
                }
            }
        }
    }
}
