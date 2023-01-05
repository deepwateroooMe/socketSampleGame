using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class NetManager : MonoBehaviour {

    static Socket socket;
    static byte[] readBuff = new byte[1024];

    public delegate void MsgListener(string str);
    private static Dictionary<string, MsgListener> listeners = new Dictionary<string, MsgListener>();
    static List<string> msgList = new List<string>();

    public static void AddListener(string msgName, MsgListener listener) {
        // listeners.Add(msgName, listener);
        listeners [msgName] = listener;
    }
    public static string Getdesc() {
        if (socket == null)
            return "";
        if (!socket.Connected)
            return "";
        return socket.LocalEndPoint.ToString();
    }
    public static void Connect(string ip, int Port) {
        // socket
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        // connect 
        socket.Connect(ip, Port);
        // BeginReceive 
        socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallBack, socket);
    }
    private static void ReceiveCallBack(IAsyncResult ar) {
        try {
            Socket socket = (Socket)ar.AsyncState;
            int count = socket.EndReceive(ar);
            string recvStr = System.Text.Encoding.Default.GetString(readBuff, 0, count);
            msgList.Add(recvStr);
            socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallBack, socket);
        }
        catch (Exception ex ) {
            Debug.Log("Socket ,接收失败" + ex);
        }
    }
    public static void Send(string SendStr) {
        if (socket == null)
            return;
        if (!socket.Connected)
            return;
        byte[] sendBytes = System.Text.Encoding.Default.GetBytes(SendStr);
        socket.Send(sendBytes);
    }

    public static void Update() {
        if (msgList.Count <= 0) {
            return;
        }
        string msgStr = msgList[0];
        msgList.RemoveAt(0);
        string[] split = msgStr.Split('|');
        string msgName = split[0];
        string msgArgs = split[1];
        foreach (var item in listeners) {
            Debug.Log(item.Key);
        }
        Debug.Log(msgName);
        if (listeners.ContainsKey(msgName)) {
            listeners[msgName](msgArgs);
        }
    }
}
