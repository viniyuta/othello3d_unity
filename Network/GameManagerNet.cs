using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerNet : NetworkBehaviour
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
    private UIManagerNet uiManager;

    private Dictionary<Player, Disc> discPrefabs = new Dictionary<Player, Disc>();
    private GameState gameState = new GameState();
    private static readonly int[] COLS = Board.COLS;
    private static readonly int[] ROWS = Board.ROWS;
    private List<Disc[,]> discs = new()
    {
        new Disc[COLS[0], ROWS[0]],
        new Disc[COLS[1], ROWS[1]], 
        new Disc[COLS[2], ROWS[2]]
    };
    private List<GameObject> highlights = new();
    private Player localPlayer;


    // Start is called before the first frame update
    private void Start()
    {
        discPrefabs[Player.Black] = discBlackUp;
        discPrefabs[Player.White] = discWhiteUp;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (NetworkManager.Singleton.LocalClientId == 0)
        {
            localPlayer = Player.Black;
        }
        else 
        {
            localPlayer = Player.White;
        }
        if (IsServer) AddStartDiscs();
        ShowLegalMoves();
        uiManager.SetPlayerText(gameState.CurrentPlayer);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                Vector3 impact = hitInfo.point;
                Position boardPosition = SceneToBoardPos(impact);
                OnBoardClicked(boardPosition, localPlayer);
            }
        }
    }

    private void ShowLegalMoves()
    {
        ShowLegalMovesRpc();
    }

    [Rpc(SendTo.Server)]
    private void ShowLegalMovesRpc()
    {
        foreach (Position boardPos in  gameState.LegalMoves.Keys)
        {
            Vector3 scenePos = BoardToScenePos(boardPos);
            Quaternion sceneRot = BoardToSceneRot(boardPos);
            GameObject highlight = Instantiate(highlightPrefab, scenePos, sceneRot);
            highlight.GetComponent<NetworkObject>().Spawn(true);
            highlights.Add(highlight);
        }
    }

    private void HideLegalMoves()
    {
        HideLegalMovesRpc();
    }

    [Rpc(SendTo.Server)]
    private void HideLegalMovesRpc()
    {
        highlights.ForEach(Destroy);
        foreach(GameObject highlight in highlights)
        {
            highlight.GetComponent<NetworkObject>().Despawn();
        }
        highlights.Clear();
    }

    private void OnBoardClicked(Position boardPos, Player localPlayer)
    {
        OnBoardClickedRpc(boardPos.Col, boardPos.Row, boardPos.Floor, localPlayer);
    }

    [Rpc(SendTo.Server)]
    private void OnBoardClickedRpc(int col, int row, int floor, Player localPlayer)
    {
        if (gameState.CurrentPlayer != localPlayer)
        {
            Debug.Log($"It's {gameState.CurrentPlayer}'s turn!");
            return;
        }

        Position boardPos = new (col, row, floor);
        if (gameState.MakeMove(boardPos, out MoveInfo moveInfo))
        {
            StartCoroutine(OnMoveMade(moveInfo));
        }
        else
        {
            Debug.Log("Invalid move!");
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
        int floor = GetFloor(scenePos.y);

        for (int prevFloor = 0; prevFloor < floor; prevFloor++)
        {
            int floorCol = Board.floorPositions[prevFloor].Col;
            int floorRow = Board.floorPositions[prevFloor].Row;
            col -= floorCol;
            row -= floorRow;
        }

        Debug.Log($"Clicked: ({col}, {row}, {floor})");
        return new Position(col, row, floor);
    }

    private int GetFloor(float y)
    {
        float floorAdjust = 0.016f;
        int floor = (int)(y - floorAdjust);
        if (y - floor > floorAdjust)
        {
            floor += 1;
        }
        return floor;
    }

    private Vector3 BoardToScenePos(Position boardPos)
    {
        float x = boardPos.Col + 0.75f;  // オセロ盤の各マスのサイズが1.0fのためx方向の調整は0.75fになる
        float y = boardPos.Floor;
        float z = boardPos.Row + 0.75f;  // オセロ盤の各マスのサイズが1.0fのためz方向の調整は0.75fになる

        for (int prevFloor = 0; prevFloor < boardPos.Floor; prevFloor++)  // ベースフロアに対する位置調整
        {
            int floorCol = Board.floorPositions[prevFloor].Col;
            int floorRow = Board.floorPositions[prevFloor].Row;
            x += floorCol;
            z += floorRow;
        }

        if (Board.IsFloorBorder(boardPos))  // 横向き駒
        {
            float adjustVertical = 0.51f;
            float adjustHorizontal = 0.47f;
            y -= adjustVertical;  // 横向きの駒の場合はマスの中央になるために半マス分位置を下げる

            int floorCols = COLS[boardPos.Floor] - 1;
            int floorRows = ROWS[boardPos.Floor] - 1;

            if (boardPos.Col == 0)         { x += adjustHorizontal; }
            if (boardPos.Col == floorCols) { x -= adjustHorizontal; }
            if (boardPos.Row == 0)         { z += adjustHorizontal; }
            if (boardPos.Row == floorRows) { z -= adjustHorizontal; }
        }

        return new Vector3(x, y, z);
    }

    private Quaternion BoardToSceneRot(Position boardPos)
    {
        if (Board.IsFloorBorder(boardPos))
        {
            int floorCols = COLS[boardPos.Floor] - 1;
            int floorRows = ROWS[boardPos.Floor] - 1;

            if (boardPos.Col == 0)         { return Quaternion.Euler(0.0f, 0.0f, 90.0f); }
            if (boardPos.Col == floorCols) { return Quaternion.Euler(0.0f, 0.0f, -90.0f); }
            if (boardPos.Row == 0)         { return Quaternion.Euler(-90.0f, 0.0f, 0.0f); }
            if (boardPos.Row == floorRows) { return Quaternion.Euler(90.0f, 0.0f, 0.0f); }
        }
        return Quaternion.identity;
    }

    private void SpawnDisc(Player player, Position boardPos)
    {   
        SpawnDiscRpc(boardPos.Col, boardPos.Row, boardPos.Floor, player);
    }

    [Rpc(SendTo.Server)]
    private void SpawnDiscRpc(int col, int row, int floor, Player player)
    {
        Position boardPos = new Position(col, row, floor);
        Vector3 scenePos = BoardToScenePos(boardPos);
        Quaternion sceneRot = BoardToSceneRot(boardPos);

        Disc disc = Instantiate(discPrefabs[player], scenePos, sceneRot);
        disc.GetComponent<NetworkObject>().Spawn(true);

        discs[floor][col, row] = disc;
    }

    private void AddStartDiscs()
    {
        foreach (Position boardPos in gameState.OccupiedPositions())
        {
            Player player = gameState.Board[boardPos];
            SpawnDisc(player, boardPos);
        }
    }

    private void FlipDiscs(List<Position> positions)
    {
        foreach (Position boardPos in positions)
        {
            discs[boardPos.Floor][boardPos.Col, boardPos.Row].Flip();
        }
    }

    private IEnumerator ShowMove(MoveInfo moveInfo)
    {
        SpawnDisc(moveInfo.Player, moveInfo.Position);
        yield return new WaitForSeconds(0.33f);  // 駒の生成アニメーションが終わるのを待つ時間
        FlipDiscs(moveInfo.Outflanked);
        yield return new WaitForSeconds(0.83f);  // 駒をひっくり返すアニメーションの終了を待つ処理
    }

    private IEnumerator ShowTurnSkipped(Player skippedPlayer)
    {
        uiManager.SetSkippedPlayer(skippedPlayer);
        yield return uiManager.AnimateTopText();
    }

    private IEnumerator ShowGameOver(Player winner, string topText="ゲームオーバー！")
    {
        uiManager.SetTopText(topText);
        yield return uiManager.AnimateTopText();

        uiManager.HidePlayUI();

        yield return uiManager.ShowScoreText();
        yield return new WaitForSeconds(0.5f);

        yield return ShowCounting();

        uiManager.SetWinnerText(winner);
        yield return uiManager.ShowEndScreen();
    }

    private IEnumerator ShowTurnOutcome(MoveInfo moveInfo)
    {
        if (gameState.GameOver)
        {
            yield return ShowGameOver(gameState.Winner);
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
            Player player = gameState.Board[pos];

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

            discs[pos.Floor][pos.Col, pos.Row].Twitch();
            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator RestartGame()
    {
        yield return uiManager.HideEndScreen();
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }

    private void GoToInitialScene()
    {
        Scene introScene = SceneManager.GetSceneByName("HomeScene");
        SceneManager.LoadScene(introScene.name);
    }

    public void OnPlayAgainClicked()
    {
        StartCoroutine(RestartGame());
    }

    public void OnExitClicked()
    {
        Debug.Log("Exit Clicked");
        GoToInitialScene();
    }

    public void OnSurrenderClicked()
    {
        Debug.Log("Surrender clicked");
        StartCoroutine(uiManager.ShowSurrenderConfirmationScreen());
    }

    public void OnSurrenderConfimationClicked()
    {
        Player surrenderPlayer = gameState.CurrentPlayer;
        StartCoroutine(uiManager.HideSurrenderConfirmationScreen());
        StartCoroutine(ShowGameOver(surrenderPlayer.Opponent()));
    }

    public void OnCancelSurrenderClicked()
    {
        StartCoroutine(uiManager.HideSurrenderConfirmationScreen());
    }
}

