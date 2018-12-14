using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DpadButtons {

    bool first = true;
    public bool up, down, left, right;
    private bool Lastup, Lastdown, Lastleft, Lastright;
    public bool firstUp, firstDown, firstLeft, firstRight;

    public void GetDpad()
    {
        if (first)
        {
            up = down = left = right = false;
            first = false;
        }

        Lastup = up;
        Lastright = right;
        Lastleft = left;
        Lastdown = down;
        up = down = left = right = false;
        firstUp = firstDown = firstLeft = firstRight = false;

        if (Input.GetAxis("DpadX") >= .6)
        {
            right = true;

            if (!Lastright)
                firstUp = true;
        }
        if (Input.GetAxis("DpadX") <= -.6)
        {
            left = true;

            if (!Lastleft)
                firstLeft = true;
        }

        if (Input.GetAxis("DpadY") >= .6)
        {
            up = true;

            if (!Lastup)
                firstUp = true;
        }
        if (Input.GetAxis("DpadY") <= -.6)
        {
            down = true;

            if (!Lastdown)
                firstDown = true;
        }
    }
}
