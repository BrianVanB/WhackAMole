using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : MonoBehaviour
{
    //Access to the hammer model to enable/disable it when needed
    public GameObject hammer;

    //The starting height of the hammer above the floor
    public float hammerHeight = 3f;

    //Hammer swing animation parameters
    private bool swinging = false;
    private float swingAnimTime = 0.3f; //seconds
    private float swingTime = 0;

    //A raycast mask to prevent oddities while updating hammer position
    [SerializeField] LayerMask holeLayer;

    private GameController controller;
    private int hitComboCount = 0; //Consecutive moles hit

    private void Start()
    {
        controller = FindObjectOfType<GameController>();
        hammer.SetActive(false);
    }

    void Update()
    {
        if (!controller.Running)
            return;

        //Only update the hammer position while not swinging
        //This stops the hammer from moving while mid-swing
        if(!swinging)
            hammer.transform.position = UpdatePosition();

        //Let the player swing the hammer only when not swinging
        if (!swinging && Input.GetMouseButtonDown(0))
        {
            hammer.SetActive(true);
            DoSwing();
            CheckHit();
        }

        //Continues the swing animation
        if (swinging)
            DoSwing();
    }

    //UpdatePosition moves the hammer above the point where the player tapped the screen
    private Vector3 UpdatePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, (1<<holeLayer)))
            return new Vector3(hit.point.x, hammerHeight, hit.point.z);
        else
            return Vector3.zero;
    }

    //CheckHit looks whether the player tapped on a hole, and if a hole was hit, 
    //checks if the mole in the hole was hit
    private void CheckHit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Hole h = hit.transform.GetComponent<Hole>();
            if (h != null)
            {
                //if and only if a mole was hit, increase the combo. Otherwise reset to 0
                if (h.Hit())
                {
                    hitComboCount++;
                    controller.IncreaseScore(hitComboCount);
                }
            }
            else
                hitComboCount = 0;
        }
    }

    //DoSwing updates the hammer position during the swing animation using linear interpolation
    private void DoSwing()
    {
        swinging = true;

        if(swingTime > swingAnimTime)
        {
            swinging = false;
            swingTime = 0;
            hammer.SetActive(false);
            return;
        }

        //starting position
        Vector3 upPos = new Vector3(hammer.transform.position.x, hammerHeight, hammer.transform.position.z);
        //target position
        Vector3 downPos = new Vector3(hammer.transform.position.x, 0, hammer.transform.position.z);

        //First half of the frames the hammer moves up->down,
        //next half back down->up, swapping start and target position
        if (swingTime < swingAnimTime / 2)
            hammer.transform.position = Vector3.Lerp(upPos, downPos, swingTime / swingAnimTime);
        else
            hammer.transform.position = Vector3.Lerp(downPos, upPos, swingTime / swingAnimTime);

        swingTime += Time.deltaTime;
    }
}
