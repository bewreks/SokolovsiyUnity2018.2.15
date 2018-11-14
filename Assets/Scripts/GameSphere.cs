using UnityEngine;

public class GameSphere : MonoBehaviour, IHittable
{
    private bool     _ascenting = false;
    private float    _speed;
    private float    _score;
    private Renderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    public void StartAscent(float speed, float score)
    {
        _ascenting = true;
        _speed     = speed;
        _score     = score;

        _renderer.material.SetColor("_Color", new Color(Random.value, Random.value, Random.value));
    }

    private void Update()
    {
        if ( !_ascenting ) {
            return;
        }

        transform.localPosition += Vector3.up * _speed * Time.deltaTime;

        if ( !_renderer.isVisible ) {
            Hide();
        }
    }

    private void Hide()
    {
        SimplePool.Realize(gameObject);
    }

    public void OnHit()
    {
        Main.OnScoreIncrease?.Invoke(_score);
        Hide();
    }
}