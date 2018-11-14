using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

[Serializable]
public struct GameSettings
{
    public int        SecondsBeforeStart;
    public float      CreationTimeout;
    public float      RoundDurationSeconds;
    public GameObject GameTextes;
    public GameObject ScoreTextes;
}

[Serializable]
public struct BubbleSettings
{
    public float      StartSpeed;
    public float      IncreaseSpeed;
    public float      SpeedIncreaseTimeout;
    public Vector2    Size;
    public Vector2    SpeedBySize;
    public Vector2    ScoreBySize;
    public GameObject SpherePrefab;
}

public class Main : MonoBehaviour
{
    public static UnityAction<float> OnScoreIncrease;
    public static UnityAction<int>   OnScoreUpdate;
    public static UnityAction<int>   OnGameEnd;
    public static UnityAction<int>   OnGameStart;

    public GameSettings GameSettings = new GameSettings {
                                                            CreationTimeout      = 1,
                                                            RoundDurationSeconds = 60,
                                                            SecondsBeforeStart   = 3
                                                        };

    public BubbleSettings BubbleSettings = new BubbleSettings {
                                                                  IncreaseSpeed        = 0.1f,
                                                                  Size                 = new Vector2(1,   5),
                                                                  SpeedBySize          = new Vector2(3,   0),
                                                                  ScoreBySize          = new Vector2(100, 20),
                                                                  SpeedIncreaseTimeout = 1,
                                                                  StartSpeed           = 1
                                                              };

    private int   _beforeStartSeconds;
    private bool  _gameStarted;
    private bool  _timerStarted;
    private float _score;
    private float _timer;
    private float _creationTimer;
    private float _increaseSpeedTimer;
    private float _cameraHalfWidthInUnits;
    private float _increasedSpeed;

    private void Awake()
    {
        _cameraHalfWidthInUnits =  Camera.main.orthographicSize * Camera.main.aspect;
        _beforeStartSeconds     =  GameSettings.SecondsBeforeStart;
        OnScoreIncrease         += OnScoreIncreased;

        GameSettings.GameTextes.SetActive(true);
        GameSettings.ScoreTextes.SetActive(false);
    }

    private void OnScoreIncreased(float score)
    {
        _score += score;
        OnScoreUpdate?.Invoke((int) _score);
    }

    private IEnumerator StartTimer()
    {
        if ( _beforeStartSeconds <= 0 ) {
            _gameStarted  = true;
            _timerStarted = false;
            _timer        = GameSettings.RoundDurationSeconds;
            _score        = 0;

            GameSettings.GameTextes.SetActive(false);
            GameSettings.ScoreTextes.SetActive(true);
        } else {
            OnGameStart?.Invoke(_beforeStartSeconds);

            yield return new WaitForSeconds(1);

            _beforeStartSeconds--;
            StartCoroutine(StartTimer());
        }
    }

    private void Update()
    {
        if ( !_gameStarted ) {
            if ( Input.GetKeyUp(KeyCode.Space) && !_timerStarted ) {
                _timerStarted = true;
                StartCoroutine(StartTimer());
            }

            return;
        }

        _timer              -= Time.deltaTime;
        _creationTimer      -= Time.deltaTime;
        _increaseSpeedTimer -= Time.deltaTime;

        if ( _timer <= 0 ) {
            _gameStarted = false;

            GameSettings.GameTextes.SetActive(true);
            GameSettings.ScoreTextes.SetActive(false);

            OnGameEnd?.Invoke((int) _score);

            return;
        }

        if ( _increaseSpeedTimer <= 0 ) {
            _increaseSpeedTimer =  BubbleSettings.SpeedIncreaseTimeout;
            _increasedSpeed     += BubbleSettings.IncreaseSpeed;
        }

        if ( _creationTimer <= 0 ) {
            CreateBubble();
        }

        if ( Input.GetButtonDown("Fire1") ) {
            var        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if ( Physics.Raycast(ray, out hit) ) {
                var hittable = hit.collider.gameObject.GetComponent<IHittable>();
                hittable?.OnHit();
            }
        }
    }

    private void CreateBubble()
    {
        var size           = Random.Range(BubbleSettings.Size.x, BubbleSettings.Size.y);
        var halfSize       = size * .5f;
        var spherePosition = Vector3.right * Random.Range(-_cameraHalfWidthInUnits + halfSize, _cameraHalfWidthInUnits - halfSize);
        spherePosition.y -= halfSize;
        CreateBubble(size, spherePosition);
    }

    private void CreateBubble(float size, Vector3 position)
    {
        _creationTimer = GameSettings.CreationTimeout;

        var sphere = SimplePool.GetSphere(BubbleSettings.SpherePrefab);

        var sphereTransform = sphere.transform;
        sphereTransform.localScale    = Vector3.one * size;
        sphereTransform.localPosition = position;

        var gameSphere = sphere.GetComponent<GameSphere>();

        if ( !gameSphere ) {
            gameSphere = sphere.AddComponent<GameSphere>();
        }

        var sizeLerp = Mathf.InverseLerp(BubbleSettings.Size.x, BubbleSettings.Size.y, size);

        var speedBySize = Mathf.Lerp(BubbleSettings.SpeedBySize.x, BubbleSettings.SpeedBySize.y, sizeLerp);
        var scoreBySize = Mathf.Lerp(BubbleSettings.ScoreBySize.x, BubbleSettings.ScoreBySize.y, sizeLerp);

        gameSphere.StartAscent(BubbleSettings.StartSpeed + _increasedSpeed + speedBySize, scoreBySize);
    }
}