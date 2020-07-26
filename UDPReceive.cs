/*  
    -----------------------
    UDP-Receive
    -----------------------
    // Code adapted from: https://forum.unity.com/threads/simple-udp-implementation-send-read-via-mono-c.15900/
   
*/

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;

public class UDPReceive : MonoBehaviour
{
    // Receiving Thread
    static Thread receiveThread;
    static bool isRunning;
 
    private static UdpClient client;
    private static int port = 8051;



    // Received Message
    private static string receivedData = "";
    internal static bool MessageReceived = false;

    internal static float maxDisplacement;
    internal static List<Vector3> displacements = new List<Vector3>();

    


    // Starts receiving thread when Unity starts
    void Start()
    {
        // Create a new thread to read incoming UDP messages.
        if (receiveThread == null) 
        { 
            receiveThread = new Thread(new ThreadStart(ReceiveData));
            receiveThread.IsBackground = true;
            receiveThread.Start();
            isRunning = true;
        }
    }


    // Receive Data via UDP  
    private void ReceiveData()
    {
        if (client == null) client = new UdpClient(port);

        while (isRunning)
        {
            try
            {
                // Bytes received
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);

                // Decode bytes into a string with the UTF8 encoding
                string stringData = Encoding.UTF8.GetString(data);

                // Update the received data
                receivedData = stringData;

                // Decode the received data
                DecodeMessage(receivedData);

                // Triggers the MessageReceived event to
                MessageReceived = true;
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }


    // Decodes the Message received from Grasshopper
    private void DecodeMessage(string msg)
    {
        maxDisplacement = float.Parse(msg);

        //string[] data = msg.Split(',');
        //displacements.Clear());

        //for (int i = 0; i < data.Length; i += 3)
        //{
        //    Vector3 displacement = new Vector3(float.Parse(data[i]), float.Parse(data[i + 1]), float.Parse(data[i + 2]));
        //    displacements.Add(displacement);
        //}
    }


    // Closes the UDP connection and aborts the receiving thread when application is closed
    void OnApplicationQuit()
    {
        isRunning = false;

        if (receiveThread.IsAlive)
            receiveThread.Abort(); // Does NOT WORK

        client.Close();
    }
}
