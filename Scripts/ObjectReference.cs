/*****************************************************************************
* Project: AiSheep
* File   : ObjectReference.cs
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

public class ObjectReference
{
    public eObject eObj;
    public Vector2Int pos;

    public ObjectReference(eObject _eObj, Vector2Int _pos)
    {
        eObj = _eObj;
        pos = _pos;
    }
}
