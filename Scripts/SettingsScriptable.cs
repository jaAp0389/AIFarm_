/*****************************************************************************
* Project: AiSheep
* File   : SettingsScriptable.cs
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

[CreateAssetMenu(fileName = "SettingsScriptable", menuName = "ScriptableObjects/SettingsScriptable", order = 1)]
public class SettingsScriptable : ScriptableObject
{
    [Header("Map")]
    public int mapSize;
    public float objDistance;
    public int mainNew;
    public int neighboursNew;
    public int neighbourRepeat;


    [Header("Noise")]
    public ObjectData[] MainObjects;
    public ObjectData[] NeighbourObjects;
}