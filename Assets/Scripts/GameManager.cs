// 싱글톤 클래스
// 전체 게임 플로우 관리.
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static readonly int kBoardX = 7;
    public static readonly int kBoardY = 6;
    public static readonly int kTotalCoinType = 4;

    public GameObject GameCoinPrefab;
    private List<GameCoin> _gameCoins;
    
    void Awake()
    {
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
}
