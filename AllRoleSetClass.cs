﻿using SuperNewRoles.CustomRPC;
using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Hazel;
using SuperNewRoles.CustomOption;
using SuperNewRoles.Roles;
using SuperNewRoles.Mode;

namespace SuperNewRoles
{
    [HarmonyPatch(typeof(RoleManager), nameof(RoleManager.SelectRoles))]
    class RoleManagerSelectRolesPatch
    {
        public static void Postfix()
        {
            ModeHandler.ClearAndReload();
            Roles.RoleClass.clearAndReloadRoles();
            if (ModeHandler.thisMode == ModeId.Default)
            {
                AllRoleSetClass.OneOrNotListSet();
                AllRoleSetClass.AllRoleSet();
            }
            else if (ModeHandler.isMode(ModeId.SuperHostRoles)) {
                Mode.SuperHostRoles.RoleSelectHandler.RoleSelect();
            }
        }
    }
    class AllRoleSetClass
    {
        public static List<RoleId> Impoonepar;
        public static List<RoleId> Imponotonepar;
        public static List<RoleId> Neutonepar;
        public static List<RoleId> Neutnotonepar;
        public static List<RoleId> Crewonepar;
        public static List<RoleId> Crewnotonepar;
        public static List<PlayerControl> CrewMatePlayers;
        public static List<PlayerControl> ImpostorPlayers;

        public static int ImpostorPlayerNum;
        public static int NeutralPlayerNum;
        public static int CrewMatePlayerNum;

