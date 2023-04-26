﻿using System;
using System.IO;
using System.Collections.Generic;

public class CPHInline
{
    public bool Execute()
    {
        //Declarations
        string str_stat, str_msg, str_srs, str_scene;
        bool bool_gam, bool_tit, bool_serious;
        int int_wait;

        //Initializations
        str_stat = args["status"].ToString();
        str_msg = "";
        str_srs = "seriousMode";
        str_scene = "SS_MidScreen";
        bool_gam = Convert.ToBoolean(args["gameUpdate"]);
        bool_tit = Convert.ToBoolean(args["statusUpdate"]);
        int_wait = 2000;

        try
        {
            //... to get serious.
            bool_serious = CPH.GetGlobalVar<bool>(str_srs);
        }//try
        catch (Exception e)
        {
            //... key exception.
            bool_serious = false;
            CPH.SetGlobalVar(str_srs, false, true);
        }//catch (Exception e)

        //If the game is not serious...
        if (!bool_serious)
        {
            //... update message to be only the "game."
            str_msg = "『SYSTEM CHECK』 ";
        }//if(!bool_serious)	
        else
        {
            //... update message to be "serious game."
            str_msg = "『SERIOUS UPDATE』 ";
        }//else

        CPH.LogInfo("『SERIOUS CHECK』: " + bool_serious);

        //If the title updates...
        if (bool_tit)
        {
            //... add status to the message.
            str_msg += str_stat;
            //... log stuff.
            CPH.LogInfo("『STATUS UPDATE』: Title update to \'" + str_stat + "\'!");
            CPH.LogInfo("『MARKER』: STATUS_UPDATE");
        }//if (bool_tit)

        //If both update at the same time...
        if (bool_tit && bool_gam)
        {
            //... add a divider.
            str_msg += " | ";
        }//if (bool_tit && bool_gam)  	

        //If the game updates...
        if (bool_gam)
        {
            //... run Game Handler
            gameHandler();
            //... get game info.
            str_game[0] = args["gameName"].ToString();
            str_game[1] = args["gameBoxArt"].ToString();
            str_game[2] = args["oldGameName"].ToString();

            //... show the game box art change.
            CPH.ObsSetBrowserSource(str_scene, "New GameBox Art", str_game[1]);
            CPH.Wait(int_wait);
            CPH.ObsHideSource(str_scene, "Old GameBox Art");
            CPH.ObsShowSource(str_scene, "New GameBox Art");
            CPH.Wait(int_wait);
            CPH.ObsHideSource(str_scene, "New GameBox Art");

            //... log stuff.
            CPH.LogInfo("『GAME UPDATE』: Game update from \'" + str_game[2] + "\' to \'" + str_game[0] + "\'!");
            str_msg += str_game[2] + " -> " + str_game[0];

            //... If OBS is streaming...
            if (CPH.ObsIsStreaming())
            {
                //... create a marker.
                CPH.CreateStreamMarker("CHANGE - " + str_game[2]);

            }//if (CPH.ObsIsStreaming())

            CPH.LogInfo("『MARKER』: GAME_UPDATE");
        }//if (bool_gam)

        //Feedback in Chat
        CPH.TwitchAnnounce(str_msg,
            true,
            "purple");

        return true;
    }//public bool Execute()

    public void gameHandler()
    {
        //Declarations
        List<string> list_actions;
        List<TwitchReward> list_rewards;
        string[] str_rewardGroups, str_scene, str_game;
        string str_srs;
        int int_id;
        bool bool_srs;

        //Initializations
        list_actions = CPH.GetGlobalVar<List<string>>("soundInteractActions");
        list_rewards = CPH.TwitchGetRewards();
        str_rewardGroups = new string[]
        {
            "Standard",
            "Standard - Sounds",
            "GS - DD2",
            "GS - PoE"
        };
        str_scene = new string[]
        {
            "SS_MidScreen",
            "SS_KiyoPro_FancyCam"
        };
        str_game = new string[]
        {
            args["gameName"].ToString(),
            args["gameBoxArt"].ToString(),
            args["oldGameName"].ToString(),
            args["oldGameBoxArt"].ToString()
        };
        str_srs = "seriousMode";
        int_id = 0;
        bool_srs = false;

        //Get Game ID Global
        int_id = Convert.ToInt32(args["gameId"].ToString());

        //Show the old game Box Art.
        CPH.ObsSetBrowserSource(str_scene[0], "Old GameBox Art", str_game[3]);
        CPH.ObsShowSource(str_scene[0], "Old GameBox Art");

        //Disable Game Specific Rewards.
        switch (int_id)
        {
            //	Resident Evil 4: Remake
            case 322503446:
                bool_srs = true;
                str_mode = "Game Type - Serious";
                break;
            //	Path of Exile
            case 29307:
                if (list_rewards[3].Enabled)
                {
                    CPH.TwitchRewardGroupDisable(str_rewardGroups[2]);
                }//if
                break;
            //	Darkest Dungeon II
            case 511471:
                if (list_rewards[30].Enabled)
                {
                    CPH.TwitchRewardGroupDisable(str_rewardGroups[3]);
                }//if
                break;
            //	Every other Game
            default:
                if (list_rewards[3].Enabled)
                {
                    CPH.TwitchRewardGroupDisable(str_rewardGroups[2]);
                }//if
                if (list_rewards[30].Enabled)
                {
                    CPH.TwitchRewardGroupDisable(str_rewardGroups[3]);
                }//if
                break;
        }//switch(int_id)

        CPH.SetGlobalVar(str_srs, bool_srs, true);

        if (bool_srs)
        {
            //Show Serious Visualizer.
            CPH.ObsShowSource("SS_KiyoPro_FancyCam", "VM_Visualizer_Serious");
            CPH.ObsHideSource("SS_KiyoPro_FancyCam", "VM_Visualizer_Normal");
            CPH.ObsHideSource("SS_MidScreen", "soPlayer");

            //Disable Sound Actions.
            foreach (string s in list_actions)
            {
                CPH.DisableAction(s);
            }//foreach

            //Disable all Rewards.
            for (int i = 0; i < str_rewardGroups.Length - 1; i++)
            {
                CPH.TwitchRewardGroupDisable(str_rewardGroups[i]);
            }//for
        }//if
        else
        {
            //Show Normal Visualizer.
            CPH.ObsHideSource(str_scene[1], "VM_Visualizer_Serious");
            CPH.ObsShowSource(str_scene[1], "VM_Visualizer_Normal");
            CPH.ObsShowSource(str_scene[0], "soPlayer");

            //Check if Standard Rewards are Already Enabled
            if (!list_rewards[0].Enabled)
            {
                //	Enable Normal Rewards.
                CPH.TwitchRewardGroupEnable(str_rewardGroups[0]);
                CPH.TwitchRewardGroupEnable(str_rewardGroups[1]);
                //	Enable Sound Actions.
                foreach (string s in list_actions)
                {
                    CPH.EnableAction(s);
                }//foreach
            }//if
        }//else
    }
}//public class CPHInline