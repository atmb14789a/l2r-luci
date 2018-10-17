﻿using Discord.Commands;
using Luci.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Luci.KillListService;

namespace Luci.Modules
{
    [Name("Log")]
    [Summary("Kill List")]
    public class KillListModule : ModuleBase<SocketCommandContext>
    {   /// <summary>
        /// RECENT
        /// </summary>
        /// <returns></returns>
        [Command("Recent")]
        [Summary("Recent Kills")]
        public async Task Recent()
        {
            IConfiguration _config = ConfigService._configuration;
            string RecentKillList = "```RECENT LIST```";

            List<KillListItem> KillLog = await KillListService.GetKillLogAsync();
            if (KillLog.Count != 0)
            {

                foreach (KillListItem killItem in KillLog)
                {
                    if (killItem.P1 != _config["killlist:specialname"] || killItem.P2 != _config["killlist:specialname"])
                    {

                        //Choose the KillList format for victory or defeat
                        string RecentKillListFormat = "";
                        if (killItem.Clan1 == _config["killlist:clanname"])
                        {
                            RecentKillListFormat += _config["killlist:victoryformat"] + "\r\n";
                        }
                        else
                        {
                            RecentKillListFormat += _config["killlist:defeatformat"] + "\r\n";
                        }

                        //Create formatted string for return
                        RecentKillList += string.Format(RecentKillListFormat,
                                    killItem.P1,
                                    (killItem.P1KillCount < 0) ? Convert.ToString(killItem.P1KillCount) : "+" + killItem.P1KillCount,
                                    killItem.Clan1,
                                    (killItem.Clan1KillCount < 0) ? Convert.ToString(killItem.Clan1KillCount) : "+" + killItem.Clan1KillCount,
                                    killItem.P2,
                                    (killItem.P2KillCount < 0) ? Convert.ToString(killItem.P2KillCount) : "+" + killItem.P2KillCount,
                                    killItem.Clan2,
                                    (killItem.Clan2KillCount < 0) ? Convert.ToString(killItem.Clan2KillCount) : "+" + killItem.Clan2KillCount,
                                    DateTime.Now);

                        //Return formatted string to Discord
                        await ReplyAsync(RecentKillList);
                    }

                }
            }
            else
            {
                await ReplyAsync("Kill List Currently Empty.");
            }
        }
    }


    /// <summary>
    /// RECENT
    /// </summary>
    /// <returns></returns>
    [Group("kills"), Name("Kills")]
    [Summary("vs")]
    public class Kills : ModuleBase
    {
        [Command("vs")]
        public async Task KillsForSpecificPvP(string P1, string P2)
        {
            IConfiguration _config = ConfigService._configuration;
            string RecentKillList = "";
            string RecentKillListFormat = "Player {0} has {1} kill(s) Vs. Player {2} has {3} kill(s).\r\n";
            int P1Count = 0;
            int P2Count = 0;

            List<KillListItem> KillLog = await KillListService.GetKillLogAsync();

            foreach (KillListItem killItem in KillLog)
            {

                if (killItem.P1.ToLower() == P1.ToLower() && killItem.P2.ToLower() == P2.ToLower())
                {

                    if (killItem.Clan1 == _config["killlist:clanname"])
                    {
                        P1Count++;
                        P2Count--;
                    }
                    else
                    {
                        P2Count++;
                        P1Count--;
                    }


                }


            }
            //Create formatted string for return
            RecentKillList += string.Format(RecentKillListFormat, P1, P1Count, P2, P2Count);

            //Return formatted string to Discord
            await ReplyAsync(RecentKillList);

        }


        /// <summary>
        /// RECENT
        /// </summary>
        /// <returns></returns>
        [Command("for")]
        [Summary("My Kills")]
        public async Task KillCountByPlayerAsync(string player)
        {
            int result = await KillListService.GetCountAsync(player, KillListType.Personal);
            string response = "";

            switch (player)
            {
                default:
                    response = string.Format("That asshole {0} has {1} kills.", player, result);
                    break;
            }

            await ReplyAsync(response);
        }




        /// <summary>
        /// RECENT
        /// </summary>
        /// <returns></returns>
        [Command("for clan")]
        [Summary("My Kills")]
        public async Task KillCountByClanAsync(string clan)
        {
            IConfiguration _config = ConfigService._configuration;
            int result = await KillListService.GetCountAsync(clan, KillListType.Clan);
            string response = "";
            bool UseRandom = true;

            string configmsg = _config["killlist:clanmessages:" + clan];
            if (configmsg != null)
            {
                response = string.Format(_config["killlist:clanmessages:" + clan], clan, result);
            }

            //if (UseConfigMessages)
            //{
            //    response = string.Format(_config["killlist:clanmessages:" + clan], clan, result);
            //}
            //else
            //{ 
            //    switch (clan)
            //    {
            //        case "RogueSquad":
            //            response = string.Format("Those pussies {0} have {1} kills... but they go down like a MOTHERFUCKER!", clan, result);
            //            break;

            //        case "Ascension":
            //            response = string.Format("Those bitches {0} have {1} kills... but they got good dick.", clan, result);
            //            break;

            //        case "Legacy":
            //            response = string.Format("Those crazy fuckers {0} have {1} kills...Oh look! Something shiney!!", clan, result);
            //            break;
            //    } 
            //}

            if (UseRandom && response == "")
            {
                Random Rnd = new Random();
                int selection = Rnd.Next(4);

                switch (selection)
                {
                    case 0:
                        response = string.Format("Those dipshits {0} have {1} kills.", clan, result);
                        break;
                    case 1:
                        response = string.Format("Those losers {0} have {1} kills.", clan, result);
                        break;
                    case 2:
                        response = string.Format("Those dicks {0} have {1} kills.", clan, result);
                        break;
                    case 3:
                        response = string.Format("Those crybabies {0} have {1} kills.", clan, result);
                        break;
                    case 4:
                        response = string.Format("Those asshats {0} have {1} kills.", clan, result);
                        break;
                }
            }


            await ReplyAsync(response);
        }
    }

}
