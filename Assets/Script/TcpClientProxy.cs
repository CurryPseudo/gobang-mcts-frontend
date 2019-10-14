using System.Collections;
using System.IO;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TcpReceiveEvent : UnityEvent<string> { }
public class TcpClientProxy : MonoBehaviour
{
    private StreamWriter writer;
    private TcpClient client;
    private NetworkStream stream;
    public TcpReceiveEvent ReceiveEvent;
    private void Awake()
    {
        client = new TcpClient("localhost", 3333);
        stream = client.GetStream();
        writer = new StreamWriter(stream);
    }
    IEnumerator Main()
    {
        StreamReader reader = new StreamReader(stream);
        while(true)
        {
            if(stream.DataAvailable)
            {
                string s = reader.ReadLine();
                Debug.Log(s);
                ReceiveEvent.Invoke(s);
            }
            yield return null;
        }
    }
    public void Send(string s)
    {
        writer.Write(s);
        writer.Flush();
    }
    private void OnDestroy()
    {
        client.Close();
    }
}
