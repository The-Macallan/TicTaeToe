using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }
    [SerializeField] private TMP_Text gameStateInfo;
    [SerializeField] private Player player1;
    [SerializeField] private Player player2;

    void Awake()
    {
        Instance = this;
    }
    
    public bool IsPlayerMove(Mark mark)
    {
        return (GameManager.Instance.GameMode == GameMode.PVP || GameManager.Instance.GameMode == GameMode.PVE && mark == player1.Mark);
    } 
    public bool IsComputerMove(Mark mark) 
    {
        return (GameManager.Instance.GameMode == GameMode.PVE && mark == player2.Mark);
    }

    public void EndGameActions(Mark mark)
    {
        IncreasePlayerScores(mark, 1);
        UpdateGameStateInfo(mark);
    }
    public void UpdateGameStateInfo(Mark mark)
    {
        if (GameManager.Instance.IsGameOver)
        {
            if (mark == Mark.Null)
                gameStateInfo.text = "Draw!";
            else
                gameStateInfo.text = Player(mark).NameText.text + " won!";
        }
        else
        {
            if (GameManager.Instance.GameMode == GameMode.PVE)
            {
                gameStateInfo.text = (Player(mark).Number == 1) ? "Your move..." : "Computer's move...";
            }
            else
            {
                gameStateInfo.text = Player(mark).NameText.text + " moves...";
            }
        }
    }
    public void IncreasePlayerScores(Mark mark, int points)
    {
        if (mark == Mark.Null)
        {
            player1.AddPoints(points);
            player2.AddPoints(points);
        }
        else
        {
            Player(mark).AddPoints(1);
        }
    }
    
    public void SwitchMark()
    {
        player1.SwitchMark();
        player2.SwitchMark();
    }
    
    private Player Player(Mark mark)
    {
        return (player1.Mark == mark) ? player1 : player2;
    }
}

