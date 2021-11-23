using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum level_data
{
    level,
    next_level
}

public class LevelManager : MonoBehaviour
{
    public ProfileHandler profileHandler;
    private Profile userProfile = null;
    public int[] level = { 0, 0 };
    public Sprite[] sprites;
    private Sprite new_sprite;
    private string[] level_texts =
        {
        "Loading..."         ,
        "Common Squirrel"    ,
        "Fine Stoat"         ,
        "Silver Fox"         ,
        "Golden Hare"        ,
        "Guardian Bear"      ,
        "Ruby Swan"          ,
        "Diamond Ringed Seal",
        "Legendary Lynx"     ,
        "Divine Snowy Owl"   ,
        "Mythical Moose"
        };

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (userProfile == null)
        {
            UpdateLevels();

            /*
            switch (level)
            {
                case 1:
                    sprite = Resources.Load<Sprite>("squirrel_level1");
                    break;
                case 2:
                    sprite = Resources.Load<Sprite>("stoat_level2");
                    break;
                case 3:
                    sprite = Resources.Load<Sprite>("fox_level3");
                    break;
                case 4:
                    sprite = Resources.Load<Sprite>("rabbit_level4");
                    break;
                case 5:
                    sprite = Resources.Load<Sprite>("bear_level5");
                    break;
                case 6:
                    sprite = Resources.Load<Sprite>("swan_level6");
                    break;
                case 7:
                    sprite = Resources.Load<Sprite>("squirrel_level1");
                    break;
                case 8:
                    sprite = Resources.Load<Sprite>("squirrel_level1");
                    break;
                case 9:
                    sprite = Resources.Load<Sprite>("squirrel_level1");
                    break;
                default:
                    sprite = Resources.Load<Sprite>("squirrel_level1");
                    break;
            }
            */
        }
    }

    public void UpdateLevels()
    {
        userProfile = profileHandler.userProfile;
        level = userProfile.GetProfileLevel();
        new_sprite = sprites[level[(int)level_data.level]];

        //image = transform.FindChild("level");
        foreach (Transform child in transform)
        {
            //LEVEL IMAGE & SLIDER
            if (child.name == "level")
            {
                //Set image
                child.GetComponent<Image>().sprite = new_sprite;

                //SLIDER
                foreach (Transform child_2 in child.transform)
                {
                    if (child_2.name == "Slider")
                    {
                        //Set Current progress
                        child_2.GetComponent<Slider>().value = 100;
                        child_2.GetComponent<Slider>().minValue = 0;
                        child_2.GetComponent<Slider>().maxValue = level[(int)level_data.next_level];
                    }
                }
            }

            //LEVEL TEXT
            if (child.name == "Name")
            {
                foreach (Transform child_2 in child.transform)
                {
                    if (child_2.name == "levelText")
                    {
                        child_2.GetComponent<Text>().text = level_texts[level[(int)level_data.level]];
                    }
                }
            }
        }
    }
}
