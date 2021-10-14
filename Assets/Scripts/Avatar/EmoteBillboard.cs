using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmoteBillboard : MonoBehaviour
{
	public Transform cameraTransform;
	private Transform spriteTransform;
	public bool alignNotLook = true;

	public List<Sprite> emoteList = new List<Sprite>();

	void Start()
	{
		InitializeEmote();
	}

	void LateUpdate()
	{
		if (alignNotLook)
			spriteTransform.forward = cameraTransform.forward;
		else
			spriteTransform.LookAt(cameraTransform, Vector3.up);
	}

    //private void OnEnable()
    //{
    //    InitializeEmote();
    //}

    public void UseEmote(int i)
    {
		gameObject.GetComponent<SpriteRenderer>().sprite = emoteList[i];
		gameObject.SetActive(true);

		StartCoroutine(ShowEmote(i));
    }

	IEnumerator ShowEmote (int i) 
	{
		yield return new WaitForSeconds(2.5f);

		gameObject.SetActive(false);
	}

	private void InitializeEmote()
    {
		spriteTransform = this.transform;
		cameraTransform = Camera.main.transform;
	}
}