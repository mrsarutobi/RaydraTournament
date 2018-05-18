using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace RaydraTournament
{
 
    class RaydraCharger
    {
        /*! \class RaydraCharger 
     The class controls the charging ablilities of the weapon */

        //! private field that indicates weither or not the weapon is charged
        private bool IsCharged;
        //!The Timer class that determines how long the weapon takes to charge
        private X8Timer ChargeTimer= new X8Timer(1f);

        /*! Constructor for the charger that takes a charge time and intializes the private variables
         * \param ChargeTime is the time that the charger will take to fully charge
         *  */
        public RaydraCharger(float ChargeTime)
        {       if (ChargeTime < 1f)
                  ChargeTime = 1f;
                ChargeTimer = new X8Timer(ChargeTime);
                ChargeTimer.TimeLimitReached = FullyCharged; //call is medthod whenever the timer has reached its limit
                IsCharged = false;

        }

        //! public value that is set to true when the weapon is fully charged
        public bool IsFullyCharged 
        {
            get{return IsCharged;}

        }

        /*! Medthods that charges the weapon*/
        public void Charging()
        {
           ChargeTimer.IncrementTimer();

           
        }

        /*! Enables the charger */
        public void EnableCharger()//
        {

            ChargeTimer.TurnTimerOn();
        }

        /*! This is called when the charger reaches its time limit */
        public void FullyCharged()
        {
            IsCharged = true;
            return;
        }


        //! Indicates if the weapon is charging 
        public bool IsCharging
        {

            get
            {
              return ChargeTimer.IsEnabled;

            }

     }

        /*! Resets the weapon charge */
        public void ChargerReset()//resets the timer
        {
           
            IsCharged = false;
            ChargeTimer.Reset();
        }
       
    }
}