        public static void AllRoleSet()
        {
            if (!AmongUsClient.Instance.AmHost) return;
            SetPlayerNum();
            CrewOrImpostorSet();
            try
            {
                ImpostorRandomSelect();
            } catch (Exception e)
            {
                SuperNewRolesPlugin.Logger.LogInfo("RoleSelectError:"+e);
            }


            try
            {
                NeutralRandomSelect();
            }
            catch (Exception e)
            {
                SuperNewRolesPlugin.Logger.LogInfo("RoleSelectError:" + e);
            }


            try
            {
                CrewMateRandomSelect();
            }
            catch (Exception e)
            {
                SuperNewRolesPlugin.Logger.LogInfo("RoleSelectError:" + e);
            }


            try
            {
                QuarreledRandomSelect();
            }
            catch (Exception e)
            {
                SuperNewRolesPlugin.Logger.LogInfo("RoleSelectError:" + e);
            }
        }
        public static void QuarreledRandomSelect()
        {
            if (!CustomOption.CustomOptions.QuarreledOption.getBool()) return;
            List<PlayerControl> SelectPlayers = new List<PlayerControl>();
            if (CustomOption.CustomOptions.QuarreledOnlyCrewMate.getBool())
            {
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    if (!p.Data.Role.IsImpostor && !p.isNeutral())
                    {
                        SelectPlayers.Add(p);
                    }
                }
            } else
            {
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    SelectPlayers.Add(p);
                }
            }
            for (int i = 0; i < CustomOptions.QuarreledTeamCount.getFloat(); i++)
            {
                if (!(SelectPlayers.Count == 1 || SelectPlayers.Count == 0))
                {
                    var Listdate = new List<PlayerControl>();
                    for (int i2 = 0; i2 < 2; i2++)
                    {
                        var player = ModHelpers.GetRandomIndex<PlayerControl>(SelectPlayers);
                        Listdate.Add(SelectPlayers[player]);
                        SelectPlayers.RemoveAt(player);
                    }
                    RoleHelpers.SetQuarreled(Listdate[0],Listdate[1]);
                    RoleHelpers.SetQuarreledRPC(Listdate[0],Listdate[1]);
                }
            }
        }
        public static void SetPlayerNum()
        {
            ImpostorPlayerNum = (int)CustomOption.CustomOptions.impostorRolesCountMax.getFloat();
            NeutralPlayerNum = (int)CustomOption.CustomOptions.neutralRolesCountMax.getFloat();
            CrewMatePlayerNum = (int)CustomOption.CustomOptions.crewmateRolesCountMax.getFloat();
        }
        public static void ImpostorRandomSelect()
        {
            if (ImpostorPlayerNum == 0 || (Impoonepar.Count == 0 && Imponotonepar.Count == 0))
            {
                return;
            }
            bool IsNotEndRandomSelect= true;
            while (IsNotEndRandomSelect)
            {
                if (Impoonepar.Count != 0)
                {
                    int SelectRoleDateIndex = ModHelpers.GetRandomIndex(Impoonepar);
                    RoleId SelectRoleDate = Impoonepar[SelectRoleDateIndex];
                    
                    if (SelectRoleDate == RoleId.EvilSpeedBooster)
                    {
                        try
                        {
                            for (int i1 = 1; i1 <= 15; i1++)
                            {
                                for (int i = 1; i <= Imponotonepar.Count; i++)
                                {
                                    if (Crewnotonepar[i - 1] == RoleId.SpeedBooster)
                                    {
                                        Crewnotonepar.RemoveAt(i - 1);
                                    }
                                }
                            }
                            Crewonepar.Remove(RoleId.SpeedBooster);
                        } catch
                        {

                        }
                    }

                    int PlayerCount = (int)GetPlayerCount(SelectRoleDate);
                    if (PlayerCount >= ImpostorPlayerNum)
                    {
                        for (int i = 1; i <= ImpostorPlayerNum; i++)
                        {
                            PlayerControl p = ModHelpers.GetRandom(ImpostorPlayers);
                            p.setRoleRPC(SelectRoleDate);
                            ImpostorPlayers.Remove(p);
                        }
                        IsNotEndRandomSelect = false;
                        
                    } else if (PlayerCount >= ImpostorPlayers.Count)
                    {
                        foreach (PlayerControl Player in ImpostorPlayers)
                        {
                            ImpostorPlayerNum--;
                            Player.setRoleRPC(SelectRoleDate);
                        }
                        IsNotEndRandomSelect = false;
                    }
                    else
                    {
                        for (int i = 1; i <= PlayerCount; i++)
                        {
                            ImpostorPlayerNum--;
                            PlayerControl p = ModHelpers.GetRandom(ImpostorPlayers);
                            p.setRoleRPC(SelectRoleDate);
                            ImpostorPlayers.Remove(p);
                        }
                    }
                    Impoonepar.RemoveAt(SelectRoleDateIndex);
                } else
                {
                    int SelectRoleDateIndex = ModHelpers.GetRandomIndex(Imponotonepar);
                    RoleId SelectRoleDate = Imponotonepar[SelectRoleDateIndex];
                    int PlayerCount = (int)GetPlayerCount(SelectRoleDate);
                    if (PlayerCount >= ImpostorPlayerNum)
                    {
                        for (int i = 1; i <= ImpostorPlayerNum; i++)
                        {
                            PlayerControl p = ModHelpers.GetRandom(ImpostorPlayers);
                            p.setRoleRPC(SelectRoleDate);
                            ImpostorPlayers.Remove(p);
                        }
                        IsNotEndRandomSelect = false;
                        
                    } else if (PlayerCount >= ImpostorPlayers.Count) {
                        foreach (PlayerControl Player in ImpostorPlayers)
                        {
                            Player.setRoleRPC(SelectRoleDate);
                        }
                        IsNotEndRandomSelect = false;
                    }
                    else
                    {
                        for (int i = 1; i <= PlayerCount; i++)
                        {
                            ImpostorPlayerNum--;
                            PlayerControl p = ModHelpers.GetRandom(ImpostorPlayers);
                            p.setRoleRPC(SelectRoleDate);
                            ImpostorPlayers.Remove(p);
                        }
                    }
                    for (int i1 = 1; i1<= 15; i1++)
                    {
                        for (int i = 1; i <= Imponotonepar.Count; i++)
                        {
                            if (Imponotonepar[i - 1] == SelectRoleDate)
                            {
                                Imponotonepar.RemoveAt(i - 1);
                            }
                        }
                    }
                }
            }
        }
        public static void NeutralRandomSelect()
        {
            if (NeutralPlayerNum == 0 || (Neutonepar.Count == 0 && Neutnotonepar.Count == 0))
            {
                return;
            }
            bool IsNotEndRandomSelect = true;
            while (IsNotEndRandomSelect)
            {
                if (Neutonepar.Count != 0)
                {
                    int SelectRoleDateIndex = ModHelpers.GetRandomIndex(Neutonepar);
                    RoleId SelectRoleDate = Neutonepar[SelectRoleDateIndex];
                    int PlayerCount = (int)GetPlayerCount(SelectRoleDate);
                    if (PlayerCount >= NeutralPlayerNum)
                    {
                        for (int i = 1; i <= NeutralPlayerNum; i++)
                        {
                            PlayerControl p = ModHelpers.GetRandom(CrewMatePlayers);
                            p.setRoleRPC(SelectRoleDate);
                            CrewMatePlayers.Remove(p);
                        }
                        IsNotEndRandomSelect = false;

                    }
                    else if (PlayerCount >= CrewMatePlayers.Count)
                    {
                        foreach (PlayerControl Player in CrewMatePlayers)
                        {
                            NeutralPlayerNum--;
                            Player.setRoleRPC(SelectRoleDate);
                        }
                        IsNotEndRandomSelect = false;
                    }
                    else
                    {
                        for (int i = 1; i <= PlayerCount; i++)
                        {
                            NeutralPlayerNum--;
                            PlayerControl p = ModHelpers.GetRandom(CrewMatePlayers);
                            p.setRoleRPC(SelectRoleDate);
                            CrewMatePlayers.Remove(p);
                        }
                    }
                    Neutonepar.RemoveAt(SelectRoleDateIndex);
                }
                else
                {
                    int SelectRoleDateIndex = ModHelpers.GetRandomIndex(Neutnotonepar);
                    RoleId SelectRoleDate = Neutnotonepar[SelectRoleDateIndex];
                    int PlayerCount = (int)GetPlayerCount(SelectRoleDate);
                    if (PlayerCount >= NeutralPlayerNum)
                    {
                        for (int i = 1; i <= NeutralPlayerNum; i++)
                        {
                            PlayerControl p = ModHelpers.GetRandom(CrewMatePlayers);
                            p.setRoleRPC(SelectRoleDate);
                            CrewMatePlayers.Remove(p);
                        }
                        IsNotEndRandomSelect = false;

                    }
                    else if (PlayerCount >= CrewMatePlayers.Count)
                    {
                        foreach (PlayerControl Player in CrewMatePlayers)
                        {
                            Player.setRoleRPC(SelectRoleDate);
                        }
                        IsNotEndRandomSelect = false;
                    }
                    else
                    {
                        for (int i = 1; i <= PlayerCount; i++)
                        {
                            NeutralPlayerNum--;
                            PlayerControl p = ModHelpers.GetRandom(CrewMatePlayers);
                            p.setRoleRPC(SelectRoleDate);
                            CrewMatePlayers.Remove(p);
                        }
                    }
                    for (int i1 = 1; i1 <= 15; i1++)
                    {
                        for (int i = 1; i <= Neutnotonepar.Count; i++)
                        {
                            if (Neutnotonepar[i - 1] == SelectRoleDate)
                            {
                                Neutnotonepar.RemoveAt(i - 1);
                            }
                        }
                    }
                }
            }
        }
        public static void CrewMateRandomSelect()
        {
            if (CrewMatePlayerNum == 0 || (Crewonepar.Count == 0 && Crewnotonepar.Count == 0))
            {
                return;
            }
            bool IsNotEndRandomSelect = true;
            while (IsNotEndRandomSelect)
            {
                if (Crewonepar.Count != 0)
                {
                    int SelectRoleDateIndex = ModHelpers.GetRandomIndex(Crewonepar);
                    RoleId SelectRoleDate = Crewonepar[SelectRoleDateIndex];
                    int PlayerCount = (int)GetPlayerCount(SelectRoleDate);
                    if (PlayerCount >= CrewMatePlayerNum)
                    {
                        for (int i = 1; i <= CrewMatePlayerNum; i++)
                        {
                            PlayerControl p = ModHelpers.GetRandom(CrewMatePlayers);
                            p.setRoleRPC(SelectRoleDate);
                            CrewMatePlayers.Remove(p);
                        }
                        IsNotEndRandomSelect = false;

                    }
                    else if (PlayerCount >= CrewMatePlayers.Count)
                    {
                        foreach (PlayerControl Player in CrewMatePlayers)
                        {
                            CrewMatePlayerNum--;
                            Player.setRoleRPC(SelectRoleDate);
                        }
                        IsNotEndRandomSelect = false;
                    }
                    else
                    {
                        for (int i = 1; i <= PlayerCount; i++)
                        {
                            CrewMatePlayerNum--;
                            PlayerControl p = ModHelpers.GetRandom(CrewMatePlayers);
                            p.setRoleRPC(SelectRoleDate);
                            CrewMatePlayers.Remove(p);
                        }
                    }
                    Crewonepar.RemoveAt(SelectRoleDateIndex);
                }
                else
                {
                    int SelectRoleDateIndex = ModHelpers.GetRandomIndex(Crewnotonepar);
                    RoleId SelectRoleDate = Crewnotonepar[SelectRoleDateIndex];
                    int PlayerCount = (int)GetPlayerCount(SelectRoleDate);
                    if (PlayerCount >= CrewMatePlayerNum)
                    {
                        for (int i = 1; i <= CrewMatePlayerNum; i++)
                        {
                            PlayerControl p = ModHelpers.GetRandom(CrewMatePlayers);
                            p.setRoleRPC(SelectRoleDate);
                            CrewMatePlayers.Remove(p);
                        }
                        IsNotEndRandomSelect = false;

                    }
                    else if (PlayerCount >= CrewMatePlayers.Count)
                    {
                        foreach (PlayerControl Player in CrewMatePlayers)
                        {
                            Player.setRoleRPC(SelectRoleDate);
                        }
                        IsNotEndRandomSelect = false;
                    }
                    else
                    {
                        for (int i = 1; i <= PlayerCount; i++)
                        {
                            CrewMatePlayerNum--;
                            PlayerControl p = ModHelpers.GetRandom(CrewMatePlayers);
                            p.setRoleRPC(SelectRoleDate);
                            CrewMatePlayers.Remove(p);
                        }
                    }
                    for (int i1 = 1; i1 <= 15; i1++)
                    {
                        for (int i = 1; i <= Crewnotonepar.Count; i++)
                        {
                            if (Crewnotonepar[i - 1] == SelectRoleDate)
                            {
                                Crewnotonepar.RemoveAt(i - 1);
                            }
                        }
                    }
                }
            }
        }
        public static float GetPlayerCount(RoleId RoleDate)
        {
            switch (RoleDate)
            {
                case (RoleId.SoothSayer):
                    return CustomOption.CustomOptions.SoothSayerPlayerCount.getFloat();
                case (RoleId.Jester):
                    return CustomOption.CustomOptions.JesterPlayerCount.getFloat();
                case (RoleId.Lighter):
                    return CustomOption.CustomOptions.LighterPlayerCount.getFloat();
                case (RoleId.EvilLighter):
                    return CustomOption.CustomOptions.EvilLighterPlayerCount.getFloat();
                case (RoleId.EvilScientist):
                    return CustomOption.CustomOptions.EvilScientistPlayerCount.getFloat();
                case (RoleId.Sheriff):
                    return CustomOption.CustomOptions.SheriffPlayerCount.getFloat();
                case (RoleId.MeetingSheriff):
                    return CustomOption.CustomOptions.MeetingSheriffPlayerCount.getFloat();
                case (RoleId.Jackal):
                    return CustomOption.CustomOptions.JackalPlayerCount.getFloat();
                case (RoleId.Teleporter):
                    return CustomOption.CustomOptions.TeleporterPlayerCount.getFloat();
                case (RoleId.SpiritMedium):
                    return CustomOption.CustomOptions.SpiritMediumPlayerCount.getFloat();
                case (RoleId.SpeedBooster):
                    return CustomOption.CustomOptions.SpeedBoosterPlayerCount.getFloat();
                case (RoleId.EvilSpeedBooster):
                    return CustomOption.CustomOptions.EvilSpeedBoosterPlayerCount.getFloat();
                case (RoleId.Tasker):
                    return CustomOption.CustomOptions.TaskerPlayerCount.getFloat();
                case (RoleId.Doorr):
                    return CustomOption.CustomOptions.DoorrPlayerCount.getFloat();
                case (RoleId.EvilDoorr):
                    return CustomOption.CustomOptions.EvilDoorrPlayerCount.getFloat();
                case (RoleId.Sealdor):
                    return CustomOption.CustomOptions.SealdorPlayerCount.getFloat();
                case (RoleId.Speeder):
                    return CustomOption.CustomOptions.SpeederPlayerCount.getFloat();
                case (RoleId.Freezer):
                    return CustomOption.CustomOptions.FreezerPlayerCount.getFloat();
                case (RoleId.Guesser):
                    return CustomOption.CustomOptions.GuesserPlayerCount.getFloat();
                case (RoleId.EvilGuesser):
                    return CustomOption.CustomOptions.EvilGuesserPlayerCount.getFloat();
                case (RoleId.Vulture):
                    return CustomOption.CustomOptions.VulturePlayerCount.getFloat();
                case (RoleId.NiceScientist):
                    return CustomOption.CustomOptions.NiceScientistPlayerCount.getFloat();
                case (RoleId.Clergyman):
                    return CustomOption.CustomOptions.ClergymanPlayerCount.getFloat();
                case (RoleId.MadMate):
                    return CustomOption.CustomOptions.MadMatePlayerCount.getFloat();
                case (RoleId.Bait):
                    return CustomOption.CustomOptions.BaitPlayerCount.getFloat();
                case (RoleId.HomeSecurityGuard):
                    return CustomOption.CustomOptions.HomeSecurityGuardPlayerCount.getFloat();
                case (RoleId.StuntMan):
                    return CustomOption.CustomOptions.StuntManPlayerCount.getFloat();
                case (RoleId.Moving):
                    return CustomOption.CustomOptions.MovingPlayerCount.getFloat();
                case (RoleId.Opportunist):
                    return CustomOption.CustomOptions.OpportunistPlayerCount.getFloat();
                case (RoleId.NiceGambler):
                    return CustomOption.CustomOptions.NiceGamblerPlayerCount.getFloat();
                case (RoleId.EvilGambler):
                    return CustomOption.CustomOptions.EvilGamblerPlayerCount.getFloat();
                case (RoleId.Bestfalsecharge):
                    return CustomOption.CustomOptions.BestfalsechargePlayerCount.getFloat();
                case (RoleId.Researcher):
                    return CustomOption.CustomOptions.ResearcherPlayerCount.getFloat();
                case (RoleId.SelfBomber):
                    return CustomOption.CustomOptions.SelfBomberPlayerCount.getFloat();
                case (RoleId.God):
                    return CustomOption.CustomOptions.GodPlayerCount.getFloat();
                case (RoleId.AllCleaner):
                    return CustomOption.CustomOptions.AllCleanerPlayerCount.getFloat();
                case (RoleId.NiceNekomata):
                    return CustomOption.CustomOptions.NiceNekomataPlayerCount.getFloat();
                case (RoleId.EvilNekomata):
                    return CustomOption.CustomOptions.EvilNekomataPlayerCount.getFloat();
                    case (RoleId.JackalFriends):
                    return CustomOption.CustomOptions.JackalFriendsPlayerCount.getFloat();
                    case (RoleId.Doctor):
                    return CustomOption.CustomOptions.DoctorPlayerCount.getFloat();
                    case (RoleId.CountChanger):
                    return CustomOption.CustomOptions.CountChangerPlayerCount.getFloat();
                    case (RoleId.Pursuer):
                    return CustomOption.CustomOptions.PursuerPlayerCount.getFloat();
                    case (RoleId.Minimalist):
                    return CustomOption.CustomOptions.MinimalistPlayerCount.getFloat();
                    case (RoleId.Hawk):
                    return CustomOption.CustomOptions.HawkPlayerCount.getFloat();
                    case (RoleId.Egoist):
                    return CustomOption.CustomOptions.EgoistPlayerCount.getFloat();
                    case (RoleId.NiceRedRidingHood):
                    return CustomOption.CustomOptions.NiceRedRidingHoodPlayerCount.getFloat();
                    //プレイヤーカウント
            }
            return 1;
        }
        public static void CrewOrImpostorSet()
        {
            CrewMatePlayers = new List<PlayerControl>();
            ImpostorPlayers = new List<PlayerControl>();
            foreach(PlayerControl Player in PlayerControl.AllPlayerControls)
            {
                if (Player.Data.Role.IsImpostor)
                {
                    ImpostorPlayers.Add(Player);
                } else
                {
                    CrewMatePlayers.Add(Player);
                }
            }
        }
        public static void OneOrNotListSet()
        {
            Impoonepar = new List<RoleId>();
            Imponotonepar = new List<RoleId>();
            Neutonepar = new List<RoleId>();
            Neutnotonepar = new List<RoleId>();
            Crewonepar = new List<RoleId>();
            Crewnotonepar = new List<RoleId>();
            if (!(CustomOption.CustomOptions.SoothSayerOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.SoothSayerOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.SoothSayer;
                if (OptionDate == 10)
                {
                    Crewonepar.Add(ThisRoleId);
                } else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Crewnotonepar.Add(ThisRoleId);
                    }
                }
            }
            if (!(CustomOption.CustomOptions.JesterOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.JesterOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.Jester;
                if (OptionDate == 10)
                {
                    Neutonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Neutnotonepar.Add(ThisRoleId);
                    }
                }
            }
             if (!(CustomOption.CustomOptions.LighterOption.getString().Replace("0%", "") == ""))
            {
                SuperNewRolesPlugin.Logger.LogInfo("Lighterあああああ");
                int OptionDate = int.Parse(CustomOption.CustomOptions.LighterOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.Lighter;
                if (OptionDate == 10)
                {
                    Crewonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Crewnotonepar.Add(ThisRoleId);
                    }
                }
            }
            /**
            if (!(CustomOption.CustomOptions.EvilLighterOption.getString().Replace("0%", "") == ""))
            {
                SuperNewRolesPlugin.Logger.LogInfo("EvilLighterSelected!!!!");
                int OptionDate = int.Parse(CustomOption.CustomOptions.EvilLighterOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.EvilLighter;
                if (OptionDate == 10)
                {
                    Impoonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Imponotonepar.Add(ThisRoleId);
                    }
                }
            }
            */
            if (!(CustomOption.CustomOptions.EvilScientistOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.EvilScientistOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.EvilScientist;
                if (OptionDate == 10)
                {
                    Impoonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Imponotonepar.Add(ThisRoleId);
                    }
                }
            }
            if (!(CustomOption.CustomOptions.SheriffOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.SheriffOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.Sheriff;
                if (OptionDate == 10)
                {
                    Crewonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Crewnotonepar.Add(ThisRoleId);
                    }
                }
            }
            if (!(CustomOption.CustomOptions.MeetingSheriffOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.MeetingSheriffOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.MeetingSheriff;
                if (OptionDate == 10)
                {
                    Crewonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Crewnotonepar.Add(ThisRoleId);
                    }
                }
            }
            if (!(CustomOption.CustomOptions.JackalOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.JackalOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.Jackal;
                if (OptionDate == 10)
                {
                    Neutonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Neutnotonepar.Add(ThisRoleId);
                    }
                }
            }
            if (!(CustomOption.CustomOptions.TeleporterOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.TeleporterOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.Teleporter;
                if (OptionDate == 10)
                {
                    Impoonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Imponotonepar.Add(ThisRoleId);
                    }
                }
            }
            if (!(CustomOption.CustomOptions.SpiritMediumOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.SpiritMediumOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.SpiritMedium;
                if (OptionDate == 10)
                {
                    Crewonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Crewnotonepar.Add(ThisRoleId);
                    }
                }
            }
            if (!(CustomOption.CustomOptions.SpeedBoosterOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.SpeedBoosterOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.SpeedBooster;
                if (OptionDate == 10)
                {
                    Crewonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Crewnotonepar.Add(ThisRoleId);
                    }
                }
            }
            if (!(CustomOption.CustomOptions.EvilSpeedBoosterOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.EvilSpeedBoosterOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.EvilSpeedBooster;
                if (OptionDate == 10)
                {
                    Impoonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Imponotonepar.Add(ThisRoleId);
                    }
                }
            }
            /**
            if (!(CustomOption.CustomOptions.TaskerOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.TaskerOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.Tasker;
                if (OptionDate == 10)
                {
                    Impoonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Imponotonepar.Add(ThisRoleId);
                    }
                }
            }
            **/
            if (!(CustomOption.CustomOptions.DoorrOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.DoorrOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.Doorr;
                if (OptionDate == 10)
                {
                    Crewonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Crewnotonepar.Add(ThisRoleId);
                    }
                }
            }
            if (!(CustomOption.CustomOptions.EvilDoorrOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.EvilDoorrOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.EvilDoorr;
                if (OptionDate == 10)
                {
                    Impoonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Imponotonepar.Add(ThisRoleId);
                    }
                }
            }
            /**
            if (!(CustomOption.CustomOptions.SealdorOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.SealdorOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.Sealdor;
                if (OptionDate == 10)
                {
                    Crewonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Crewnotonepar.Add(ThisRoleId);
                    }
                }
            }
            if (!(CustomOption.CustomOptions.SpeederOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.SpeederOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.Speeder;
                if (OptionDate == 10)
                {
                    Impoonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Imponotonepar.Add(ThisRoleId);
                    }
                }
            }
            if (!(CustomOption.CustomOptions.FreezerOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.FreezerOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.Freezer;
                if (OptionDate == 10)
                {
                    Impoonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Imponotonepar.Add(ThisRoleId);
                    }
                }
            }
            if (!(CustomOption.CustomOptions.GuesserOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.GuesserOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.Guesser;
                if (OptionDate == 10)
                {
                    Crewonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Crewnotonepar.Add(ThisRoleId);
                    }
                }
            }
            if (!(CustomOption.CustomOptions.EvilGuesserOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.EvilGuesserOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.EvilGuesser;
                if (OptionDate == 10)
                {
                    Impoonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Imponotonepar.Add(ThisRoleId);
                    }
                }
            }
            if (!(CustomOption.CustomOptions.VultureOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.VultureOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.Vulture;
                if (OptionDate == 10)
                {
                    Neutonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Neutnotonepar.Add(ThisRoleId);
                    }
                }
            }
            */
            if (!(CustomOption.CustomOptions.NiceScientistOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.NiceScientistOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.NiceScientist;
                if (OptionDate == 10)
                {
                    Crewonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Crewnotonepar.Add(ThisRoleId);
                    }
                }
            }
            if (!(CustomOption.CustomOptions.ClergymanOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.ClergymanOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.Clergyman;
                if (OptionDate == 10)
                {
                    Crewonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Crewnotonepar.Add(ThisRoleId);
                    }
                }
            }
            if (!(CustomOption.CustomOptions.MadMateOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.MadMateOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.MadMate;
                if (OptionDate == 10)
                {
                    Crewonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Crewnotonepar.Add(ThisRoleId);
                    }
                }
            }
            if (!(CustomOption.CustomOptions.BaitOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.BaitOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.Bait;
                if (OptionDate == 10)
                {
                    Crewonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Crewnotonepar.Add(ThisRoleId);
                    }
                }
            }
            if (!(CustomOption.CustomOptions.HomeSecurityGuardOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.HomeSecurityGuardOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.HomeSecurityGuard;
                if (OptionDate == 10)
                {
                    Crewonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Crewnotonepar.Add(ThisRoleId);
                    }
                }
            }
            if (!(CustomOption.CustomOptions.StuntManOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.StuntManOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.StuntMan;
                if (OptionDate == 10)
                {
                    Crewonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Crewnotonepar.Add(ThisRoleId);
                    }
                }
            }
            if (!(CustomOption.CustomOptions.MovingOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.MovingOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.Moving;
                if (OptionDate == 10)
                {
                    Crewonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Crewnotonepar.Add(ThisRoleId);
                    }
                }
            }
            if (!(CustomOption.CustomOptions.OpportunistOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.OpportunistOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.Opportunist;
                if (OptionDate == 10)
                {
                    Neutonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Neutnotonepar.Add(ThisRoleId);
                    }
                }
            }
            /**
            if (!(CustomOption.CustomOptions.NiceGamblerOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.NiceGamblerOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.NiceGambler;
                if (OptionDate == 10)
                {
                    Crewonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Crewnotonepar.Add(ThisRoleId);
                    }
                }
            }
            **/
            if (!(CustomOption.CustomOptions.EvilGamblerOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.EvilGamblerOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.EvilGambler;
                if (OptionDate == 10)
                {
                    Impoonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Imponotonepar.Add(ThisRoleId);
                    }
                }
            }
            if (!(CustomOption.CustomOptions.BestfalsechargeOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.BestfalsechargeOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.Bestfalsecharge;
                if (OptionDate == 10)
                {
                    Crewonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Crewnotonepar.Add(ThisRoleId);
                    }
                }
            }
            /**
        if (!(CustomOption.CustomOptions.ResearcherOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.ResearcherOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.Researcher;
                if (OptionDate == 10)
                {
                    Neutonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Neutnotonepar.Add(ThisRoleId);
                    }
                }
            }
            **/
            if (!(CustomOption.CustomOptions.SelfBomberOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.SelfBomberOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.SelfBomber;
                if (OptionDate == 10)
                {
                    Impoonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Imponotonepar.Add(ThisRoleId);
                    }
                }
            }
        if (!(CustomOption.CustomOptions.GodOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.GodOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.God;
                if (OptionDate == 10)
                {
                    Neutonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Neutnotonepar.Add(ThisRoleId);
                    }
                }
            }
        /**
        if (!(CustomOption.CustomOptions.AllCleanerOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.AllCleanerOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.AllCleaner;
                if (OptionDate == 10)
                {
                    Impoonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Imponotonepar.Add(ThisRoleId);
                    }
                }
            }
            **/
        if (!(CustomOption.CustomOptions.NiceNekomataOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.NiceNekomataOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.NiceNekomata;
                if (OptionDate == 10)
                {
                    Crewonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Crewnotonepar.Add(ThisRoleId);
                    }
                }
            }
        if (!(CustomOption.CustomOptions.EvilNekomataOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.EvilNekomataOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.EvilNekomata;
                if (OptionDate == 10)
                {
                    Impoonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Imponotonepar.Add(ThisRoleId);
                    }
                }
            }
        if (!(CustomOption.CustomOptions.JackalFriendsOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.JackalFriendsOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.JackalFriends;
                if (OptionDate == 10)
                {
                    Crewonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Crewnotonepar.Add(ThisRoleId);
                    }
                }
            }
        if (!(CustomOption.CustomOptions.DoctorOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.DoctorOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.Doctor;
                if (OptionDate == 10)
                {
                    Crewonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Crewnotonepar.Add(ThisRoleId);
                    }
                }
            }
        if (!(CustomOption.CustomOptions.CountChangerOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.CountChangerOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.CountChanger;
                if (OptionDate == 10)
                {
                    Impoonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Imponotonepar.Add(ThisRoleId);
                    }
                }
            }
        if (!(CustomOption.CustomOptions.PursuerOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.PursuerOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.Pursuer;
                if (OptionDate == 10)
                {
                    Impoonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Imponotonepar.Add(ThisRoleId);
                    }
                }
            }
        if (!(CustomOption.CustomOptions.MinimalistOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.MinimalistOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.Minimalist;
                if (OptionDate == 10)
                {
                    Impoonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Imponotonepar.Add(ThisRoleId);
                    }
                }
            }
        if (!(CustomOption.CustomOptions.HawkOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.HawkOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.Hawk;
                if (OptionDate == 10)
                {
                    Impoonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Imponotonepar.Add(ThisRoleId);
                    }
                }
            }
        if (!(CustomOption.CustomOptions.EgoistOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.EgoistOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.Egoist;
                if (OptionDate == 10)
                {
                    Neutonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Neutnotonepar.Add(ThisRoleId);
                    }
                }
            }
        if (!(CustomOption.CustomOptions.NiceRedRidingHoodOption.getString().Replace("0%", "") == ""))
            {
                int OptionDate = int.Parse(CustomOption.CustomOptions.NiceRedRidingHoodOption.getString().Replace("0%", ""));
                RoleId ThisRoleId = RoleId.NiceRedRidingHood;
                if (OptionDate == 10)
                {
                    Crewonepar.Add(ThisRoleId);
                }
                else
                {
                    for (int i = 1; i <= OptionDate; i++)
                    {
                        Crewnotonepar.Add(ThisRoleId);
                    }
                }
            }
        //セットクラス
        }
    }
}
