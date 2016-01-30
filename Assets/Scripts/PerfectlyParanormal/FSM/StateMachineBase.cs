using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace PerfectlyParanormal.FSM
{

    public class StateMachineBase : MonoBehaviour
    {

        public Action DoUpdate = DoNothing;
        public Action DoLateUpdate = DoNothing;
        public Action DoFixedUpdate = DoNothing;
        public Action<Collider> DoOnTriggerEnter = DoNothingCollider;
        public Action<Collider> DoOnTriggerStay = DoNothingCollider;
        public Action<Collider> DoOnTriggerExit = DoNothingCollider;
        public Action<Collision> DoOnCollisionEnter = DoNothingCollision;
        public Action<Collision> DoOnCollisionStay = DoNothingCollision;
        public Action<Collision> DoOnCollisionExit = DoNothingCollision;
        public Action DoOnMouseEnter = DoNothing;
        public Action DoOnMouseUp = DoNothing;
        public Action DoOnMouseDown = DoNothing;
        public Action DoOnMouseOver = DoNothing;
        public Action DoOnMouseExit = DoNothing;
        public Action DoOnMouseDrag = DoNothing;
        public Action DoOnGUI = DoNothing;
        public Func<IEnumerator> ExitState = DoNothingCoroutine;

        public List<Action> test = new List<Action>();

        private Enum _currentState;
        public Enum currentState
        {
            get
            {
                return _currentState;
            }
            set
            {
                _currentState = value;
                ConfigureCurrentState();
            }
        }


        // Use this for initialization
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            DoUpdate();
        }

        static void DoNothing() { }
        static void DoNothingCollider(Collider col) { }
        static void DoNothingCollision(Collision col) { }
        static IEnumerator DoNothingCoroutine() { yield return null; }

        void ConfigureCurrentState()
        {
            //if we have an exit state, then start it as a coroutine

            if (ExitState != null)
                StartCoroutine(ExitState());

            //now we need to configure all of the methods
            DoUpdate = ConfigureDelegate<Action>("Update", DoNothing);
            DoOnGUI = ConfigureDelegate<Action>("OnGUI", DoNothing);
            DoLateUpdate = ConfigureDelegate<Action>("LateUpdate", DoNothing);
            DoFixedUpdate = ConfigureDelegate<Action>("FixedUpdate", DoNothing);

            DoOnMouseUp = ConfigureDelegate<Action>("OnMouseUp", DoNothing);
            DoOnMouseDown = ConfigureDelegate<Action>("OnMouseDown", DoNothing);
            DoOnMouseEnter = ConfigureDelegate<Action>("OnMouseEnter", DoNothing);
            DoOnMouseExit = ConfigureDelegate<Action>("OnMouseExit", DoNothing);
            DoOnMouseDrag = ConfigureDelegate<Action>("OnMouseDrag", DoNothing);
            DoOnMouseOver = ConfigureDelegate<Action>("OnMouseOver", DoNothing);

            DoOnTriggerEnter = ConfigureDelegate<Action<Collider>>("OnTriggerEnter", DoNothingCollider);
            DoOnTriggerExit = ConfigureDelegate<Action<Collider>>("OnTriggerExit", DoNothingCollider);
            DoOnTriggerStay = ConfigureDelegate<Action<Collider>>("OnTriggerStay", DoNothingCollider);

            DoOnCollisionEnter = ConfigureDelegate<Action<Collision>>("OnCollisionEnter", DoNothingCollision);
            DoOnCollisionExit = ConfigureDelegate<Action<Collision>>("OnCollisionExit", DoNothingCollision);
            DoOnCollisionStay = ConfigureDelegate<Action<Collision>>("OnCollisionStay", DoNothingCollision);

            Func<IEnumerator> enterState = ConfigureDelegate<Func<IEnumerator>>("EnterState", DoNothingCoroutine);
            ExitState = ConfigureDelegate<Func<IEnumerator>>("ExitState", DoNothingCoroutine);

            //Optimization, turn off GUI if we dont have an OnGUI method

            //EnableGUI();

            //Start the current state
            StartCoroutine(enterState());
        }


        //define a generic method that returns a delegate
        //note the where clause - we need to ensure that the type passed in is a class and not a value type or our cast (As T) will not work
        T ConfigureDelegate<T>(string methodRoot, T Default) where T : class
        {
            //find a method called CURRENTSTATE_METHODROOT
            //this method can either be public or private
            var mtd = GetType().GetMethod(_currentState.ToString() + "_" + methodRoot, 
                System.Reflection.BindingFlags.Instance | 
                System.Reflection.BindingFlags.Public | 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.InvokeMethod);

            //if we found a method
            if (mtd != null)
            {
                //Create a delegate of the type that this generic instance needs and cast it
                return Delegate.CreateDelegate(typeof(T), this, mtd) as T;
            }
            else
            {
                //if we didnt find a method return the default
                return Default;
            }
        }
    }

}