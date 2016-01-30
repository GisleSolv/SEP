using UnityEngine;
using System.Collections;
using System;

namespace PerfectlyParanormal.FSM
{

    public class StateMapping
    {
        public Enum state;

        public Func<Enum, Hashtable, IEnumerator> Enter = StateMachineBehaviour.DoNothingStateChange;
        public Func<Enum, Hashtable, IEnumerator> Exit = StateMachineBehaviour.DoNothingStateChange;
        public Action<Enum> LateEnter = StateMachineBehaviour.DoNothing;
        public Action<Enum> TransitionUpdate = StateMachineBehaviour.DoNothing;
        public Action Update = StateMachineBehaviour.DoNothing;
        public Action LateUpdate = StateMachineBehaviour.DoNothing;
        public Action FixedUpdate = StateMachineBehaviour.DoNothing;

        public StateMapping()
        {
            //this.state = state;
        }
        
    }
}