﻿using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

class MyTcpListener
{
  public delegate void Client();

static TcpListener server = null;
  static ManualResetEvent re = new ManualResetEvent(true);
  public static void ReadStream()
  {
      Byte[] bytes = new Byte[256];
      String data = null;
        Console.Write("Waiting for a connection... ");
        

        // Perform a blocking call to accept requests.
        // You could also use server.AcceptSocket() here.
        TcpClient client = server.AcceptTcpClient();
        re.Set();
        Console.WriteLine("Connected!");

        data = null;

        // Get a stream object for reading and writing
        NetworkStream stream = client.GetStream();

        int i;

        // Loop to receive all the data sent by the client.
        while((i = stream.Read(bytes, 0, bytes.Length))!=0)
        {
          // Translate data bytes to a ASCII string.
          data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
          Console.WriteLine("Received: {0}", data);

          // Process the data sent by the client.
          data = data.ToUpper();
        //   Console.WriteLine(data);
          if (data == "Q\r\n") 
          {
              goto CLOSE;
          }
          if (data == "A" || data == "A\r\n" || data == "A\n")
          {
              data = "You've sent \"A\"";
          }
        //   client.Close();

          byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

          // Send back a response.
          stream.Write(msg, 0, msg.Length);
          Console.WriteLine("Sent: {0}", data);
        }

        // Shutdown and end connection
    CLOSE:
        client.Close();
        return;
  }
  public static void Main()
  {
    // TcpListener server=null;
    try
    {
      // Set the TcpListener on port 13000.
      Int32 port = 13000;
      IPAddress localAddr = IPAddress.Parse("127.0.0.1");

      // TcpListener server = new TcpListener(port);
      server = new TcpListener(localAddr, port);

      // Start listening for client requests.
      server.Start();

      // Buffer for reading data


      // Enter the listening loop.
      Client curClient = new Client(ReadStream);
      while(true)
      {
        curClient = new Client(ReadStream);
        re.WaitOne();
        Task task = new Task(()=>curClient.Invoke());
        re.Reset();
        task.Start();
      }
    }
    catch(SocketException e)
    {
      Console.WriteLine("SocketException: {0}", e);
    }
    finally
    {
       // Stop listening for new clients.
       server.Stop();
    }

    Console.WriteLine("\nHit enter to continue...");
    Console.Read();
  }
}