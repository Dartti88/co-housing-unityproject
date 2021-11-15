using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SetItemIDs : MonoBehaviour
{
    private static string SelectedTag = "Item";
    [MenuItem("Helpers/Give IDs to items")]
    public static void GiveIDs()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(SelectedTag);
        int i = 1;
        Debug.Log("objects length: " + objects.Length);
        foreach (GameObject obj in objects)
        {
            
            if (obj.GetComponent<ClickableObject>() != null)
            {

                if (obj.GetComponent<ClickableObject>().TargetGroup.Count > 0)
                {
                    foreach (ClickableObject clickable in obj.GetComponent<ClickableObject>().TargetGroup)
                    {
                        if (clickable.gameObject.GetComponent<ItemGameObject>() != null)
                        {
                            Debug.Log("i = " + i);
                            clickable.gameObject.GetComponent<ItemGameObject>()._itemID = i;
                        }
                    }
                }
                else
                {
                    if (obj.GetComponent<ItemGameObject>() != null)
                    {
                        Debug.Log("i = " + i);
                        obj.GetComponent<ItemGameObject>()._itemID = i;
                    }
                }
                
                i++;
            }
        }
    }

}
