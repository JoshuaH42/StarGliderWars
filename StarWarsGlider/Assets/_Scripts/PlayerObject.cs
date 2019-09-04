using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using VSX.UniversalVehicleCombat;

public class PlayerObject : NetworkBehaviour {

    //public GameObject PlayerUnitPrefab;
    private GameObject playerLoadoutToActivate;

    private void Awake()
    {
        playerLoadoutToActivate = GameObject.FindGameObjectWithTag("playerLoadoutToActivate");
    }

    void Start () {
		if(isLocalPlayer == false)
        {
            return;
        }

        CmdActivatePlayerLoadout();
	}
	
	void Update () {
		
	}

    // Commands only get executed on the server.
    [Command]
    void CmdActivatePlayerLoadout()
    {
        playerLoadoutToActivate.transform.GetChild(0).gameObject.SetActive(true);
    }
}
