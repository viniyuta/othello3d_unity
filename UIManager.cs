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

    public void SetSkippedPlayer(Player skippedPlayer)
    {
        if (skippedPlayer == Player.Black)
        {
            topText.text = "SKIP!  <sprite name=DiscBlackUp>";
        }

        else if (skippedPlayer == Player.White)
        {
            topText.text = "SKIP!  <sprite name=DiscWhiteUp>";
        }
    }

    public IEnumerator AnimateTopText()
    {
        topText.transform.LeanScale(Vector3.one * 1.2f, 0.25f).setLoopPingPong(2);
        yield return new WaitForSeconds(1.0f);
    }
}
