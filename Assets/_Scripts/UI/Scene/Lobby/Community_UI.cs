﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Community_UI : UI_Scene
{
    [SerializeField]
    GameObject friendsItemRoot;

    enum Buttons
    {
        Exit_Btn,
        AddFriends_Btn,
    }
    enum GameObjects
    {
        FriendsItemRoot
    }

    private LobbyScene lobbyScene;

    public override void Init()
    {
        base.Init();

        Bind<UIButton>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        lobbyScene = Managers.Scene.CurrentScene as LobbyScene;

        Get<UIButton>((int)Buttons.Exit_Btn).onClick.Add(new EventDelegate(() =>
        {
            OnClose();
        }));
        Get<UIButton>((int)Buttons.AddFriends_Btn).onClick.Add(new EventDelegate(() =>
        {
            //TODO : 현재 입력되어있는 닉네임(혹은 고유값)으로 친추추가 요청 패킷전송
        }));

        friendsItemRoot = Get<GameObject>((int)GameObjects.FriendsItemRoot);
        
        SetFriendsInfo();

        
    }

    void SetFriendsInfo()
    {
        //플레이어 데이터를 긁어와서 그만큼 FriendsItem을 생성한다.
        StartCoroutine(CorSetFriendsInfo());
    }
    IEnumerator CorSetFriendsInfo()
    {
        for (int i = 0; i < 8; i++)
        {
            AsyncOperationHandle<GameObject> handle = Managers.UI.MakeSubItemAsync<FriendsItem>(friendsItemRoot.transform);
            yield return new WaitUntil(() => { return handle.IsDone; });
            
            FriendsItem item = handle.Result.GetComponent<FriendsItem>();
            //item.GetComponent<UIPanel>().depth = friendsItemRoot.transform.parent.GetComponent<UIPanel>().depth + 1;
            item.SetInfo($"temp{i}");

            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
            //Vector3 moveVector = Vector3.up * friendsItemRoot.GetComponent<UIGrid>().cellHeight * i;

            //item.transform.localPosition -= moveVector;
        }
        friendsItemRoot.GetComponent<UIGrid>().Reposition();

        lobbyScene.OnLoadedCommunityUI();
    }

    public override void OnClose()
    {
        base.OnClose();
        lobbyScene.ChangeToLobbyCamera();
        Managers.UI.CloseSceneUI(this);
    }
    public override void OnActive()
    {
        base.OnActive();
        //Managers.UI.PushToUILayerStack(this);
    }
}
