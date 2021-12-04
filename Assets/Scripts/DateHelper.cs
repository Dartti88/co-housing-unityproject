using System;

public class DateHelper
{
    public int _day;
    public int _month;
    public int _year;

    private string _originalDateString;

    // Default constructor, uses current day to initialize
    public DateHelper()
    {
        DateTime defaultDate = DateTime.Now.Date;

        _year = defaultDate.Year;
        _month = defaultDate.Month;
        _day = defaultDate.Day;

        _originalDateString = GetDateString();
    }

    // dateString muodossa yyyy-mm-dd
    public DateHelper (string dateString)
    {
        _originalDateString = dateString;
        string[] bits = dateString.Split('-');
        _year = Int32.Parse(bits[0]);
        _month = Int32.Parse(bits[1]);
        _day = Int32.Parse(bits[2]);
    }

    // Palauttaa muodossa dd.mm.yyyy
    public string GetNormalDateFormat ()
    {
        return _day.ToString() + "." + _month.ToString() + "." + _year.ToString();
    }

    // Day name in English
    public string GetDayName()
    {
        return DateTime.Parse(GetDateString()).DayOfWeek.ToString();
    }

    // Short day name
    public string GetShortDayName()
    {
        return GetDayName().Substring(0, 3).ToUpper();
    }

    // Palauttaa muodossa yyyy-mm-dd
    public string GetDateString()
    {
        return _year.ToString() + "-" + _month.ToString() + "-" + _day.ToString();
    }

    // Get next day from parameter currentDay, returns yyyy-mm-dd
    public string GetNextDay(string currentDay)
    {
        return DateTime.Parse(currentDay).AddDays(1).ToString("yyyy-M-d");
    }

    // Get previous day from parameter currentDay, returns yyyy-mm-dd
    public string GetPreviousDay(string currentDay)
    {
        return DateTime.Parse(currentDay).AddDays(-1).ToString("yyyy-M-d");
    }

    public string GetSeasonName()
    {
        string seasonName = "";

        // 1.3. - 31.5. (maalis - touko)
        if (_month >= 3 && _month < 6)
        {
            seasonName = "spring";
        }
        // 1.6. - 31.8. (kesä - elo)
        else if (_month >= 6 && _month < 9)
        {
            seasonName = "summer";
        }
        // 1.9. - 30.11 (syys - marras)
        else if (_month >= 9 && _month < 12)
        {
            seasonName = "autumn";
        }
        // 1.12. - 28.2. (joulu - helmi)
        else if (_month >= 12 && _month < 3)
        {
            seasonName = "winter";
        }

        return seasonName;
    }

    // 0 = spring, 1 = summer, 2 = autumn, 3 = winter
    public int GetSeasonInt()
    {
        int seasonInt = -1;

        // 1.3. - 31.5. (maalis - touko)
        if (_month >= 3 && _month < 6)
        {
            seasonInt = 0;
        }
        // 1.6. - 31.8. (kesä - elo)
        else if (_month >= 6 && _month < 9)
        {
            seasonInt = 1;
        }
        // 1.9. - 30.11 (syys - marras)
        else if (_month >= 9 && _month < 12)
        {
            seasonInt = 2;
        }
        // 1.12. - 28.2. (joulu - helmi)
        else if (_month >= 12 && _month < 3)
        {
            seasonInt = 3;
        }

        return seasonInt;
    }

    public string GetOriginalDateString()
    {
        return _originalDateString;
    }

    public string GetFullMonthName()
    {
        string monthName = "";

        switch (_month)
        {
            case 1:
                monthName = "January";
                break;
            case 2:
                monthName = "February";
                break;
            case 3:
                monthName = "March";
                break;
            case 4:
                monthName = "April";
                break;
            case 5:
                monthName = "May";
                break;
            case 6:
                monthName = "June";
                break;
            case 7:
                monthName = "July";
                break;
            case 8:
                monthName = "August";
                break;
            case 9:
                monthName = "September";
                break;
            case 10:
                monthName = "October";
                break;
            case 11:
                monthName = "November";
                break;
            case 12:
                monthName = "December";
                break;
        }

        return monthName;
    }

    public string GetShortMonthName()
    {
        string monthName = "";

        switch (_month)
        {
            case 1:
                monthName = "Jan";
                break;
            case 2:
                monthName = "Feb";
                break;
            case 3:
                monthName = "Mar";
                break;
            case 4:
                monthName = "Apr";
                break;
            case 5:
                monthName = "May";
                break;
            case 6:
                monthName = "June";
                break;
            case 7:
                monthName = "July";
                break;
            case 8:
                monthName = "Aug";
                break;
            case 9:
                monthName = "Sept";
                break;
            case 10:
                monthName = "Oct";
                break;
            case 11:
                monthName = "Nov";
                break;
            case 12:
                monthName = "Dec";
                break;
        }

        return monthName;
    }

}
