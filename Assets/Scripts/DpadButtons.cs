using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DpadButtons {
    public int playerNumber = 1;
    private string a, b, x, y, dhorizontal, dvertical;

    bool first = true;
    public bool A, B, X, Y;
    public bool firstA, firstB, firstX, firstY;
    private bool LastA, LastB, LastX, LastY;
    public bool up, down, left, right;
    private bool Lastup, Lastdown, Lastleft, Lastright;
    public bool firstUp, firstDown, firstLeft, firstRight;

    public void GetDpad()
    {
        if (first)
        {
            up = down = left = right = false;
            A = B = X = Y = false;
            first = false;

            if (playerNumber == 1)
            {
                a = "AButton";
                b = "BButton";
                x = "XButton";
                y = "YButton";
                dhorizontal = "DpadX";
                dvertical = "DpadY";
            }
            if (playerNumber == 2)
            {
                a = "AButton2";
                b = "BButton2";
                x = "XButton2";
                y = "YButton2";
                dhorizontal = "DpadX2";
                dvertical = "DpadY2";
            }
            for (int i = 0; i < Input.GetJoystickNames().Length; i++)
            {
                Debug.Log(Input.GetJoystickNames()[i]);
            }
        }
        LastA = A;
        LastB = B;
        LastX = X;
        LastY = Y;
        Lastup = up;
        Lastright = right;
        Lastleft = left;
        Lastdown = down;
        up = down = left = right = false;
        A = B = X = Y = false;
        firstUp = firstDown = firstLeft = firstRight = false;
        firstA = firstB = firstX = firstY = false;

        if (Input.GetButton(a))
        {
            A = true;

            if (!LastA)
            {
                firstA = true;
            }
        }

        if (Input.GetButton(b))
        {
            B = true;

            if (!LastB)
            {
                firstB = true;
            }
        }

        if (Input.GetButton(x))
        {
            X = true;

            if (!LastX)
            {
                firstX = true;
            }
        }

        if (Input.GetButton(y))
        {
            Y = true;

            if (!LastY)
            {
                firstY = true;
            }
        }

        if (Input.GetAxis(dhorizontal) >= .6)
        {
            right = true;

            if (!Lastright)
                firstUp = true;
        }
        if (Input.GetAxis(dhorizontal) <= -.6)
        {
            left = true;

            if (!Lastleft)
                firstLeft = true;
        }

        if (Input.GetAxis(dvertical) >= .6)
        {
            up = true;

            if (!Lastup)
                firstUp = true;
        }
        if (Input.GetAxis(dvertical) <= -.6)
        {
            down = true;

            if (!Lastdown)
                firstDown = true;
        }
    }
}
