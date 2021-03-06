using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum season
{
    winter,
    spring,
    summer,
    fall
}

public class SeasonChange : MonoBehaviour
{
    [SerializeField]
    private GameObject change;

    [SerializeField]
    private Texture[] textures;

    private Renderer rend;

    public int currentTexture;

    public int currentSeason;

    private void Start()
    {
        rend = change.GetComponent<Renderer>();
        ChangeTextureAwake();
    }

    private void Awake()
    {
        System.DateTime localTime = System.DateTime.Now;
        int monthInt = localTime.Month;

        if (monthInt == 12 || monthInt == 1 || monthInt == 2)
        {
            currentSeason = (int)season.winter;
            Debug.Log("It's winter!");
        }
        if (monthInt >= 3 && monthInt <= 5)
        {
            currentSeason = (int)season.spring;
            Debug.Log("It's spring!");
        }
        if (monthInt >= 6 && monthInt <= 8)
        {
            currentSeason = (int)season.summer;
            Debug.Log("It's summer!");
        }
        if (monthInt >= 9 && monthInt <= 11)
        {
            currentSeason = (int)season.fall;
            Debug.Log("It's fall!");
        }
        Debug.Log("Season int: " + currentSeason);
        Debug.Log("Month int: " + monthInt);
        Debug.Log(localTime);
    }
 

    private void ChangeTextureAwake()
    {
        switch (currentSeason)
        {
            case 0:
                currentTexture = 0;
                break;
            case 1:
                currentTexture = 1;
                break;
            case 2:
                currentTexture = 2;
                break;
            case 3:
                currentTexture = 3;
                break;
        }

        rend.sharedMaterial.mainTexture = textures[currentTexture];
    }
}
