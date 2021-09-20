/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;


//"The maximum safe UDP payload is <= 508 bytes"


public class Client : MonoBehaviour
{
    UdpClient udpClient;
    int localPort = 52000;

    IPEndPoint serverEndPoint; // server's endpoint
    string serverIP = "127.0.0.1";
    int serverPort = 54000;

    Thread receiveThread;

    int sessionID; // given us by the server when connection has been established

    MessageHandler messageHandler;

    // Start is called before the first frame update
    void Start()
    {
        udpClient = new UdpClient(localPort);
        //udpClient.Client.Blocking = false;
        //udpClient.Client.ReceiveTimeout = 1000;
        serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);

        messageHandler = new MessageHandler();

        receiveThread = new Thread(new ThreadStart(ReceiveFunc));
        receiveThread.IsBackground = true;
        receiveThread.Start();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RequestLogin(string username, string password)
    {

    }

    public void RequestSignUp(string username, string password)
    {

    }

    void ReceiveFunc()
    {
        udpClient.Connect(serverEndPoint);

        // Establish communications with this first message
        Packet requestConnectPacket = Packet.CreatePacket_requestSingUp("Tester", "Jorma666", "123");
        SendData(requestConnectPacket);

        while (true)
        {
            try
            {
                byte[] receivedBytes = udpClient.Receive(ref serverEndPoint);

                if (receivedBytes.Length < 1)
                {
                    // failed to receive packet?
                }
                else
                {
                    Packet packet = new Packet(receivedBytes);
                    messageHandler.ProcessMessage(packet);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }
    }


    void SendData(byte[] sendBuffer)
    {
        try
        {
            udpClient.Send(sendBuffer, sendBuffer.Length);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    void SendData(Packet packet)
    {
        try
        {
            udpClient.Send(packet.buffer, packet.buffer.Length);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    void SendData(string message)
    {
        try
        {
            byte[] sendBuffer = System.Text.Encoding.UTF8.GetBytes(message);
            udpClient.Send(sendBuffer, sendBuffer.Length);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    void OnApplicationQuit()
    {
        if (receiveThread.IsAlive)
            receiveThread.Abort();

        udpClient.Close();
    }
}
*/