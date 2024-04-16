using UnityEngine;
using UnityEngine.Audio;

public class PauseManager : MonoBehaviour 
{
	[SerializeField] private AudioMixerSnapshot paused;
	[SerializeField] private AudioMixerSnapshot unpaused;
	[SerializeField] private Canvas canvas;

	void Awake()
	{
		if (canvas == null)
			canvas = GetComponent<Canvas>();
	}
	
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			canvas.enabled = !canvas.enabled;
			TogglePause();
		}
	}
	
	public void TogglePause()
	{
		Time.timeScale = Time.timeScale == 0 ? 1 : 0;
		LowPassAudio();
	}
	
	private void LowPassAudio()
	{
		if (Time.timeScale == 0)
		{
			paused.TransitionTo(.01f);
		}
		else
		{
			unpaused.TransitionTo(.01f);
		}
	}
}