using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct EndGame
{
    public bool isGameOver;
    public Mark winner;
    public int[] winLine;

    public EndGame(bool isGameOver, Mark winner, int[] winLine)
    {
        this.isGameOver = isGameOver;
        this.winner = winner;
        this.winLine = winLine;
    }
}

public class Field : MonoBehaviour
{
    public Mark CurrentMark { get; private set; }
    private Mark[] marks;
    private Cell[] cells;
    private int cellsCount;
    private int minimaxDepth;
    
    private void Awake()
    {
        CurrentMark = Mark.X;  
        GetComponent<SpriteRenderer>().sprite = GameManager.Instance.GetFieldSprite();
        
        if (GameManager.Instance.FieldSize == FieldSize._3x3)
        {
            cellsCount    = 9;
            minimaxDepth  = 8;
        }
        else
        {
            cellsCount    = 25;
            minimaxDepth  = 3;
        }

        marks = new Mark[cellsCount];
        cells = new Cell[cellsCount];

        CreatingFieldCells();
    }
    
    private void Start()
    {
        PlayerManager.Instance.UpdateGameStateInfo(CurrentMark);
        
        if (PlayerManager.Instance.IsComputerMove(CurrentMark))  
            StartCoroutine(ComputerMove());
    }
    
    private void CreatingFieldCells()
    {
        var CellPrefab = GameManager.Instance.GetCellPrefab();
        var transform = GetComponent<Transform>();

        int i = 0;
        float ymax, xmin, ymin, ydelta, xmax, xdelta;
        if (GameManager.Instance.FieldSize == FieldSize._3x3)
        { ymax = 2; ymin = -2; ydelta = 2; xmax = 2; xmin = -2; xdelta = 2; }
        else
        { ymax = 2.6f; ymin = -2.6f; ydelta = 1.3f; xmax = 2.6f; xmin = -2.6f; xdelta = 1.3f; }

        for (var y = ymax; y >= ymin; y -= ydelta)
        {
            for (var x = xmin; x <= xmax; x += xdelta)
            {
                var cell = Instantiate(CellPrefab, transform);
                cell.transform.SetSiblingIndex(i);
                cell.transform.position = new Vector3(x, y);
                cells[i] = cell.GetComponent<Cell>();
                marks[i] = Mark.Null;
                i++;
            }
        }
    }
    
    public void MakeMove(Cell cell)
    {
        cell.UpdateByCurrentMark();

        marks[cell.Index] = CurrentMark;
       
        EndGame endGame = CheckEndGame();
        if (endGame.isGameOver)
        {
            if (endGame.winner != Mark.Null)
                DrawWinLine(endGame.winLine);

            GameManager.Instance.IsGameOver = true;
            PlayerManager.Instance.EndGameActions(endGame.winner);
            return;
        }
    
        SwitchMark();

        if (PlayerManager.Instance.IsComputerMove(CurrentMark))
            StartCoroutine(ComputerMove());
    }

