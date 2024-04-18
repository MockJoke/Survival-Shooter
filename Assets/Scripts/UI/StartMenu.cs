using UnityEngine;

public class StartMenu : MonoBehaviour
{
    [SerializeField] private Canvas startMenuCanvas;
    [SerializeField] private Canvas inGameHUDCanvas;
    [SerializeField] private Canvas enemyHealthCanvas;

    void Awake()
    {
        Time.timeScale = 0;
    }

    public void OnPlay()
    {
        startMenuCanvas.enabled = false;
        inGameHUDCanvas.enabled = true;
        enemyHealthCanvas.enabled = true;
        Time.timeScale = 1;
    }
}