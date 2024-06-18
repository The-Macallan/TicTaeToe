using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum Mark
{
    Null,
    X,
    O
}

public enum GameMode
{
    PVE,
    PVP
}

public enum FieldSize
{
    _3x3,
    _5x5
}

public class GameManager : MonoBehaviour
{
    static public GameManager Instance { get; private set; }
    public bool IsGameOver { get; set; }
    public GameMode GameMode { get; set; }
    public FieldSize FieldSize { get; set; }
    public int PlayerNumberWhoGoesFirst { get; set; }

    [SerializeField] private GameObject fieldPrefab;
    [SerializeField] private GameObject cell3x3Prefab;
    [SerializeField] private GameObject cell5x5Prefab;
    [SerializeField] private Sprite spriteField3x3;
    [SerializeField] private Sprite spriteField5x5;
    [SerializeField] private Sprite spriteX;
    [SerializeField] private Sprite spriteO;
    [SerializeField] private Color colorX;
    [SerializeField] private Color colorO;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartGame()
    {
        IsGameOver = false;
        SceneManager.LoadScene("Game");
    }
    public void RestartGame()
    {
        IsGameOver = false;

        var field = GameObject.FindGameObjectWithTag("GameField");
        if (field != null)
        {
            Destroy(field);
            Instantiate(fieldPrefab);            
        }
    }
    public void ExitToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public GameObject GetCellPrefab()
    {
        return (FieldSize == FieldSize._3x3) ? cell3x3Prefab : cell5x5Prefab;
    }
    public Sprite GetMarkSprite(Mark mark)
    {
        return (mark == Mark.X) ? spriteX : spriteO;
    }
    public Sprite GetFieldSprite()
    {
        return (FieldSize == FieldSize._3x3) ? spriteField3x3 : spriteField5x5;
    }
    public Color GetMarkColor(Mark mark)
    {
        return (mark == Mark.X) ? colorX : colorO;
    }
   
    public Mark OppositeMark(Mark mark)
    {
        return (mark == Mark.X) ? Mark.O : Mark.X;
    }
}
