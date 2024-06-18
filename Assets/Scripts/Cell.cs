using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public int Index { get; private set; }
    public Mark Mark { get; private set; }
    
    private SpriteRenderer spriteRenderer;
    private Field field;

    private void Awake()
    {
        spriteRenderer  = GetComponent<SpriteRenderer>();
        field           = GetComponentInParent<Field>();
        Index           = transform.GetSiblingIndex();
        Mark            = Mark.Null;
    }
    
    private bool IsMoveAllowed()
    {
        return !GameManager.Instance.IsGameOver && PlayerManager.Instance.IsPlayerMove(field.CurrentMark) && Mark == Mark.Null;
    } 
    
    public void UpdateByCurrentMark()
    {
        this.Mark = field.CurrentMark;
        spriteRenderer.sprite  = GameManager.Instance.GetMarkSprite(Mark);
        spriteRenderer.color   = GameManager.Instance.GetMarkColor(Mark);      
    }
    
    private void OnMouseUpAsButton()
    {
        if (!IsMoveAllowed())
            return;

        field.MakeMove(this);
    }
}
