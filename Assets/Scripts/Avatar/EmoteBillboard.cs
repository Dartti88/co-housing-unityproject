using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmoteBillboard : MonoBehaviour
{
	public Transform cameraTransform;
	private Transform spriteTransform;
	public bool alignNotLook = true;

	public Animator animator;
	private static readonly int hashEmoteTrigger = Animator.StringToHash("EmoteTrigger");
	private bool showPlaying = false;

	public List<Sprite> emoteList = new List<Sprite>();
	public List<GameObject> particleSystems = new List<GameObject>();

	private void Start()
	{
		InitializeEmote();
	}

    private void LateUpdate()
	{
		if (alignNotLook)
			spriteTransform.forward = cameraTransform.forward;
		else
			spriteTransform.LookAt(cameraTransform, Vector3.up);
	}

    public void UseEmote(int i)
    {
		if(showPlaying) { return; }

		gameObject.GetComponent<SpriteRenderer>().sprite = emoteList[i];
		gameObject.SetActive(true);

		animator.SetTrigger(hashEmoteTrigger);
		particleSystems[i].GetComponent<ParticleSystem>().Play();

		showPlaying = true;
    }

	public void HideEmote()
    {
		animator.SetTrigger(hashEmoteTrigger);

		showPlaying = false;
	}

	private void InitializeEmote()
    {
		spriteTransform = this.transform;
		cameraTransform = Camera.main.transform;
	}
}