using UnityEngine;
using System.Collections;


namespace RaydraTournament
{
    public class RaydraCell : Component
    {



        public RaydraCell()
        {

            CellState = CellState.IsInHand; //!Initalize the orb to be in the hand of the player
            //CellPlayState = CellAnimationState.isNotPlaying;

        }



        public CellState CellState
        { //! This property is the current state of the ball
            get;
            set;
        }


       public CellAnimationState CellPlayState
        { //! This property is the state of orb Animaton and  indicate weather or not the animatin is still playing
            get;
            set;
        }




    }

}

