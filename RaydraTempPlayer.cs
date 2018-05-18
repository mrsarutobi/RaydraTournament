using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RaydraTournament
{
    public class RaydraTempPlayer : RaydraPlayerz
    {
        // Use this for initialization
        void Start()
        {
            innerShield = gameObject.GetComponentInChildren<RaydraShield>(); //!search for the sheild in the children of the parent object

        }

        // Update is called once per frame
        void Update()
        {

        }


    }
}