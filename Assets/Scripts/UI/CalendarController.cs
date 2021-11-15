using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalendarController : MonoBehaviour
{
    public List<GameObject> roomPanels = new List<GameObject>();
    public GameObject buttonTimePrefab;

    public int panelAmount;
    public int timeButtonAmount;

    public GameObject buttonNextDay;
    public GameObject buttonPrevDay;

    public GameObject buttonNextRoom;
    public GameObject buttonPrevRoom;

    public GameObject roomsInfo;
    private RoomInfo[] roomsList;
    public int selectedRoom;
    private List<int> roomsShownList = new List<int>();
    [SerializeField]
    private int roomsShownOffset;

    private void Start()
    {
        roomsList = roomsInfo.GetComponentsInChildren<RoomInfo>();
        roomsShownOffset = selectedRoom > roomsList.Length - panelAmount ? roomsList.Length - panelAmount : selectedRoom;

        ShowBookings();

        buttonNextRoom.GetComponent<Button>().onClick.AddListener(NextRoom);
        buttonPrevRoom.GetComponent<Button>().onClick.AddListener(PrevRoom);

    }

    // Show bookings for active room
    private void ShowBookings()
    {
        MakeRoomNameList();

        for (int i = 0; i < panelAmount; i++)
        {
            GameObject roomPanel = roomPanels[i];
            CalendarElement calendarElement = roomPanel.GetComponent<CalendarElement>();

            // Add room names and floor
            string floorName;

            switch(roomsList[roomsShownList[i]].floor) {
                case 0:
                    floorName = "Outside";
                    break;
                case 1:
                    floorName = "First floor";
                    break;
                case 2:
                    floorName = "Second floor";
                    break;
                case 3:
                    floorName = "Third floor";
                    break;
                default:
                    floorName = "";
                    break;
            }

            calendarElement.roomName.text = roomsList[roomsShownList[i]].roomName + "\n" + floorName;

            // Add booking times
            AddTimeButtons(roomPanel, MakeFakeBookings());
            calendarElement.scrollContent.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 750.0f, 0);
        }

        for (int i = 0; i < roomsShownList.Count; i++)
        {
            GameObject roomPanel = roomPanels[i];
            CalendarElement calendarElement = roomPanel.GetComponent<CalendarElement>();

            if (selectedRoom == roomsShownList[i])
            {
                calendarElement.roomSelectButton.gameObject.GetComponent<Image>().color = new Color32(170, 170, 170, 255);
            } 
            else {
                calendarElement.roomSelectButton.gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }
        }

        AddRoomButtonFunctionality();
    }

    private void AddTimeButtons(GameObject calendarElement, List<bool> bookedList)
    {
        CalendarElement elementScript = calendarElement.GetComponent<CalendarElement>();
        
        // Destroy old children
        foreach (Transform child in elementScript.scrollContent.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < timeButtonAmount; i++)
        {
            GameObject newTimeButton = Instantiate(buttonTimePrefab, buttonTimePrefab.transform.position, buttonTimePrefab.transform.rotation);
            newTimeButton.transform.SetParent(elementScript.scrollContent.transform, false);
            newTimeButton.transform.GetChild(0).GetComponent<Text>().text = i.ToString() + ":00";

            if (bookedList[i]) { newTimeButton.transform.GetChild(1).GetComponent<Text>().text = "Booked"; }
            else { newTimeButton.transform.GetChild(1).GetComponent<Text>().text = ""; }
        }
    }

    private void NextRoom()
    {
        if(!(roomsShownList[roomsShownList.Count - 1] >= roomsList.Length - 1))
        {
            roomsShownOffset = roomsShownOffset + 1 > roomsList.Length - panelAmount ? roomsList.Length - panelAmount : roomsShownOffset + 1;
            ShowBookings();
        } 
        else
        {
            Debug.Log("Ei Next");
        }
    }

    private void PrevRoom()
    {
        if (!(roomsShownList[0] <= 0))
        {
            roomsShownOffset = roomsShownOffset - 1 < 0 ? 0 : roomsShownOffset - 1;
            ShowBookings();
        }
        else
        {
            Debug.Log("Ei Prev");
        }
    }

    private void MakeRoomNameList()
    {
        int tmpRoom = roomsShownOffset < 0 ? 0 : roomsShownOffset;
        roomsShownList.Clear();

        if (roomsList.Length - tmpRoom >= panelAmount)
        {
            for (int i = 0; i < panelAmount; i++)
            {
                roomsShownList.Add(tmpRoom + i);
            }
        } 
        else
        {
            for (int i = 0; i < panelAmount; i++)
            {
                roomsShownList.Add(roomsList.Length - panelAmount + i);
            }
        }

        //Debug.Log("Showing rooms: " + roomsShownList[0] + ", " + roomsShownList[1] + ", " + roomsShownList[2] + ", " + roomsShownList[3] + ", " + roomsShownList[4]);
    }

    private void AddRoomButtonFunctionality()
    {
        for (int i = 0; i < roomPanels.Count; i++)
        {
            int tmp = roomsShownList[i];
            Button roomButton = roomPanels[i].GetComponent<CalendarElement>().roomSelectButton;
            roomButton.onClick.RemoveAllListeners();
            roomButton.onClick.AddListener(delegate { RoomButtonOnClick(tmp); });
        }
    }

    private void RoomButtonOnClick(int i)
    {
        Debug.Log("Clicked button " + i);
        ShowBookings();
    }

    // Testifunktio
    private List<bool> MakeFakeBookings()
    {
        List<bool> fakeBookingList = new List<bool>();

        for (int i = 0; i < timeButtonAmount; i++) { fakeBookingList.Add(Random.Range(0, 2) == 0); }

        return fakeBookingList;
    }
}
