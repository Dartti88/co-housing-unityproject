using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarSelection : MonoBehaviour
{
    [SerializeField] RawImage avatarImage;
    [SerializeField] Texture[] avatarImages;
    ProfileHandler pHandler;

    int imageIndex;
    void Start()
    {
        pHandler = FindObjectOfType<ProfileHandler>();
        if (pHandler != null && pHandler.userProfile != null)
            imageIndex = pHandler.userProfile.avatarID;
        else
            imageIndex = 0;
        ChangeImage();
    }

    void ChangeImage()
    {
        avatarImage.texture = avatarImages[imageIndex];
        pHandler.ChangeAvatarID(imageIndex);
    }

    public void LeftButton()
    {
        imageIndex--;
        if (imageIndex < 0)
            imageIndex = avatarImages.Length - 1;
        ChangeImage();
    }

    public void RightButton()
    {
        imageIndex++;
        if (imageIndex > avatarImages.Length - 1)
            imageIndex = 0;
        ChangeImage();
    }

}