    private void DrawWinLine(int[] winLine)
    {
        var color = GameManager.Instance.GetMarkColor(CurrentMark);
        color.a = .9f;

        var lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, transform.GetChild(winLine[0]).position);
        lineRenderer.SetPosition(1, transform.GetChild(winLine[winLine.Length - 1]).position);
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }

    private void SwitchMark()
    {
        CurrentMark = (CurrentMark == Mark.X) ? Mark.O : Mark.X;
        PlayerManager.Instance.UpdateGameStateInfo(CurrentMark);
    }

    IEnumerator ComputerMove()
    {
        yield return new WaitForSeconds(1);
        
        MakeMove(FindBestCellForComputerMove());
    }
    private Cell FindBestCellForComputerMove()
    {
        int bestScore = Int32.MinValue, bestPos, score;
        var moves = new List<int>();

        for (int i = 0; i < cellsCount; i++)
        {
            if (marks[i] != Mark.Null)
                continue;
            
            marks[i] = CurrentMark;
            score = Minimax(GameManager.Instance.OppositeMark(CurrentMark), 0, Int32.MinValue, Int32.MaxValue);
            marks[i] = Mark.Null;
    
            if (bestScore < score)
            {
                bestScore   = score;
                moves.Clear();
                moves.Add(i);    
            }
            else if (bestScore == score)
            {
                moves.Add(i);
            }
        }
        bestPos = moves[new System.Random().Next(moves.Count)];

        return cells[bestPos];
    }   
    private int Minimax(Mark mark, int depth, int alpha, int beta)
    {
        var endGame = CheckEndGame();
        if (depth == minimaxDepth || endGame.isGameOver)
            return PositionEvaluation(endGame.winner, depth);

        int score; 
        for (int i = 0; i < cellsCount; i++)
        {
            if (marks[i] != Mark.Null)
                continue;

            marks[i] = mark;
            score = Minimax(GameManager.Instance.OppositeMark(mark), depth + 1, alpha, beta);
            marks[i] = Mark.Null;
            
            if (mark == CurrentMark && score > alpha)
                alpha = score;
            else if (mark != CurrentMark && score < beta)             
                beta = score;

            if (alpha > beta)
                break;
        }

        return mark == CurrentMark ? alpha : beta;
    } 
    private int PositionEvaluation(Mark winner, int depth)
    {
        return (GameManager.Instance.FieldSize == FieldSize._3x3) ? SimplePositionEvaluation(winner, depth) : HeuristicPositionEvaluation(depth);
    } 
    private int SimplePositionEvaluation(Mark winner, int depth)
    {
        if (winner == Mark.Null)
            return 0;
        else
            return (winner == CurrentMark) ? 10 - depth : depth - 10; 
    }
    private int HeuristicPositionEvaluation(int depth)
    {
        int score = 0;
        var lines = Lines();
        foreach (var line in lines)
        {
            int yourCount = 0, oppCount = 0;
            for (int i = 0; i < line.Length; i++)
            {
                if (marks[line[i]] == CurrentMark)
                    yourCount++;
                else if (marks[line[i]] == GameManager.Instance.OppositeMark(CurrentMark))
                    oppCount++;
            }

            if (yourCount != 0 && oppCount != 0)
                continue;
            else if (yourCount == 1)
                score += 10 - depth;
            else if (yourCount == 2)
                score += 100 - depth * 10;
            else if (yourCount == 3)
                score += 1000 - depth * 100;
            else if (yourCount == 4)
                score += 10000 - depth * 1000;
            else if (oppCount == 1)
                score += depth - 10;
            else if (oppCount == 2)
                score += depth * 10 - 100;
            else if (oppCount == 3)
                score += depth * 100 - 1000;
            else if (oppCount == 4)
                score += depth * 1000 - 10000;
        }
        return score;
    }

    private EndGame CheckEndGame()
    {
        var lines = Lines();
        foreach (var line in lines) 
        {
            if (IsWinLine(line))
                return new EndGame(true, marks[line[0]], line);
        }
        
        return (AreAllCellsUnavailable()) ? new EndGame(true, Mark.Null, new int[] {}) : new EndGame(false, Mark.Null, new int[] {});
    }  
    private List<int[]> Lines()
    {
        var Lines = new List<int[]>();
        if (GameManager.Instance.FieldSize == FieldSize._3x3)
        {
            Lines.Add(new int[] { 0, 1, 2 }); 
            Lines.Add(new int[] { 3, 4, 5 }); 
            Lines.Add(new int[] { 6, 7, 8 }); 
            Lines.Add(new int[] { 0, 3, 6 });
            Lines.Add(new int[] { 1, 4, 7 }); 
            Lines.Add(new int[] { 2, 5, 8 }); 
            Lines.Add(new int[] { 0, 4, 8 }); 
            Lines.Add(new int[] { 2, 4, 6 });
        }
        else 
        {
            Lines.Add(new int[] { 0, 1, 2, 3 }); 
            Lines.Add(new int[] { 1, 2, 3, 4 }); 
            Lines.Add(new int[] { 5, 6, 7, 8 }); 
            Lines.Add(new int[] { 6, 7, 8, 9 });
            Lines.Add(new int[] { 10, 11, 12, 13 });
            Lines.Add(new int[] { 11, 12, 13, 14 });
            Lines.Add(new int[] { 15, 16, 17, 18 });
            Lines.Add(new int[] { 16, 17, 18, 19 });
            Lines.Add(new int[] { 20, 21, 22, 23 });
            Lines.Add(new int[] { 21, 22, 23, 24 });
            Lines.Add(new int[] { 0, 5, 10, 15 });
            Lines.Add(new int[] { 5, 10, 15, 20 });
            Lines.Add(new int[] { 1, 6, 11, 16 });
            Lines.Add(new int[] { 6, 11, 16, 21 });
            Lines.Add(new int[] { 2, 7, 12, 17 });
            Lines.Add(new int[] { 7, 12, 17, 22 });
            Lines.Add(new int[] { 3, 8, 13, 18 });
            Lines.Add(new int[] { 8, 13, 18, 23 });
            Lines.Add(new int[] { 4, 9, 14, 19 });
            Lines.Add(new int[] { 9, 14, 19, 24 });
            Lines.Add(new int[] { 0, 6, 12, 18 });
            Lines.Add(new int[] { 6, 12, 18, 24 });
            Lines.Add(new int[] { 4, 8, 12, 16 });
            Lines.Add(new int[] { 8, 12, 16, 20 });
            Lines.Add(new int[] { 1, 7, 13, 19 });
            Lines.Add(new int[] { 5, 11, 17, 23 });
            Lines.Add(new int[] { 3, 7, 11, 15 });
            Lines.Add(new int[] { 9, 13, 17, 21 });
        }
        return Lines;
    }
    private bool IsWinLine(int[] line)
    {
        var mark = marks[line[0]];
        if (mark == Mark.Null)
            return false;

        for (int i = 1; i < line.Length; i++)
        {
            if (mark != marks[line[i]])
                return false;
        }
        return true;
    }
    private bool AreAllCellsUnavailable()
    {
        foreach (var mark in marks) 
        {
            if (mark == Mark.Null)
                return false;
        }
        return true;
    }
}
