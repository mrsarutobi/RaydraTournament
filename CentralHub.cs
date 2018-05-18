using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace RaydraTournament
{
    /*! \class CentralHub
   This class is used for the creation of the main control center for all the activity 
   that goes on in the game it creates and keeps tract of the characters that are in the level.
   It will as remove any players that need to be removed from the level

 */
    public class CentralHub : MonoBehaviour
    {


       static readonly public COSINE COS = new COSINE(1800); //! A class the contains the consine math data
       static readonly public SINE SIN= new SINE(1800); //! A class that contains sin data information


        /**The Awake medthod is called before the game starts and is used to initialize variables"*/
        void Awake()
        {
      

      

    }

        void Start()
        {

        }


        void Update()
        {

        }




        /*! This method adds a Player to the list of players 
         
             \param Player this is a reference to the player that will
             be added to the scene*/
        void AddPlayer(RaydraPlayerz Player) //adds a player to the current List
        {

       


        }

        /*! A class that has an read only indexer to return cos data */
        public class COSINE
        {
            private float cosoffset;//!the offset for the cosine array lookup-table
            private float[] cos; //! COSINE Look up table
            private int indexjump;// this is number of value each angle angle is spit into
            private float [] offsetslookup; //this angle jump value for fast look up
            public COSINE( float size)
            {
              // size = 0;
             // while (size < Camera.main.pixelWidth)
                   // size += 360;
                 

                cos = new float[(int)size];
                cosoffset = 360 / size; //were are diving each angle in the look up table so angle by the cosoffset value each time its incremented or decremented
                float offset = 0;
                float currentangle;
                indexjump = (int)size / 360; // this number of indices in the array to jump for each angle between 0 and 359
                offsetslookup = new float [indexjump];

                for (int i = 0; i < indexjump; ++i)
                    offsetslookup[i] = i * cosoffset;



                for (int i = 0; i < cos.Length; ++i)
                {
                    //the current angle stays with the proper index range and then add the offest to it
                     currentangle = (int)(i/ indexjump) + ( offset);
                    //convert the input to radians
                    //I really should be using an exception here to catch memory allocation errors
                    cos[i] = (float)Mathf.Cos(currentangle * (Mathf.PI / 180f));
                   // cos[cos.Length/4] = 0;
                   // cos[cos.Length / 2] = -1;
                    //cos[(int)(cos.Length *.75f)] = 0;

                    offset += cosoffset;

                    if (offset >= 1f)
                        offset = 0;
                            

                }

         
            }


            public int IndexJump
            {
                get { return indexjump; }
            }


            public float CosOffSet{
                    get {return cosoffset;}
                }
            public float this[int index]
            {
                get { return cos[index]; }

            }
            /*!This property returns an array angle offset values to lookup */
            public float [] Offsetslookup
            {
                get { return offsetslookup; }
            }

        }//end of COS class
    /*! A class that has an read only indexer to return sin data */
        /*! A class that has an read only indexer to return sin data */
        public class SINE
        {

            private int indexjump;

            private float sinoffset;//!the offset for the sin array lookup-table
            private float[] sin; //! COSINE Look up table
            private float[] offsetslookup;

            public SINE(float size)
            {
                //size = 0;
              //  while (size < Camera.main.pixelWidth)
                   // size += 360; 
                    

                sinoffset = 360 / size; //
                sin = new float[(int)size];
                indexjump = (int) size/360; //

                offsetslookup = new float[indexjump];

                for (int i = 0; i < indexjump; ++i)
                    offsetslookup[i] = i * sinoffset;

                float offset = 0;
                //convert the input to radians
                //I really should be using an exception here to catch memory allocation errors
                for (int i = 0; i < sin.Length; ++i)
                {
                    //convert the input to radians
                    //I really should be using an exception here to catch memory allocation errors

                    float currentangle = (int)(i/ indexjump) + ( offset);

                    sin[i] = (float)Mathf.Sin(currentangle * (Mathf.PI / 180f));
                    offset += sinoffset;

                    if (offset >= 1f)
                        offset = 0;

                }
               // sin[0] = 0;
               // sin[(int)(sin.Length / 4)] = 1;
              //  sin[(int)(sin.Length / 2)] = 0;
               // sin[(int)(sin.Length * .75f)] = -1;

            }
            /*!This property returns an array angle offset values to lookup */
            public float[] Offsetslookup
            {
                get { return offsetslookup; }
            }

            public float SinOffSet
            {
                get { return sinoffset; }
            }


    public int IndexJump
    {
        get { return indexjump; }
    }

    public float this[int index]
        {
            get { return sin[index]; }

        }


        }//end of SIN class
       
        /** converts the vector in the x,z(ground) plane into*/
      static public float VectorToAngle(Vector3 DestinationVector) //converts a vector to a angle
        {

            float DestinationAngle = Vector2.Angle(new Vector2(1, 0), new Vector2(DestinationVector.x, DestinationVector.z));

            DestinationAngle = AdjustAngle(DestinationAngle);

            if (DestinationVector.z < 0)
                DestinationAngle = 360 - DestinationAngle;

            return DestinationAngle;
        }

        /*! This medthod makes sure the the angle does not go beyond 360 or less than 0 */
        static public  float AdjustAngle(float Angle)
        {
            if ((int)Angle > 359)
                Angle %= 360;
            else
             if (Angle < 0)
                Angle += 360;

            return Angle;

        }

    }//end of class
}//end of namespace

