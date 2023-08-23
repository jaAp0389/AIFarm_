/*****************************************************************************
* Project: AiSheep
* File   : MonoBehaviour.cs
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour, ObjectBehaviour
{
    [SerializeField] public float waitTime;
    [SerializeField] public GameObject trigger;
    //public Vector3 pos;
    public Vector3 ReturnMovePos()
    {
        return transform.position + new Vector3(Random.Range(-0.4f, 0.4f), 0, Random.Range(-0.4f, 0.4f));
    }
    public float ReturnWaitTime()
    {
        return Random.Range(1, waitTime);
    }
    public eObject ReturnType()
    {
        return eObject.Field;
    }
}
