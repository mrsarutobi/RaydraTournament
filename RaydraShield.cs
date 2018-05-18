using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RaydraTournament
{
    public class RaydraShield : MonoBehaviour
    {
        private IEnumerator coroutine;

        X8Timer ShieldTimer = new X8Timer(7f); //!This timer  determine how long it take to recharge the shields after being hit

        /*! \class RaydraShield 
          this class handles the sheild for the players

             */

        //!this is how much of the shield power the player currently has
        private uint shieldPercentage = 100;
        // Use this for initialization
        void Start()
        {
            ShieldTimer.TimeLimitReached = RechargeShield; //when the timer has reached its limit the RechargeShield will be called
        }

        //when the player is shoot with a bullet the sheilds go doen
        //  void  OnCollisionStay(Collision Entity)


        void DecreaseShield()
        {

       




            shieldPercentage -= 5; //decrement the shields when the shield are hit



            if (shieldPercentage < 0)
                shieldPercentage = 0; //make sure we stay in  the bounds of the number we want

            if (shieldPercentage > 100)
                shieldPercentage = 100; //make sure we stay in  the bounds of the number we want



            ShieldTimer.Reset(); //reset the time to 
            ShieldTimer.TurnTimerOn(); //turn the timer back on



        }
        
        void OnTriggerEnter(Collider Entity)
        {
            if (Entity.gameObject.tag == "Bolt")
                DecreaseShield();        
     


        }


        //! Sets the shield back to 100 percent
        void RechargeShield()
        {
            shieldPercentage = 100;
        }
        

     
        // Update is called once per frame
        void FixedUpdate()
        {
            
            if (shieldPercentage < 100)
                ShieldTimer.IncrementTimer();



        }
    }
}