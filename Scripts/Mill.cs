/*****************************************************************************
* Project: AiSheep
* File   : Mill.cs
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

public class Mill : MonoBehaviour, ObjectBehaviour
{
    [SerializeField] public float waitTime;
    [SerializeField] public GameObject doorObj;
    [SerializeField] GameObject door;
    public Vector3 ReturnMovePos()
    {
        return door.transform.position;
    }
    public float ReturnWaitTime()
    {
        return waitTime;
    }
    public eObject ReturnType()
    {
        return eObject.Mill;
    }
}
