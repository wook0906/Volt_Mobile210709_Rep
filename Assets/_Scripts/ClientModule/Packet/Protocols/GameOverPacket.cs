﻿using UnityEngine;

public class GameOverPacket : Packet
{
    public override void UnPack(byte[] buffer)
    {
        int winner = ByteConverter.ToInt(buffer, 13);
        //Debug.Log("################################# GAMEOVER ###############################");
        //Debug.Log("Winner : " + winner);

        GameController.instance.gameData.winner = winner;
        GameController.instance.ChangePhase(new GameOver());

        //if (Volt_GameManager.S.pCurPhase == Phase.waitSync)
        //    Volt_DontDestroyPanel.S.OnDisconnected();
        //else
        //    Volt_GameManager.S.GameOver(winner);
        
    }
}