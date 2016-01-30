using UnityEngine;
using System.Collections;
using PerfectlyParanormal.FSM;
using System;

public class StateMachineTest : PerfectlyParanormal.FSM.StateMachineBehaviour
{

    public enum States
    {
        Normal,
        Limp,
        Paralyzed
    }

    void Awake()
    {
        InitializeStates<States>(typeof(StateMappingTest));
        
    }

    // Use this for initialization
    void Start()
    {
        ChangeState(States.Normal);
    }

    #region Normal
    IEnumerator Normal_Enter(Enum prevState)
    {
        Debug.Log("Enter normal "+prevState.ToString());
        yield return null;
    }

    void Normal_Update()
    {
        Debug.Log("Update normal");

        if (Input.GetKeyDown(KeyCode.A))
        {
            ChangeState(States.Limp);
        }
    }

    void Normal_Bonkers()
    {
        Debug.Log("Normal bonkers");
    }

    #endregion

    #region Limp
    IEnumerator Limp_Enter(Enum prevState)
    {
        Debug.Log("Enter Limp " + prevState.ToString());
        yield return new WaitForSeconds(5);

        Debug.Log("Limp Entered");
    }

    void Limp_Update()
    {
        Debug.Log("Update Limp");

        if (Input.GetKeyDown(KeyCode.A))
        {
            ChangeState(States.Paralyzed);
        }
    }

    void Limp_Bonkers()
    {
        Debug.Log("Limp bonkers");
    }

    #endregion


    public override void Default_Update()
    {
        Debug.Log("Update Default");

        if (Input.GetKeyDown(KeyCode.A))
        {
            ChangeState(States.Normal);
        }
    }

    public override void Default_FixedUpdate()
    {

        if (Input.GetKeyDown(KeyCode.B))
        {
            ((StateMappingTest)CurrentState).Bonkers();
        }
    }

    public override void OnStateMethodBinding(StateMapping s, System.Enum stateName)
    {
        base.OnStateMethodBinding(s, stateName);

        ((StateMappingTest)s).Bonkers = ConfigureDelegate<Action>("Bonkers", stateName, DoNothing);
    }

}
