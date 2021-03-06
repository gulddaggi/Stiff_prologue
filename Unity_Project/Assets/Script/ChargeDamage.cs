using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeDamage : MonoBehaviour
{

    [SerializeField]
    private Transform player;

    [SerializeField]
    private Status status;


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            status.DecreaseHP(5);
        }
    }
}
