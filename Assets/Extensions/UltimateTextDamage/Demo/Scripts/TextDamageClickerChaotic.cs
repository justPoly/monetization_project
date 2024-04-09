using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Guirao.UltimateTextDamage
{
    public class TextDamageClickerChaotic : MonoBehaviour
    {
        public UltimateTextDamageManager textManager;
        public Transform overrideTransform;

        void Start( )
        {
            if( textManager == null )
                textManager = FindObjectOfType<UltimateTextDamageManager>( );
        }
        private void OnMouseUpAsButton( )
        {
                textManager.Add( "-" + 5, overrideTransform != null ? overrideTransform : transform,"critical");  
        }

        public bool autoclicker = true;
        public float clickRate = 1;

        float lastTimeClick;
        private void Update( )
        {
            
            if(Input.GetKeyDown(KeyCode.Space))
            {
                //lastTimeClick = Time.time;
                textManager.Add(5f.ToString(), overrideTransform != null ? overrideTransform : transform, "default");
            }
           
            
        }
    }
}
