using UnityEngine;
using System.Collections;


 namespace RaydraTournament
{

    /* \enum CellState  an enumeration of the state of the RaydraCell */
    public enum CellState
	{
        //! The ball is in the air 
        IsInAir,
        /*!The ball is on the ground*/
        IsOnGround,
        /*! The player in pocession of the ball*/
        IsInHand,
        /*<The ball is being thrown  */


    }


    /* \enum  Enumeration of what caused the  cell to become airborne  */
    public enum CellMotionState
	{
        //!The cell is not doing anything special
        Neutral,
        //!  The cell was thrown from a players hand
        wasThrownFromHand,
        //!  The cell was shot from a player weapon
        wasShotFromWeapon,
  
        
           

    }




}

