using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using JetBrains.Annotations;

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

    public void SetTopText(string message)
    {
        topText.text = message;
    }

    public IEnumerator AnimateTopText()
    {
        topText.transform.LeanScale(Vector3.one * 1.2f, 0.25f).setLoopPingPong(2);
        yield return new WaitForSeconds(1.0f);
    }

    private IEnumerator ScaleDown(RectTransform rect)
    {
        rect.LeanScale(Vector3.zero, 0.2f);
        yield return new WaitForSeconds(0.2f);
        rect.gameObject.SetActive(false);
    }

    private IEnumerator ScaleUp(RectTransform rect)
    {
        rect.gameObject.SetActive(true);        
        rect.localScale = Vector3.zero;
        rect.LeanScale(Vector3.one, 0.2f);
        yield return new WaitForSeconds(0.2f);
    }

    public IEnumerator ShowScoreText()
    {
        yield return ScaleDown(topText.rectTransform);
        yield return ScaleUp(blackScoreText.rectTransform);
        yield return ScaleUp(whiteScoreText.rectTransform);
    }

    public void SetBlackScoreText(int score)
    {
        blackScoreText.text = $"<sprite name=DiscBlackUp>{score}";
    }

    public void SetWhiteScoreText(int score)
    {
        whiteScoreText.text = $"<sprite name=DiscWhiteUp>{score}";
    }
}
