/*****************************************************************************
* Project: AiSheep
* File   : Actor.cs
* Date   : 25.11.2021
* Author : Jan Apsel (JA)
*
* These coded instructions, statements, and computer programs contain
* proprietary information of the author and are protected by Federal
* copyright law. They may not be disclosed to third parties or copied
* or duplicated in any form, in whole or in part, without the prior
* written consent of the author.
*
* History:
*   23.11.2021	JA	Created
******************************************************************************/

using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// Actor gets a move direction from house to field to mill to market
/// and back to house
/// </summary>
public class Actor : MonoBehaviour
{
    [SerializeField] GameObject actorObj;
    NavMeshAgent agent;
    Vector3 target;
    ObjectBehaviour oTarget;
    float wait;
    public House home { private get; set; }
    MapGen mapGen;
    bool newDestination = true;
    bool isInside = false;

    private void Awake()
    {
        mapGen = GameObject.FindObjectOfType<MapGen>().GetComponent<MapGen>();
        agent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {

    }
    private void FixedUpdate()
    {
        if (newDestination)
            if (CheckDestinationReached())
            {
                newDestination = false;
                EnterAction(oTarget);
            }

    }
    public void EnterAction(ObjectBehaviour _object)
    {
        //QuestionNextTarget(_object.ReturnType());
        if (_object.ReturnType() == eObject.Field) isInside = false;
        else isInside = true;
        oTarget = QuestionNextTarget(_object.ReturnType());
        target = oTarget.ReturnMovePos();
        wait = _object.ReturnWaitTime();
        Wait();
    }

    public void ReceiveEnter(ObjectBehaviour _object)
    {
        //QuestionNextTarget(_object.ReturnType());
        //wait = _object.ReturnWaitTime();
        //Wait();
    }
    void Wait()
    {
        if (isInside) actorObj.SetActive(false);

        Invoke("GoToTarget", wait);
    }
    void GoToTarget()
    {
        newDestination = true;
        actorObj.SetActive(true);
        agent.SetDestination(target);
    }
    ObjectBehaviour QuestionNextTarget(eObject posType)
    {
        switch (posType)
        {
            case eObject.House:
                return mapGen.fieldLst[Random.Range(0, mapGen.fieldLst.Count - 1)];
            case eObject.Field:
                return mapGen.millLst[Random.Range(0, mapGen.millLst.Count - 1)];
            case eObject.Market:
                return home;
            case eObject.Mill:
                return mapGen.marketLst[Random.Range(0, mapGen.marketLst.Count - 1)];
            default:
                return home;
        }
    }

    [SerializeField] float closeDistance;
    bool CheckDestinationReached()
    {
        Vector3 offset = target - transform.position;
        float sqrLen = offset.sqrMagnitude;

        if (sqrLen < closeDistance * closeDistance)
        {
            return true;
        }
        return false;
    }
}