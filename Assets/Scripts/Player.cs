using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int Number { get; private set; }  
    public Mark Mark { get; private set; }
    [field: SerializeField] public TMP_Text NameText { get; private set; }
    [field: SerializeField] public TMP_Text ScoreText { get; private set; }  
    
    private int score;  
    
    void Awake()
    {
        Number  = transform.GetSiblingIndex() + 1;
        Mark    = (Number == GameManager.Instance.PlayerNumberWhoGoesFirst) ? Mark.X : Mark.O;
        
        NameText.color   = GameManager.Instance.GetMarkColor(Mark);
        ScoreText.color  = GameManager.Instance.GetMarkColor(Mark);
        
        if (GameManager.Instance.GameMode == GameMode.PVE && Number == 2)
            NameText.text = "Computer";     
    }

    public void AddPoints(int points)
    {
        score += points;
        ScoreText.text = "score: " + score;
    }
    
    public void SwitchMark()
    {
        Mark = GameManager.Instance.OppositeMark(Mark);
        NameText.color   = GameManager.Instance.GetMarkColor(Mark);
        ScoreText.color  = GameManager.Instance.GetMarkColor(Mark);
    }

}
