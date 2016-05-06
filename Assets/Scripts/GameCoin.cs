using UnityEngine;
using System.Collections;

public class GameCoin : MonoBehaviour
{
    public enum GameState
    {
        Live,
        Dead,
        Select
    };
    private int _type;
    private GameState _state;

    public Sprite[] CoinSprites;
    private SpriteRenderer _spriteRenderer;
    
    public int Type
    {
        get { return _type; }
        set {
            _type = value;
            _spriteRenderer.sprite = CoinSprites[_type];
        }
    }

    public GameState State
    {
        get { return _state; }
        set { _state = value; }
    }

    public bool IsVisible
    { 
        get
        {
            return _spriteRenderer.enabled;
        }
        set
        {
            _spriteRenderer.enabled = value;
        }
    }

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _type = 0;
        _state = GameState.Live;
    }

    //public void Init()
    //{

    //}
}
