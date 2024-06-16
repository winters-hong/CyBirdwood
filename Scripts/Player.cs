using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public event System.Action<byte> OnPlayerNumberChanged;

    static public readonly List<Player> playerList = new List<Player>();
    public GameController controller;
    public GameObject bubble;

    [SyncVar(hook = nameof(PlayerNumberChanged))]
    public byte playerNumber = 0;

    //This is called by the hook of playerNumber SyncVar above
    void PlayerNumberChanged(byte _, byte newPlayerNumber)
    {
        OnPlayerNumberChanged?.Invoke(newPlayerNumber);
    }

    private void FindStuffs()
    {
        controller = GameObject
            .FindGameObjectWithTag("GameController")
            .GetComponent<GameController>();
        bubble = GameObject.Find("Bubble");
    }

    #region Server
    public override void OnStartServer()
    {
        base.OnStartServer();
        playerList.Add(this);
    }

    public override void OnStopServer()
    {
        CancelInvoke();
        playerList.Remove(this);
    }

    [ServerCallback]
    internal static void ResetPlayerNumbers()
    {
        byte playerNumber = 0;
        foreach (Player player in playerList)
        {
            player.playerNumber = playerNumber++;
        }
    }
    #endregion

    #region Client
    public override void OnStartClient()
    {
        Debug.Log("OnStartClient");

        OnPlayerNumberChanged = this.OnPlayerNumberChanged_Func;
        OnPlayerNumberChanged.Invoke(playerNumber);
    }

    public override void OnStartLocalPlayer()
    {
        Debug.Log("OnStartLocalPlayer");
    }

    public override void OnStopClient()
    {
        OnPlayerNumberChanged = null;
        Debug.Log("Client stopped");
    }
    #endregion

    void HandleRotation()
    {
        if (isLocalPlayer)
        {
            float rotateHorizontal = Input.GetAxis("Horizontal");
            float rotateVertical = Input.GetAxis("Vertical");
            bubble.transform.Rotate(new Vector3(5f * rotateVertical, 5f * rotateHorizontal, 0f));
        }
    }
    void Start()
    {
        FindStuffs();
        if (controller == null)
            Debug.Log("Controller is NULL!");
    }

    void Update()
    {
        //HandleRotation();
        if (isLocalPlayer)
            PlayerRayDetection();
    }

    public void PlayerRayDetection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isLocalPlayer)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    Debug.Log(hit.point);
                    CmdBubbleHit(hit.point);
                }
                else
                {
                    Debug.Log("No out ray");
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.F) && isLocalPlayer)
        {
            CmdBubbleDissolve();
        }
    }

    [Command]
    public void CmdBubbleHit(Vector3 hitPos)
    {
        RpcBubbleHit(hitPos);
    }

    [ClientRpc]
    public void RpcBubbleHit(Vector3 hitPos)
    {
        bubble.GetComponent<Shield>().HitShield(hitPos);
    }

    [Command]
    public void CmdBubbleDissolve()
    {
        RpcBubbleDissolve();
    }

    [ClientRpc]
    public void RpcBubbleDissolve()
    {
        bubble.GetComponent<Shield>().OpenCloseShield();
    }

    [Command]
    public void CmdChangeMode()
    {
        Debug.Log("Cmd mode change");
        RpcChangeMode();
    }

    [ClientRpc]
    public void RpcChangeMode()
    {
        Debug.Log("Rpc mode change");
        controller.ChangeMode_Controller();
    }

    public void OnPlayerNumberChanged_Func(byte newPlayerNumber)
    {
        //playerNameText.text = string.Format("Player {0:00}", newPlayerNumber);
    }
}
