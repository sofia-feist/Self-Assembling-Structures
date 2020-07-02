/*
    -----------------------
    UDP-Send
    -----------------------
    // Code adapted from: https://forum.unity.com/threads/simple-udp-implementation-send-read-via-mono-c.15900/

*/

using System;
using System.Text;
using System.Net.Sockets;
using UnityEngine;

public class UDPSend : MonoBehaviour
{
    // Connection properties
    private string IP = "127.0.0.1";  // default localhost  
    private int port = 8050;
    UdpClient client;




    // Connect to the remote host when Unity starts
    void Start()
    {
        client = new UdpClient(IP, port);
    }


    // Closes the UDP connection when application is closed
    void OnApplicationQuit()
    {
        client.Close();
    }



    // Encode Message to Send: includes the coordinates of the center of the cells and cell size
    internal string EncodeMessage(Agent[] listAgents)
    {
        string msg = "";

        foreach (Agent agent in listAgents)
        {
            Vector3 center = agent.Cell.Center;
            string x = center.x.ToString();
            string y = center.z.ToString();  // Swap y with z for Rhino Coordinates
            string z = center.y.ToString();  //

            string size = agent.Cell.CellSize.ToString();

            // Construct Message
            msg += $"{x},{y},{z},{size},";
        }

        msg = msg.Remove(msg.Length - 1);
        return msg;
    }


    // Send Data via UDP
    internal void SendData(string message)
    {
        try
        {
            // Print status
            //print("Sending to " + IP + " : " + port);

            // Encodes data in binary format using the UTF8 encoding
            byte[] data = Encoding.UTF8.GetBytes(message);

            // Sends the data to the remote host
            client.Send(data, data.Length);
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }
}
