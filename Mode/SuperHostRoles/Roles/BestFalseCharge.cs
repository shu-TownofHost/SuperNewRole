﻿using BepInEx.IL2CPP.Utils;
using HarmonyLib;
using SuperNewRoles.Roles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SuperNewRoles.Mode.SuperHostRoles.Roles
{
    class BestFalseCharge
    {
        public static void WrapUp() { 
            if (AmongUsClient.Instance.AmHost && !RoleClass.Bestfalsecharge.IsOnMeeting)
            {
                foreach (PlayerControl p in RoleClass.Bestfalsecharge.BestfalsechargePlayer)
                {
                    IEnumerator BestFalseCoro(PlayerControl target)
                    {
                        yield return new WaitForSeconds(1);
                        target.RpcMurderPlayer(target);
                    }
                    AmongUsClient.Instance.StartCoroutine(BestFalseCoro(p));
                }
                RoleClass.Bestfalsecharge.IsOnMeeting = true;
            }
        }
    }
}
