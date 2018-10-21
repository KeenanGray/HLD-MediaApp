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
    public bool full; //Whether the swipe is a complete swipe. False before user lifts finger.
    public float swipeTime;

   public SwipeData(float val, Direction direction, bool final=false){
        value = val;     
        dir = direction;
        full = final;
        swipeTime = 0;
    }

    public float SwipeSpeed (){
        return Mathf.Abs(this.value) / this.swipeTime;
    }
}

public class InputManager : MonoBehaviour {
    //whether or not to reset the input check, happens on mouse up (when swipe/touch ends)
    bool clearInputs;

    public delegate void OnSwipe(SwipeData swipe);
    public static event OnSwipe SwipeDelegate;

    float SwipeVal;
    Direction SwipeDir;

    private void Start()
    {
        SwipeDelegate += PrintSwipeData;

        StartCoroutine("CheckSwipeInput");
        StartCoroutine("ContinousSwipeDetection");
    }

    private void Update()
    {
       
    }
    
    SwipeData CreateSwipe(Vector3 startPos, Vector3 endPos){
        var swipe = new SwipeData();
        PrintSwipeData(swipe);
        return swipe;
    }

    void PrintSwipeData(SwipeData swipe){
        if (swipe.full)
        {
           // Debug.Log(swipe.dir + " swipe detected with value " + swipe.SwipeSpeed());
        }
        else {
           //Debug.Log("user is swiping " + swipe.dir);
        }

    }

    IEnumerator CheckSwipeInput(){
        Vector2 firstPoint;
        Vector2 secondPoint;

        float StartTime = 0;
        float EndTime = 0;

        firstPoint = Vector3.negativeInfinity;
        secondPoint = Vector3.negativeInfinity;
        while (true)
        {
            //First check for full swipes.

            //Get the initial touch point
            if (Input.GetMouseButtonDown(0))
            {
                clearInputs = false;
                firstPoint = Input.mousePosition;
                StartTime = Time.time;
            }

            //Get the second touch point
            if (Input.GetMouseButtonUp(0))
            {
                clearInputs = true;
                secondPoint = Input.mousePosition;
                EndTime = Time.time;
            }

            //Determine the direction of the swipe
            if (firstPoint.x < -100 || secondPoint.x < -100)
            {
                //no swipe
            }
            else
            {
                //swipe
                var diff = secondPoint - firstPoint;

                //disallow diagonal swiping
                if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
                {
                    //left or right swipe
                    SwipeVal = diff.x;
                    if (SwipeVal > 0)
                        SwipeDir = Direction.RIGHT;
                    else
                        SwipeDir = Direction.LEFT;
                }
                else
                {
                    //up or down swipe
                    SwipeVal = diff.y;
                    if (SwipeVal > 0)
                        SwipeDir = Direction.UP;
                    else
                        SwipeDir = Direction.DOWN;
                }

                //Create a new swipe delegate so that other functions can handle the fullswipe         
                SwipeData FinalSwipe = new SwipeData(SwipeVal,SwipeDir,true);
                FinalSwipe.swipeTime = EndTime - StartTime;

                SwipeDelegate(new SwipeData(SwipeVal, SwipeDir,true));


            }

            if (clearInputs)
            {
                firstPoint = Vector3.negativeInfinity;
                secondPoint = Vector3.negativeInfinity;
            }

            yield return null;
        }
    }

    IEnumerator ContinousSwipeDetection()
    {
        while (true)
        {
            //While the mouse is held down
            //We need to determine the direction the user is swiping continously

            Vector2 current;
            Vector2 last;

            if (Input.GetMouseButton(0))
            {
                current = Input.mousePosition;
                yield return new WaitForFixedUpdate();
                last = Input.mousePosition;

                //swipe
                var diff = last - current;

                //disallow diagonal swiping
                if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
                {
                    //left or right swipe
                    SwipeVal = diff.x;
                    if (SwipeVal > 0)
                        SwipeDir = Direction.RIGHT;
                    else
                        SwipeDir = Direction.LEFT;
                }
                else
                {
                    //up or down swipe
                    SwipeVal = diff.y;
                    if (SwipeVal > 0)
                        SwipeDir = Direction.UP;
                    else
                        SwipeDir = Direction.DOWN;
                }

                //Create a new swipe delegate so that other functions can handle the fullswipe
                SwipeDelegate(new SwipeData(SwipeVal, SwipeDir));
            }

            yield return null;
        }
    }
}
