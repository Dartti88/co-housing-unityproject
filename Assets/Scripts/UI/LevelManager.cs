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
    float timer = 0;
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

    private Image image;
    private Slider slider;
    private Text level_text;
    private Text progress_text;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            //LEVEL IMAGE & SLIDER
            if (child.name == "level")
            {
                //Set image
                image = child.GetComponent<Image>();

                //SLIDER
                foreach (Transform child_2 in child.transform)
                {
                    if (child_2.name == "Slider")
                    {
                        //Set Current progress to slider
                        slider = child_2.GetComponent<Slider>();

                        foreach (Transform child_3 in child_2.transform)
                        {
                            if (child_3.name == "progress_text")
                            {
                                //Set Current progress to slider text
                                progress_text = child_3.GetComponent<Text>();
                            }
                        }
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
                        level_text = child_2.GetComponent<Text>();
                    }
                }
            }
        }
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
        else if (timer<=0)
        {
           

            if (slider.GetComponent<RectTransform>().rect.width > 0)
            {
                //slider.GetComponent<RectTransform>().rect.width;
                slider.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 180, slider.GetComponent<RectTransform>().rect.width - 1);
            }
            else
            {
                slider.gameObject.SetActive(false);
            }

            if (slider.GetComponent<RectTransform>().rect.width > 75)
            {
                progress_text.text = "";
            }
            
        }
        else
        {
            if (slider.GetComponent<RectTransform>().rect.width < 200)
            {
                slider.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 180, slider.GetComponent<RectTransform>().rect.width+1);
            }
            timer -= Time.deltaTime;
        }
            
    }

    public void UpdateLevels()
    {
        userProfile = profileHandler.userProfile;
        level = userProfile.GetProfileLevel();
        new_sprite = sprites[level[(int)level_data.level]];

        image.sprite = new_sprite;
        slider.value = userProfile.socialScore;
        slider.minValue = 0;
        slider.maxValue = level[(int)level_data.next_level];
        slider.gameObject.SetActive(!slider.gameObject.activeSelf);
        progress_text.text = userProfile.socialScore.ToString() + "/" + level[(int)level_data.next_level].ToString();
        level_text.text = level_texts[level[(int)level_data.level]];
        timer = 7;
        /*
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
                        //Set Current progress to slider
                        child_2.GetComponent<Slider>().value = userProfile.socialScore;
                        child_2.GetComponent<Slider>().minValue = 0;
                        child_2.GetComponent<Slider>().maxValue = level[(int)level_data.next_level];
                        child_2.gameObject.SetActive(!child_2.gameObject.activeSelf);

                        foreach (Transform child_3 in child_2.transform)
                        {
                            if (child_3.name == "currentExp")
                            {
                                //Set Current progress to slider text
                                child_3.GetComponent<Text>().text = userProfile.socialScore.ToString();
                             }

                            if (child_3.name == "targetExp")
                            {
                                //Set Current progress to slider text
                                child_3.GetComponent<Text>().text = level[(int)level_data.next_level].ToString();
                            }

                            if (child_3.name == "progress_text")
                            {
                                //Set Current progress to slider text
                                child_3.GetComponent<Text>().text = userProfile.socialScore.ToString() + "/" + level[(int)level_data.next_level].ToString();
                            }
                            
                        }
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
        */
    }
}
