using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkButtons : MonoBehaviour
{

    [SerializeField] UIManagerNet uiManager;

    public void OnHostClicked()
    {
        NetworkManager.Singleton.StartHost();
        uiManager.HideNetworkButtons();
    }

    public void OnClientClicked()
    {
        NetworkManager.Singleton.StartClient();
        uiManager.HideNetworkButtons();
    }
}
