using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextureChangeTest : MonoBehaviour
{
    [SerializeField]
    private GameObject change;

    [SerializeField]
    private Texture[] textures;

    private Renderer rend;

    private int textureIndex;

    private void Start()
    {
        rend = change.GetComponent<Renderer>();
        gameObject.GetComponent<Button>().onClick.AddListener(ChangeTexture);
    }

    private void ChangeTexture()
    {
        textureIndex = Random.Range(0, textures.Length);
        rend.material.mainTexture = textures[textureIndex];
    }
}
