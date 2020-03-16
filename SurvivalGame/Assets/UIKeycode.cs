using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIKeycode : MonoBehaviour
{
    private bool activated;
    private Inventory theInventory;
    private InputNumber theInputNumber;

    void Start()
    {
        theInventory = FindObjectOfType<Inventory>();
        theInputNumber = FindObjectOfType<InputNumber>();
    }

    // Update is called once per frame
    void Update()
    {
        ESC();
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if(theInputNumber.go_Base.activeSelf)
            theInputNumber.OK();
        }
    

    }

    private void ESC()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (theInputNumber.go_Base.activeSelf)
            {
                theInputNumber.Cancel();
            }
            else
            {
                Inventory.inventoryActivated = false;

                if (!Inventory.inventoryActivated)
                {
                    theInventory.CloseInventory();
                }
            }

        }
    }
}
