using System;
using System.Collections;
using UnityEngine;
using PerfectlyParanormal.FSM;

namespace PerfectlyParanormal.YieldRoutines
{
    /// <summary>
    /// Custom yield routines based on CustomYieldInstruction, yields until StateMachineBehaviour is no longer in transition
    /// </summary>
    public class WaitForStateTransition : CustomYieldInstruction
    {
        private PerfectlyParanormal.FSM.StateMachineBehaviour m_state;

        public override bool keepWaiting
        {
            get
            {
                return m_state.InTransition;
            }
        }

        public WaitForStateTransition(PerfectlyParanormal.FSM.StateMachineBehaviour st)
        {
            m_state = st;
        }

        

    }
}
