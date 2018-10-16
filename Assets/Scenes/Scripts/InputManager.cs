using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    LEFT,
    RIGHT,
    UP,
    DOWN
}

public struct SwipeData{
  public Direction dir;
   public float value;

   public SwipeData(float val, Direction direction){
        value = val;
        dir = direction;
    }
}

public class InputManager : MonoBehaviour {
    Vector3 firstPoint;
    Vector3 secondPoint;

    //whether or not to reset the input check, happens on mouse up (when swipe/touch ends)
    bool clearInputs;

    delegate void OnSwipe(SwipeData swipe);
    OnSwipe SwipeDelegate;

    private void Start()
    {
        SwipeDelegate += PrintSwipeData;

        firstPoint = Vector3.negativeInfinity;
        secondPoint = Vector3.negativeInfinity;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            clearInputs = false;
            firstPoint = Input.mousePosition;
        }

        if(Input.GetMouseButtonUp(0))
        {
            clearInputs = true;
            secondPoint = Input.mousePosition;
        }

        //TODO:update swipe manager for continuous swiping
        if (firstPoint.x < -100 || secondPoint.x < -100)
        {
            //no swipe
        }
        else
        {
            //swipe
            var diff = secondPoint - firstPoint;

            float value;
            Direction dir;

            //disallow diagonal swiping
            if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
            {
                //left or right swipe
                value = diff.x;
                if (value > 0)
                    dir = Direction.RIGHT;
                else
                    dir = Direction.LEFT;
            }
            else
            {
                //up or down swipe
                value = diff.y;
                if (value > 0)
                    dir = Direction.UP;
                else
                    dir = Direction.DOWN;
            }


                SwipeDelegate(new SwipeData(value, dir));

        }

        if(clearInputs){
            firstPoint = Vector3.negativeInfinity;
            secondPoint = Vector3.negativeInfinity;
        }
    }

    SwipeData CreateSwipe(Vector3 startPos, Vector3 endPos){
        var swipe = new SwipeData();
        PrintSwipeData(swipe);
        return swipe;
    }

    void PrintSwipeData(SwipeData swipe){
        Debug.Log(swipe.dir + " swipe detected with value " + swipe.value);

    }
}
