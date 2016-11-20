using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SMBSceneManager : SMBSingleton<SMBSceneManager> {

	public delegate void ActionBetweenScenes();

	private string _lastSceneName;
	public string LastSceneName { get { return _lastSceneName; } }

	private int _lastScene;
	public int LastScene { get { return _lastScene; } }

	private string _currentSceneName;
	public string CurrentSceneName { get { return _currentSceneName; } }

	private int _currentScene;
	public int CurrentScene { get { return _currentScene; } }
			
	public void LoadScene (string sceneName, bool showLoadingScreen = true, ActionBetweenScenes actioneBetweenScenes = null) {

		SMBSceneManager.Instance.StartCoroutine(SceneSwitchCoroutine(sceneName, showLoadingScreen, actioneBetweenScenes));
	}

	IEnumerator SceneSwitchCoroutine (string sceneName, bool showLoadingScreen, ActionBetweenScenes actioneBetweenScenes) {

		_lastScene = SceneManager.GetActiveScene ().buildIndex;
		_lastSceneName = SceneManager.GetActiveScene ().name;

		if(showLoadingScreen) 
			SceneManager.LoadScene("LoadingScene");

		yield return new WaitForSeconds(0.5f);

		if (actioneBetweenScenes != null)
			actioneBetweenScenes();

		SceneManager.LoadScene(sceneName);

		_currentScene = SceneManager.GetActiveScene ().buildIndex;
		_currentSceneName = SceneManager.GetActiveScene ().name;
	}
}

