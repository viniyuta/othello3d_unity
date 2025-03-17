using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI topText;

    [SerializeField]
    private TextMeshProUGUI blackScoreText;

    [SerializeField]
    private TextMeshProUGUI whiteScoreText;

    [SerializeField]
    private TextMeshProUGUI winnerText;

    [SerializeField]
    private Image blackOverlay;

    [SerializeField]
    private RectTransform playAgainButton;

    public void SetPlayerText(Player currentPlayer)
    {
        if (currentPlayer == Player.Black)
        {
            topText.text = "Black's Turn   <sprite name=DiscBlackUp>";
        }
        else if (currentPlayer == Player.White)
        {
            topText.text = "White's Turn   <sprite name=DiscWhiteUp>";
        }
    }
}
