using System;
using System.Collections.Generic;


namespace PerfectlyParanormal.YieldRoutines
{
    public static class TaskHelpers
    {
       

        public static WaitForStateTransition WaitForStateTransition(this PerfectlyParanormal.FSM.StateMachineBehaviour st)
        {
            return new WaitForStateTransition(st);
        }
    }
}
