using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// singleton controller for playing through the demostration cases
public class DemoController : Singleton<DemoController>
{
    public int CurrentCase = 0;
    // which case phase the player is in
    public int CurrentCaseObjective = 0;

    // different UI views to show during the cases
    public GameObject DefaultView;
    public GameObject BookRoomView;
    public GameObject MessageView;
    public GameObject ProductView;
    public GameObject TaskView;

    public GameObject TaskElement;

    public GameObject SelectCaseButton;

    public void StartCase(int caseNumber)
    {
        CurrentCase = caseNumber;
        StartCoroutine(caseNumber == 1 ? CaseOne() : CaseTwo());
    }

    // this is called by timers or clickable elements in the house
    // this progresses the cases
    public void TryCompleteObjective(int objective)
    {
        Debug.Log("TRY COMPLETE " + objective);
        if (objective == CurrentCaseObjective) CurrentCaseObjective++;
    }

    // open default UI view and close others
    public void OpenDefaultView()
    {
        DefaultView.SetActive(true);
        BookRoomView.SetActive(false);
        MessageView.SetActive(false);
        ProductView.SetActive(false);
        TaskView.SetActive(false);
    }

    // function which plays the cases stops to wait until player makes the correct interaction to progress the current case
    IEnumerator CaseOne()
    {
        SelectCaseButton.SetActive(false);
        PlayerController.Instance.ResetPosition();

        CurrentCaseObjective = 0;
        UIController.Instance.SetName("Sophia");
        UIController.Instance.Notification.SetActive(false);
        
        while (CurrentCaseObjective == 0) // wait until clicked board
        {
            yield return null;
        }
        // ui show mow lawn mission, pledge
        DefaultView.gameObject.SetActive(false);
        TaskView.gameObject.SetActive(true);

        while (CurrentCaseObjective == 1) // wait until pledged and confirmed
        {
            yield return null;
        }
        //DefaultView.gameObject.SetActive(true);
        TaskElement.SetActive(false);

        yield return new WaitForSeconds(3);

        UIController.Instance.Notification.SetActive(true);

        while (CurrentCaseObjective == 2) // wait until message opened
        {
            yield return null;
        }

        TaskView.SetActive(false);
        TaskElement.SetActive(true);
        UIController.Instance.AddMoney(40);
        UIController.Instance.AddPoints(10);

        // receive message job was rated well
        DefaultView.SetActive(false);
        MessageView.SetActive(true);
        UIController.Instance.SetMessage("Your completed task 'Mow the lawn' was rated as well done! You have received the reward: \n40€ and 10p");

        yield return new WaitForSeconds(7);
        

        yield return UIController.Instance.ShowNarrativeText("Next month");

        // get message from gardener
        MessageView.SetActive(false);
        PlayerController.Instance.ResetPosition();

        yield return UIController.Instance.HideNarrativeText();

        yield return new WaitForSeconds(2);

        CurrentCaseObjective++;

        UIController.Instance.Notification.SetActive(true);
        while (CurrentCaseObjective == 4) // wait until message opened
        {
            yield return null;
        }

        DefaultView.SetActive(false);
        MessageView.SetActive(true);
        UIController.Instance.AddMoney(-50);
        UIController.Instance.SetMessage("Nobody completed 'Mow the lawn' and a gardener was hired to do it. The payment (-50€) has been deducted from your funds.");
        // lose money
        
        SelectCaseButton.SetActive(true);

    }

    IEnumerator CaseTwo()
    {
        SelectCaseButton.SetActive(false);
        PlayerController.Instance.ResetPosition();
        CurrentCaseObjective = 10;
        UIController.Instance.SetName("Ana");
        UIController.Instance.Notification.SetActive(false);
        OpenDefaultView();

        while (CurrentCaseObjective == 10) // wait until clicked door
        {
            yield return null;
        }

        DefaultView.SetActive(false);
        BookRoomView.SetActive(true);

        while (CurrentCaseObjective == 11) // wait until room is booked
        {
            yield return null;
        }

        BookRoomView.SetActive(false);
        UIController.Instance.AddMoney(-40);

        yield return new WaitForSeconds(4.0f);

        yield return UIController.Instance.ShowNarrativeText("Yoga lesson passed, Ana went to the grocery store and came back home");

        MessageView.SetActive(false);
        DefaultView.SetActive(false);
        PlayerController.Instance.ResetPosition();

        yield return new WaitForSeconds(3);
        yield return UIController.Instance.HideNarrativeText();

        yield return new WaitForSeconds(3.0f);

        UIController.Instance.Notification.SetActive(true);
        while (CurrentCaseObjective == 12) // wait notification clicked
        {
            yield return null;
        }

        UIController.Instance.SetMessage("Received participation payments from Yoga lesson: 70€");
        DefaultView.SetActive(false);
        MessageView.SetActive(true);
        UIController.Instance.AddMoney(70);

        while (CurrentCaseObjective == 13) // wait until table is clicked
        {
            yield return null;
        }

        // open UI to sell cookies
        MessageView.SetActive(false);
        DefaultView.SetActive(false);
        ProductView.SetActive(true);

        while (CurrentCaseObjective == 14) // wait until confirm is clicked
        {
            yield return null;
        }

        ProductView.SetActive(false);

        yield return new WaitForSeconds(4.0f);

        UIController.Instance.Notification.SetActive(true);
        while (CurrentCaseObjective == 15) // wait until notification is clicked
        {
            yield return null;
        }

        UIController.Instance.SetMessage("Sophia bought product 'Cookies' from you. You received 4€");
        MessageView.SetActive(true);
        UIController.Instance.AddMoney(4);
        SelectCaseButton.SetActive(true);
        // get message, Sophia bought cookies
        // get money
        
    }
}
