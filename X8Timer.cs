using UnityEngine;
using System.Collections;



namespace RaydraTournament
{
    /*!* \class X8Timer 
     * a timer class */
    public class X8Timer  {


        public delegate void LimitReached();//!delegate that fires when the time limit is reached
        private float CurrentTimeInSeconds = 0f;//! The number frames that have elapsed since the timer was fire
        public LimitReached TimeLimitReached;//reference to the delegate for
              


        /*The amount of time in seconds this timer will last"*/

        private bool TimerOn;//!Indicates if the timer is turn on or ot
        private bool Pause=false;//!



        private float  TimeLimit;//!The timer limit for the timer

        /*!contrucstor for the timer class thiat take a time limit 
         *  \param LifeLimit this is the  time limit on the timer. 
         *  When timer is started the timer will keep going until the Lifelimt is reached 
         *  or the Reset() medthod is called  
             \sa Reset()
             
             */
        public X8Timer(float LifeLimit)
        {

            TimeLimit = LifeLimit; //initialize how long the object will live
          //initialize the current time in frimas
            TimerOn = false; //the timer is initialy not on
            TimeLimitReached = null;
        }

        // Use this for initialization


        // Update is called once per frame

        /** Update is called once per frame */
        public void IncrementTimer() {


            if (Pause == false) //if the timer is not pause
            {
                if (TimerOn == true)// if the timer is set to on mode then we count
                    CurrentTimeInSeconds += Time.deltaTime;// 1f / 60f;//update the timer;

                if (CurrentTimeInSeconds > TimeLimit)  //! if we go over the time limit reset the timer values
                {
                    
                    TimerOn = false; //reset the timer 
                    CurrentTimeInSeconds = 0f;
                    if (TimeLimitReached != null)
                        TimeLimitReached();

     
                }

            }
	}

        /*! how many frames the timer has been running  for */
        public float AgeInFrames
    {
        get
        { return CurrentTimeInSeconds* 60f;}
           
   }
        /*! how long the Timer has been ticking in seconds */
        public float AgeinSeconds
         {
        get
        {        return CurrentTimeInSeconds; } //this is will give us the frames in seconds;
        }
        
        /*! Indicate weather or not the timer is on or not*/
        public bool IsEnabled
        {
            get { return TimerOn; }
            

        }
        /*!The time limit for the timer*/
        public float  AgeLimit
            {
                get {return TimeLimit;}
            }
        /*!  this medthod will turn the timer on */
        public void TurnTimerOn()//
        {
            if(TimerOn== false)
            TimerOn = true;
        }
        /*!  Pause the timer */
        public void PauseTimer()
        {
            //resest the state of the timer

            Pause = true;

        }
        /*! Unpause the timer */
        public void UnPauseTimer()
        {
            //resest the state of the timer

            Pause = false;

        }
        /*!  Resets the timer */
        public void Reset()
        {
            //resest the state of the timer
            TimerOn = false;
            CurrentTimeInSeconds = 0f;

        }


    }


}

