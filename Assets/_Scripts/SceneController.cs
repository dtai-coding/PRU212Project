
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using Unity.VisualScripting;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;

    public SceneAsset startMap;
    public Button start;
    public Button quit;
    //public Button option;


    private bool setShowOption;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

	public void Start()
	{
		quit.onClick.AddListener(QuitGame);
		start.onClick.AddListener(PlayGame);
	}
	//private void Update()
	//{
	//    option.GameObject.SetActive(setShowOption);
	//}
	public void NextLevel()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadScreen(string name)
    {
        SceneManager.LoadSceneAsync(name);
    }

    private void PlayGame()
    {
        SceneManager.LoadScene(startMap.name);
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
