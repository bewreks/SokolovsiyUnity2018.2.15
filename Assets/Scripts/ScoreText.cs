using UnityEngine;
using UnityEngine.UI;

public class ScoreText : MonoBehaviour {
	private Text _text;

	private void Awake()
	{
		_text = GetComponent<Text>();
		
		Main.OnScoreUpdate += OnScoreUpdate;
	}

	private void OnScoreUpdate(int score)
	{
		_text.text = $"Score : {score}";
	}
}
