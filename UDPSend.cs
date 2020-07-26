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
    private static string IP = "127.0.0.1";  // default localhost  
    private static int port = 8050;
    private static UdpClient client;




    // Connect to the remote host when Unity starts
    void Start()
    {
        if (client == null) client = new UdpClient(IP, port);
    }


    // Closes the UDP connection when application is closed
    void OnApplicationQuit()
    {
        client.Close();
    }



    // Encode Message to Send: includes the coordinates of the center of the cells and cell size
    internal static string EncodeMessage(Vector3[] listPositions)
    {
        string msg = "";

        foreach (Vector3 position in listPositions)
        {
            string x = position.x.ToString();
            string y = position.z.ToString();  // Swap y with z for Rhino Coordinates
            string z = position.y.ToString();  //

            // Construct Message
            msg += $"{x},{y},{z},";
        }

        string size = Cell.CellSize.ToString();
        msg += size;

        return msg;
    }


    // Send Data via UDP
    internal static void SendData(string message)
    {
        try
        {
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
