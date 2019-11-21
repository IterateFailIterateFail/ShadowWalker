using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class MusicController : MonoBehaviour {

	public AudioMixerSnapshot soundOverworld;
	public AudioMixerSnapshot soundUpsidedown;
	public GameObject player;

	private float transitionTime = 0.5f;
	private PlayerControl playerControl;
	private bool inOverworld = true;

	// Use this for initialization
	void Start()  {
		playerControl = player.GetComponent<PlayerControl>();
	}

	void Update() {
		if (playerControl.isUpsideDown()) {
			if (inOverworld) soundUpsidedown.TransitionTo(transitionTime);
			inOverworld = false;
		} else {
			if (!inOverworld) soundOverworld.TransitionTo(transitionTime);
			inOverworld = true;
		}
	}

}
