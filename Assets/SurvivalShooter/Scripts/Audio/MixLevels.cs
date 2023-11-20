using UnityEngine;
using UnityEngine.Audio;

public class MixLevels : MonoBehaviour 
{
	[SerializeField] private AudioMixer masterMixer;

	public void SetSfxLvl(float sfxLvl)
	{
		masterMixer.SetFloat("sfxVol", sfxLvl);
	}

	public void SetMusicLvl(float musicLvl)
	{
		masterMixer.SetFloat("musicVol", musicLvl);
	}
}
