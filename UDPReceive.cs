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
using UnityEngine;

public class UDPReceive : MonoBehaviour
{
    // Receiving Thread
    Thread receiveThread;
 
    UdpClient client;
    private int port = 8051;


    // Received Message
    private string receivedData = "";
    private event EventHandler MessageReceived;




    // Starts receiving thread when Unity starts
    void Start()
    {
        // Create a new thread to read incoming UDP messages.
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();

        // Decodes the message when Message Received event is triggered
        MessageReceived += new EventHandler((s, e) => DecodeMessage(s, e, receivedData));
    }



    // Closes the UDP connection and aborts the receiving thread when application is closed
    void OnApplicationQuit()
    {
        if (receiveThread.IsAlive)
            receiveThread.Abort();
        
        client.Close();
    }



    // Receive Data via UDP  
    private void ReceiveData()
    {
        client = new UdpClient(port);

        while (true)
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
                //print("Message Received: " + receivedData);

                // Triggers the MessageReceived event to decode the message
                OnMessageReceived(EventArgs.Empty);
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }


    // Trigger the MessageReceived event, which in turn triggers the DecodeMessage method
    protected virtual void OnMessageReceived(EventArgs e)
    {
        MessageReceived?.Invoke(this, e);
    }


    // Decode the string Message received from Grasshopper (just coordinates for now)
    private static void DecodeMessage(object sender, EventArgs e, string msg)
    {
        string[] data = msg.Split(',');
            
        Vector3 center = new Vector3(float.Parse(data[0]), float.Parse(data[1]), float.Parse(data[2]));

        print(center);
    }
}
