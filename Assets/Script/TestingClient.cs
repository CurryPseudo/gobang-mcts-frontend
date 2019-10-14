using UnityEngine;
using UnityEngine.UI;

public class TestingClient : MonoBehaviour
{
    public Button TestButton;
    public InputField TestInput;
    // Start is called before the first frame update
    void Start()
    {
        TcpClientProxy client = GetComponent<TcpClientProxy>();
        TestButton.onClick.AddListener(() =>
        {
            client?.Send(TestInput.text);
            TestInput.text = "";
        });
        client?.ReceiveEvent.AddListener((s) =>
        {
            Debug.Log(s);
        });

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
