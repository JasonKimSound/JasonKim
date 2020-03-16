using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler 
{

    public Item item; //획득한 아이템
    public int itemCount; //획득한 아이템의 개수
    public Image itemImage; //아이템의 이미지

    [SerializeField]
    private Text text_Count;
    [SerializeField]
    private GameObject go_CountImage;

    private WeaponManager theWeaponManager;

    //private Rect baseRect;
    private RectTransform baseRectTransform;

    private InputNumber theInputNumber;

    

    void Start()
    {
        baseRectTransform = transform.parent.parent.GetComponent<RectTransform>();
        //baseRect = transform.parent.parent.GetComponent<RectTransform>().rect;
        theWeaponManager = FindObjectOfType<WeaponManager>();
        theInputNumber = FindObjectOfType<InputNumber>();
        
    }

    //이미지 투명도 조절
    private void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
        
    }

    //아이템 획득
    public void AddItem(Item _item, int _count = 1)
    {
        item = _item;
        itemCount = _count;
        itemImage.sprite = item.itemImage;


        if(item.itemType != Item.ItemType.Equipment)
        {
            go_CountImage.SetActive(true);
            text_Count.text = itemCount.ToString();
        }
        else
        {
            
            text_Count.text = "0";
            go_CountImage.SetActive(false);
        }

        SetColor(1);
    }

    //아이템 개수 조정
    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        text_Count.text = itemCount.ToString();   

        if(itemCount <= 0)
        {
            ClearSlot();
        }
    }

    //슬롯 초기화
    private void ClearSlot()
    {
        item = null;
        itemCount = 0;
        itemImage.sprite = null;
        SetColor(0);

        text_Count.text = "0";
        go_CountImage.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData) //이 스크립트가 적용된 객체에 마우스를 가져간 후 우클릭을 하면 일어나는 이벤트 
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            if(item != null)
            {
                if(item.itemType == Item.ItemType.Equipment)
                {
                    
                    StartCoroutine(theWeaponManager.ChangeWeaponCoroutine(item.weaponType, item.itemName));
                    

                }
                else
                {
                    Debug.Log(item.itemName + "을 사용했습니다.");
                    SetSlotCount(-1);
                }
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            DragSlot.instance.dragSlot = this;
            DragSlot.instance.DragSetItemImage(itemImage);
           //transform.position = eventData.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            DragSlot.instance.transform.position = eventData.position;
        }
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        Vector2 pointerPos;
        pointerPos = new Vector2(DragSlot.instance.transform.position.x, DragSlot.instance.transform.position.y);
        
        if(!RectTransformUtility.RectangleContainsScreenPoint(baseRectTransform, pointerPos))
        {
            if(DragSlot.instance.dragSlot != null)
            {

                theInputNumber.Call();
            }
            

         
        }
        else
        {

            DragSlot.instance.dragSlot = null;
            DragSlot.instance.SetColor(0);
        }

        /*if(DragSlot.instance.transform.localPosition.x < baseRect.xMin || DragSlot.instance.transform.localPosition.x > baseRect.xMin ||
            DragSlot.instance.transform.localPosition.y < baseRect.yMin || DragSlot.instance.transform.localPosition.y > baseRect.yMin)
        {
            Debug.Log("인벤토리 영역을 벗어났습니다.");
        }*/

        
        
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(DragSlot.instance.dragSlot != null)
        {
            ChangeSlot();
        }
        
    }

    private void ChangeSlot()
    {
        Item _tempItem = item;
        int _tempItemCount = itemCount;

        AddItem(DragSlot.instance.dragSlot.item, DragSlot.instance.dragSlot.itemCount);

        if(_tempItem != null)
        {
            DragSlot.instance.dragSlot.AddItem(_tempItem, _tempItemCount);
        }
        else
        {
            DragSlot.instance.dragSlot.ClearSlot();
        }

    }
}
