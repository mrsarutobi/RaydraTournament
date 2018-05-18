using UnityEngine;
using System.Collections;


namespace RaydraTournament
{

public class ForceGenerator 
{
        /*!The contrustor for the force Generator class */
        /* \param Entity this is the game object that the force will act on*/
		public ForceGenerator(GameObject Enitity) 
												
		{
		

		}

	
	public Vector3 Force; /*! Force is the actual force vector that will be applied to the game object */

	/*! This medthod updates the force every frame so the the physics can be properly applied.
        \param RPlayer is will update the the Raydra players forces that are being acted upon it
            */
	public void updateForce(RaydraPlayer RPlayer)
	{
			//calculate the new force for this frame and add it to the Force vector variable
	
	}


}


    public class ForceRegistration 
	{
        /*! class ForceRegistration 
that contains an object in the world and a force that can effect it 
    this
    */
        /*!Constructor for the force generator class*/
           public ForceRegistration(){

           }

        public	GameObject GameEntity;/*!any object, player or otherwise that can have a force exerted on it*/
        public ForceGenerator fg; /*the type of force generator that is registred with the object.*/
				
		
	}




	public class GameSpringGenerator: ForceGenerator
	{


		public GameSpringGenerator(GameObject GameEntity):base(GameEntity)
		{


		}


			


	}



}

