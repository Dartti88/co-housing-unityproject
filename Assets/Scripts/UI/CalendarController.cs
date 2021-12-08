using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

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

    private Booking currentBooker;



    // If this is true -> block any attempt to trigger a "new update" until this goes back to false (made false by our OnCompletionCallback passed to the Client's funcs..)
    bool bookingUpdateInProgress = false;
    // Contains room id - name mapping (how they are named in the database!)
    Dictionary<int, string> roomNameMapping_database;

    // *Needed an "intermediary" to convert the downloaded data into a suitable format..
    [Serializable]
    class TempBookingData
    {
        public int startTime;
        public string date;
        public string bookerName;
    }
    class TempBookingsContainer
    {
        public TempBookingData[] bookings;
    }
    // This will end up containing booking data of the currently selected room.
    TempBookingsContainer fetchedBookings;

    private void Start()
    {
        roomNameMapping_database = new Dictionary<int, string>()
        {
            { 0, "Bookings_ConcertDanceHall"},
            { 1, "Bookings_MultipurposeHall"},
            { 2, "Bookings_SummerHall"},
            { 3, "Bookings_SteamWorkshop"},
            { 4, "Bookings_TraditionalSauna"},
            { 5, "Bookings_SummerHall2"},
            { 6, "Bookings_Bedroom2"},
            { 7, "Bookings_Bedroom"},
            { 8, "Bookings_MultipurposeConferenceHall"}
        };

        roomsList = roomsInfo.GetComponentsInChildren<RoomInfo>();
        
        buttonNextRoom.GetComponent<Button>().onClick.AddListener(NextRoom);
        buttonPrevRoom.GetComponent<Button>().onClick.AddListener(PrevRoom);

        buttonNextDay.GetComponent<Button>().onClick.AddListener(NextDay);
        buttonPrevDay.GetComponent<Button>().onClick.AddListener(PrevDay);

        bwBook.onClick.AddListener(BookRoom);
        bwCancel.onClick.AddListener(CancelBooking);
        bwClose.onClick.AddListener(CloseBooking);

        InitializeCalendar(selectedRoom);
    }

    // Show bookings for active room
    private void ShowBookings_LOCAL_VISUAL()
    {
        UpdateRoomNames();

        for (int i = 0; i < panelAmount; i++)
        {
            GameObject roomPanel = roomPanels[i];
            CalendarElement calendarElement = roomPanel.GetComponent<CalendarElement>();

            // Display dates
            DateHelper dh = new DateHelper(datesShownList[i]);
            calendarElement.dayName.text = dh.GetShortDayName();
            calendarElement.date.text = dh.GetNormalDateFormat();

            // Add booking times
            AddTimeButtons(roomPanel, datesShownList[i]);
            calendarElement.scrollContent.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 750.0f, 0);
        }

    }

    // Adds booking times to the calendar
    private void AddTimeButtons(GameObject calendarElement, string panelDate)
    {
        DateHelper dh = new DateHelper();

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

            string bookerNameTmp = "-";

            foreach (Booking b in bookingsListPerRoom)
            {
                if (b._startingTime == i && dh.DatesEqual(b._date, panelDate)/*b._date == panelDate*/ && b._roomID == selectedRoom)
                {
                    bookingElement.booked = true;
                    bookerNameTmp = b._bookerName;
                    break;
                } else
                {
                    bookingElement.booked = false;
                }
            }

            bookingElement.bookingInfo = new Booking(
                i,
                panelDate,
                bookerNameTmp,
                selectedRoom);

            if (bookingElement.booked) { newTimeButton.transform.GetChild(1).GetComponent<Text>().text = "Booked"; }
            else { newTimeButton.transform.GetChild(1).GetComponent<Text>().text = ""; }

            int startTime = i;
            int endTime = i + 1;

            newTimeButton.GetComponent<Button>().onClick.AddListener(delegate { OpenBookingWindow(bookingElement); });
        }
    }

    // ----------- ROOM BUTTONS -----------

    private void NextRoom()
    {
        if(!(roomsShownList[roomsShownList.Count - 1] >= roomsList.Length - 1))
        {
            roomsShownOffset = roomsShownOffset + 1 > roomsList.Length - panelAmount ? roomsList.Length - panelAmount : roomsShownOffset + 1;
            UpdateRoomNames();
        } 
    }

    private void PrevRoom()
    {
        if (!(roomsShownList[0] <= 0))
        {
            roomsShownOffset = roomsShownOffset - 1 < 0 ? 0 : roomsShownOffset - 1;
            UpdateRoomNames();
        }
    }

    // ----------- DAY BUTTONS -----------

    private void NextDay()
    {
        DateHelper dh = new DateHelper();

        datesShownList.RemoveAt(0);
        datesShownList.Add(dh.GetNextDay(datesShownList[datesShownList.Count - 1]));

        BeginRoomBookingUpdate();

        //Debug.Log("Next Day");
    }

    private void PrevDay()
    {
        DateHelper dh = new DateHelper();

        datesShownList.RemoveAt(datesShownList.Count - 1);
        datesShownList.Insert(0, dh.GetPreviousDay(datesShownList[0]));

        BeginRoomBookingUpdate();

        //Debug.Log("Prev Day");
    }

    // Which rooms are currently displayed at the top
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

    // Update rooms displayed
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

    // Opens the screen to make or view a booking
    private void OpenBookingWindow(BookingElement booking)
    {
        // TEST USER
        string currentUser = "UnityTestUser";

        currentBooker = new Booking(
            booking.bookingInfo._startingTime,
            booking.bookingInfo._date,
            currentUser,
            booking.bookingInfo._roomID);

        bookingWindow.SetActive(true);

        if (booking.booked)
        {
            if (booking.bookingInfo._bookerName == currentUser)
            {
                bwCancel.gameObject.SetActive(true);
                bwBook.gameObject.SetActive(false);
            } else
            {
                bwCancel.gameObject.SetActive(false);
                bwBook.gameObject.SetActive(false);
            }
        } 
        else
        {
            bwCancel.gameObject.SetActive(false);
            bwBook.gameObject.SetActive(true);
        }

        BookingWindow bw = bookingWindow.GetComponent<BookingWindow>();

        int startTime = booking.bookingInfo._startingTime;
        int endTime = startTime + 1;

        DateHelper dh = new DateHelper(booking.bookingInfo._date);

        bw.roomName.text = roomsList[selectedRoom].roomName;
        bw.bookingTime.text = startTime.ToString() + ":00 - " + endTime.ToString() + ":00";
        bw.bookingDate.text = dh.GetNormalDateFormat();
        bw.bookingCost.text = roomsList[selectedRoom].creditPerHour.ToString() + " credits";
        bw.roomSize.text = roomsList[selectedRoom].size.ToString() + "m≤";
        bw.bookerName.text = booking.bookingInfo._bookerName;
        bw.roomImage.sprite = roomsList[selectedRoom].roomPicture;
    }

    // ----------- BOOKING WINDOW BUTTONS -----------

    private void BookRoom()
    {
        // Aika, p‰iv‰m‰‰r‰, varaajan nimi, huoneen ID
        //bookingsListPerRoom.Add(new Booking(currentBooker._startingTime, currentBooker._date, currentBooker._bookerName, currentBooker._roomID));

        Client.Instance.BeginRequest_MakeRoomBooking(
            roomNameMapping_database[currentBooker._roomID], 
            currentBooker._startingTime, 
            currentBooker._date, 
            currentBooker._bookerName, 
            CloseBookingWindow
        );
    }
    private void CancelBooking()
    {
        Client.Instance.BeginRequest_CancelRoomBooking(
            roomNameMapping_database[currentBooker._roomID],
            currentBooker._startingTime,
            currentBooker._date,
            currentBooker._bookerName,
            CloseBookingWindow
        );
    }

    void CloseBookingWindow(string serverResponse)
    {
        BeginRoomBookingUpdate();
        bookingWindow.SetActive(false);
    }

    private void CloseBooking()
    {
        BeginRoomBookingUpdate();
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
        //Debug.Log("Clicked button " + i);
        BeginRoomBookingUpdate();
    }

    // Show the first 5 dates starting from today
    private void InitDates()
    {
        DateHelper dh = new DateHelper();
        string currentDay = dh.GetDateString();

        for (int i = 0; i < panelAmount; i++)
        {
            datesShownList.Add(currentDay);

            currentDay = dh.GetNextDay(currentDay);
        }
    }
    


    private void UpdateBookingsFromDatabase(string serverResponse)
    {
        // Remember to clear the prev list
        bookingsListPerRoom.Clear();

        string json = "{\"bookings\": " + serverResponse + "}";
        fetchedBookings = new TempBookingsContainer();
        try
        {
            fetchedBookings = JsonUtility.FromJson<TempBookingsContainer>(json);

            if (fetchedBookings.bookings != null)
            {
                if (fetchedBookings.bookings.Length > 0)
                {
                    // Make the actual booking to the calendar obj
                    foreach(TempBookingData tbd in fetchedBookings.bookings)
                        bookingsListPerRoom.Add(new Booking(tbd.startTime, tbd.date, tbd.bookerName, selectedRoom));
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("No bookings found for requested room");
        }
        
        ShowBookings_LOCAL_VISUAL();
        bookingUpdateInProgress = false;
    }

    // ----------- INITIALIZE -----------

    public void InitializeCalendar(int roomID)
    {
        selectedRoom = roomID;
        roomsShownOffset = selectedRoom > roomsList.Length - panelAmount ? roomsList.Length - panelAmount : selectedRoom;

        // TEST BOOKINGS
        /*MakeTestBookings();

        InitDates();
        ShowBookings();*/

        InitDates();
        BeginRoomBookingUpdate();
    }

    // Begins the attempt to fetch room bookings from the database (DOESN'T IMMEDIATELY DO ANY LOCAL CHANGES)
    void BeginRoomBookingUpdate()
    {
        bookingUpdateInProgress = true;
        Client.Instance.BeginRequest_GetRoomBookings(roomNameMapping_database[selectedRoom], UpdateBookingsFromDatabase);
    }
}

public class Booking
{
    public int _startingTime;
    public string _date;
    public string _bookerName;
    public int _roomID;

    public Booking(int startTime, string date, string bookerName, int roomID)
    {
        _startingTime = startTime;
        _date = date;
        _bookerName = bookerName;
        _roomID = roomID;
    }
}