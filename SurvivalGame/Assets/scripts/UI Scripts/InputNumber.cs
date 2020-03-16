using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputNumber : MonoBehaviour
{
    [SerializeField]
    private Text text_Preview;

    [SerializeField]
    private Text text_Input;
    
    [SerializeField]
    private ActionController thePlayer;

    [SerializeField]
    private InputField inputField;

    public GameObject go_Base;




    public void Call()
    {
        go_Base.SetActive(true);
        inputField.ActivateInputField();
        inputField.text = "";
        text_Preview.text = DragSlot.instance.dragSlot.itemCount.ToString();

        //inputField.Select();

    }

    public void Cancel()
    {
        go_Base.SetActive(false);
        DragSlot.instance.SetColor(0);
        DragSlot.instance.dragSlot = null;
    }

    public void OK()
    {
        DragSlot.instance.SetColor(0);
        int num = 0;
        if(text_Input.text != "")
        {
            if (CheckNumber(text_Input.text))
            {
                num = int.Parse(text_Input.text);

                if (num > DragSlot.instance.dragSlot.itemCount)
                {
                    num = DragSlot.instance.dragSlot.itemCount;
                }
                else
                {
                    num = int.Parse(text_Input.text);
                }
            }
            else
            {
                num = 1;
            }
        }
        else
            num = int.Parse(text_Preview.text);


        StartCoroutine(DropItemCoroutine(num));

    }

    IEnumerator DropItemCoroutine(int _num)
    {
        for (int i = 0; i < _num; i++)
        {
            if(DragSlot.instance.dragSlot.item.itemPrefab != null)
            {
                Instantiate(DragSlot.instance.dragSlot.item.itemPrefab, thePlayer.transform.position + thePlayer.transform.forward, Quaternion.identity);

            }
            
            DragSlot.instance.dragSlot.SetSlotCount(-1);
            yield return new WaitForSeconds(0.05f);
        }
        
        DragSlot.instance.dragSlot = null;
        go_Base.SetActive(false);
    }

    private bool CheckNumber(string _argString)
    {
        char[] _tempChararray = _argString.ToCharArray();
        bool isNumber = true;

        for (int i = 0; i < _tempChararray.Length; i++)
        {
            if(_tempChararray[i] >= 48 && _tempChararray[i] <= 57)
                continue;
            isNumber = false;
        }
        return isNumber;

    }

}
