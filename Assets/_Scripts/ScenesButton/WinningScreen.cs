using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinningScreen : MonoBehaviour
{
	public SceneAsset sceneToLoad;
	public Button start;
	public Button quit;
	// Start is called before the first frame update
	void Start()
	{
		quit.onClick.AddListener(QuitGame);
		start.onClick.AddListener(PlayGame);
	}

	// Update is called once per frame
	void Update()
	{

	}

	private void PlayGame()
	{
		SceneController.instance.LoadScreen(sceneToLoad.name);
	}

	//private void Option()
	//{
	//    SceneManager.LoadScene(startMap.name);
	//}

	private void QuitGame()
	{
		UnityEditor.EditorApplication.isPlaying = false;
		Application.Quit();
	}
}
