using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    [SerializeField] private GameObject doorOpeningLeft;
    [SerializeField] private GameObject doorOpeningRight;

    private bool open = false;


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!open)
            {
                if (doorOpeningLeft != null)
                {
                    doorOpeningLeft.SetActive(false);
                }
                if (doorOpeningRight != null)
                {
                    doorOpeningRight.SetActive(false);
                }
                open = true;
            }
    
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (open)
            {
                if (doorOpeningLeft != null)
                {
                    doorOpeningLeft.SetActive(true);
                }
                if (doorOpeningRight != null)
                {
                    doorOpeningRight.SetActive(true);
                }
                open = false;
            }
        }
    }
}
