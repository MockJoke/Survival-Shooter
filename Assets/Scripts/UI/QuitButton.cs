using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuitButton : MonoBehaviour
{
    public void OnQuit()
    {
#if UNITY_EDITOR 
        EditorApplication.isPlaying = false;
#else 
		Application.Quit();
#endif
    }
}
