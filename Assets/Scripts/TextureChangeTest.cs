using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextureChangeTest : MonoBehaviour
{
    [SerializeField]
    public GameObject obj;

    [SerializeField]
    private Texture[] texturesKoivu;

    private Renderer rend;

    public int currentTexture;


    private void Start()
    {
        rend = obj.GetComponent<Renderer>();
        gameObject.GetComponent<Button>().onClick.AddListener(ChangeTexture);

    }

    private void ChangeTexture()
    {
        //textureIndex = Random.Range(0, textures.Length);
        if (currentTexture < 3)
        {
            currentTexture++;
        } else
        {
            currentTexture = 0;
        }

        rend.sharedMaterial.mainTexture = texturesKoivu[currentTexture];
    }
}
