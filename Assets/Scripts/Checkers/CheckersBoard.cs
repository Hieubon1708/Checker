using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckersBoard : MonoBehaviour
{
    //khoi tao ban co
    public Vector3 boardOffset = new Vector3(-4f, 4f, 0f);
    public Vector3 pieceOffset = new Vector3(0.5f, -0.5f, 0f);
    public Piece[,] pieces = new Piece[10, 10];
    Piece selectedPiece;
    public GameObject board;
    public GameObject backgroundBlack;

    //khoi tao piece, turn nguoi choi
    public GameObject redPiece;
    public GameObject bluePiece;
    public GameObject emptyPrefab;
    public GamePlayer playerBlue;
    public GamePlayer playerRed;
    //  public GameObject rulesGame;
    public Sprite spriteKing;

    public RulesCheckers rules;

    // ket thuc, tam dung game
    public GameObject gameOverPanelRed;
    public GameObject gameOverPanelBlue;
    public GameObject gameQuit;

    // khoi tao vi tri can di tiep theo cua piece
    GameObject[] emptyPiece;

    //kiem tra game
    public bool isBlue;
    bool isBlueTurn;
    bool hasKilled;
    bool gameIsOver;

    // khoi tao list cac piece do
    List<Piece> pieceRedList = new List<Piece>();

    // khoi tao list cac piece xanh
    List<Piece> pieceBlueList = new List<Piece>();

    // khoi tao list piece co the an
    List<Piece> forcedPiece;

    // khoi tao list cac piece co the di chuyen
    public List<Piece> movePiece = new List<Piece>();

    // so piece con lai tren ban co
    public int numberPieceRed, numberPieceBlue;

    // toa do piece de di chuyen
    Vector2 mouseOver;
    Vector2 startDrag;
    Vector2 endDrag;

    // am thanh game
    [SerializeField]
    private AudioSource au_source;
    [SerializeField]
    private AudioClip selectClip, correctClip, winClip;

    private void Awake()
    {
        GererateBoard();
    }

    // Use this for initialization
    void Start()
    {
        if(PlayerPrefs.GetInt("CheckerTutorial") == 0)
        {
            backgroundBlack.SetActive(true);
            rules.gameObject.SetActive(true);
            rules.Show();
        }
        Time.timeScale = 1;
    }
    public void StartGame()
    {
        if (PlayerPrefs.GetInt("CheckerTutorial") == 0)
        {
            PlayerPrefs.SetInt("CheckerTutorial", 1);
            rules.Hide();
            DisableBackground();
        }
        gameOverPanelBlue.SetActive(false);
        gameOverPanelRed.SetActive(false);
        gameQuit.SetActive(false);
        isBlueTurn = true;
        SetPlayer(playerRed, playerBlue);
        forcedPiece = new List<Piece>();
        numberPieceRed = pieceRedList.Count;
        numberPieceBlue = pieceBlueList.Count;
    }

    // khoi tao ban co
    void GererateBoard()
    {
        for (int x = 0; x < 3; x++)
        {
            bool oddCol = (x % 2 == 0);
            for (int y = 0; y < 10; y += 2)
            {
                dropPiece(x, (oddCol) ? y : y + 1);
            }
        }
        for (int x = 9; x > 6; x--)
        {
            bool oddCol = (x % 2 == 0);
            for (int y = 0; y < 10; y += 2)
            {
                dropPiece(x, (oddCol) ? y : y + 1);
            }
        }
        board.transform.rotation = Quaternion.Euler(Vector3.forward * 90f);
    }
    void dropPiece(int x, int y)
    {
        bool isPieceBlue = (x < 3) ? true : false;
        GameObject go = Instantiate((isPieceBlue) ? bluePiece : redPiece) as GameObject;
        go.transform.SetParent(board.transform, false);
        Piece p = go.GetComponent<Piece>();
        pieces[x, y] = p;
        MovePiece(p, x, y);
        if (isPieceBlue)
        {
            go.name = "blue";
            pieceBlueList.Add(pieces[x, y]);
        }
        else
        {
            go.name = "red";
            pieceRedList.Add(pieces[x, y]);
        }

    }
    void MovePiece(Piece p, int x, int y)
    {
        p.transform.position = (Vector3.right * x) + (Vector3.down * y) + boardOffset + pieceOffset;
    }

    void DisableBackground()
    {
        backgroundBlack.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 0;
            gameQuit.SetActive(true);
        }
        if (gameIsOver)
            return;

        UpdateMouseOver();
        // Debug.Log(mouseOver);
        // if it is my turn
        if ((isBlue) ? isBlueTurn : !isBlueTurn)
        {
            int x = (int)mouseOver.x;
            int y = (int)mouseOver.y;

            if (Input.GetMouseButtonDown(0))
            {
                SelectPiece(x, y);
            }
        }
    }
    // tro lai game
    public void ButtonResume()
    {
        Time.timeScale = 1;
        gameQuit.SetActive(false);
    }
    //dung game
    public void ButtonPause()
    {
        Time.timeScale = 0;
        gameQuit.SetActive(true);
    }


    //lay toa do piece
    void UpdateMouseOver()
    {
        if (!Camera.main)
        {
            Debug.Log("Unable to find main camera");
            return;
        }

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, LayerMask.GetMask("Board")))
        {

            mouseOver.x = Mathf.Abs((int)(hit.point.x - boardOffset.x));
            mouseOver.y = Mathf.Abs((int)(hit.point.y - boardOffset.y));
        }

    }

    // piece duoc chon va vi tri piece co the di
    void SelectPiece(int x, int y)
    {
        if (x < 0 || x >= 10 || y < 0 || y >= 10)
            return;

        Piece p = pieces[x, y];
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit) && hit.collider.tag == "Piece")
        {
            au_source.PlayOneShot(selectClip);
            if (p != null && p.isBlue == isBlue)
            {
                selectedPiece = p;
                startDrag = mouseOver;
                if (emptyPiece == null)
                {
                    SetMovePiece(x, y);
                }
                if (emptyPiece != null)
                {
                    for (int i = 0; i < emptyPiece.Length; i++)
                    {
                        Destroy(emptyPiece[i]);
                    }
                    SetMovePiece(x, y);
                }
            }
            else
            {
                if (forcedPiece.Find(fp => fp == p) == null)
                    return;

                selectedPiece = p;
                startDrag = mouseOver;
                if (emptyPiece == null)
                {
                    SetMovePiece(x, y);
                }
                if (emptyPiece != null)
                {
                    for (int i = 0; i < emptyPiece.Length; i++)
                    {
                        Destroy(emptyPiece[i]);
                    }
                    SetMovePiece(x, y);
                }
            }

        }
        if (Physics.Raycast(ray, out hit) && hit.collider.tag == "Piece Empty")
        {
            TryMove((int)startDrag.x, (int)startDrag.y, x, y);

            for (int i = 0; i < emptyPiece.Length; i++)
            {
                Destroy(emptyPiece[i]);
            }
        }
    }

    // xu li vi tri piece 
    void SetMovePiece(int x, int y)
    {
        if (isBlue && pieces[x, y].name == "blue")
        {
            if (selectedPiece.isKing == false)
            {
                SetPieceBlue(x, y);
            }
            else SetPieceKing(x, y);
        }
        else if (!isBlue && pieces[x, y].name == "red")
        {
            if (selectedPiece.isKing == false)
            {
                SetPieceRed(x, y);
            }
            else SetPieceKing(x, y);
        }
        emptyPiece = GameObject.FindGameObjectsWithTag("Piece Empty");
    }

    // khoi tao vi tri piece king
    void SetPieceKing(int x, int y)
    {
        if (x >= 0 && x <= 8)
        {
            SetPieceBlue(x, y);
        }
        if (x <= 9 && x >= 1)
        {
            SetPieceRed(x, y);
        }

    }

    // khoi tao vi tri piece blue
    void SetPieceBlue(int x, int y)
    {

        if (y < 9 && pieces[x + 1, y + 1] == null)
        {
            GameObject g = Instantiate(emptyPrefab) as GameObject;
            g.transform.position = (Vector3.right * (x + 1)) + (Vector3.down * (y + 1)) + boardOffset + pieceOffset;
            g.transform.SetParent(board.transform, false);
        }
        else if (y <= 7 && x <= 7 && pieces[x + 1, y + 1] != null && pieces[x + 2, y + 2] == null && pieces[x, y].name != pieces[x + 1, y + 1].name)
        {
            GameObject g = Instantiate(emptyPrefab) as GameObject;
            g.transform.position = (Vector3.right * (x + 2)) + (Vector3.down * (y + 2)) + boardOffset + pieceOffset;
            g.transform.SetParent(board.transform, false);

        }
        if (y > 0 && pieces[x + 1, y - 1] == null)
        {
            GameObject g = Instantiate(emptyPrefab) as GameObject;
            g.transform.position = (Vector3.right * (x + 1)) + (Vector3.down * (y - 1)) + boardOffset + pieceOffset;
            g.transform.SetParent(board.transform, false);
        }
        else if (y >= 2 && x <= 7 && pieces[x + 1, y - 1] != null && pieces[x + 2, y - 2] == null && pieces[x, y].name != pieces[x + 1, y - 1].name)
        {
            GameObject g = Instantiate(emptyPrefab) as GameObject;
            g.transform.position = (Vector3.right * (x + 2)) + (Vector3.down * (y - 2)) + boardOffset + pieceOffset;
            g.transform.SetParent(board.transform, false);
        }


    }

    // khoi tao vi tri piece red
    void SetPieceRed(int x, int y)
    {
        if (y < 9 && pieces[x - 1, y + 1] == null)
        {
            GameObject g = Instantiate(emptyPrefab) as GameObject;
            g.transform.position = (Vector3.right * (x - 1)) + (Vector3.down * (y + 1)) + boardOffset + pieceOffset;
            g.transform.SetParent(board.transform, false);
        }
        else if (y <= 7 && x >= 2 && pieces[x - 1, y + 1] != null && pieces[x - 2, y + 2] == null && pieces[x, y].name != pieces[x - 1, y + 1].name)
        {
            GameObject g = Instantiate(emptyPrefab) as GameObject;
            g.transform.position = (Vector3.right * (x - 2)) + (Vector3.down * (y + 2)) + boardOffset + pieceOffset;
            g.transform.SetParent(board.transform, false);
        }

        if (y > 0 && pieces[x - 1, y - 1] == null)
        {
            GameObject g = Instantiate(emptyPrefab) as GameObject;
            g.transform.position = (Vector3.right * (x - 1)) + (Vector3.down * (y - 1)) + boardOffset + pieceOffset;
            g.transform.SetParent(board.transform, false);
        }
        else if (y >= 2 && x >= 2 && pieces[x - 1, y - 1] != null && pieces[x - 2, y - 2] == null && pieces[x, y].name != pieces[x - 1, y - 1].name)
        {
            GameObject g = Instantiate(emptyPrefab) as GameObject;
            g.transform.position = (Vector3.right * (x - 2)) + (Vector3.down * (y - 2)) + boardOffset + pieceOffset;
            g.transform.SetParent(board.transform, false);
        }
    }

    // xu li di chuyen pice va an piece doi phuong
    void TryMove(int x1, int y1, int x2, int y2)
    {
        forcedPiece = ScanForPossibleMove();
        startDrag = new Vector2(x1, y1);
        endDrag = new Vector2(x2, y2);
        selectedPiece = pieces[x1, y1];

        if (x2 < 0 || x2 >= 10 || y2 < 0 || y2 >= 10)
        {
            if (selectedPiece != null)
                MovePiece(selectedPiece, x1, y1);
            startDrag = Vector2.zero;
            selectedPiece = null;
            return;
        }
        if (selectedPiece != null)
        {
            if (startDrag == endDrag)
            {
                MovePiece(selectedPiece, x1, y1);
                startDrag = Vector2.zero;
                selectedPiece = null;
                return;
            }

            if (selectedPiece.ValidMove(pieces, x1, y1, x2, y2))
            {
                if (Mathf.Abs(x1 - x2) == 2)
                {
                    Piece p = pieces[(x1 + x2) / 2, (y1 + y2) / 2];
                    if (p != null)
                    {
                        pieces[(x1 + x2) / 2, (y1 + y2) / 2] = null;
                        if (p.name == "red")
                        {
                            pieceRedList.Remove(p);
                            numberPieceRed--;
                        }
                        else if (p.name == "blue")
                        {
                            pieceBlueList.Remove(p);
                            numberPieceBlue--;
                        }
                        au_source.PlayOneShot(correctClip);
                        Destroy(p.gameObject);
                        hasKilled = true;
                    }
                }
                // if move can killed, you have to move
                if (forcedPiece.Count != 0 && !hasKilled)
                {
                    MovePiece(selectedPiece, x1, y1);
                    startDrag = Vector2.zero;
                    selectedPiece = null;
                    return;
                }

                pieces[x2, y2] = selectedPiece;
                pieces[x1, y1] = null;
                MovePiece(selectedPiece, x2, y2);

                EndTurn();
            }
            else
            {
                MovePiece(selectedPiece, x1, y1);
                startDrag = Vector2.zero;
                selectedPiece = null;
                return;
            }
        }
    }

    // kiem tra cac truong hop cua game sau khi di chuyen
    void EndTurn()
    {
        int x = (int)endDrag.x;
        int y = (int)endDrag.y;

        if (selectedPiece != null)
        {
            if (selectedPiece.isBlue && !selectedPiece.isKing && x == 9)
            {
                selectedPiece.isKing = true;
                selectedPiece.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = spriteKing;
                //  Debug.Log("king blue");
            }
            if (!selectedPiece.isBlue && !selectedPiece.isKing && x == 0)
            {
                selectedPiece.isKing = true;
                selectedPiece.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = spriteKing;
                //   Debug.Log("king red");
            }
        }

        selectedPiece = null;
        startDrag = Vector2.zero;
        // neu co the an tiep, vay tiec tuc khi khong con an dc nua
        if (ScanForPossibleMove(selectedPiece, x, y).Count != 0 && hasKilled == true)
            return;

        isBlueTurn = !isBlueTurn;
        hasKilled = false;
        CheckVictory();
        if (!gameIsOver)
        {
            isBlue = !isBlue;
            if (isBlue)
                SetPlayer(playerRed, playerBlue);
            else
                SetPlayer(playerBlue, playerRed);
        }
    }

    // kiem tra dieu kien gameover
    void CheckVictory()
    {

        movePiece = ScanCanMovePieces();
        if (movePiece.Count == 0 || movePiece.Count == 0 || numberPieceBlue == 0 || numberPieceRed == 0)
        {
            au_source.PlayOneShot(winClip);
            StartCoroutine(Victory());
        }
    }

    // dua ra nguoi choi chien thang
    IEnumerator Victory()
    {
        yield return new WaitForSeconds(1.0f);
        if (numberPieceBlue > numberPieceRed)
        {
            gameOverPanelBlue.SetActive(true);
        }
        else if (numberPieceBlue < numberPieceRed)
        {
            gameOverPanelRed.SetActive(true);
        }
        gameIsOver = true;
    }

    // hien thi turn nguoi choi
    void SetPlayer(GamePlayer newplayer, GamePlayer oldplayer)
    {
        newplayer.panel.SetActive(false);
        oldplayer.panel.SetActive(true);
        newplayer.text.text = "Your Turn!";
        oldplayer.text.text = "Your Turn!";
    }

    // list piece co the an
    List<Piece> ScanForPossibleMove(Piece p, int x, int y)
    {
        forcedPiece = new List<Piece>();
        if (pieces[x, y].IsForceToMove(pieces, x, y))
        {
            forcedPiece.Add(pieces[x, y]);
        }
        return forcedPiece;
    }

    List<Piece> ScanForPossibleMove()
    {
        forcedPiece = new List<Piece>();
        //check all the pieces
        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
                if (pieces[i, j] != null && pieces[i, j].isBlue == isBlueTurn)
                    if (pieces[i, j].IsForceToMove(pieces, i, j))
                        forcedPiece.Add(pieces[i, j]);
        return forcedPiece;
    }

    // tao list cac piece co the di chuyen
    List<Piece> ScanCanMovePieces()
    {
        movePiece = new List<Piece>();
        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
                if (pieces[i, j] != null && pieces[i, j].isBlue == isBlueTurn)
                    if (pieces[i, j].CantMovePiece(pieces, i, j))
                        movePiece.Add(pieces[i, j]);
        return movePiece;

    }
}

[System.Serializable]
public class GamePlayer
{
    public GameObject panel;
    public Text text;
    public Text score;

}
