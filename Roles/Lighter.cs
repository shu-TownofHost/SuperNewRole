﻿using HarmonyLib;
using Hazel;
using System;
using SuperNewRoles.Patches;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SuperNewRoles.Buttons;
using SuperNewRoles.CustomOption;

namespace SuperNewRoles.Roles
{
    class Lighter
    {

        public static void ResetCoolDown()
        {
            HudManagerStartPatch.LighterLightOnButton.MaxTimer = RoleClass.Lighter.CoolTime;
            RoleClass.Lighter.ButtonTimer = DateTime.Now;
        }
        public static bool isLighter(PlayerControl Player)
        {
            if (RoleClass.Lighter.LighterPlayer.IsCheckListPlayerControl(Player))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static void LightOnStart()
        {
            RoleClass.Lighter.IsLightOn = true;
        }
       
        public static void LightOutEnd()
        {
            if (!RoleClass.Lighter.IsLightOn) return;
            RoleClass.Lighter.IsLightOn = false;

        }
        public static void EndMeeting()
        {
            HudManagerStartPatch.LighterLightOnButton.MaxTimer = RoleClass.Lighter.CoolTime;
            RoleClass.Lighter.ButtonTimer = DateTime.Now;
            RoleClass.Lighter.IsLightOn = false;
        }
    }
}
