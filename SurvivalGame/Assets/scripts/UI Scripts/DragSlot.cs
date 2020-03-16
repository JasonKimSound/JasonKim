using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragSlot : MonoBehaviour
{
    static public DragSlot instance;
    public Slot dragSlot;

    [SerializeField]
    private Image imageItem;
    
    void Start()
    {
        instance = this;
        
    }

    public void DragSetItemImage(Image _itemimage)
    {
        imageItem.sprite = _itemimage.sprite;
        SetColor(1);
    }

    public void SetColor(float _alpha)
    {
        Color color = imageItem.color;
        color.a = _alpha;
        imageItem.color = color;
    }
        

}
