using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ServerSwitch : MonoBehaviour
{
    public bool isServer = false;
    private NetworkManager nwm;
    // Start is called before the first frame update
    void Start()
    {
        nwm = GetComponent<NetworkManager>();
        if (isServer)
        {
            nwm.StartServer();
        }
        else
        {
            nwm.StartClient();
        }
    }

    // Update is called once per frame
    void Update() { }
}
