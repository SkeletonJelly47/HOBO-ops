﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    public float XAxis, YAxis, LeftShift;

    //Axis states
    private float previousXAxis, currentXAxis;
    private float previousYAxis, currentYAxis;
    private float previousLeftShift, currentLeftShift;

    /// <summary>
    /// Check if axis is pressed
    /// </summary>
    /// <param name="axis">Input axis</param>
    /// <returns>Pressed axis button</returns>
    public bool AxisPressed(string axis)
    {
        if (axis == "Horizontal")
        {
            if (currentXAxis != 0 && previousXAxis == 0)
            {
                return true;
            }
        }
        else if (axis == "Vertical")
        {
            if (currentYAxis != 0 && previousYAxis == 0)
            {
                return true;
            }
        }
        else if (axis == "Fire3")
        {
            if (currentLeftShift != 0 && previousLeftShift == 0)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Check if axis is released
    /// </summary>
    /// <param name="axis">Input axis</param>
    /// <returns>Realised axis button</returns>
    public bool AxisUp(string axis)
    {
        if (axis == "Horizontal")
        {
            if (currentXAxis == 0 && previousXAxis != 0)
            {
                return true;
            }
        }
        if (axis == "Vertical")
        {
            if (currentYAxis == 0 && previousYAxis != 0)
            {
                return true;
            }
        }
        else if (axis == "Fire3")
        {
            if (currentLeftShift == 0 && previousLeftShift != 0)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Check if axis is held down
    /// </summary>
    /// <param name="axis">Input axis</param>
    /// <returns>Held down axis button</returns>
    public bool AxisDown(string axis)
    {
        if (axis == "Horizontal")
        {
            if (currentXAxis != 0)
            {
                return true;
            }
        }
        else if (axis == "Vertical")
        {
            if (currentYAxis != 0)
            {
                return true;
            }
        }
        else if (axis == "Fire3")
        {
            if (currentLeftShift != 0)
            {
                return true;
            }
        }

        return false;
    }


    private void Awake()
    {
        //Make sure that there is no more than one of this instance
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(transform.gameObject);
    }

    private void LateUpdate()
    {
        XAxis = Input.GetAxisRaw("Horizontal");
        YAxis = Input.GetAxisRaw("Vertical");
        LeftShift = Input.GetAxisRaw("Fire3");


        previousXAxis = currentXAxis;
        currentXAxis = XAxis;
        previousYAxis = currentYAxis;
        currentYAxis = YAxis;
        previousLeftShift = currentLeftShift;
        currentLeftShift = LeftShift;
    }
}