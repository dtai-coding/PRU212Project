using UnityEngine;

public class LogHandlerSetup : MonoBehaviour
{
	void Awake()
	{
		// Set the custom log handler
		Debug.unityLogger.logHandler = new CustomLogHandler(Debug.unityLogger.logHandler);
	}
}
