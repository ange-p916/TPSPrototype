using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{

    public LayerMask enemies;

    public float health;
    public float armor;

    public float ammo;

    public float damage;
    
    private void Start()
    {
        int last = 0;

        health = 100f;
        armor = 0f;
    }
    

    private void Update()
    {
        RaycastHit hit;

        if(Input.GetMouseButtonDown(0))
        {
            if(Physics.Raycast(transform.position, Vector3.forward, out hit))
            {
                if(hit.transform.gameObject.layer == 12)
                {

                }
            }
        }
    }
}
