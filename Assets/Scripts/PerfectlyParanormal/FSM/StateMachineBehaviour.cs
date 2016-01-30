/*
 * Copyright (c) 2012 Made With Mosnter Love (Pty) Ltd
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to 
 * deal in the Software without restriction, including without limitation the 
 * rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the 
 * Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included 
 * in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR 
 * OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
 * ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
 * OTHER DEALINGS IN THE SOFTWARE.
 */

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace PerfectlyParanormal.FSM
{

    public class StateMachineBehaviour : MonoBehaviour
    {
        private StateMapping m_transitionState;
        public StateMapping TransitionState { get { return m_transitionState; } }
        private StateMapping _currentState;
        private StateMapping m_currentState
        {
            get { return _currentState; }
            set
            {
                _currentState = value;
                OnStateUpdated(_currentState);
            }
        }
        public StateMapping CurrentState
        {
            get
            {
                return m_currentState;
            }
        }
        public Enum State
        {
            get
            {
                if (m_currentState == null)
                    return  NoneState.None;
                return m_currentState.state;
            }
        }

        public enum NoneState
        {
            None
        }

        private Dictionary<Enum, StateMapping> m_stateLookup;

        private bool m_isInTransition = false;
        public bool InTransition
        {
            get { return m_isInTransition; }
        }

        private bool m_stateLocked = false;
        public bool StateLocked
        {
            get { return m_stateLocked; }
        }

        // Use this for initialization
        void Start()
        {
            
        }

        public virtual void OnStateUpdated(StateMapping s) { }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (m_currentState != null)
            {
                m_currentState.FixedUpdate();
            }
        }

        void Update()
        {
            if (m_currentState != null && !m_isInTransition)
            {
                m_currentState.Update();
            }
            else if (m_currentState != null && m_isInTransition)
            {
                m_currentState.TransitionUpdate(m_transitionState.state);
            }
        }

        void LateUpdate()
        {
            if (m_currentState != null && !m_isInTransition)
            {
                m_currentState.LateUpdate();
            }
        }

        public void InitializeStates<T>(Type sm) 
        {
            m_stateLookup = new Dictionary<Enum, StateMapping>();

            var values = Enum.GetValues(typeof(T));

            for (int i = 0; i < values.Length; i++)
            {
                StateMapping m = (StateMapping)GetNewStateMapping(sm);
                m.state = (Enum)values.GetValue(i);
                m_stateLookup.Add(m.state, m);
            }

            bindStates();
        }

        private void bindStates() {

            foreach(KeyValuePair<Enum, StateMapping> entry in m_stateLookup) {
                OnStateMethodBinding(entry.Value, entry.Key);
            }

        }

        public void SetState(Enum newState, bool force=false)
        {
            if (m_stateLocked && !force) return;

            if (!m_stateLookup.ContainsKey(newState))
            {
                throw new Exception("No state with the name " + newState.ToString() + " can be found. Please make sure you are called the correct type the statemachine was initialized with");
            }

            m_currentState = m_stateLookup[newState];
            OnStateUpdated(m_currentState);
        }

        public void LockState(bool locked)
        {
            m_stateLocked = locked;
            if(locked)
                Debug.Log("SamStates: STATE LOCKED: " + m_currentState.state.ToString());
            else Debug.Log("SamStates: STATE UNLOCKED: " + m_currentState.state.ToString());
        }

        public bool ChangeState(Enum newState, Hashtable data=null, bool transitionToSelf=false)
        {
            if (m_stateLocked) return false;

            if (m_stateLookup == null)
            {
                throw new Exception("States have not been configured, please call initialized before trying to set state");
            }

            if (!m_stateLookup.ContainsKey(newState))
            {
                throw new Exception("No state with the name " + newState.ToString() + " can be found. Please make sure you are called the correct type the statemachine was initialized with");
            }

            var nextState = m_stateLookup[newState];
            
            if (m_currentState == nextState && !transitionToSelf) return false;

            if (m_isInTransition)
            {
                if (m_transitionState == nextState)
                    return false;
                return false;
            }
            m_transitionState = nextState;
            m_isInTransition = true;
            StartCoroutine(ChangeToNewStateRoutine(nextState, data));
            return true;
        }

        private IEnumerator ChangeToNewStateRoutine(StateMapping newState, Hashtable data)
        {
            m_isInTransition = true;

            if (m_currentState != null)
            {

                var exitRoutine = m_currentState.Exit(newState.state, data);

                if (exitRoutine != null)
                {
                    //yield return StartCoroutine(RadicalRoutine.Run(exitRoutine));
                    yield return StartCoroutine(exitRoutine);
                }
            }
            Enum prevState = m_currentState!=null ? m_currentState.state : NoneState.None;
            m_currentState = newState;
            Debug.Log("SamStates: CHANGED TO: "+m_currentState.state.ToString()+" FROM: "+prevState.ToString());
            
            if (m_currentState != null)
            {
                var enterRoutine = m_currentState.Enter(prevState, data);

                if (enterRoutine != null)
                {
                    //yield return StartCoroutine(RadicalRoutine.Run(enterRoutine));
                    yield return StartCoroutine(enterRoutine);
                }
            }

            m_isInTransition = false;

            m_currentState.LateEnter(prevState);
        }

        public virtual void OnStateMethodBinding(StateMapping s, Enum stateName)
        {
            s.Enter = ConfigureDelegate<Func<Enum, Hashtable, IEnumerator>>("Enter", stateName, DoNothingStateChange);
            s.Exit = ConfigureDelegate<Func<Enum, Hashtable, IEnumerator>>("Exit", stateName, DoNothingStateChange);
            s.LateEnter = ConfigureDelegate<Action<Enum>>("LateEnter", stateName, DoNothing);
            s.Update = ConfigureDelegate<Action>("Update", stateName, Default_Update);
            s.TransitionUpdate = ConfigureDelegate<Action<Enum>>("TransitionUpdate", stateName, DoNothing);
            s.FixedUpdate = ConfigureDelegate<Action>("FixedUpdate", stateName, Default_FixedUpdate);
            s.LateUpdate = ConfigureDelegate<Action>("LateUpdate", stateName, Default_LateUpdate);
        }

        //define a generic method that returns a delegate
        //note the where clause - we need to ensure that the type passed in is a class and not a value type or our cast (As T) will not work
        protected T ConfigureDelegate<T>(string methodRoot, Enum state, T Default) where T : class
        {
            //find a method called CURRENTSTATE_METHODROOT
            //this method can either be public or private
            var mtd = GetType().GetMethod(state.ToString() + "_" + methodRoot,
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

        public static T GetNewStateMapping<T>(Enum e) where T : StateMapping, new()
        {
            return new T{state=e};
        }

        public static object GetNewStateMapping(Type t) 
        {
            return t.GetConstructor(new Type[]{}).Invoke(new object[]{});
            //return new t { state = e };
        }

        public virtual void Default_Update(){}
        public virtual void Default_FixedUpdate() { }
        public virtual void Default_LateUpdate() { }

        public static void DoNothing()
        {
        }
        public static void DoNothing(Enum prevState)
        {
        }
        public static void DoNothingCollider(Collider other)
        {
        }
        public static void DoNothingCollision(Collision other)
        {
        }
        public static IEnumerator DoNothingCoroutine()
        {
            yield return null;
        }

        public static IEnumerator DoNothingStateChange(Enum s, Hashtable data)
        {
            yield return null;
        }
    }
}