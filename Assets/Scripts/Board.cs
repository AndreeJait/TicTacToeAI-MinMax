using System;
using UnityEngine;
using UnityEngine.Events;


static class Constants
{
    public const int INIFINITY = 9999999;
}

public class Board : MonoBehaviour
{
    [Header("Input Settings: ")]
    [SerializeField] private LayerMask boxesLayerMask;
    [SerializeField] private float touchRadius;

    [Header("Mark Spirtes : ")]
    [SerializeField] private Sprite spriteX;
    [SerializeField] private Sprite spriteO;

    [Header("Mark Colors : ")]
    [SerializeField] private Color colorX;
    [SerializeField] private Color colorO;

    private LineRenderer lineRenderer;

    public UnityAction<Mark, Color> OnWinAction;

    public Mark[] marks;

    private Camera cam;
    public Mark currentMark;

    public bool canPlay = false;
    public bool isAiTurn = false;

    private void Start()
    {
        cam = Camera.main;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;


        marks = new Mark[9];
        for (int i = 0; i < 9; i++)
        {
            marks[i] = Mark.None;
        }
  
    }
    public bool isBoardEmpty()
    {
        foreach(Mark m in this.marks)
        {
            if (!m.Equals(Mark.None))
            {
                return false;
            }
        }
        return true;
    }

    public void BestMove()
    {
        if (isBoardEmpty())
        {
            marks[4] = Mark.X;
            Transform t = transform.GetChild(4);
            Box b = t.GetComponent<Box>();
            HitBox(b);
        }
        else
        {
            int bestScore = -Constants.INIFINITY;
            int bestMove = 0;
            for (int i = 0; i < 9; i++)
            {
                if(marks[i] == Mark.None)
                {
                    marks[i] = Mark.X;
                    int score = MiniMax(0, false);
                    marks[i] = Mark.None;
                    if(score > bestScore)
                    {
                        bestScore = score;
                        bestMove = i;
                    }
                }
            }
            Transform t = transform.GetChild(bestMove);
            Box b = t.GetComponent<Box>();
            HitBox(b);
        }
        isAiTurn = false;
    }

    private int MiniMax(int depth, bool isMaximazing)
    {
        WinResponse won = CheckIfWin();
        if(won != null)
        {
            return won.code;
        }
        if (isMaximazing)
        {
            int bestScore = -Constants.INIFINITY;
            for(int i = 0; i < 9; i++)
            {
                if(marks[i] == Mark.None)
                {
                    marks[i] = Mark.X;
                    int score = MiniMax(depth + 1, false);
                    marks[i] = Mark.None;
                    bestScore = Math.Max(score, bestScore);
                }
            }
            return bestScore;
        }
        else
        {
            int bestScore = Constants.INIFINITY;
            for (int i = 0; i < 9; i++)
            {
                if (marks[i] == Mark.None)
                {
                    marks[i] = Mark.O;
                    int score = MiniMax(depth + 1, true);
                    marks[i] = Mark.None;
                    bestScore = Math.Min(score, bestScore);
                }
            }
            return bestScore;
        }
    }

    private void Update()
    {
        if (canPlay && Input.GetMouseButtonUp(0))
        {
            if (!isAiTurn)
            {
                Vector2 touchPosition = cam.ScreenToWorldPoint(Input.mousePosition);

                Collider2D hit = Physics2D.OverlapCircle(touchPosition, touchRadius, boxesLayerMask);

                if (hit)
                {
                    HitBox(hit.GetComponent<Box>());
                    isAiTurn = true;
                    BestMove();
                }
            }
        }
    }

    private void HitBox(Box box)
    {
        if (!box.isMarked)
        {
            marks[box.index] = currentMark;

            box.SetAsMarked(GetSprite(),currentMark, GetColor());
            WinResponse won = CheckIfWin();
            if (won != null)
            {
                if(won.code != 0)
                {
                    OnWinAction.Invoke(currentMark, GetColor());
                    DrawLine(won.indexs[0], won.indexs[2]);
                }
                else
                {
                    OnWinAction.Invoke(Mark.None, Color.white);
                }
                canPlay = false;
                return;
            }
            SwitchPlayer();
        }
    }

    public class WinResponse
    {
        public int code;
        public Mark mark;
        public int[] indexs; 

        public WinResponse(int code, Mark mark, int[] indexs)
        {
            this.code = code;
            this.mark = mark;
            this.indexs = indexs;
        }
    }

    private WinResponse CheckIfWin()
    {
        int[] indexs = new int[3] { 0, 0, 0 };
        
        int[,] win = new int[8,3] {
            {0,1,2},
            {3,4,5},
            {6,7,8},
            {0,3,6},
            {1,4,7},
            {2,5,8},
            {0,4,8},
            {2,4,6}
        };
        for(int i = 0; i < 8; i++)
        {
            if (AreBoxeSMatched(win[i,0], win[i,1], win[i, 2])){
                indexs[0] = win[i, 0];
                indexs[1] = win[i, 1];
                indexs[2] = win[i, 2];
                return marks[win[i, 0]] == Mark.X ? new WinResponse(1, Mark.X, indexs) : new WinResponse(-1, Mark.O, indexs);
            }
        }
        for(int i = 0; i < 9; i++)
        {
            if(marks[i] == Mark.None)
            {
                return null;
            }
        }
        return  new WinResponse(0, Mark.None, indexs);
    }

    private bool AreBoxeSMatched(int i, int j, int k)
    {
        return marks[i] == marks[j] && marks[j] == marks[k] && marks[i] != Mark.None;
    }

    private void DrawLine(int i, int k)
    {
        lineRenderer.SetPosition(0, transform.GetChild(i).position);
        lineRenderer.SetPosition(1, transform.GetChild(k).position);
        Color color = GetColor();
        color.a = .3f;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;

        lineRenderer.enabled = true;
    }

    private void SwitchPlayer()
    {
        currentMark = (currentMark == Mark.X) ? Mark.O : Mark.X;
    }

    private Color GetColor()
    {
        return (currentMark == Mark.X) ? colorX : colorO;
    }

    private Sprite GetSprite()
    {
        return (currentMark == Mark.X) ? spriteX : spriteO;
    }
}
