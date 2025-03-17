using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using JetBrains.Annotations;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Camera cam;

    [SerializeField]
    private Disc discBlackUp;

    [SerializeField]
    private Disc discWhiteUp;

    [SerializeField]
    private GameObject highlightPrefab;

    [SerializeField]
    private UIManager uiManager;

    private Dictionary<Player, Disc> discPrefabs = new Dictionary<Player, Disc>();
    private GameState gameState = new GameState();
    private const int COLS = 8;
    private const int ROWS = 8;
    private Disc[,] discs = new Disc[COLS, ROWS];
    private List<GameObject> highlights = new();


    // Start is called before the first frame update
    private void Start()
    {
        discPrefabs[Player.Black] = discBlackUp;
        discPrefabs[Player.White] = discWhiteUp;

        AddStartDiscs();
        ShowLegalMoves();
        uiManager.SetPlayerText(gameState.CurrentPlayer);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                Vector3 impact = hitInfo.point;
                Position boardPosition = SceneToBoardPos(impact);
                OnBoardClicked(boardPosition);
            }
        }
    }

    private void ShowLegalMoves()
    {
        foreach (Position boardPos in  gameState.LegalMoves.Keys)
        {
            Vector3 scenePos = BoardToScenePos(boardPos);
            GameObject highlight = Instantiate(highlightPrefab, scenePos, Quaternion.identity);
            highlights.Add(highlight);
        }
    }

    private void HideLegalMoves()
    {
        highlights.ForEach(Destroy);
        highlights.Clear();
    }

    private void OnBoardClicked(Position boardPos)
    {
        if (gameState.MakeMove(boardPos, out MoveInfo moveInfo))
        {
            StartCoroutine(OnMoveMade(moveInfo));
        }
    }

    private IEnumerator OnMoveMade(MoveInfo moveInfo)
    {
        HideLegalMoves();
        yield return ShowMove(moveInfo);
        yield return ShowTurnOutcome(moveInfo);
        ShowLegalMoves();
    }

    private Position SceneToBoardPos(Vector3 scenePos)
    {
        // タップした位置をボードのPositionに変換する
        int col = (int)(scenePos.x - 0.25f);  // オセロ盤の各マスのサイズが1.0fのためx方向の調整は0.25fになる
        int row = (int)(scenePos.z - 0.25f);  // オセロ盤の各マスのサイズが1.0fのためz方向の調整は0.25fになる

        return new Position(col, row);
    }

    private Vector3 BoardToScenePos(Position boardPos)
    {
        float x = boardPos.Col + 0.75f;  // オセロ盤の各マスのサイズが1.0fのためx方向の調整は0.75fになる
        float z = boardPos.Row + 0.75f;  // オセロ盤の各マスのサイズが1.0fのためz方向の調整は0.75fになる
        return new Vector3(x, 0, z);
    }

    private void SpawnDisc(Disc prefab, Position boardPos)
    {
        Vector3 scenePos = BoardToScenePos(boardPos);
        discs[boardPos.Col, boardPos.Row] = Instantiate(prefab, scenePos, Quaternion.identity);
    }

    private void AddStartDiscs()
    {
        foreach (Position boardPos in gameState.OccupiedPositions())
        {
            Player player = gameState.Board[boardPos.Col, boardPos.Row];
            SpawnDisc(discPrefabs[player], boardPos);
        }
    }

    private void FlipDiscs(List<Position> positions)
    {
        foreach (Position boardPos in positions)
        {
            discs[boardPos.Col, boardPos.Row].Flip();
        }
    }

    private IEnumerator ShowMove(MoveInfo moveInfo)
    {
        SpawnDisc(discPrefabs[moveInfo.Player], moveInfo.Position);
        yield return new WaitForSeconds(0.33f);  // 駒の生成アニメーションが終わるのを待つ時間
        FlipDiscs(moveInfo.Outflanked);
        yield return new WaitForSeconds(0.83f);  // 駒をひっくり返すアニメーションの終了を待つ処理
    }

    private IEnumerator ShowTurnSkipped(Player skippedPlayer)
    {
        uiManager.SetSkippedPlayer(skippedPlayer);
        yield return uiManager.AnimateTopText();
    }

    private IEnumerator ShowGameOver()
    {
        uiManager.SetTopText("Neither player can play!");
        yield return uiManager.AnimateTopText();

        yield return uiManager.ShowScoreText();
        yield return new WaitForSeconds(0.5f);

        yield return ShowCounting();
    }

    private IEnumerator ShowTurnOutcome(MoveInfo moveInfo)
    {
        if (gameState.GameOver)
        {
            ShowGameOver();
            yield break;
        }

        if (gameState.CurrentPlayer == moveInfo.Player)
        {
            yield return ShowTurnSkipped(gameState.CurrentPlayer.Opponent());
        }

        uiManager.SetPlayerText(gameState.CurrentPlayer);
    }

    private IEnumerator ShowCounting()
    {
        int blackCount = 0; int whiteCount = 0;

        foreach (Position pos in gameState.OccupiedPositions())
        {
            Player player = gameState.Board[pos.Col, pos.Row];

            if (player == Player.Black)
            {
                blackCount++;
                uiManager.SetBlackScoreText(blackCount);
            }
            else if (player == Player.White)
            {
                whiteCount++;
                uiManager.SetWhiteScoreText(whiteCount);
            }

            discs[pos.Col, pos.Row].Twitch();
            yield return new WaitForSeconds(0.05f);
        }
    }
}
