﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Volt_LobbyRobotViewSection : MonoBehaviour
{
    public static Volt_LobbyRobotViewSection S;
    private Dictionary<RobotType, GameObject> robots = new Dictionary<RobotType, GameObject>();
    private RobotType selectRobotType = RobotType.Volt;
    public RobotType SelectRobotType
    {
        get { return selectRobotType; }
        set
        {
            if ((int)value == (int)RobotType.Max)
            {
                selectRobotType = RobotType.Volt;
                return;
            }
            else if ((int)value < (int)RobotType.Volt)
            {
                selectRobotType = RobotType.Reaper;
                return;
            }
            else
            {
                selectRobotType = value;
            }
        }
    }
    public UISprite nameSprite;

    private void Awake()
    {
        S = this;
    }

    public void Init()
    {
        PlayerPrefs.SetInt("SELECTED_ROBOT", 0);

        for (int i = 0; i < (int)RobotType.Max; ++i)
        {
            RobotType robotType = (RobotType)i;
            SkinType skinType;
            RobotSkin value;
            if (Volt_PlayerData.instance.selectdRobotSkins.TryGetValue(robotType, out value))
            {
                
                if (DBManager.instance.userSkinCondition.ContainsKey(value.skinID) == false ||
                    DBManager.instance.userSkinCondition[value.skinID] == false)
                {
                    Debug.Log($"실제 소유하고 있지 않은 스킨... skin id:[{value.skinID}]");
                    string key = $"{robotType}_skin";
                    PlayerPrefs.SetInt(key, 0);

                    RobotSkinObject skinData;
                    if (Managers.Data.SkinDatas.TryGetValue(robotType, out skinData))
                    {
                        Volt_PlayerData.instance.selectdRobotSkins[robotType] = skinData.RobotSkins[0];
                    }
                    skinType = SkinType.Origin;
                }
                else
                    skinType = Volt_PlayerData.instance.selectdRobotSkins[robotType].SkinType;
            }
            else
                skinType = SkinType.Origin;

            
            robots.Add(robotType, null);
            CreateRobot(robotType, skinType);
        }
    }


    public void OnClickNextModelBtnforTutorial()
    {
        OnClickNextModelBtn();
        if (Volt_TutorialManager.S)
        {
            Volt_TutorialManager.S.DoNextTutorial();
        }
    }
    public void OnClickNextModelBtn()
    {

        robots[SelectRobotType].SetActive(false);
        SelectRobotType++;
        robots[SelectRobotType].SetActive(true);

        SavePlayerSelectRobot();
    }
    public void OnClickPrevModelBtn()
    {
        robots[SelectRobotType].SetActive(false);
        SelectRobotType--;
        robots[SelectRobotType].SetActive(true);

        SavePlayerSelectRobot();
    }
    private void SavePlayerSelectRobot()
    {
        PlayerPrefs.SetInt("SELECTED_ROBOT", (int)SelectRobotType);
        //print("현재선택 " + PlayerPrefs.GetInt("SELECTED_ROBOT"));

        RobotInfo_Popup robotInfoPopup = Managers.UI.GetPopupUI<RobotInfo_Popup>("RobotInfo_Popup");
        if (robotInfoPopup != null)
            robotInfoPopup.SetInfo(SelectRobotType);
    }

    public void PlayLobbyAnimation()
    {
        robots[SelectRobotType].GetComponent<Animator>().Play("lobby");
    }

    public void DestroyRobot(RobotType robotType)
    {
        Destroy(robots[robotType]);
    }

    public void CreateRobot(RobotType robotType, SkinType skinType)
    {
        Debug.Log($"Create Robot Model {robotType}_{skinType}");
        Managers.Resource.InstantiateAsync($"Robots/{robotType}/{robotType}_{skinType}.prefab",
                (result) =>
                {
                    Debug.Log("Instantiate Robot Model");
                    GameObject go = result.Result;
                    Volt_ModelRobot model = go.GetOrAddComponent<Volt_ModelRobot>();
                    go.transform.parent = transform;

                    if (robots[robotType] != null)
                    {
                        Managers.Resource.DestoryAndRelease(robots[robotType]);
                    }
                    robots[robotType] = go;
                    model.Init(robotType);
                    if (robotType == (RobotType)PlayerPrefs.GetInt("SELECTED_ROBOT"))
                    {
                        go.SetActive(true);
                        return;
                    }
                    go.SetActive(false);
                });
    }

    public void Clear()
    {
        foreach (var item in robots.Values)
        {
            Managers.Resource.DestoryAndRelease(item);
        }
    }
}
