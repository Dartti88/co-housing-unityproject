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

    public GameObject bookingWindow;
    public Button bwBook;
    public Button bwCancel;
    public Button bwClose;

    public GameObject roomsInfo;
    private RoomInfo[] roomsList;
    public int selectedRoom;
    private List<int> roomsShownList = new List<int>();
    [SerializeField]
    private int roomsShownOffset;

    private List<Booking> bookingsListPerRoom = new List<Booking>();

    private List<string> datesShownList = new List<string>();

    private void Start()
    {
        roomsList = roomsInfo.GetComponentsInChildren<RoomInfo>();
        roomsShownOffset = selectedRoom > roomsList.Length - panelAmount ? roomsList.Length - panelAmount : selectedRoom;

        buttonNextRoom.GetComponent<Button>().onClick.AddListener(NextRoom);
        buttonPrevRoom.GetComponent<Button>().onClick.AddListener(PrevRoom);

        buttonNextDay.GetComponent<Button>().onClick.AddListener(NextDay);
        buttonPrevDay.GetComponent<Button>().onClick.AddListener(PrevDay);

        bwBook.onClick.AddListener(BookRoom);
        bwCancel.onClick.AddListener(CancelBooking);
        bwClose.onClick.AddListener(CloseBooking);

        // TEST BOOKINGS
        MakeTestBookings();

        // TEST DATES
        MakeTestDates();

        ShowBookings();
    }

    // Show bookings for active room
    private void ShowBookings()
    {
        UpdateRoomNames();

        for (int i = 0; i < panelAmount; i++)
        {
            GameObject roomPanel = roomPanels[i];
            CalendarElement calendarElement = roomPanel.GetComponent<CalendarElement>();

            // Display dates
            calendarElement.dayName.text = "";
            calendarElement.date.text = datesShownList[i];

            // Add booking times
            AddTimeButtons(roomPanel);
            calendarElement.scrollContent.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 750.0f, 0);
        }

    }

    private void AddTimeButtons(GameObject calendarElement)
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
            BookingElement bookingElement = newTimeButton.AddComponent<BookingElement>();

            //if (bookingHelperList[i]._boolBooked) { newTimeButton.transform.GetChild(1).GetComponent<Text>().text = "Booked"; }
            //else { newTimeButton.transform.GetChild(1).GetComponent<Text>().text = ""; }

            int startTime = i;
            int endTime = i + 1;

            newTimeButton.GetComponent<Button>().onClick.AddListener(delegate { 
                OpenBookingWindow(new BWContainer(
                    roomsList[selectedRoom].roomName,
                    startTime.ToString() + ":00 - " + endTime.ToString() + ":00",
                    "2021-1-3",
                    roomsList[selectedRoom].creditPerHour,
                    roomsList[selectedRoom].size,
                    "-",
                    selectedRoom)); 
            });
        }
    }

    private void NextRoom()
    {
        if(!(roomsShownList[roomsShownList.Count - 1] >= roomsList.Length - 1))
        {
            roomsShownOffset = roomsShownOffset + 1 > roomsList.Length - panelAmount ? roomsList.Length - panelAmount : roomsShownOffset + 1;
            UpdateRoomNames();
        } 
        else
        {
            Debug.Log("No more Next");
        }
    }

    private void PrevRoom()
    {
        if (!(roomsShownList[0] <= 0))
        {
            roomsShownOffset = roomsShownOffset - 1 < 0 ? 0 : roomsShownOffset - 1;
            UpdateRoomNames();
        }
        else
        {
            Debug.Log("No more Prev");
        }
    }

    private void NextDay()
    {
        Debug.Log("Next Day");
    }

    private void PrevDay()
    {
        Debug.Log("Prev Day");
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

    private void UpdateRoomNames()
    {
        MakeRoomNameList();

        for (int i = 0; i < panelAmount; i++)
        {
            GameObject roomPanel = roomPanels[i];
            CalendarElement calendarElement = roomPanel.GetComponent<CalendarElement>();

            // Add room names and floor
            string floorName;

            switch (roomsList[roomsShownList[i]].floor)
            {
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
        }

        for (int i = 0; i < roomsShownList.Count; i++)
        {
            GameObject roomPanel = roomPanels[i];
            CalendarElement calendarElement = roomPanel.GetComponent<CalendarElement>();

            if (selectedRoom == roomsShownList[i])
            {
                calendarElement.roomSelectButton.gameObject.GetComponent<Image>().color = new Color32(170, 170, 170, 255);
            }
            else
            {
                calendarElement.roomSelectButton.gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }
        }

        AddRoomButtonFunctionality();
    }

    private void OpenBookingWindow(BWContainer bwContainer)
    {
        bookingWindow.SetActive(true);
        BookingWindow bw = bookingWindow.GetComponent<BookingWindow>();

        bw.roomName.text = bwContainer.roomName;
        bw.bookingTime.text = bwContainer.bookingTime;
        bw.bookingDate.text = bwContainer.bookingDate;
        bw.bookingCost.text = bwContainer.bookingCost.ToString() + " credits";
        bw.roomSize.text = bwContainer.roomSize.ToString() + "m≤";
        bw.bookerName.text = bwContainer.bookerName;
        bw.roomImage.sprite = roomsList[bwContainer.pictureID].roomPicture;
    }

    private void BookRoom()
    {
        // huoneen nimi, p‰iv‰m‰‰r‰, tunti
        Debug.Log("Room booked: " + selectedRoom + ", " + roomsList[selectedRoom].roomName);
    }

    private void CancelBooking()
    {
        Debug.Log("Room booking canceled");
    }

    private void CloseBooking()
    {
        bookingWindow.SetActive(false);
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
        selectedRoom = i;
        Debug.Log("Clicked button " + i);
        ShowBookings();
    }

    private void MakeTestDates()
    {
        for (int i = 0; i < panelAmount; i++)
        {
            int x = 10 + i;
            datesShownList.Add("2022-12-" + x.ToString());
        }
    }

    private void MakeTestBookings()
    {
        bookingsListPerRoom.Clear();

        bookingsListPerRoom.Add(new Booking("Mˆm", "2022-12-10", 9, "JJ"));
        bookingsListPerRoom.Add(new Booking("Mdasm", "2022-12-11", 12, "Jds"));
        bookingsListPerRoom.Add(new Booking("Mhffm", "2022-12-10", 11, "Jrw"));
        bookingsListPerRoom.Add(new Booking("Mkmytm", "2022-12-12", 4, "Jn"));
        bookingsListPerRoom.Add(new Booking("Msada", "2022-12-13", 22, "Jhgj"));
    }

}

public class Booking
{
    public string roomName;
    public string date;
    public int startingTime;
    public string bookerName;

    public Booking(string rn, string d, int st, string bn)
    {
        roomName = rn;
        date = d;
        startingTime = st;
        bookerName = bn;
    }
}

public class BWContainer
{
    public string roomName;
    public string bookingTime;
    public string bookingDate;
    public float bookingCost;
    public float roomSize;
    public string bookerName;
    public int pictureID;

    public BWContainer (string roomname, string booktime, string bookdate, float bookingcost, float roomsize, string bookername, int picID)
    {
        roomName = roomname;
        bookingTime = booktime;
        bookingDate = bookdate;
        bookingCost = bookingcost;
        roomSize = roomsize;
        bookerName = bookername;
        pictureID = picID;
    }
}