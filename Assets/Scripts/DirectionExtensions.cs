using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DirectionExtensions
{
    public static int ToDegrees(this Direction direction){
        switch(direction){
            case Direction.UpperRight: return 270;
            case Direction.UpperLeft: return 180;
            case Direction.DownLeft: return 90;
            case Direction.DownRight: return 0;
        }

        return 0;
    }

    public static Direction GetDirectionFromAngle(float angle){
        angle = Mathf.Round(angle);
        if(angle < 0) angle += 360;
        else if(angle >= 360) angle -= 360;

        switch(angle){
            case 270: return Direction.UpperRight;
            case 180: return Direction.UpperLeft;
            case 90: return Direction.DownLeft;
            case 0: return Direction.DownRight;
        }

        return 0;
    }

    public static Direction GetMirroredDirection(this Direction direction)
        => GetDirectionFromAngle(ToDegrees(direction) + 180);
}

public enum Direction{
    UpperLeft,
    UpperRight,
    DownLeft,
    DownRight
}