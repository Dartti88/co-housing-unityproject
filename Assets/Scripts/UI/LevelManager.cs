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
    public int[] arr_level = { 1, 100 };
    
    //public Sprite[] sprites;
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
    public ProfileUIController UI_controller;
    private string slider_text = "";



    // Start is called before the first frame update
    void Start()
    {
        profileHandler = FindObjectOfType<ProfileHandler>();

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
        }
        else if (timer<=0)
        {
            if (slider.GetComponent<RectTransform>().rect.width > 0)
            {
                //slider.GetComponent<RectTransform>().rect.width;
                slider.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 180, slider.GetComponent<RectTransform>().rect.width - 5);
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
                slider.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 180, slider.GetComponent<RectTransform>().rect.width+5);
            }

            progress_text.text = (slider.GetComponent<RectTransform>().rect.width < 75) ? "" : slider_text;

            timer -= Time.deltaTime;
        }
            
    }

    public int[] GetProfileLevel()
    {
        int level = 1;
        int i = 100;
        while (i <= userProfile.socialScore)
        {
            level++;
            i *= 2;
        }
        int[] array_return = { level, i };
        Debug.Log("Level: " + level + "- Scores: " + userProfile.socialScore);
        return array_return;
    }

    //public void AddSocialPoints(int amount)
    //{
    //    int current_level = profileHandler.userProfile.GetProfileLevel()[(int)level_data.level];
    //    profileHandler.userProfile.socialScore += amount;
    //    int new_level = profileHandler.userProfile.GetProfileLevel()[(int)level_data.level];
    //
    //    level_manager.UpdateLevels();
    //}

    public void UpdateLevels()
    {
        int old_level = arr_level[0];
        userProfile = profileHandler.userProfile;
        arr_level = GetProfileLevel();
        new_sprite = UI_controller.levelPictures[arr_level[(int)level_data.level]];

        image.sprite = new_sprite;
        slider.value = userProfile.socialScore;
        slider.minValue = (arr_level[(int)level_data.level] <= 1) ? 0 : arr_level[(int)level_data.next_level]/2;
        slider.maxValue = arr_level[(int)level_data.next_level];
        slider.gameObject.SetActive(!slider.gameObject.activeSelf);
        slider_text = userProfile.socialScore.ToString() + "/" + arr_level[(int)level_data.next_level].ToString();
        progress_text.text = slider_text;
        level_text.text = level_texts[arr_level[(int)level_data.level]];
        timer = 7;

        if (arr_level[(int)level_data.level] > old_level)
        {
            //ToDo - Trigger levelup event
        }

        //Debug.Log(userProfile.socialScore.ToString());
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
