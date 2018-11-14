using UnityEngine;
using UnityEngine.UI;

public class MainTextes : MonoBehaviour {
	private Text _text;

	private void Awake()
	{
		_text = GetComponent<Text>();

		Main.OnGameStart += OnGameStart;
		Main.OnGameEnd += OnGameEnd;
	}

	private void OnGameEnd(int finalScore)
	{
		_text.text = $"Bubble game ends with {finalScore} score\r\nPress \"Space\" to restart";
	}

	private void OnGameStart(int secondsBeforeStart)
	{
		_text.text = $"Bubble game\r\nGame will start in {secondsBeforeStart} seconds";
	}
}
