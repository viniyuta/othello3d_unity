using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManagerNet : MonoBehaviour
{
    // ボタン
    [SerializeField] private RectTransform playAgainButton;

    [SerializeField] private RectTransform exitButton;

    [SerializeField] private RectTransform surrenderButton;

    [SerializeField] private RectTransform surrenderConfirmationButton;

    [SerializeField] private RectTransform cancelSurrenderButton;

    [SerializeField] private RectTransform hostButton;

    [SerializeField] private RectTransform clientButton;


    // テキスト
    [SerializeField] private TextMeshProUGUI topText;

    [SerializeField] private TextMeshProUGUI blackScoreText;

    [SerializeField] private TextMeshProUGUI whiteScoreText;

    [SerializeField] private TextMeshProUGUI winnerText;

    [SerializeField] private TextMeshProUGUI surrenderConfirmationText;


    // 背景・オーバーレイ
    [SerializeField] private Image blackOverlay;

    [SerializeField] private Image moveCameraOverlay;

    [SerializeField] private Image header;


    public void SetPlayerText(Player currentPlayer)
    {
        if (currentPlayer == Player.Black)
        {
            topText.text = "黒プレイヤーの番  <sprite name=DiscBlackUp>";
        }
        else if (currentPlayer == Player.White)
        {
            topText.text = "白プレイヤーの番   <sprite name=DiscWhiteUp>";
        }
    }

    public void SetSkippedPlayer(Player skippedPlayer)
    {
        if (skippedPlayer == Player.Black)
        {
            topText.text = "パス  <sprite name=DiscBlackUp>";
        }

        else if (skippedPlayer == Player.White)
        {
            topText.text = "パス  <sprite name=DiscWhiteUp>";
        }
    }

    public void SetTopText(string message)
    {
        topText.text = message;
    }

    public IEnumerator AnimateTopText()
    {
        topText.transform.LeanScale(Vector3.one * 1.05f, 0.25f).setLoopPingPong(2);
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

    private IEnumerator ShowOverlay()
    {
        blackOverlay.gameObject.SetActive(true);
        moveCameraOverlay.gameObject.SetActive(false);
        blackOverlay.color = Color.clear;
        blackOverlay.rectTransform.LeanAlpha(0.8f, 1.0f);
        yield return new WaitForSeconds(1.0f);
    }

    private IEnumerator HideOverlay()
    {
        blackOverlay.rectTransform.LeanAlpha(0, 1.0f);
        yield return new WaitForSeconds(1.0f);
        blackOverlay.gameObject.SetActive(false);
        moveCameraOverlay.gameObject.SetActive(true);
    }

    private IEnumerator MoveScoresDown()
    {
        blackScoreText.rectTransform.LeanMoveY(0, 0.5f);
        whiteScoreText.rectTransform.LeanMoveY(0, 0.5f);
        yield return new WaitForSeconds(0.5f);
    }

    public void SetWinnerText(Player winner)
    {
        winnerText.text = winner switch
        {
            Player.Black => "黒プレイヤーの勝ち！",
            Player.White => "白プレイヤーの勝ち！",
            _ => "引き分け！",
        };
    }

    public IEnumerator ShowEndScreen()
    {
        yield return ShowOverlay();
        yield return MoveScoresDown();
        yield return ScaleUp(winnerText.rectTransform);
        yield return ScaleUp(playAgainButton);
        yield return ScaleUp(exitButton);
    }

    public IEnumerator HideEndScreen()
    {
        ShowPlayUI();
        StartCoroutine(ScaleDown(winnerText.rectTransform));
        StartCoroutine(ScaleDown(blackScoreText.rectTransform));
        StartCoroutine(ScaleDown(whiteScoreText.rectTransform));
        StartCoroutine(ScaleDown(playAgainButton));
        StartCoroutine(ScaleDown(exitButton));

        yield return new WaitForSeconds(0.5f);
        yield return HideOverlay();
    }

    public IEnumerator ShowSurrenderConfirmationScreen()
    {
        HidePlayUI();
        yield return ShowOverlay();
        yield return ScaleUp(surrenderConfirmationText.rectTransform);
        yield return ScaleUp(surrenderConfirmationButton);
        yield return ScaleUp(cancelSurrenderButton);
    }

    public IEnumerator HideSurrenderConfirmationScreen()
    {
        ShowPlayUI();
        StartCoroutine(ScaleDown(surrenderConfirmationText.rectTransform));
        StartCoroutine(ScaleDown(surrenderConfirmationButton));
        StartCoroutine(ScaleDown(cancelSurrenderButton));

        yield return new WaitForSeconds(0.5f);
        yield return HideOverlay();
    }

    public void HidePlayUI()
    {
        header.gameObject.SetActive(false);
        surrenderButton.gameObject.SetActive(false);
    }

    private void ShowPlayUI()
    {
        header.gameObject.SetActive(true);
        surrenderButton.gameObject.SetActive(true);
    }

    public void HideNetworkButtons()
    {
        hostButton.gameObject.SetActive(false);
        clientButton.gameObject.SetActive(false);
        blackOverlay.gameObject.SetActive(false);
        ShowPlayUI();
    }
}