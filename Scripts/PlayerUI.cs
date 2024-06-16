using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("Player Components")]
    public Image image;

    [Header("Child Text Objects")]
    public Text playerNameText;

    // Sets a highlight color for the local player
    public void SetLocalPlayer()
    {
        // add a visual background for the local player in the UI
        image.color = new Color(1f, 1f, 1f, 0.1f);
    }

    // This value can change as clients leave and join
    public void OnPlayerNumberChanged(byte newPlayerNumber)
    {
        playerNameText.text = string.Format("Player {0:00}", newPlayerNumber);
    }
}
