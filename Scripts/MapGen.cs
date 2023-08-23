/*****************************************************************************
* Project: AiSheep
* File   : MapGen.cs
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


/// <summary>
/// Generates a map. Maintains a array map of objectcontainers, picks random 
/// positions to place objects and checks if there are only empty neighbours
/// or neighbours of a defined type before placement. So fields can spawn next 
/// to mills and houses next to markets but not the other way around.
/// </summary>
public class MapGen : MonoBehaviour
{
    [SerializeField] SettingsScriptable currSet;
    [SerializeField] GameObject floorObj;
    [SerializeField] GameObject actorTmpl;

    ObjectData[,] objField;
    List<ObjectReference> objRefList;
    public List<House> houseLst;
    public List<Mill> millLst;
    public List<Field> fieldLst;
    public List<Market> marketLst;

    //list house etc, actor controller-> setdestination, triggerareas, door placement

    private void Awake()
    {
        matArray = CreateMaterials();

        objRefList = new List<ObjectReference>();
        houseLst = new List<House>();
        millLst = new List<Mill>();
        fieldLst = new List<Field>();
        marketLst = new List<Market>();

        floorObj.transform.position = 
            new Vector3(currSet.mapSize/2, 0, currSet.mapSize/2);

        float mapSizeRaw = currSet.mapSize * currSet.objDistance;

        floorObj.transform.localScale = new Vector3(mapSizeRaw, 0, mapSizeRaw);
    }

    private void Start()
    {
        GenerateMap();
        PlaceObjects();
        CreateActors();
    }

    public void GenerateMap()
    {
        objField = new ObjectData[currSet.mapSize, currSet.mapSize];

        for (int i = 0; i < currSet.mainNew; i++)
            GenerateRandom(currSet.MainObjects);

        for (int i = 0; i <= currSet.neighboursNew; i++)
            GenerateRandom(currSet.NeighbourObjects);

        for (int i = 0; i < currSet.neighbourRepeat; i++)
            GenerateNeighbours(currSet.NeighbourObjects);
    }
    void CreateActors()
    {
        foreach(House house in houseLst)
        {
            GameObject newActor = Instantiate(actorTmpl, house.ReturnMovePos(), 
                transform.rotation);
            newActor.transform.SetParent(house.transform);
            Material matColor = matArray[Random.Range(0, matArray.Length - 1)];
            newActor.GetComponentInChildren<Renderer>().material = matColor;

            Actor actorS = newActor.GetComponent<Actor>();
            actorS.home = house;
            actorS.EnterAction(house);
            house.SetColor(matColor);
        }
    }

    void PlaceObjects()
    {
        for (int x = 0; x < currSet.mapSize; x++)
        {
            for (int y = 0; y < currSet.mapSize; y++)
            {
                if(objField[x,y] != null)
                {
                    GameObject newObj = Instantiate(objField[x, y].obj, 
                        new Vector3(x* currSet.objDistance, 0, 
                        y*currSet.objDistance), transform.rotation);

                    newObj.transform.SetParent(transform);
                    AdjustObject(objField[x, y].type, new Vector2Int(x, y), newObj);
                }
            }
        }
    }
    void AdjustObject(eObject type, Vector2Int pos, GameObject obj)
    {
        if(type != eObject.Field)
        {
            int doorPos = CheckFreeSpace(pos.x, pos.y);
            if (type == eObject.House)
            {
                House house = obj.GetComponent<House>();
                house.doorObj.transform.Rotate(0, 90 * doorPos, 0);
                houseLst.Add(house);
            }
            if (type == eObject.Mill)
            {
                Mill mill = obj.GetComponent<Mill>();
                mill.doorObj.transform.Rotate(0, 90 * doorPos, 0);
                millLst.Add(mill);
            }
            if (type == eObject.Market)
            {
                Market market = obj.GetComponent<Market>();
                market.doorObj.transform.Rotate(0, 90 * doorPos, 0);
                marketLst.Add(market);
            }
        }
        if (type == eObject.Field)
        {
            Field field = obj.GetComponent<Field>();
            //field.pos = new Vector3(pos.x, 0, pos.y);
            fieldLst.Add(field);
        }
    }
    int CheckFreeSpace(int _x, int _y)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if(x+y>1 || x+y==0 || x+y<-1 )continue;
                if (IsOutsideArray(_x + x, _y + y))
                    continue;
                if (objField[_x + x, _y + y] == null)
                {
                    if (x == 0 && y == -1) return 1;//n
                    if (x == 1 && y == 0) return 0;//o
                    if (x == 0 && y == 1) return 3;//s
                    if (x == -1 && y == 0) return 2;//w
                }
            }
        }
        return -1;
    }
    void GenerateRandom(ObjectData[] objectDatas)
    {
        foreach (ObjectData obData in objectDatas)
        {
            for (int i = 0; i < 100; i++)
            {
                Vector2Int pos = new Vector2Int(Random.Range(0, currSet.mapSize - 1),
                                                Random.Range(0, currSet.mapSize - 1));

                if (objField[pos.x, pos.y] != null) continue;

                if (!CheckNeighbours(pos.x, pos.y, 2)) continue;

                objField[pos.x, pos.y] = obData;
                objRefList.Add(new ObjectReference(obData.type, pos));
                break;
            }
        }
    }

    void GenerateNeighbours(ObjectData[] objectDatas)
    {
        foreach (ObjectData obData in objectDatas)
        {
            for (int i = 0; i < 50; i++)
            {
                ObjectReference oRef = FindNeighbour(obData);

                if (oRef == null) continue;

                Vector2Int pos = GetPlacement(oRef.pos.x, oRef.pos.y);

                if (pos.x < 0) 
                    continue;
                if (!CheckNeighbourType(pos.x, pos.y, 1, obData.nearType))
                    continue;

                objField[pos.x, pos.y] = obData;
                objRefList.Add(new ObjectReference(obData.type, pos));

                ShuffleList(objRefList);
                break;
            }
        }
    }

    ObjectReference FindNeighbour(ObjectData nOd)
    {
        foreach (ObjectReference oRef in objRefList)
        {
            foreach (eObject nType in nOd.nearType)
            {
                if (oRef.eObj != nType) continue;
                return oRef;
            }
        }
        return null;
    }

    bool CheckNeighbours(int _x, int _y, int _rad)
    {
        for (int x = -_rad; x <= _rad; x++)
        {
            for (int y = -_rad; y <= _rad; y++)
            {
                if (IsOutsideArray(_x + x, _y + y))
                    continue;
                if (objField[_x + x, _y + y] != null)
                    return false;
            }
        }
        return true;
    }
    bool CheckNeighbourType(int _x, int _y, int _rad, params eObject[] types)
    {
        for (int x = -_rad; x <= _rad; x++)
        {
            for (int y = -_rad; y <= _rad; y++)
            {
                if (IsOutsideArray(_x + x, _y + y))
                    continue;
                if (objField[_x + x, _y + y] != null)
                {
                    bool isType = false;
                    foreach (eObject type in types)
                        if (objField[_x + x, _y + y].type == type)
                            isType = true;

                    if (!isType) return false;
                }
            }
        }
        return true;
    }
    Vector2Int GetPlacement(int _x, int _y)
    {
        List<Vector2Int> placementList = new List<Vector2Int>();
        for (int i = -1; i <= 1; i++)
        {
            if (i == 0) continue;

            if (!IsOutsideArray(_x + i, _y))
                if (objField[_x + i, _y] == null) placementList.Add(new Vector2Int(_x + i, _y));
            if (!IsOutsideArray(_x, _y + i))
                if (objField[_x, _y + i] == null) placementList.Add(new Vector2Int(_x, _y + i));
        }
        if (placementList.Count < 2) return new Vector2Int(-1, 0);

        return placementList[Random.Range(0, placementList.Count - 1)];
    }
    bool IsOutsideArray(int _x, int _y)
    {
        if (_x < 0 || _x > currSet.mapSize - 1 ||
            _y < 0 || _y > currSet.mapSize - 1)
            return true;

        return false;
    }

    //https://forum.unity.com/threads/clever-way-to-shuffle-a-list-t-in-one-line-of-c-code.241052/
    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int idx = Random.Range(0, list.Count);
            var tmp = list[i];
            list[i] = list[idx];
            list[idx] = tmp;
        }

    }
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawSphere(new Vector3(0, 0, 0), 0.5f);
    //    float currmapS = currSet.mapSize + currSet.objDistance * currSet.mapSize;
    //    Gizmos.DrawSphere(new Vector3(currmapS, 0, currmapS), 0.5f);
    //}

    [SerializeField] Material mMaterial;
    Material[] matArray;
    Color PickColor(int _color)
    {
        switch (_color)
        {
            case 0: return Color.white;
            case 1: return Color.cyan;
            case 2: return Color.blue;
            case 3: return Color.green;
            case 4: return Color.red;
            case 5: return Color.yellow;
            default: return Color.black;
        }
    }
    Material[] CreateMaterials()
    {
        Material[] tempMatArr = new Material[7];
        for (int i = 0; i < 6; i++)
        {
            Material tempMat = new Material(mMaterial);
            tempMat.color = PickColor(i);
            tempMatArr[i] = tempMat;
        }
        return tempMatArr;
    }

}
