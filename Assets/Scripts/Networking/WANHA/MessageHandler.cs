/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageHandler
{

    public void ProcessMessage(Packet packet)
    {
        byte messageType = packet.GetType();

        switch (messageType)
        {
            case NetworkingCommon.MESSAGE_signUpConfirmSuccess:
                SignUpConfirmSuccess(packet);
                break;
            default:
                break;
        }
    }

    void SignUpConfirmSuccess(Packet packet)
    {
        // Just for testing atm!
        int clientID = packet.GetClientID();

        byte[] data = packet.GetData();

        string str = System.Text.Encoding.UTF8.GetString(data, 0, data.Length);
        Debug.Log("server >> " + str);
    }

    void ConnectionSuccess(Packet packet)
    {
    }
}
*/