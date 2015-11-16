using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using apiTest.App_Code;
using System.Text.RegularExpressions;
using System.Net;
namespace apiTest.Controllers
{
    public class TFFController : Controller
    {
        //
        // GET: /TFF/
        [HttpGet]
        public JsonResult GetScorers()// string SessionKey, string Country, string City)
        {
            List<Scorer> list = new List<Scorer>();

            WebClient client = new WebClient();
            string playerPage = client.DownloadString("http://www.tff.org/default.aspx?pageID=821");

            int playerCount = 0;
            string playerTdlist = "";

            int start = playerPage.IndexOf("<table cellspacing=\"0\" cellpadding=\"2\" width=\"100%\">");
            if (start != -1)
            {
                int end = playerPage.IndexOf("</table>", start);
                int len = end - start;
                playerTdlist = playerPage.Substring(start, len).Trim();
                playerCount = Regex.Matches(playerTdlist, "<tr>").Count;
            }

            string[] playerArr = new string[playerCount];
            string[] teamArr = new string[playerCount];
            string[] goalCountArr = new string[playerCount];


            int lastInputInx = 0;
            for (int j = 0; j < playerCount; j++)
            {
                Scorer player = new Scorer();

                int linkStart = playerTdlist.IndexOf("kisiID", lastInputInx);
                linkStart = playerTdlist.IndexOf("\">", linkStart);
                if (linkStart != -1)
                {
                    int linkEnd = playerTdlist.IndexOf("</a>", linkStart);
                    int linkLen = linkEnd - linkStart - 2;

                    try
                    {
                        player.Name = playerTdlist.Substring(linkStart + 2, linkLen);
                        lastInputInx = linkEnd;
                    }
                    catch (Exception)
                    {
                    }
                }
                //----
                int teamStart = playerTdlist.IndexOf("kulupID", lastInputInx);
                teamStart = playerTdlist.IndexOf("\">", teamStart);
                if (teamStart != -1)
                {
                    int imgEnd = playerTdlist.IndexOf("</a>", teamStart);
                    int imgLen = imgEnd - teamStart - 2;

                    player.Team = playerTdlist.Substring(teamStart + 2, imgLen);

                    if (player.Team == "SUAT ALTIN İNŞAAT KAYSERİ ERCİYESSPOR")
                    {
                        player.Team = "KAYSERİ ERCİYESSPOR";
                    }
                    else if (player.Team == "AKHİSAR BELEDİYE GENÇLİK VE SPOR")
                    {
                        player.Team = "AKHİSAR";
                    }
                    else if (player.Team == "AKHİSAR BELEDİYE GENÇLİK VE SPOR")
                    {
                        player.Team = "AKHİSAR";
                    }

                    lastInputInx = imgEnd;
                }

                int scoreStart = playerTdlist.IndexOf("lblGolSayisi", lastInputInx);
                if (scoreStart != -1)
                {
                    int scoreEnd = playerTdlist.IndexOf("</span>", scoreStart);
                    int scoreLen = scoreEnd - scoreStart;

                    player.Score = Convert.ToInt32(playerTdlist.Substring(scoreStart + 14, scoreLen - 14));

                    lastInputInx = scoreEnd;
                }
                //----
                list.Add(player);
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetActualList()
        {
            List<Teams> list = new List<Teams>();

            WebClient client = new WebClient();
            string teamsPage = client.DownloadString("http://www.tff.org/Default.aspx?pageID=198");

            int teamCount = 0;
            string teamTdlist = "";

            int start = teamsPage.IndexOf("<table width=\"100%\" border=\"0\" cellpadding=\"1\" cellspacing=\"1\">");
            if (start != -1)
            {
                int end = teamsPage.IndexOf("</table>", start);
                int len = end - start;
                teamTdlist = teamsPage.Substring(start, len).Trim();
                teamCount = Regex.Matches(teamTdlist, "<tr>").Count;
            }

            string[] playerArr = new string[teamCount];
            string[] teamArr = new string[teamCount];
            string[] goalCountArr = new string[teamCount];


            int lastInputInx = 0;
            for (int j = 1; j < teamCount + 1; j++)
            {
                Teams team = new Teams();

                int linkStart = teamTdlist.IndexOf("kulupID", lastInputInx);
                int linkStart1 = teamTdlist.IndexOf("\">", linkStart);
                if (linkStart1 != -1)
                {
                    int linkEnd = teamTdlist.IndexOf("</a>", linkStart1);
                    int linkLen = linkEnd - linkStart1;

                    try
                    {
                        team.Name = teamTdlist.Substring(linkStart1 + 2, linkLen - 2);
                        if (team.Name.Contains("SUAT ALTIN İNŞAAT KAYSERİ ERCİYESSPOR"))
                        {
                            team.Name = team.Name.Replace("SUAT ALTIN İNŞAAT KAYSERİ ERCİYESSPOR", "KAYSERİ ERCİYESSPOR");
                        }
                        else if (team.Name.Contains("AKHİSAR BELEDİYE GENÇLİK VE SPOR"))
                        {
                            team.Name = team.Name.Replace("AKHİSAR BELEDİYE GENÇLİK VE SPOR", "AKHİSAR");
                        }
                        else if (team.Name.Contains("İSTANBUL BAŞAKŞEHİR A.Ş."))
                        {
                            team.Name = team.Name.Replace("İSTANBUL BAŞAKŞEHİR A.Ş.", "İST BAŞAKŞEHİR");
                        }
                        lastInputInx = linkEnd;
                    }
                    catch (Exception)
                    {
                    }


                    int gameStart = teamTdlist.IndexOf("_lblOyun\">", lastInputInx);
                    if (gameStart != -1)
                    {
                        int gameEnd = teamTdlist.IndexOf("</span>", gameStart);
                        int gameLen = gameEnd - gameStart;

                        team.Game = teamTdlist.Substring(gameStart + 10, gameLen - 10);

                        lastInputInx = gameEnd;
                    }

                    int winStart = teamTdlist.IndexOf("_Label4\">", lastInputInx);
                    if (winStart != -1)
                    {
                        int winEnd = teamTdlist.IndexOf("</span>", winStart);
                        int winLen = winEnd - winStart;

                        team.Win = teamTdlist.Substring(winStart + 9, winLen - 9);

                        lastInputInx = winEnd;
                    }


                    int drawStart = teamTdlist.IndexOf("_lblKazanma\">", lastInputInx);
                    if (drawStart != -1)
                    {
                        int drawEnd = teamTdlist.IndexOf("</span>", drawStart);
                        int drawLen = drawEnd - drawStart;

                        team.Draw = teamTdlist.Substring(drawStart + 13, drawLen - 13);

                        lastInputInx = drawEnd;
                    }

                    int lostStart = teamTdlist.IndexOf("_lblPuan\">", lastInputInx);
                    if (lostStart != -1)
                    {
                        int lostEnd = teamTdlist.IndexOf("</span>", lostStart);
                        int lostLen = lostEnd - lostStart;

                        team.Lost = teamTdlist.Substring(lostStart + 10, lostLen - 10);

                        lastInputInx = lostEnd;
                    }

                    int plusStart = teamTdlist.IndexOf("_Label1\">", lastInputInx);
                    if (plusStart != -1)
                    {
                        int plusEnd = teamTdlist.IndexOf("</span>", plusStart);
                        int plusLen = plusEnd - plusStart;

                        team.Plus = teamTdlist.Substring(plusStart + 9, plusLen - 9);

                        lastInputInx = plusEnd;
                    }

                    int minusStart = teamTdlist.IndexOf("_Label2\">", lastInputInx);
                    if (minusStart != -1)
                    {
                        int minusEnd = teamTdlist.IndexOf("</span>", minusStart);
                        int minusLen = minusEnd - minusStart;

                        team.Minus = teamTdlist.Substring(minusStart + 9, minusLen - 9);

                        lastInputInx = minusEnd;
                    }

                    int avgStart = teamTdlist.IndexOf("_Label5\">", lastInputInx);
                    if (avgStart != -1)
                    {
                        int avgEnd = teamTdlist.IndexOf("</span>", avgStart);
                        int avgLen = avgEnd - avgStart;

                        team.Average = teamTdlist.Substring(avgStart + 9, avgLen - 9);

                        lastInputInx = avgEnd;
                    }

                    int puanStart = teamTdlist.IndexOf("_Label3\">", lastInputInx);
                    if (puanStart != -1)
                    {
                        int puanEnd = teamTdlist.IndexOf("</span>", puanStart);
                        int puanLen = puanEnd - puanStart;

                        team.Point = teamTdlist.Substring(puanStart + 9, puanLen - 9);

                        lastInputInx = puanEnd;
                    }
                }
                list.Add(team);
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetFixture(int id)
        {
            List<Matchs> list = new List<Matchs>();

            WebClient client = new WebClient();
            string teamsPage = client.DownloadString("http://www.tff.org/Default.aspx?pageID=198&hafta=" + id.ToString());

            int teamCount = 0;
            int lastInput = 0;

            teamCount = Regex.Matches(teamsPage, "<table width=\"100%\" border=\"0\" cellpadding=\"3\" cellspacing=\"0\">").Count;
            lastInput = teamsPage.IndexOf("<table width=\"100%\" border=\"0\" cellpadding=\"3\" cellspacing=\"0\">");
            string[] scoreDiv = new string[teamCount];

            for (int i = 0; i < teamCount; i++)
            {
                int listStart = teamsPage.IndexOf("<div id=", lastInput);
                int listEnd = teamsPage.IndexOf("</div>", listStart);
                int listLen = listEnd - listStart;
                scoreDiv[i] = teamsPage.Substring(listStart, listLen + 6);
                lastInput = listEnd;
            }

            for (int i = 0; i < scoreDiv.Length; i++)
            {
                Matchs row = new Matchs();
                int lastInputInx = 0;
                int dateStart = scoreDiv[i].IndexOf("lblTarih", lastInputInx);
                if (dateStart != -1)
                {
                    int dateEnd = scoreDiv[i].IndexOf("</span>", dateStart);
                    int dateLen = dateEnd - dateStart;
                    row.MatchDate = scoreDiv[i].Substring(dateStart + 10, dateLen - 10);
                    lastInputInx = dateEnd;
                }

                int timeStart = scoreDiv[i].IndexOf("lblSaat", lastInputInx);
                if (timeStart != -1)
                {
                    int timeEnd = scoreDiv[i].IndexOf("</span>", timeStart);
                    int timeLen = timeEnd - timeStart;
                    row.MatchTime = scoreDiv[i].Substring(timeStart + 9, timeLen - 9);
                    lastInputInx = timeEnd;
                }

                int homeTeamStart = scoreDiv[i].IndexOf("Label4", lastInputInx);
                if (homeTeamStart != -1)
                {
                    int homeTeamEnd = scoreDiv[i].IndexOf("</span>", homeTeamStart);
                    int homeTeamLen = homeTeamEnd - homeTeamStart;
                    row.HomeTeam = scoreDiv[i].Substring(homeTeamStart + 8, homeTeamLen - 8);
                    lastInputInx = homeTeamEnd;
                    if (row.HomeTeam == "SUAT ALTIN İNŞAAT KAYSERİ ERCİYESSPOR")
                    {
                        row.HomeTeam = "KAYSERİ ERCİYESSPOR";
                        row.HomeTeamLogo = "kayserierciyes.png";
                    }
                    else if (row.HomeTeam == "AKHİSAR BELEDİYE GENÇLİK VE SPOR")
                    {
                        row.HomeTeam = "AKHİSAR";
                        row.HomeTeamLogo = "akhisar.png";
                    }
                    else if (row.HomeTeam == "İSTANBUL BAŞAKŞEHİR A.Ş.")
                    {
                        row.HomeTeam = "İST BAŞAKŞEHİR";
                        row.HomeTeamLogo = "basaksehir.png";
                    }
                    else if (row.HomeTeam == "KARDEMİR KARABÜKSPOR")
                    {
                        row.HomeTeam = "KARABÜKSPOR";
                        row.HomeTeamLogo = "karabuk.png";
                    }
                    else if (row.HomeTeam == "FENERBAHÇE A.Ş.")
                    {
                        row.HomeTeamLogo = "fenerbahce.png";
                    }
                    else if (row.HomeTeam == "BEŞİKTAŞ A.Ş.")
                    {
                        row.HomeTeamLogo = "besiktas.png";
                    }
                    else if (row.HomeTeam == "GALATASARAY A.Ş.")
                    {
                        row.HomeTeamLogo = "galatasaray.png";
                    }
                    else if (row.HomeTeam == "BURSASPOR")
                    {
                        row.HomeTeamLogo = "bursaspor.png";
                    }
                    else if (row.HomeTeam == "TRABZONSPOR A.Ş.")
                    {
                        row.HomeTeamLogo = "trabzonspor.png";
                    }
                    else if (row.HomeTeam == "GAZİANTEPSPOR")
                    {
                        row.HomeTeamLogo = "gaziantep.png";
                    }
                    else if (row.HomeTeam == "MERSİN İDMANYURDU")
                    {
                        row.HomeTeamLogo = "mersin.png";
                    }
                    else if (row.HomeTeam == "KASIMPAŞA A.Ş.")
                    {
                        row.HomeTeamLogo = "kasimpasa.png";
                    }
                    else if (row.HomeTeam == "GENÇLERBİRLİĞİ")
                    {
                        row.HomeTeamLogo = "genclerbirligi.png";
                    }
                    else if (row.HomeTeam == "ESKİŞEHİRSPOR")
                    {
                        row.HomeTeamLogo = "eskisehir.png";
                    }
                    else if (row.HomeTeam == "TORKU KONYASPOR")
                    {
                        row.HomeTeamLogo = "konyaspor.png";
                    }
                    else if (row.HomeTeam == "ÇAYKUR RİZESPOR A.Ş.")
                    {
                        row.HomeTeamLogo = "rizespor.png";
                    }
                    else if (row.HomeTeam.Contains("SİVAS"))
                    {
                        row.HomeTeamLogo = "sivasspor.png";
                    }
                    else if (row.HomeTeam == "BALIKESİRSPOR")
                    {
                        row.HomeTeamLogo = "balikesirspor.png";
                    }
                }

                int homeScoreStart = scoreDiv[i].IndexOf("Label5", lastInputInx);
                if (homeScoreStart != -1)
                {
                    int homeScoreEnd = scoreDiv[i].IndexOf("</span>", homeScoreStart);
                    int homeScoreLen = homeScoreEnd - homeScoreStart;
                    row.HomeScore = scoreDiv[i].Substring(homeScoreStart + 8, homeScoreLen - 8);
                    lastInputInx = homeScoreEnd;
                }

                int displacementScoreStart = scoreDiv[i].IndexOf("Label6", lastInputInx);
                if (displacementScoreStart != -1)
                {
                    int displacementScoreEnd = scoreDiv[i].IndexOf("</span>", displacementScoreStart);
                    int displacementScoreLen = displacementScoreEnd - displacementScoreStart;
                    row.DisplacementScore = scoreDiv[i].Substring(displacementScoreStart + 8, displacementScoreLen - 8);
                    lastInputInx = displacementScoreEnd;
                }

                int displacementTeamStart = scoreDiv[i].IndexOf("Label1", lastInputInx);
                if (displacementTeamStart != -1)
                {
                    int displacementTeamEnd = scoreDiv[i].IndexOf("</span>", displacementTeamStart);
                    int displacementTeamLen = displacementTeamEnd - displacementTeamStart;
                    row.DisplacementTeam = scoreDiv[i].Substring(displacementTeamStart + 8, displacementTeamLen - 8);
                    lastInputInx = displacementTeamEnd;
                    if (row.DisplacementTeam == "SUAT ALTIN İNŞAAT KAYSERİ ERCİYESSPOR")
                    {
                        row.DisplacementTeam = "KAYSERİ ERCİYESSPOR";
                        row.DisplacementLogo = "kayserierciyes.png";
                    }
                    else if (row.DisplacementTeam == "AKHİSAR BELEDİYE GENÇLİK VE SPOR")
                    {
                        row.DisplacementTeam = "AKHİSAR";
                        row.DisplacementLogo = "akhisar.png";
                    }
                    else if (row.DisplacementTeam == "İSTANBUL BAŞAKŞEHİR A.Ş.")
                    {
                        row.DisplacementTeam = "İST BAŞAKŞEHİR";
                        row.DisplacementLogo = "basaksehir.png";
                    }
                    else if (row.DisplacementTeam == "KARDEMİR KARABÜKSPOR")
                    {
                        row.DisplacementTeam = "KARABÜKSPOR";
                        row.DisplacementLogo = "karabuk.png";
                    }
                    else if (row.DisplacementTeam == "FENERBAHÇE A.Ş.")
                    {
                        row.DisplacementLogo = "fenerbahce.png";
                    }
                    else if (row.DisplacementTeam == "BEŞİKTAŞ A.Ş.")
                    {
                        row.DisplacementLogo = "besiktas.png";
                    }
                    else if (row.DisplacementTeam == "GALATASARAY A.Ş.")
                    {
                        row.DisplacementLogo = "galatasaray.png";
                    }
                    else if (row.DisplacementTeam == "BURSASPOR")
                    {
                        row.DisplacementLogo = "bursaspor.png";
                    }
                    else if (row.DisplacementTeam == "TRABZONSPOR A.Ş.")
                    {
                        row.DisplacementLogo = "trabzonspor.png";
                    }
                    else if (row.DisplacementTeam == "GAZİANTEPSPOR")
                    {
                        row.DisplacementLogo = "gaziantep.png";
                    }
                    else if (row.DisplacementTeam == "MERSİN İDMANYURDU")
                    {
                        row.DisplacementLogo = "mersin.png";
                    }
                    else if (row.DisplacementTeam == "KASIMPAŞA A.Ş.")
                    {
                        row.DisplacementLogo = "kasimpasa.png";
                    }
                    else if (row.DisplacementTeam == "GENÇLERBİRLİĞİ")
                    {
                        row.DisplacementLogo = "genclerbirligi.png";
                    }
                    else if (row.DisplacementTeam == "ESKİŞEHİRSPOR")
                    {
                        row.DisplacementLogo = "eskisehir.png";
                    }
                    else if (row.DisplacementTeam == "TORKU KONYASPOR")
                    {
                        row.DisplacementLogo = "konyaspor.png";
                    }
                    else if (row.DisplacementTeam == "ÇAYKUR RİZESPOR A.Ş.")
                    {
                        row.DisplacementLogo = "rizespor.png";
                    }
                    else if (row.DisplacementTeam.Contains("SİVAS"))
                    {
                        row.DisplacementLogo = "sivasspor.png";
                    }
                    else if (row.DisplacementTeam == "BALIKESİRSPOR")
                    {
                        row.DisplacementLogo = "balikesirspor.png";
                    }
                }


                list.Add(row);
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult LiveScore(int id)
        {
            string url = "";

            if (id == 1)
            {
                url = "http://m.futbolingo.com/canli-sonuclar";
            }
            else if (id == 0)//Testing to html scrap
            {
                url = "http://m.futbolingo.com/mac-sonuclari/2015/01/24";
            }
            else
            {
                url = "http://m.futbolingo.com/mac-sonuclari";
            }
            List<LiveScore> list = new List<LiveScore>();
            WebClient client = new WebClient();
            client.Encoding = System.Text.Encoding.UTF8;
            string liveScorePage = client.DownloadString(url);
            string canliMaclar = "";

            int lastIndex = 0;
            int skorBoxStart = liveScorePage.IndexOf("<div class=\"scorebox\">", lastIndex);

            if (skorBoxStart != -1)
            {
                int skorBoxEnd = liveScorePage.IndexOf("<div class=\"cls\">", skorBoxStart);
                int skorBoxLen = skorBoxEnd - skorBoxStart;
                string maclar = liveScorePage.Substring(skorBoxStart, skorBoxLen);


                int superLigStart = maclar.IndexOf("Türkiye - Spor Toto SüperLig", 0);
                if (superLigStart != -1)
                {
                    //if there is any other game
                    try
                    {
                        int superLigEnd = maclar.IndexOf("<div class='title'>", superLigStart);
                        int superLigLen = superLigEnd - superLigStart;
                        canliMaclar = maclar.Substring(superLigStart, superLigLen);

                        int macSayisi = Regex.Matches(canliMaclar, "<div class='trow1'>").Count;
                        string[] scoreDiv = new string[macSayisi];

                        int lastInput = 0;
                        for (int i = 0; i < macSayisi; i++)
                        {
                            int listStart = canliMaclar.IndexOf("<div class='trow1'>", lastInput);
                            int listEnd = canliMaclar.IndexOf("class='tname_a'", listStart);
                            listEnd = canliMaclar.IndexOf("</div>", listEnd);
                            int listLen = listEnd - listStart;
                            scoreDiv[i] = canliMaclar.Substring(listStart, listLen + 6);
                            lastInput = listEnd;
                        }

                        for (int i = 0; i < scoreDiv.Length; i++)
                        {
                            int lastInputInx = 0;
                            LiveScore skor = new LiveScore();

                            int kodStart = scoreDiv[i].IndexOf("class='tc'>", lastInputInx);
                            if (kodStart != -1)
                            {
                                int kodEnd = scoreDiv[i].IndexOf("</div>", kodStart);
                                int kodLen = kodEnd - kodStart;
                                skor.Code = scoreDiv[i].Substring(kodStart + 11, kodLen - 11);
                                lastInputInx = kodEnd;
                            }

                            int dakikaStart = scoreDiv[i].IndexOf("class='tc'>", lastInputInx);
                            if (dakikaStart != -1)
                            {
                                int dakikaEnd = scoreDiv[i].IndexOf("</div>", dakikaStart);
                                int dakikaLen = dakikaEnd - dakikaStart;
                                skor.Minute = scoreDiv[i].Substring(dakikaStart + 11, dakikaLen - 12);
                                if (!skor.Minute.Contains("M") && !skor.Minute.Contains("I"))
                                {
                                    skor.Minute = skor.Minute + "  dk";
                                }
                                lastInputInx = dakikaEnd;
                            }

                            int homeTeamStart = scoreDiv[i].IndexOf("class='tname_h'>", lastInputInx);
                            if (homeTeamStart != -1)
                            {
                                int homeTeamEnd = scoreDiv[i].IndexOf("</div>", homeTeamStart);
                                int homeTeamLen = homeTeamEnd - homeTeamStart;
                                skor.HomeTeam = scoreDiv[i].Substring(homeTeamStart + 16, homeTeamLen - 16);
                                if (skor.HomeTeam.Contains("img"))
                                {
                                    int startImg = skor.HomeTeam.IndexOf("<img");
                                    startImg = skor.HomeTeam.IndexOf(">", startImg);
                                    skor.HomeTeam = skor.HomeTeam.Substring(startImg + 1, skor.HomeTeam.Length - 1 - startImg);
                                }

                                if (skor.HomeTeam.Contains("Kayseri"))
                                {
                                    skor.HomeTeam = "Kayseri Erciyes";
                                    skor.HomeTeamLogo = "kayserierciyes.png";
                                }
                                else if (skor.HomeTeam.Contains("Akhisar"))
                                {
                                    skor.HomeTeam = "Akhisar";
                                    skor.HomeTeamLogo = "akhisar.png";
                                }
                                else if (skor.HomeTeam.Contains("İstanbul"))
                                {
                                    skor.HomeTeam = "İst Başakşehir";
                                    skor.HomeTeamLogo = "basaksehir.png";
                                }
                                else if (skor.HomeTeam.Contains("Karabükspor"))
                                {
                                    skor.HomeTeam = "Karabükspor";
                                    skor.HomeTeamLogo = "karabuk.png";
                                }
                                else if (skor.HomeTeam.Contains("Fenerbahçe"))
                                {
                                    skor.HomeTeamLogo = "fenerbahce.png";
                                }
                                else if (skor.HomeTeam.Contains("Beşiktaş"))
                                {
                                    skor.HomeTeamLogo = "besiktas.png";
                                }
                                else if (skor.HomeTeam.Contains("Galatasaray"))
                                {
                                    skor.HomeTeamLogo = "galatasaray.png";
                                }
                                else if (skor.HomeTeam.Contains("BURSASPOR"))
                                {
                                    skor.HomeTeamLogo = "bursaspor.png";
                                }
                                else if (skor.HomeTeam.Contains("Trabzonspor"))
                                {
                                    skor.HomeTeamLogo = "trabzonspor.png";
                                }
                                else if (skor.HomeTeam.Contains("Gaziantepspor"))
                                {
                                    skor.HomeTeamLogo = "gaziantep.png";
                                }
                                else if (skor.HomeTeam.Contains("Mersin"))
                                {
                                    skor.HomeTeamLogo = "mersin.png";
                                }
                                else if (skor.HomeTeam.Contains("Kasımpaşa"))
                                {
                                    skor.HomeTeamLogo = "kasimpasa.png";
                                }
                                else if (skor.HomeTeam.Contains("Gençlerbirliği"))
                                {
                                    skor.HomeTeamLogo = "genclerbirligi.png";
                                }
                                else if (skor.HomeTeam.Contains("Eskişehirspor"))
                                {
                                    skor.HomeTeamLogo = "eskisehir.png";
                                }
                                else if (skor.HomeTeam.Contains("Konyaspor"))
                                {
                                    skor.HomeTeamLogo = "konyaspor.png";
                                }
                                else if (skor.HomeTeam.Contains("Rizespor"))
                                {
                                    skor.HomeTeamLogo = "rizespor.png";
                                }
                                else if (skor.HomeTeam.Contains("Sivasspor"))
                                {
                                    skor.HomeTeamLogo = "sivasspor.png";
                                }
                                else if (skor.HomeTeam.Contains("Balıkesirspor"))
                                {
                                    skor.HomeTeamLogo = "balikesirspor.png";
                                }

                                lastInputInx = homeTeamEnd;
                            }

                            int sonucStart = scoreDiv[i].IndexOf("class='mscore'>", lastInputInx);
                            if (sonucStart != -1)
                            {
                                int sonucEnd = scoreDiv[i].IndexOf("</a>", sonucStart);
                                int sonucLen = sonucEnd - sonucStart;
                                skor.Score = scoreDiv[i].Substring(sonucStart + 15, sonucLen - 15);
                                lastInputInx = sonucEnd;
                            }

                            int deplasmanStart = scoreDiv[i].IndexOf("class='tname_a'>", lastInputInx);
                            if (deplasmanStart != -1)
                            {
                                int deplasmanEnd = scoreDiv[i].IndexOf("</div>", deplasmanStart);
                                int deplasmanLen = deplasmanEnd - deplasmanStart;
                                skor.DisplacementTeam = scoreDiv[i].Substring(deplasmanStart + 16, deplasmanLen - 16);
                                if (skor.DisplacementTeam.Contains("img"))
                                {
                                    int startImg = skor.DisplacementTeam.IndexOf("<img");
                                    skor.DisplacementTeam = skor.DisplacementTeam.Substring(0, startImg);
                                }
                                if (skor.DisplacementTeam.Contains("Kayseri"))
                                {
                                    skor.DisplacementTeam = "Kayseri Erciyes";
                                    skor.DisplacementTeamLogo = "kayserierciyes.png";
                                }
                                else if (skor.DisplacementTeam.Contains("Akhisar"))
                                {
                                    skor.DisplacementTeam = "Akhisar";
                                    skor.DisplacementTeamLogo = "akhisar.png";
                                }
                                else if (skor.DisplacementTeam.Contains("İstanbul"))
                                {
                                    skor.DisplacementTeam = "İst Başakşehir";
                                    skor.DisplacementTeamLogo = "basaksehir.png";
                                }
                                else if (skor.DisplacementTeam.Contains("Karabükspor"))
                                {
                                    skor.DisplacementTeam = "Karabükspor";
                                    skor.DisplacementTeamLogo = "karabuk.png";
                                }
                                else if (skor.DisplacementTeam.Contains("Fenerbahçe"))
                                {
                                    skor.DisplacementTeamLogo = "fenerbahce.png";
                                }
                                else if (skor.DisplacementTeam.Contains("Beşiktaş"))
                                {
                                    skor.DisplacementTeamLogo = "besiktas.png";
                                }
                                else if (skor.DisplacementTeam.Contains("Galatasaray"))
                                {
                                    skor.DisplacementTeamLogo = "galatasaray.png";
                                }
                                else if (skor.DisplacementTeam.Contains("Bursaspor"))
                                {
                                    skor.DisplacementTeamLogo = "bursaspor.png";
                                }
                                else if (skor.DisplacementTeam.Contains("Trabzonspor"))
                                {
                                    skor.DisplacementTeamLogo = "trabzonspor.png";
                                }
                                else if (skor.DisplacementTeam.Contains("Gaziantepspor"))
                                {
                                    skor.DisplacementTeamLogo = "gaziantep.png";
                                }
                                else if (skor.DisplacementTeam.Contains("Mersin"))
                                {
                                    skor.DisplacementTeamLogo = "mersin.png";
                                }
                                else if (skor.DisplacementTeam.Contains("Kasımpaşa"))
                                {
                                    skor.DisplacementTeamLogo = "kasimpasa.png";
                                }
                                else if (skor.DisplacementTeam.Contains("Gençlerbirliği"))
                                {
                                    skor.DisplacementTeamLogo = "genclerbirligi.png";
                                }
                                else if (skor.DisplacementTeam.Contains("Eskişehirspor"))
                                {
                                    skor.DisplacementTeamLogo = "eskisehir.png";
                                }
                                else if (skor.DisplacementTeam.Contains("Konyaspor"))
                                {
                                    skor.DisplacementTeamLogo = "konyaspor.png";
                                }
                                else if (skor.DisplacementTeam.Contains("Rizespor"))
                                {
                                    skor.DisplacementTeamLogo = "rizespor.png";
                                }
                                else if (skor.DisplacementTeam.Contains("Sivasspor"))
                                {
                                    skor.DisplacementTeamLogo = "sivasspor.png";
                                }
                                else if (skor.DisplacementTeam.Contains("Balıkesirspor"))
                                {
                                    skor.DisplacementTeamLogo = "balikesirspor.png";
                                }

                                lastInputInx = deplasmanEnd;
                            }
                            list.Add(skor);
                        }

                    }
                    catch (Exception)
                    {
                        int superLigEnd = maclar.IndexOf("</div></div>", superLigStart);
                        int superLigLen = superLigEnd - superLigStart;
                        canliMaclar = maclar.Substring(superLigStart, superLigLen);

                        int macSayisi = Regex.Matches(canliMaclar, "<div class='trow1'>").Count;
                        string[] scoreDiv = new string[macSayisi];

                        int lastInput = 0;
                        for (int i = 0; i < macSayisi; i++)
                        {
                            int listStart = canliMaclar.IndexOf("<div class='trow1'>", lastInput);
                            int listEnd = canliMaclar.IndexOf("class='tname_a'", listStart);
                            listEnd = canliMaclar.IndexOf("</div>", listEnd);
                            listEnd = canliMaclar.IndexOf("</div>", listEnd);
                            int listLen = listEnd - listStart;
                            scoreDiv[i] = canliMaclar.Substring(listStart, listLen + 6);
                            lastInput = listEnd;
                        }

                        for (int i = 0; i < scoreDiv.Length; i++)
                        {
                            int lastInputInx = 0;
                            LiveScore skor = new LiveScore();

                            int kodStart = scoreDiv[i].IndexOf("class='tc'>", lastInputInx);
                            if (kodStart != -1)
                            {
                                int kodEnd = scoreDiv[i].IndexOf("</div>", kodStart);
                                int kodLen = kodEnd - kodStart;
                                skor.Code = scoreDiv[i].Substring(kodStart + 11, kodLen - 11);
                                lastInputInx = kodEnd;
                            }

                            int dakikaStart = scoreDiv[i].IndexOf("class='tc'>", lastInputInx);
                            if (dakikaStart != -1)
                            {
                                int dakikaEnd = scoreDiv[i].IndexOf("</div>", dakikaStart);
                                int dakikaLen = dakikaEnd - dakikaStart;
                                skor.Minute = scoreDiv[i].Substring(dakikaStart + 11, dakikaLen - 12);
                                if (!skor.Minute.Contains("M") && !skor.Minute.Contains("I"))
                                {
                                    skor.Minute = skor.Minute + "  dk";
                                }
                                lastInputInx = dakikaEnd;
                            }

                            int homeTeamStart = scoreDiv[i].IndexOf("class='tname_h'>", lastInputInx);
                            if (homeTeamStart != -1)
                            {
                                int homeTeamEnd = scoreDiv[i].IndexOf("</div>", homeTeamStart);
                                int homeTeamLen = homeTeamEnd - homeTeamStart;
                                skor.HomeTeam = scoreDiv[i].Substring(homeTeamStart + 16, homeTeamLen - 16);
                                if (skor.HomeTeam.Contains("img"))
                                {
                                    int startImg = skor.HomeTeam.IndexOf("<img");
                                    startImg = skor.HomeTeam.IndexOf(">", startImg);
                                    skor.HomeTeam = skor.HomeTeam.Substring(startImg + 1, skor.HomeTeam.Length - 1 - startImg);

                                }

                                if (skor.HomeTeam.Contains("Kayseri"))
                                {
                                    skor.HomeTeam = "Kayseri Erciyes";
                                    skor.HomeTeamLogo = "kayserierciyes.png";
                                }
                                else if (skor.HomeTeam.Contains("Akhisar"))
                                {
                                    skor.HomeTeam = "Akhisar";
                                    skor.HomeTeamLogo = "akhisar.png";
                                }
                                else if (skor.HomeTeam.Contains("İstanbul"))
                                {
                                    skor.HomeTeam = "İst Başakşehir";
                                    skor.HomeTeamLogo = "basaksehir.png";
                                }
                                else if (skor.HomeTeam.Contains("Karabükspor"))
                                {
                                    skor.HomeTeam = "Karabükspor";
                                    skor.HomeTeamLogo = "karabuk.png";
                                }
                                else if (skor.HomeTeam.Contains("Fenerbahçe"))
                                {
                                    skor.HomeTeamLogo = "fenerbahce.png";
                                }
                                else if (skor.HomeTeam.Contains("Beşiktaş"))
                                {
                                    skor.HomeTeamLogo = "besiktas.png";
                                }
                                else if (skor.HomeTeam.Contains("Galatasaray"))
                                {
                                    skor.HomeTeamLogo = "galatasaray.png";
                                }
                                else if (skor.HomeTeam.Contains("Bursaspor"))
                                {
                                    skor.HomeTeamLogo = "bursaspor.png";
                                }
                                else if (skor.HomeTeam.Contains("Trabzonspor"))
                                {
                                    skor.HomeTeamLogo = "trabzonspor.png";
                                }
                                else if (skor.HomeTeam.Contains("Gaziantepspor"))
                                {
                                    skor.HomeTeamLogo = "gaziantep.png";
                                }
                                else if (skor.HomeTeam.Contains("Mersin"))
                                {
                                    skor.HomeTeamLogo = "mersin.png";
                                }
                                else if (skor.HomeTeam.Contains("Kasımpaşa"))
                                {
                                    skor.HomeTeamLogo = "kasimpasa.png";
                                }
                                else if (skor.HomeTeam.Contains("Gençlerbirliği"))
                                {
                                    skor.HomeTeamLogo = "genclerbirligi.png";
                                }
                                else if (skor.HomeTeam.Contains("Eskişehirspor"))
                                {
                                    skor.HomeTeamLogo = "eskisehir.png";
                                }
                                else if (skor.HomeTeam.Contains("Konyaspor"))
                                {
                                    skor.HomeTeamLogo = "konyaspor.png";
                                }
                                else if (skor.HomeTeam.Contains("Rizespor"))
                                {
                                    skor.HomeTeamLogo = "rizespor.png";
                                }
                                else if (skor.HomeTeam.Contains("Sivasspor"))
                                {
                                    skor.HomeTeamLogo = "sivasspor.png";
                                }
                                else if (skor.HomeTeam.Contains("Balıkesirspor"))
                                {
                                    skor.HomeTeamLogo = "balikesirspor.png";
                                }

                                lastInputInx = homeTeamEnd;
                            }

                            int sonucStart = scoreDiv[i].IndexOf("class='mscore'>", lastInputInx);
                            if (sonucStart != -1)
                            {
                                int sonucEnd = scoreDiv[i].IndexOf("</a>", sonucStart);
                                int sonucLen = sonucEnd - sonucStart;
                                skor.Score = scoreDiv[i].Substring(sonucStart + 15, sonucLen - 15);
                                lastInputInx = sonucEnd;
                            }

                            int deplasmanStart = scoreDiv[i].IndexOf("class='tname_a'>", lastInputInx);
                            if (deplasmanStart != -1)
                            {
                                int deplasmanEnd = scoreDiv[i].IndexOf("</div>", deplasmanStart);
                                int deplasmanLen = deplasmanEnd - deplasmanStart;
                                skor.DisplacementTeam = scoreDiv[i].Substring(deplasmanStart + 16, deplasmanLen - 16);
                                if (skor.DisplacementTeam.Contains("img"))
                                {
                                    int startImg = skor.DisplacementTeam.IndexOf("<img");
                                    skor.DisplacementTeam = skor.DisplacementTeam.Substring(0, startImg);
                                }

                                if (skor.DisplacementTeam.Contains("Kayseri"))
                                {
                                    skor.DisplacementTeam = "Kayseri Erciyes";
                                    skor.DisplacementTeamLogo = "kayserierciyes.png";
                                }
                                else if (skor.DisplacementTeam.Contains("Akhisar"))
                                {
                                    skor.DisplacementTeam = "Akhisar";
                                    skor.DisplacementTeamLogo = "akhisar.png";
                                }
                                else if (skor.DisplacementTeam.Contains("İstanbul"))
                                {
                                    skor.DisplacementTeam = "İst Başakşehir";
                                    skor.DisplacementTeamLogo = "basaksehir.png";
                                }
                                else if (skor.DisplacementTeam.Contains("Karabükspor"))
                                {
                                    skor.DisplacementTeam = "Karabükspor";
                                    skor.DisplacementTeamLogo = "karabuk.png";
                                }
                                else if (skor.DisplacementTeam.Contains("Fenerbahçe"))
                                {
                                    skor.DisplacementTeamLogo = "fenerbahce.png";
                                }
                                else if (skor.DisplacementTeam.Contains("Beşiktaş"))
                                {
                                    skor.DisplacementTeamLogo = "besiktas.png";
                                }
                                else if (skor.DisplacementTeam.Contains("Galatasaray"))
                                {
                                    skor.DisplacementTeamLogo = "galatasaray.png";
                                }
                                else if (skor.DisplacementTeam.Contains("BURSASPOR"))
                                {
                                    skor.DisplacementTeamLogo = "bursaspor.png";
                                }
                                else if (skor.DisplacementTeam.Contains("Trabzonspor"))
                                {
                                    skor.DisplacementTeamLogo = "trabzonspor.png";
                                }
                                else if (skor.DisplacementTeam.Contains("Gaziantepspor"))
                                {
                                    skor.DisplacementTeamLogo = "gaziantep.png";
                                }
                                else if (skor.DisplacementTeam.Contains("Mersin"))
                                {
                                    skor.DisplacementTeamLogo = "mersin.png";
                                }
                                else if (skor.DisplacementTeam.Contains("Kasımpaşa"))
                                {
                                    skor.DisplacementTeamLogo = "kasimpasa.png";
                                }
                                else if (skor.DisplacementTeam.Contains("Gençlerbirliği"))
                                {
                                    skor.DisplacementTeamLogo = "genclerbirligi.png";
                                }
                                else if (skor.DisplacementTeam.Contains("Eskişehirspor"))
                                {
                                    skor.DisplacementTeamLogo = "eskisehir.png";
                                }
                                else if (skor.DisplacementTeam.Contains("Konyaspor"))
                                {
                                    skor.DisplacementTeamLogo = "konyaspor.png";
                                }
                                else if (skor.DisplacementTeam.Contains("Rizespor"))
                                {
                                    skor.DisplacementTeamLogo = "rizespor.png";
                                }
                                else if (skor.DisplacementTeam.Contains("Sivasspor"))
                                {
                                    skor.DisplacementTeamLogo = "sivasspor.png";
                                }
                                else if (skor.DisplacementTeam.Contains("Balıkesirspor"))
                                {
                                    skor.DisplacementTeamLogo = "balikesirspor.png";
                                }

                                lastInputInx = deplasmanEnd;
                            }
                            list.Add(skor);
                        }
                    }
                }
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        //iddaaTahmin sonuçları
        [HttpGet]
        public JsonResult GetCoupouns()
        {
            List<Coupon> list = new List<Coupon>();
            WebClient client = new WebClient();
            client.Encoding = System.Text.Encoding.UTF8;
            string bankoPage = client.DownloadString("https://www.iddaatahmin.com/banko-kuponlar.html");
            int lastIndex = 0;
            int kuponBoxStart = bankoPage.IndexOf("<div class=\"body score\">", lastIndex);

            if (kuponBoxStart != -1)
            {
                int kuponBoxEnd = bankoPage.IndexOf("<fb:comments", kuponBoxStart);
                int kuponBoxLen = kuponBoxEnd - kuponBoxStart;
                string kuponlar = bankoPage.Substring(kuponBoxStart, kuponBoxLen);

                int kuponSayisi = Regex.Matches(kuponlar, "<div class=\"body score h200\">").Count;
                string[] couponDiv = new string[kuponSayisi];

                int lastInput = 0;
                for (int i = 0; i < couponDiv.Length; i++)
                {
                    List<BetMatch> matchList = new List<BetMatch>();
                    int listStart = kuponlar.IndexOf("<div class=\"box w315", lastInput);
                    int listEnd = kuponlar.IndexOf("<div class=\"box w315", listStart + 20);
                    int listLen = 0;
                    if (listEnd != -1)
                    {
                        listLen = listEnd - listStart;
                    }
                    else
                    {
                        listLen = kuponlar.Length - listStart;
                    }
                    couponDiv[i] = kuponlar.Substring(listStart, listLen - 26);
                    lastInput = listEnd;
                }
                for (int c = 0; c < couponDiv.Length; c++)
                {
                    Coupon couponObject = new Coupon();
                    List<BetMatch> betMatchList = new List<BetMatch>();
                    int kuponAdilastInput = 0;
                    int macSayisi = Regex.Matches(couponDiv[c], "<div class=\"divRow\">").Count;
                    BetMatch match = new BetMatch();
                    int kuponAdiStart = couponDiv[c].IndexOf("<div class=\"head greenbg\">", kuponAdilastInput);
                    int kuponAdiEnd = couponDiv[c].IndexOf("<span class=\"shother\">", kuponAdiStart + 22);
                    int kuponAdilistLen = kuponAdiEnd - kuponAdiStart;
                    couponObject.Name = couponDiv[c].Substring(kuponAdiStart + 26, kuponAdilistLen - 26);
                    for (int j = 0; j < macSayisi; j++)
                    {
                        int codeStart = couponDiv[c].IndexOf("<div class=\"divCol first\">", kuponAdiEnd);
                        int codeEnd = couponDiv[c].IndexOf("</div>", codeStart + 22);
                        int codeLen = codeEnd - codeStart;
                        match.Code = couponDiv[c].Substring(codeStart + 26, codeLen - 26);


                        int leaugeStart = couponDiv[c].IndexOf("<div class=\"divCol w25\">", codeEnd);
                        int leaugeEnd = couponDiv[c].IndexOf("</div>", leaugeStart + 22);
                        int leaugeLen = leaugeEnd - leaugeStart;
                        match.Leauge = couponDiv[c].Substring(leaugeStart + 24, leaugeLen - 24);


                        int teamStart = couponDiv[c].IndexOf("<div class=\"divCol w145\">", leaugeEnd);
                        int teamEnd = couponDiv[c].IndexOf("</div>", teamStart + 22);
                        int teamLen = teamEnd - teamStart;
                        match.Teams = couponDiv[c].Substring(teamStart + 25, teamLen - 25);

                        int scoreStart = couponDiv[c].IndexOf("class=\"msbg\"", teamEnd);
                        int scoreEnd = couponDiv[c].IndexOf("</span>", scoreStart + 12);
                        int scoreLen = scoreEnd - scoreStart;
                        match.BetScore = couponDiv[c].Substring(scoreStart + 13, scoreLen - 13);


                        int rateStart = couponDiv[c].IndexOf("divCol w25 text-center last", scoreEnd);
                        int rateEnd = couponDiv[c].IndexOf("</div>", rateStart + 22);
                        int rateLen = rateEnd - rateStart;
                        match.Rate = couponDiv[c].Substring(rateStart + 29, rateLen - 29);
                        kuponAdilastInput = scoreEnd;
                        betMatchList.Add(match);
                    }
                    couponObject.Matchs = betMatchList;
                    list.Add(couponObject);
                }
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetGuesses()
        {
            List<Guess> list = new List<Guess>();
            try
            {
                WebClient client = new WebClient();
                client.Encoding = System.Text.Encoding.UTF8;
                string guessPage = client.DownloadString("https://www.iddaatahmin.com/");
                int lastIndex = 0;
                int guessBoxStart = guessPage.IndexOf("class=\"mt10\"", lastIndex);
                if (guessBoxStart != -1)
                {
                    int guessBoxEnd = guessPage.IndexOf("<div class=\"clear\"></div>", guessBoxStart);
                    int guessBoxLen = guessBoxEnd - guessBoxStart;
                    string guesses = guessPage.Substring(guessBoxStart, guessBoxLen);
                    int guessCount = Regex.Matches(guesses, "itemscope=\"\" itemtype=\"").Count;
                    string[] guessDiv = new string[guessCount];

                    int lastInput = 0;
                    for (int i = 0; i < guessDiv.Length; i++)
                    {
                        int listStart = guesses.IndexOf("itemtype=\"http://schema.org/BlogPosting\">", lastInput);
                        int listEnd = guesses.IndexOf("</div></div>", listStart + 20);
                        int listLen = 0;
                        if (listEnd != -1)
                        {
                            listLen = listEnd - listStart;
                        }
                        else
                        {
                            listLen = guesses.Length - listStart;
                        }
                        guessDiv[i] = guesses.Substring(listStart + 41, listLen - 62);
                        lastInput = listEnd;
                    }
                    int lastValue = 0;
                    for (int i = 0; i < guessDiv.Length; i++)
                    {
                        Guess guess = new Guess();

                        int imageStart = guessDiv[i].IndexOf("src", lastValue);
                        int imageEnd = guessDiv[i].IndexOf("height", imageStart + 2);
                        int imageLen = imageEnd - imageStart;
                        guess.ImageUrl = guessDiv[i].Substring(imageStart + 5, imageLen - 7);
                        lastValue = imageEnd;

                        guess.DetailUrl = guess.ImageUrl.Replace("/thumb/215x70/tmp", "").Replace(".jpg", ".html").Replace(".png", ".html");
                        if (guess.DetailUrl != "" || guess.DetailUrl != null)
                        {
                            string detailPage = client.DownloadString(guess.DetailUrl);
                            int detailLastIndex = 0;
                            int detailStart = detailPage.IndexOf("itemprop=\"image\"", detailLastIndex);
                            detailStart = detailPage.IndexOf("<div>", detailStart);
                            int detailEnd = detailPage.IndexOf("<div class=\"clear\"></div>", detailStart);
                            int detailLen = detailEnd - detailStart;
                            guess.Detail = detailPage.Substring(detailStart, detailLen);
                        }

                        int baslikStart = guessDiv[i].IndexOf("span>", lastValue);
                        int baslikEnd = guessDiv[i].IndexOf("</span>", baslikStart);
                        int baslikLen = baslikEnd - baslikStart;
                        guess.Title = guessDiv[i].Substring(baslikStart + 5, baslikLen - 5);
                        lastValue = baslikEnd;

                        int descStart = guessDiv[i].IndexOf("itemprop=\"url\">", lastValue);
                        int descEnd = guessDiv[i].IndexOf("</a>", descStart);
                        int descLen = descEnd - descStart;
                        guess.Desc = guessDiv[i].Substring(descStart + 15, descLen - 15);
                        lastValue = 0;

                        list.Add(guess);
                    }
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public void Test()
        {
            List<Guess> list = new List<Guess>();
            WebClient client = new WebClient();
            client.Encoding = System.Text.Encoding.UTF8;

            string guessPage = client.DownloadString("https://www.iddaatahmin.com/16-kasim-pazartesi-surpriz-iddaa-kuponlari.html");
            int lastIndex = 0;
            int guessBoxStart = guessPage.IndexOf("class=\"mt10\"", lastIndex);
            if (guessBoxStart != -1)
            {

            }
        }

    }
}
