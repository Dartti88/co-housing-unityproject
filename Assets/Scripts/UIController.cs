using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// singleton controller to control the UI elements
public class UIController : Singleton<UIController>
{
    // elements used in the case selection menu
    public Image CaseSelectionParent;
    public GameObject CaseSelectionElements;

    // notification icon used in cases
    public GameObject Notification;

    // how long black fade takes
    public float FadeTime = 2.0f;

    // text fields used in cases
    public Text NameField;
    public Text MoneyField;
    public Text PointsField;
    public Text MessageContent;

    // textfield for narrative text in cases
    public Text NarrativeField;

    private bool fading = false;

    // run once at scene load
    private void Start()
    {
        ToggleCaseSelection(true, true);
    }

    // open case selection menu
    public void ToggleCaseSelection(bool state)
    {
        ToggleCaseSelection(state, false);
    }

    public void ToggleCaseSelection(bool state, bool instant)
    {
        StartCoroutine(AnimateCaseSelection(state, instant));
    }

    // set name of the example person in a scene
    public void SetName(string name)
    {
        NameField.text = name;
    }

    // set message text field 
    public void SetMessage(string text)
    {
        MessageContent.text = text;
    }

    // set money amount
    public void SetMoney(int money)
    {
        MoneyField.text = money.ToString();
        MoneyField.GetComponent<Animation>().Play();
    }

    // add money to current money amount
    public void AddMoney(int money)
    {
        SetMoney(int.Parse(MoneyField.text) + money);
    }

    // set points
    public void SetPoints(int points)
    {
        PointsField.text = points.ToString();
        PointsField.GetComponent<Animation>().Play();
    }
    // add points to current amount
    public void AddPoints(int points)
    {
        SetPoints(int.Parse(PointsField.text) + points);
    }

    // fade black to open case selection screen
    IEnumerator AnimateCaseSelection(bool state, bool instant)
    {
        if (fading) yield break;
        fading = true;

        if (!state) CaseSelectionElements.SetActive(false);

        if (instant)
        {
            var color = CaseSelectionParent.color;
            color.a = state ? 1 : 0;
            CaseSelectionParent.color = color;
        }
        else
        {
            float t = 0.0f;
            while (t < 1.0f)
            {
                var color = CaseSelectionParent.color;
                color.a = state ? t : 1 - t;
                CaseSelectionParent.color = color;
                t += Time.deltaTime / FadeTime;
                yield return null;
            }
        }

        if (state) CaseSelectionElements.SetActive(true);
        CaseSelectionParent.raycastTarget = state;

        fading = false;
    }

    // fade to black to show given narrative text
    public IEnumerator ShowNarrativeText(string text)
    {
        if (fading) yield break;
        fading = true;

        NarrativeField.text = "";

        float t = 0.0f;
        while (t < 1.0f)
        {
            var color = CaseSelectionParent.color;
            color.a = t;
            CaseSelectionParent.color = color;
            t += Time.deltaTime / FadeTime;
            yield return null;
        }

        NarrativeField.text = text;        
    }

    // fade out of black and hide narrative text
    public IEnumerator HideNarrativeText()
    {
        yield return new WaitForSeconds(2);
        NarrativeField.text = "";

        float t = 1.0f;
        while (t > 0.0f)
        {
            var color = CaseSelectionParent.color;
            color.a = t;
            CaseSelectionParent.color = color;
            t -= Time.deltaTime / FadeTime;
            yield return null;
        }

        fading = false;
    }
}
