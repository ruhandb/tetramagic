using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragObject3D : MonoBehaviour
{
    private Vector3 mOffset;
    private float mZCoord;
    private float mYCoord;

    private void OnMouseDown()
    {
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        mYCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).y;
        mOffset = gameObject.transform.position - GetMouseWorldPos();
    }

    private Vector3 GetMouseWorldPos()
    {
        //Pixel coordinate (x,y)
        Vector3 mousePoint = Input.mousePosition;

        // z coordinate of game object on screen
        mousePoint.z = mZCoord;
        // mousePoint.z = mYCoord;

        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private void OnMouseDrag()
    {
        transform.position = GetMouseWorldPos() + mOffset;
    }
}
