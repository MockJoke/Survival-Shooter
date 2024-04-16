using UnityEngine;
using UnityEngine.UI;

public class VolumeHandler : MonoBehaviour
{
	[SerializeField] private Slider effectsSlider;

	void Awake()
	{
		if (effectsSlider == null)
			effectsSlider = GameObject.Find("EffectsSlider").GetComponent<Slider>();
	}

	void Start() 
	{
		effectsSlider.onValueChanged.AddListener(SetVolume);
	}

	void SetVolume(float volume)
	{
		GetComponent<AudioSource>().volume = volume;
	}

	void OnDestroy()
	{
		effectsSlider.onValueChanged.RemoveListener(SetVolume);
	}
}
