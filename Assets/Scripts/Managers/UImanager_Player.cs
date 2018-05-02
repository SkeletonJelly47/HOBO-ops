﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UImanager_Player : MonoBehaviour
{

    //StatusBars variables
    private const float     maxValue = 100;
    public  Image           HealthBar;
    public  Image           SanityBar;
    public  Image           StaminaBar;
    public  Text            BottleText;
    private GameObject      InventoryScreen;

    //Inventory variables
    private List<Transform> inventoryObjects = new List<Transform>();
    private bool showing = false;

    //Daytime indicator variables
    float arrowrotationChanger = 0.0f;
    public GameObject Arrow;

    private void StatusBarValueChanger()
    {
        HealthBar.fillAmount  = Mathf.Clamp01(PlayerController.pl.Health / maxValue);
        SanityBar.fillAmount  = Mathf.Clamp01(PlayerController.pl.Sanity / maxValue);
        StaminaBar.fillAmount = Mathf.Clamp01(PlayerController.pl.Stamina / maxValue);
    }

    private void Inventory()
    {
        for (int i = 0; i < transform.childCount -2; i++)
        {
            transform.GetChild(i).gameObject.SetActive(showing);
        }
        BottleText.text = PlayerController.pl.Inventory.InventoryList.Count(x => x.BaseItemID == 8).ToString();
        for (int i = 0; i < inventoryObjects.Count; i++)
        {
            inventoryObjects[i].GetComponentInChildren<Text>().text = PlayerController.pl.Inventory.InventoryList.Count(x => x.BaseItemID == i && x != null).ToString();
        }
    }

    private void UIInput()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            showing = !showing;
        }
    }
    private void DaytimeIndicator()
    {//Arrow.transform.localRotation.x
        arrowrotationChanger = GameManager.Instance.DayTimer * 0.79f;
        Arrow.GetComponent<Transform>().rotation = Quaternion.Euler(0, 0,-arrowrotationChanger);
    }

    void Start()
    {
        for (int i = 0; i < transform.GetChild(1).childCount; i++)
        {
            inventoryObjects.Add(transform.GetChild(1).GetChild(i).transform);
        }
        InventoryScreen = transform.GetChild(1).gameObject;
    }

    void Update()
    {
        UIInput();
        DaytimeIndicator();
        StatusBarValueChanger();
        Inventory();
    }
}
