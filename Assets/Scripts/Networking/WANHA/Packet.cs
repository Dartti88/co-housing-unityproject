/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public class Packet
{
    public const int USER_DATA_maxUserNameLength =      16;
    public const int USER_DATA_maxDisplayNameLength =   16;
    public const int USER_DATA_maxPasswordLength =      16;
    
    public const int PACKET_DATA_headerLength = 9;
    
    // byte position of specific things common for all packets
    public const int PACKET_DATA_POS__type = 0;         // byte
    public const int PACKET_DATA_POS__clientID = 1;     // int (4 bytes) (1 to 4)
    public const int PACKET_DATA_POS__dataLength = 5;   // int (4 bytes) (5 to 8)
    public const int PACKET_DATA_POS__dataBegin = 9;    // "byte array" (9 to 9 + 'dataLength')
    
    public byte[] buffer;

    public Packet(byte[] buf)
    {
        buffer = new byte[buf.Length];
        buf.CopyTo(buffer, 0);
    }

    public byte GetType()
    {
        if (buffer.Length <= 0)
            Debug.Log("Attempted to access empty packet! -> Packet::getType()");

        return buffer[0];
    }

    public int GetClientID()
    {
        if (buffer.Length < 4)
        {
            Debug.Log("Attempted to access empty packet! -> Packet::getClientID()");
            return -1;
        }

        return BitConverter.ToInt32(buffer, PACKET_DATA_POS__clientID);
    }

    public int GetDataLength()
    {
        if (buffer.Length < 8)
        {
            Debug.Log("Attempted to access empty packet! -> Packet::getDataLength()");
            return -1;
        }

        return BitConverter.ToInt32(buffer, PACKET_DATA_POS__dataLength);
    }

    public byte[] GetData()
    {
        if (buffer.Length < 9)
            Debug.Log("Attempted to access empty packet! -> Packet::getData()");

        Debug.Log("buffer total length : " + buffer.Length.ToString());
        int dataLength = BitConverter.ToInt32(buffer, PACKET_DATA_POS__dataLength);
        
        byte[] data = new byte[dataLength];
        Array.Copy(buffer, PACKET_DATA_POS__dataBegin, data, 0, dataLength);
        
        return data;
    }


    public static Packet CreatePacket_requestSingUp(string username, string displayName, string password)
    {
        if (username.Length > USER_DATA_maxUserNameLength)
        {
            Debug.Log("Attempted to request sing up with too long username! (max length is " + USER_DATA_maxUserNameLength.ToString() +")");
            return new Packet(null);
        }
        if (displayName.Length > USER_DATA_maxDisplayNameLength)
        {
            Debug.Log("Attempted to request sing up with too long displayName! (max length is " + USER_DATA_maxDisplayNameLength.ToString() + ")");
            return new Packet(null);
        }
        if (password.Length > USER_DATA_maxPasswordLength)
        {
            Debug.Log("Attempted to request sing up with too long password! (max length is " + USER_DATA_maxPasswordLength.ToString() + ")");
            return new Packet(null);
        }

        int totalPacketSize = PACKET_DATA_headerLength + USER_DATA_maxUserNameLength + USER_DATA_maxDisplayNameLength + USER_DATA_maxPasswordLength;
        byte[] packetData = new byte[totalPacketSize];
        packetData[0] = NetworkingCommon.MESSAGE_signUpRequest;

        byte[] bytes_finalUsername =    new byte[USER_DATA_maxUserNameLength];
        byte[] bytes_finalDisplayName = new byte[USER_DATA_maxDisplayNameLength];
        byte[] bytes_finalPassword =    new byte[USER_DATA_maxPasswordLength];

        byte[] bytes_username =     Encoding.ASCII.GetBytes(username);
        byte[] bytes_displayName =  Encoding.ASCII.GetBytes(displayName);
        byte[] bytes_password =     Encoding.ASCII.GetBytes(password);

        bytes_username.CopyTo(bytes_finalUsername, 0);
        bytes_displayName.CopyTo(bytes_finalDisplayName, 0);
        bytes_password.CopyTo(bytes_finalPassword, 0);

        Array.Copy(bytes_finalUsername, 0, packetData, 1, USER_DATA_maxUserNameLength);
        Array.Copy(bytes_finalDisplayName, 0, packetData, 1 + USER_DATA_maxUserNameLength, USER_DATA_maxDisplayNameLength);
        Array.Copy(bytes_finalPassword, 0, packetData, 1 + USER_DATA_maxUserNameLength + USER_DATA_maxDisplayNameLength, USER_DATA_maxPasswordLength);

        return new Packet(packetData);
    }
}*/