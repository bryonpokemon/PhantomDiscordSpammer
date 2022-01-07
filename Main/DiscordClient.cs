using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Threading;
using Newtonsoft.Json;
using System.Net;
using System;
using System.Collections.Generic;
using WebSocketSharp;
using Microsoft.VisualBasic;
using System.Reflection;
using System.IO;

public class DiscordClient
{
    public string token = "", language = "", userId = "", phoneNumber = "", cookieStr = "";
    public WebSocket ws;
    public bool connected, phoneVerified;
    public string client_build_number;
    public List<string> queue = new List<string>(), idQueue = new List<string>();
    private AutoReconnectData data = null;
    public bool connectedToVoice;
    public int payloads = 0;
    public string actualStatus = "online";

    public DiscordClient(string token)
    {
        try
        {
            this.token = token;
        }
        catch
        {

        }
    }

    public string GetToken()
    {
        return this.token;
    }

    public string GetCookie(DiscordProxy proxy)
    {
        if (cookieStr == "")
        {
            SetCookies(proxy);
        }

        return cookieStr;
    }

    public bool IsJoinPatched(string invite, string contextProperties, DiscordProxy proxy)
    {
        try
        {
            var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/invites/" + invite);

            if (proxy != null)
            {
                request.Proxy = proxy.GetNewProxy();
            }
            else
            {
                request.Proxy = null;
            }

            request.UseDefaultCredentials = false;
            request.AllowAutoRedirect = false;

            var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

            request.Method = "POST";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write("{}");
                streamWriter.Close();
                streamWriter.Dispose();
            }

            var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
            {
                ["Host"] = "discord.com",
                ["Connection"] = "keep-alive",
                ["Content-Length"] = "2",
                ["X-Super-Properties"] = "eyJvcyI6IldpbmRvd3MiLCJicm93c2VyIjoiQ2hyb21lIiwiZGV2aWNlIjoiIiwic3lzdGVtX2xvY2FsZSI6Iml0LUlUIiwiYnJvd3Nlcl91c2VyX2FnZW50IjoiTW96aWxsYS81LjAgKFdpbmRvd3MgTlQgMTAuMDsgV2luNjQ7IHg2NCkgQXBwbGVXZWJLaXQvNTM3LjM2IChLSFRNTCwgbGlrZSBHZWNrbykgQ2hyb21lLzk2LjAuNDY2NC40NSBTYWZhcmkvNTM3LjM2IiwiYnJvd3Nlcl92ZXJzaW9uIjoiOTYuMC40NjY0LjQ1Iiwib3NfdmVyc2lvbiI6IjEwIiwicmVmZXJyZXIiOiIiLCJyZWZlcnJpbmdfZG9tYWluIjoiIiwicmVmZXJyZXJfY3VycmVudCI6IiIsInJlZmVycmluZ19kb21haW5fY3VycmVudCI6IiIsInJlbGVhc2VfY2hhbm5lbCI6InN0YWJsZSIsImNsaWVudF9idWlsZF9udW1iZXIiOjEwNzExNiwiY2xpZW50X2V2ZW50X3NvdXJjZSI6bnVsbH0=",
                ["X-Context-Properties"] = contextProperties,
                ["X-Discord-Locale"] = GetLanguage(proxy),
                ["Authorization"] = GetToken(),
                ["Content-Type"] = "application/json",
                ["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.45 Safari/",
                ["Origin"] = "https://discord.com",
                ["Accept-Encoding"] = "gzip, deflate, br",
                ["Cookie"] = GetCookie(proxy)
            }); ;

            field.SetValue(request, headers);

            var response = request.GetResponse();
            string obtained = response.Headers["access-control-allow-headers"];

            response.Close();
            response.Dispose();

            if (obtained != "Content-Type, Authorization, X-Audit-Log-Reason, X-Track, X-Super-Properties, X-Context-Properties, X-Failed-Requests, X-Fingerprint, X-RPC-Proxy, X-Discord-Locale, X-Debug-Options, x-client-trace-id, If-None-Match, Range, X-RateLimit-Precision")
            {
                return true;
            }

            return false;
        }
        catch
        {
            return true;
        }
    }

    private string GuildJoin(string invite, string contextProperties, DiscordProxy proxy)
    {
        try
        {
            var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/invites/" + invite);

            if (proxy != null)
            {
                request.Proxy = proxy.GetNewProxy();
            }
            else
            {
                request.Proxy = null;
            }

            request.UseDefaultCredentials = false;
            request.AllowAutoRedirect = false;

            var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

            request.Method = "POST";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write("{}");
                streamWriter.Close();
                streamWriter.Dispose();
            }

            var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
            {
                ["Host"] = "discord.com",
                ["Connection"] = "keep-alive",
                ["Content-Length"] = "2",
                ["X-Super-Properties"] = "eyJvcyI6IldpbmRvd3MiLCJicm93c2VyIjoiQ2hyb21lIiwiZGV2aWNlIjoiIiwic3lzdGVtX2xvY2FsZSI6Iml0LUlUIiwiYnJvd3Nlcl91c2VyX2FnZW50IjoiTW96aWxsYS81LjAgKFdpbmRvd3MgTlQgMTAuMDsgV2luNjQ7IHg2NCkgQXBwbGVXZWJLaXQvNTM3LjM2IChLSFRNTCwgbGlrZSBHZWNrbykgQ2hyb21lLzk2LjAuNDY2NC40NSBTYWZhcmkvNTM3LjM2IiwiYnJvd3Nlcl92ZXJzaW9uIjoiOTYuMC40NjY0LjQ1Iiwib3NfdmVyc2lvbiI6IjEwIiwicmVmZXJyZXIiOiIiLCJyZWZlcnJpbmdfZG9tYWluIjoiIiwicmVmZXJyZXJfY3VycmVudCI6IiIsInJlZmVycmluZ19kb21haW5fY3VycmVudCI6IiIsInJlbGVhc2VfY2hhbm5lbCI6InN0YWJsZSIsImNsaWVudF9idWlsZF9udW1iZXIiOjEwNzExNiwiY2xpZW50X2V2ZW50X3NvdXJjZSI6bnVsbH0=",
                ["X-Context-Properties"] = contextProperties,
                ["X-Discord-Locale"] = GetLanguage(proxy),
                ["Authorization"] = GetToken(),
                ["Content-Type"] = "application/json",
                ["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.45 Safari/",
                ["Origin"] = "https://discord.com",
                ["Accept-Encoding"] = "gzip, deflate, br",
                ["Cookie"] = GetCookie(proxy)
            }); ;

            field.SetValue(request, headers);

            var response = request.GetResponse();
            string obtained = Utils.DecompressResponse(Utils.ReadFully(response.GetResponseStream()));

            response.Close();
            response.Dispose();

            return obtained;
        }
        catch
        {
            return "";
        }
    }

    public void SetCookies(DiscordProxy proxy)
    {
        try
        {
            cookieStr = "__dcfduid=" + Utils.GetUniqueKey1(32) + "; locale=it";
        }
        catch
        {

        }
    }


    public string GetUserId(DiscordProxy proxy)
    {
        try
        {
            if (userId != "")
            {
                return userId;
            }

            try
            {
                var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/users/@me");

                if (proxy != null)
                {
                    request.Proxy = proxy.GetNewProxy();
                }
                else
                {
                    request.Proxy = null;
                }

                request.UseDefaultCredentials = false;
                request.AllowAutoRedirect = false;

                var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

                request.Method = "GET";

                var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
                {
                    ["Accept-Encoding"] = "gzip, deflate, br",
                    ["Authorization"] = GetToken(),
                    ["Host"] = "discord.com"
                });

                field.SetValue(request, headers);

                var response = request.GetResponse();
                dynamic jss = JObject.Parse(Utils.DecompressResponse(Utils.ReadFully(response.GetResponseStream())));

                response.Close();
                response.Dispose();

                string id = (string)jss.id;
                string phone = (string)jss.phone;

                try
                {
                    if (phone == null || phone == "null")
                    {
                        phoneNumber = "n";
                        phoneVerified = false;
                    }
                }
                catch
                {
                    phoneNumber = (string)jss.phone;
                    phoneVerified = true;
                }

                string locale = (string)jss.locale;

                language = locale;
                userId = id;

                return id;
            }
            catch
            {
                return "";
            }
        }
        catch
        {
            return "";
        }
    }

    public bool IsPhoneVerified(DiscordProxy proxy)
    {
        try
        {
            if (phoneNumber != "")
            {
                return phoneVerified;
            }

            try
            {
                var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/users/@me");

                if (proxy != null)
                {
                    request.Proxy = proxy.GetNewProxy();
                }
                else
                {
                    request.Proxy = null;
                }

                request.UseDefaultCredentials = false;
                request.AllowAutoRedirect = false;

                var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

                request.Method = "GET";

                var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
                {
                    ["Accept-Encoding"] = "gzip, deflate, br",
                    ["Authorization"] = GetToken(),
                    ["Host"] = "discord.com"
                });

                field.SetValue(request, headers);

                var response = request.GetResponse();
                dynamic jss = JObject.Parse(Utils.DecompressResponse(Utils.ReadFully(response.GetResponseStream())));

                response.Close();
                response.Dispose();

                string id = (string)jss.id;
                string phone = (string)jss.phone;

                try
                {
                    if (phone == null || phone == "null")
                    {
                        phoneNumber = "n";
                        phoneVerified = false;
                    }
                }
                catch
                {
                    phoneNumber = (string)jss.phone;
                    phoneVerified = true;
                }

                string locale = (string)jss.locale;

                language = locale;
                userId = id;

                return phoneVerified;
            }
            catch
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
    }

    public List<string> GetGuildChannels(string guildId, DiscordProxy proxy)
    {
        try
        {
            List<string> channels = new List<string>();

            var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/guilds/" + guildId + "/channels");

            if (proxy != null)
            {
                request.Proxy = proxy.GetNewProxy();
            }
            else
            {
                request.Proxy = null;
            }

            request.UseDefaultCredentials = false;
            request.AllowAutoRedirect = false;

            var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

            request.Method = "GET";

            var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
            {
                ["Accept-Encoding"] = "gzip, deflate, br",
                ["Authorization"] = GetToken(),
                ["Host"] = "discord.com"
            });

            field.SetValue(request, headers);

            var response = request.GetResponse();
            dynamic dynJson = JsonConvert.DeserializeObject(Utils.DecompressResponse(Utils.ReadFully(response.GetResponseStream())));
            response.Close();
            response.Dispose();

            foreach (var item in dynJson)
            {
                try
                {
                    if ((string)item.type == "0")
                    {
                        channels.Add((string)item.id);
                    }
                }
                catch
                {

                }
            }

            return channels;
        }
        catch
        {

        }

        return new List<string>();
    }

    public List<string> GetGuildRoles(string guildId, DiscordProxy proxy)
    {
        try
        {
            List<string> roles = new List<string>();

            var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/guilds/" + guildId + "/roles");

            if (proxy != null)
            {
                request.Proxy = proxy.GetNewProxy();
            }
            else
            {
                request.Proxy = null;
            }

            request.UseDefaultCredentials = false;
            request.AllowAutoRedirect = false;

            var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

            request.Method = "GET";

            var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
            {
                ["Accept-Encoding"] = "gzip, deflate, br",
                ["Authorization"] = GetToken(),
                ["Host"] = "discord.com"
            });

            field.SetValue(request, headers);

            var response = request.GetResponse();
            dynamic dynJson = JsonConvert.DeserializeObject(Utils.DecompressResponse(Utils.ReadFully(response.GetResponseStream())));
            response.Close();
            response.Dispose();

            foreach (var item in dynJson)
            {
                try
                {
                    if ((string)item.name != "@everyone")
                    {
                        roles.Add((string)item.id);
                    }
                }
                catch
                {

                }
            }

            return roles;
        }
        catch
        {

        }

        return new List<string>();
    }

    public List<string> GetGroupRecipients(string channelId, DiscordProxy proxy)
    {
        try
        {
            List<string> recipients = new List<string>();

            var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/channels/" + channelId);

            if (proxy != null)
            {
                request.Proxy = proxy.GetNewProxy();
            }
            else
            {
                request.Proxy = null;
            }

            request.UseDefaultCredentials = false;
            request.AllowAutoRedirect = false;

            var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

            request.Method = "GET";

            var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
            {
                ["Accept-Encoding"] = "gzip, deflate, br",
                ["Authorization"] = GetToken(),
                ["Host"] = "discord.com"
            });

            field.SetValue(request, headers);

            var response = request.GetResponse();
            dynamic dynJson = JsonConvert.DeserializeObject(Utils.DecompressResponse(Utils.ReadFully(response.GetResponseStream())));
            response.Close();
            response.Dispose();

            foreach (var item in dynJson)
            {
                try
                {
                    foreach (var another in item)
                    {
                        try
                        {
                            foreach (var anotherino in another)
                            {
                                try
                                {
                                    recipients.Add((string)anotherino.id);
                                }
                                catch
                                {

                                }
                            }
                        }
                        catch
                        {

                        }
                    }
                }
                catch
                {

                }
            }

            return recipients;
        }
        catch
        {

        }

        return new List<string>();
    }

    public string GetLanguage(DiscordProxy proxy)
    {
        try
        {
            if (language != "")
            {
                return language;
            }

            SetCookies(proxy);

            var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/users/@me");

            if (proxy != null)
            {
                request.Proxy = proxy.GetNewProxy();
            }
            else
            {
                request.Proxy = null;
            }

            request.UseDefaultCredentials = false;
            request.AllowAutoRedirect = false;

            var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

            request.Method = "GET";

            var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
            {
                ["Accept-Encoding"] = "gzip, deflate, br",
                ["Authorization"] = GetToken(),
                ["Host"] = "discord.com"
            });

            field.SetValue(request, headers);

            var response = request.GetResponse();
            dynamic jss = JObject.Parse(Utils.DecompressResponse(Utils.ReadFully(response.GetResponseStream())));

            language = (string)jss.locale;
            userId = (string)jss.id;

            try
            {
                if ((string)jss.phone != null && (string)jss.phone != "null")
                {
                    phoneVerified = true;
                    phoneNumber = (string)jss.phone;
                }
                else
                {
                    phoneVerified = false;
                    phoneNumber = "n";
                }
            }
            catch
            {
                phoneVerified = false;
                phoneNumber = "n";
            }

            cookieStr = cookieStr.Replace("locale=it", "locale=" + language);
            return language;
        }
        catch
        {
            return "";
        }
    }

    public void JoinGuild(DiscordInvite invite, string contextProperties, string captchaBotID, string captchaBotChannelID, bool communityRules, bool reactionVerification, bool captchaBot, bool groupMode, string captchaKey, DiscordProxy proxy)
    {
        try
        {
            if (!groupMode)
            {
                string str = GuildJoin(invite.inviteCode, contextProperties, proxy);

                try
                {
                    if (communityRules)
                    {
                        dynamic jss = JObject.Parse(str);

                        if ((bool)jss.show_verification_form)
                        {
                            string rules = GetRules(invite, proxy);
                            BypassRules(invite, rules, proxy);
                        }
                    }
                }
                catch
                {

                }

                try
                {
                    if (reactionVerification)
                    {
                        BypassReactionVerification(invite, true, proxy);
                    }
                }
                catch
                {

                }

                try
                {
                    if (captchaBot)
                    {
                        BypassCaptchaBot(captchaBotID, captchaBotChannelID, captchaKey, proxy);
                    }
                }
                catch
                {

                }
            }
            else
            {
                GuildJoin(invite.inviteCode, contextProperties, proxy);
            }
        }
        catch
        {

        }
    }

    public async void BypassCaptchaBot(string captchaBotID, string captchaBotChannelID, string captchaKey, DiscordProxy proxy)
    {
        try
        {
            if (Utils.IsIDValid(captchaBotID))
            {
                string embedUrl = "", channelId = "";

                if (Utils.IsIDValid(captchaBotChannelID))
                {
                    channelId = captchaBotChannelID;
                }
                else
                {
                    channelId = GetDMChannel(captchaBotID, proxy);
                }

                embedUrl = GetEmbedURL(channelId, proxy);

                var request = (HttpWebRequest)WebRequest.Create(embedUrl);

                if (proxy != null)
                {
                    request.Proxy = proxy.GetNewProxy();
                }
                else
                {
                    request.Proxy = null;
                }

                request.UseDefaultCredentials = false;
                request.AllowAutoRedirect = false;

                var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

                request.Method = "GET";

                var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
                {
                    ["Authorization"] = GetToken(),
                    ["Host"] = "discord.com"
                });

                field.SetValue(request, headers);

                var response = request.GetResponse();
                string captchaBase64 = Convert.ToBase64String(Utils.ReadFully(response.GetResponseStream()));
                response.Close();
                response.Dispose();
                TwoCaptcha.TwoCaptcha solver = new TwoCaptcha.TwoCaptcha(captchaKey);
                TwoCaptcha.Captcha.Normal captcha = new TwoCaptcha.Captcha.Normal();
                captcha.SetBase64(captchaBase64);
                captcha.SetCaseSensitive(true);
                await solver.Solve(captcha);
                string solved = captcha.Code;

                AnswerToCaptcha(channelId, solved, proxy);
            }
        }
        catch
        {

        }
    }

    public void AnswerToCaptcha(string channelId, string solved, DiscordProxy proxy)
    {
        try
        {
            string data = "{\"content\":\"" + solved + "\"}";

            var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/channels/" + channelId + "/messages");

            if (proxy != null)
            {
                request.Proxy = proxy.GetNewProxy();
            }
            else
            {
                request.Proxy = null;
            }

            request.UseDefaultCredentials = false;
            request.AllowAutoRedirect = false;

            var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

            request.Method = "POST";

            byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes(data);
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(requestBytes, 0, requestBytes.Length);
            requestStream.Close();

            var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
            {
                ["Host"] = "discord.com",
                ["Authorization"] = GetToken(),
                ["Accept-Encoding"] = "gzip, deflate, br",
                ["Content-Length"] = requestBytes.Length.ToString(),
                ["Content-Type"] = "application/json"
            });

            field.SetValue(request, headers);
            var response = request.GetResponse();
            response.Close();
            response.Dispose();
        }
        catch
        {

        }
    }

    public void BypassReactionVerification(DiscordInvite invite, bool doThat, DiscordProxy proxy)
    {
        try
        {
            bool first = true;
            string channelDone = invite.channelId.ToString();

            foreach (string channelId in GetGuildChannels(invite.guildId.ToString(), proxy))
            {
                try
                {
                    var request1 = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/channels/" + channelId + "/messages?limit=50");

                    if (proxy != null)
                    {
                        request1.Proxy = proxy.GetNewProxy();
                    }
                    else
                    {
                        request1.Proxy = null;
                    }

                    request1.UseDefaultCredentials = false;
                    request1.AllowAutoRedirect = false;

                    var field1 = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

                    request1.Method = "GET";

                    var headers1 = new CustomWebHeaderCollection(new Dictionary<string, string>
                    {
                        ["Accept-Encoding"] = "gzip, deflate, br",
                        ["Authorization"] = GetToken(),
                        ["Host"] = "discord.com"
                    });

                    field1.SetValue(request1, headers1);

                    var response1 = request1.GetResponse();
                    string messages1 = Utils.DecompressResponse(Utils.ReadFully(response1.GetResponseStream()));
                    dynamic dynJson1 = JsonConvert.DeserializeObject(messages1);
                    response1.Close();
                    response1.Dispose();

                    foreach (var item in dynJson1)
                    {
                        try
                        {
                            foreach (var item1 in item.reactions)
                            {
                                try
                                {
                                    string reaction = "", id = "";
                                    id = item1.emoji.id;
                                    reaction = item1.emoji.name;

                                    if (id != null && id != "")
                                    {
                                        reaction += ":" + id;
                                    }

                                    if (first)
                                    {
                                        first = false;
                                    }
                                    else
                                    {
                                        Thread.Sleep(1500);
                                    }

                                    AddReaction(reaction, channelId, (string)item.id, proxy);
                                }
                                catch
                                {

                                }
                            }
                        }
                        catch
                        {

                        }
                    }
                }
                catch
                {

                }
            }
        }
        catch
        {

        }
    }

    public void AddReaction(string reaction, string channelId, string messageId, DiscordProxy proxy)
    {
        try
        {
            var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/channels/" + channelId + "/messages/" + messageId + "/reactions/" + reaction + "/@me");

            if (proxy != null)
            {
                request.Proxy = proxy.GetNewProxy();
            }
            else
            {
                request.Proxy = null;
            }

            request.UseDefaultCredentials = false;
            request.AllowAutoRedirect = false;

            var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

            request.Method = "PUT";

            var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
            {
                ["Host"] = "discord.com",
                ["Authorization"] = GetToken(),
                ["Accept-Encoding"] = "gzip, deflate, br",
                ["Content-Length"] = "0"
            });

            field.SetValue(request, headers);
            var response = request.GetResponse();
            response.Close();
            response.Dispose();
        }
        catch
        {

        }
    }

    public string GetRules(DiscordInvite invite, DiscordProxy proxy)
    {
        try
        {
            var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/guilds/" + invite.guildId + "/member-verification?with_guild=false&invite_code=" + invite.inviteCode);

            if (proxy != null)
            {
                request.Proxy = proxy.GetNewProxy();
            }
            else
            {
                request.Proxy = null;
            }

            request.UseDefaultCredentials = false;
            request.AllowAutoRedirect = false;

            var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

            request.Method = "GET";

            var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
            {
                ["Accept-Encoding"] = "gzip, deflate, br",
                ["Authorization"] = GetToken(),
                ["Host"] = "discord.com"
            });

            field.SetValue(request, headers);

            var response = request.GetResponse();
            string data = Utils.DecompressResponse(Utils.ReadFully(response.GetResponseStream())), toSend = "";
            response.Close();
            response.Dispose();

            if (data.Contains("\"form_fields\": []") || data.Contains("\"form_fields\":[]"))
            {
                string lol1 = Microsoft.VisualBasic.Strings.Split(data, "{\"version\": \"")[1];
                string lol2 = Microsoft.VisualBasic.Strings.Split(lol1, "\"")[0];

                toSend = "{\"version\": \"" + lol2 + "\",\"form_fields\": []}";
            }
            else
            {
                string lol1 = Microsoft.VisualBasic.Strings.Split(data, "}], \"description\":")[0];

                toSend = lol1 + ",\"response\":true}]}";
            }

            return toSend;
        }
        catch
        {
            return "";
        }
    }

    public void BypassRules(DiscordInvite invite, string data, DiscordProxy proxy)
    {
        try
        {
            var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/guilds/" + invite.guildId + "/requests/@me");

            if (proxy != null)
            {
                request.Proxy = proxy.GetNewProxy();
            }
            else
            {
                request.Proxy = null;
            }

            request.UseDefaultCredentials = false;
            request.AllowAutoRedirect = false;

            var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

            request.Method = "PUT";

            byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes(data);
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(requestBytes, 0, requestBytes.Length);
            requestStream.Close();

            var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
            {
                ["Host"] = "discord.com",
                ["Authorization"] = GetToken(),
                ["Accept-Encoding"] = "gzip, deflate, br",
                ["Content-Length"] = requestBytes.Length.ToString(),
                ["Content-Type"] = "application/json"
            });

            field.SetValue(request, headers);
            var response = request.GetResponse();
            response.Close();
            response.Dispose();
        }
        catch
        {

        }
    }

    public void LeaveGuild(DiscordInvite invite, bool groupMode, DiscordProxy proxy)
    {
        try
        {
            if (groupMode)
            {
                LeaveGuild(invite.channelId.ToString(), groupMode, proxy);
            }
            else
            {
                LeaveGuild(invite.guildId.ToString(), groupMode, proxy);
            }
        }
        catch
        {

        }
    }

    public void LeaveGuild(string guildId, bool groupMode, DiscordProxy proxy)
    {
        try
        {
            string data = "{\"lurking\":false}";

            if (!groupMode)
            {
                var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/users/@me/guilds/" + guildId);

                if (proxy != null)
                {
                    request.Proxy = proxy.GetNewProxy();
                }
                else
                {
                    request.Proxy = null;
                }

                request.UseDefaultCredentials = false;
                request.AllowAutoRedirect = false;

                var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

                request.Method = "DELETE";

                byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes(data);
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(requestBytes, 0, requestBytes.Length);
                requestStream.Close();

                var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
                {
                    ["Host"] = "discord.com",
                    ["Authorization"] = GetToken(),
                    ["Accept-Encoding"] = "gzip, deflate, br",
                    ["Content-Length"] = requestBytes.Length.ToString(),
                    ["Content-Type"] = "application/json"
                });

                field.SetValue(request, headers);
                var response = request.GetResponse();
                response.Close();
                response.Dispose();
            }
            else
            {
                var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/channels/" + guildId);

                if (proxy != null)
                {
                    request.Proxy = proxy.GetNewProxy();
                }
                else
                {
                    request.Proxy = null;
                }

                request.UseDefaultCredentials = false;
                request.AllowAutoRedirect = false;

                var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

                request.Method = "DELETE";

                byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes(data);
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(requestBytes, 0, requestBytes.Length);
                requestStream.Close();

                var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
                {
                    ["Host"] = "discord.com",
                    ["Authorization"] = GetToken(),
                    ["Accept-Encoding"] = "gzip, deflate, br",
                    ["Content-Length"] = requestBytes.Length.ToString(),
                    ["Content-Type"] = "application/json"
                });

                field.SetValue(request, headers);
                var response = request.GetResponse();
                response.Close();
                response.Dispose();
            }
        }
        catch
        {

        }
    }

    public void ReadChannel(string channelId, DiscordProxy proxy)
    {
        try
        {
            var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/channels/" + channelId + "/messages?limit=50");

            if (proxy != null)
            {
                request.Proxy = proxy.GetNewProxy();
            }
            else
            {
                request.Proxy = null;
            }

            request.UseDefaultCredentials = false;
            request.AllowAutoRedirect = false;

            var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

            request.Method = "GET";

            var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
            {
                ["Accept-Encoding"] = "gzip, deflate, br",
                ["Authorization"] = GetToken(),
                ["Host"] = "discord.com"
            });

            field.SetValue(request, headers);

            var response = request.GetResponse();
            response.Close();
            response.Dispose();
        }
        catch
        {

        }
    }

    public string GetEmbedURL(string channelId, DiscordProxy proxy)
    {
        try
        {
            var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/channels/" + channelId + "/messages?limit=50");

            if (proxy != null)
            {
                request.Proxy = proxy.GetNewProxy();
            }
            else
            {
                request.Proxy = null;
            }

            request.UseDefaultCredentials = false;
            request.AllowAutoRedirect = false;

            var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

            request.Method = "GET";

            var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
            {
                ["Accept-Encoding"] = "gzip, deflate, br",
                ["Authorization"] = GetToken(),
                ["Host"] = "discord.com"
            });

            field.SetValue(request, headers);

            var response = request.GetResponse();
            string messages = Utils.DecompressResponse(Utils.ReadFully(response.GetResponseStream()));
            response.Close();
            response.Dispose();
            dynamic dynJson = JsonConvert.DeserializeObject(messages);

            foreach (var item in dynJson)
            {
                try
                {
                    foreach (var item1 in item.embeds)
                    {
                        try
                        {
                            return (string)item1.image.url;
                        }
                        catch
                        {

                        }
                    }

                    break;
                }
                catch
                {

                }
            }
        }
        catch
        {

        }

        return "";
    }

    public string GetDMChannel(string userId, DiscordProxy proxy)
    {
        try
        {
            var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/users/@me/channels");

            if (proxy != null)
            {
                request.Proxy = proxy.GetNewProxy();
            }
            else
            {
                request.Proxy = null;
            }

            request.UseDefaultCredentials = false;
            request.AllowAutoRedirect = false;

            var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

            request.Method = "GET";

            var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
            {
                ["Accept-Encoding"] = "gzip, deflate, br",
                ["Authorization"] = GetToken(),
                ["Host"] = "discord.com"
            });

            field.SetValue(request, headers);

            var response = request.GetResponse();
            string dms = Utils.DecompressResponse(Utils.ReadFully(response.GetResponseStream()));
            response.Close();
            response.Dispose();
            dynamic jss = JsonConvert.DeserializeObject(dms);

            foreach (var item in jss)
            {
                try
                {
                    foreach (var another in item.recipients)
                    {
                        try
                        {
                            if ((string)another.id == userId)
                            {
                                return (string)item.id;
                            }
                        }
                        catch
                        {

                        }
                    }
                }
                catch
                {

                }
            }

            return CreateDM(userId, proxy);
        }
        catch
        {

        }

        return "";
    }

    public string CreateDM(string userId, DiscordProxy proxy)
    {
        try
        {
            string data = "{\"recipients\":[\"" + userId + "\"]}";
            var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/users/@me/channels");

            if (proxy != null)
            {
                request.Proxy = proxy.GetNewProxy();
            }
            else
            {
                request.Proxy = null;
            }

            request.UseDefaultCredentials = false;
            request.AllowAutoRedirect = false;

            var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

            request.Method = "POST";

            byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes(data);
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(requestBytes, 0, requestBytes.Length);
            requestStream.Close();

            var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
            {
                ["Host"] = "discord.com",
                ["Connection"] = "keep-alive",
                ["Content-Length"] = requestBytes.Length.ToString(),
                ["X-Super-Properties"] = "eyJvcyI6IldpbmRvd3MiLCJicm93c2VyIjoiQ2hyb21lIiwiZGV2aWNlIjoiIiwic3lzdGVtX2xvY2FsZSI6Iml0LUlUIiwiYnJvd3Nlcl91c2VyX2FnZW50IjoiTW96aWxsYS81LjAgKFdpbmRvd3MgTlQgMTAuMDsgV2luNjQ7IHg2NCkgQXBwbGVXZWJLaXQvNTM3LjM2IChLSFRNTCwgbGlrZSBHZWNrbykgQ2hyb21lLzk2LjAuNDY2NC40NSBTYWZhcmkvNTM3LjM2IiwiYnJvd3Nlcl92ZXJzaW9uIjoiOTYuMC40NjY0LjQ1Iiwib3NfdmVyc2lvbiI6IjEwIiwicmVmZXJyZXIiOiIiLCJyZWZlcnJpbmdfZG9tYWluIjoiIiwicmVmZXJyZXJfY3VycmVudCI6IiIsInJlZmVycmluZ19kb21haW5fY3VycmVudCI6IiIsInJlbGVhc2VfY2hhbm5lbCI6InN0YWJsZSIsImNsaWVudF9idWlsZF9udW1iZXIiOjEwNzExNiwiY2xpZW50X2V2ZW50X3NvdXJjZSI6bnVsbH0=",
                ["X-Context-Properties"] = "e30=",
                ["X-Discord-Locale"] = GetLanguage(proxy),
                ["Authorization"] = GetToken(),
                ["Content-Type"] = "application/json",
                ["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/93.0.4577.82 Safari/537.36",
                ["Origin"] = "https://discord.com",
                ["Accept-Encoding"] = "gzip, deflate, br",
                ["Cookie"] = GetCookie(null)
            });

            field.SetValue(request, headers);
            var response = request.GetResponse();
            string result = Utils.DecompressResponse(Utils.ReadFully(response.GetResponseStream()));
            response.Close();
            response.Dispose();
            dynamic jss = JObject.Parse(result);
            string theId = (string)jss.id;

            return theId;
        }
        catch
        {

        }

        return "";
    }

    public void ConnectToWebSocket()
    {
        try
        {
            if (!connected)
            {
                ws = new WebSocket("wss://gateway.discord.gg/?encoding=json&v=9");
                ws.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
                ws.Origin = "https://discord.com";
                ws.EnableRedirection = false;
                ws.EmitOnPing = false;

                ws.CustomHeaders = new Dictionary<string, string>
                {
                    { "Host", "gateway.discord.gg" },
                    { "Connection", "Upgrade" },
                    { "Pragma", "no-cache" },
                    { "Cache-Control", "no-cache" },
                    { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/93.0.4577.82 Safari/537.36" },
                    { "Upgrade", "websocket" },
                    { "Origin", "https://discord.com" },
                    { "Sec-WebSocket-Version", "13" },
                    { "Accept-Encoding", "gzip, deflate, br" },
                    { "Accept-Language", "it-IT,it;q=0.9,en-US;q=0.8,en;q=0.7" },
                    { "Sec-WebSocket-Extensions", "permessage-deflate; client_max_window_bits" },
                };

                new Thread(fetchQueue).Start();
                ws.OnMessage += Ws_OnMessage;
                ws.Connect();
                ws.Send("{\"op\":2,\"d\":{\"token\":\"" + GetToken() + "\",\"capabilities\":125,\"properties\":{\"os\":\"Windows\",\"browser\":\"Chrome\",\"device\":\"\",\"system_locale\":\"it-IT\",\"browser_user_agent\":\"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/93.0.4577.82 Safari/537.36\",\"browser_version\":\"93.0.4577.82\",\"os_version\":\"10\",\"referrer\":\"\",\"referring_domain\":\"\",\"referrer_current\":\"\",\"referring_domain_current\":\"\",\"release_channel\":\"stable\",\"client_build_number\":97309,\"client_event_source\":null},\"presence\":{\"status\":\"online\",\"since\":0,\"activities\":[],\"afk\":false},\"compress\":false,\"client_state\":{\"guild_hashes\":{},\"highest_last_message_id\":\"0\",\"read_state_version\":0,\"user_guild_settings_version\":-1}}}");
                connected = true;
            }
        }
        catch
        {

        }
    }

    public void DisconnectFromWebSocket()
    {
        try
        {
            if (connected)
            {
                ws.Close();
                connected = false;
            }
        }
        catch
        {

        }
    }

    private void Ws_OnMessage(object sender, MessageEventArgs e)
    {
        try
        {
            string data = System.Text.Encoding.UTF8.GetString(e.RawData);
            queue.Add(data);
            dynamic jss = JObject.Parse(data);

            if (jss.op == 10)
            {
                int heartbeat_interval = jss.d.heartbeat_interval;
                new Thread(() => doHeartbeat(heartbeat_interval)).Start();
            }

            if (jss.t == "GUILD_MEMBER_LIST_UPDATE")
            {
                idQueue.Add(data);
                payloads++;
            }
            else if ((string)jss.t == "VOICE_STATE_UPDATE")
            {
                try
                {
                    if ((string)jss.d.member.user.id == GetUserId(null))
                    {
                        if ((string)jss.d.channel_id == null || (string)jss.d.channel_id == "null")
                        {
                            connectedToVoice = false;

                            if (this.data != null)
                            {
                                if (Utils.globalAutoReconnect)
                                {
                                    JoinVoice(this.data.guildId, this.data.channelId, this.data.userIdGoLive, this.data.microphoneMuted, this.data.headphonesMuted, this.data.videoEnabled, this.data.goLive, this.data.joinGoLive, this.data.speakInStage, null);
                                }
                            }
                        }
                        else
                        {
                            connectedToVoice = true;
                        }
                    }
                }
                catch
                {
                    connectedToVoice = false;
                }
            }
        }
        catch
        {

        }
    }

    public void doHeartbeat(int heartbeat_interval)
    {
        try
        {
            while (true)
            {
                try
                {
                    Thread.Sleep(heartbeat_interval);
                    ws.Send("{\"op\":1,\"d\":null}");
                }
                catch
                {

                }
            }
        }
        catch
        {

        }
    }

    public void fetchQueue()
    {
        while (true)
        {
            Thread.Sleep(250);

            try
            {
                if (!(queue.Count <= 0))
                {
                    string data = queue[0];
                    queue.RemoveAt(0);
                }
            }
            catch
            {

            }

            try
            {
                if (!(idQueue.Count <= 0))
                {
                    string data = idQueue[0];
                    idQueue.RemoveAt(0);

                    string[] splitted = Strings.Split(data, "\"id\":\"");

                    for (int i = 1; i < splitted.Length; i++)
                    {
                        try
                        {
                            string another = splitted[i];

                            string[] anotherSplit = Strings.Split(another, "\"");
                            string finalId = anotherSplit[0];

                            if (Information.IsNumeric(finalId) && finalId.Length == 18 && data.Contains(finalId + '"' + "," + '"' + "discriminator") && !Utils.users.Contains(finalId))
                            {
                                Utils.users.Add(finalId);
                            }
                        }
                        catch
                        {

                        }
                    }

                    for (int i = 0; i < Utils.users.Count; i++)
                    {
                        try
                        {
                            for (int j = 0; j < Utils.users.Count; j++)
                            {
                                try
                                {
                                    if (i != j)
                                    {
                                        if (Utils.users[i] == Utils.users[j])
                                        {
                                            Utils.users.RemoveAt(i);
                                        }
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                        catch
                        {

                        }
                    }
                }
            }
            catch
            {

            }
        }
    }

    public void ParseGroup(DiscordInvite invite, DiscordProxy proxy)
    {
        try
        {
            Utils.users.Clear();
            Utils.users.AddRange(GetGroupRecipients(invite.channelId.ToString(), proxy));
        }
        catch
        {

        }
    }

    public void ParseGuild(DiscordInvite invite, string channelId)
    {
        try
        {
            if (Utils.lastChannelId == channelId)
            {
                payloads = 0;
                Utils.users.Clear();

                try
                {
                    Thread.Sleep(1000);
                    int first = 0, second = 99;

                    if (Utils.lastChannelId == channelId)
                    {
                        try
                        {
                            ws.Send("{\"op\":14,\"d\":{\"guild_id\":\"" + invite.guildId.ToString() + "\",\"typing\":true,\"activities\":true,\"threads\":true,\"channels\":{\"" + channelId.ToString() + "\":[[" + first.ToString() + "," + second.ToString() + "]]}}}");
                        }
                        catch
                        {

                        }
                    }
                    else
                    {
                        return;
                    }

                    ulong members = invite.membersCount;
                    Thread.Sleep(1000);

                    while (members > 100 && Utils.lastChannelId == channelId)
                    {
                        try
                        {
                            Thread.Sleep(1000);

                            if (payloads >= 2)
                            {
                                payloads = 0;
                                first += 100;
                                second += 100;
                                members -= 100;

                                if (Utils.lastChannelId != channelId)
                                {
                                    return;
                                }

                                try
                                {
                                    ws.Send("{\"op\":14,\"d\":{\"guild_id\":\"" + invite.guildId.ToString() + "\",\"typing\":true,\"activities\":true,\"threads\":true,\"channels\":{\"" + channelId.ToString() + "\":[[" + first.ToString() + "," + second.ToString() + "]]}}}");
                                }
                                catch
                                {

                                }

                                Thread.Sleep(1000);

                                if (Utils.lastChannelId != channelId)
                                {
                                    return;
                                }
                            }
                            else
                            {
                                Thread.Sleep(500);
                            }
                        }
                        catch
                        {

                        }
                    }
                }
                catch
                {

                }
            }
        }
        catch
        {

        }
    }

    public bool SendMessage(string channelId, string message, string reference, DiscordProxy proxy, bool delete = false)
    {
        try
        {
            string data = "";

            if (reference == "")
            {
                data = "{\"content\":\"" + message + "\"}";
            }
            else
            {
                data = "{\"content\":\"" + message + "\",\"message_reference\":{\"channel_id\":\"" + channelId + "\",\"message_id\":\"" + reference + "\"}}";
            }

            var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/channels/" + channelId + "/messages");

            if (proxy != null)
            {
                request.Proxy = proxy.GetNewProxy();
            }
            else
            {
                request.Proxy = null;
            }

            request.UseDefaultCredentials = false;
            request.AllowAutoRedirect = false;

            var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

            request.Method = "POST";

            byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes(data);
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(requestBytes, 0, requestBytes.Length);
            requestStream.Close();

            var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
            {
                ["Host"] = "discord.com",
                ["Authorization"] = GetToken(),
                ["Content-Length"] = requestBytes.Length.ToString(),
                ["Content-Type"] = "application/json"
            });

            requestBytes = null;

            field.SetValue(request, headers);
            var response = request.GetResponse();

            string content = Utils.DecompressResponse(Utils.ReadFully(response.GetResponseStream()));

            if (!content.Contains("\"content\":"))
            {
                return false;
            }

            if (delete)
            {
                dynamic jss = JObject.Parse(content);
                response.Close();
                response.Dispose();
                string theId = (string)jss.id;

                DeleteMessage(channelId, theId, proxy);
                return true;
            }

            response.Close();
            response.Dispose();

            return true;
        }
        catch
        {
            return false;
        }
    }

    public void SendToWS(string data)
    {
        try
        {
            if (!connected)
            {
                ConnectToWebSocket();
            }

            ws.Send(data);
        }
        catch
        {

        }
    }

    public void SetStatus(UserStatus status, DiscordProxy proxy)
    {
        try
        {
            string theStatus = "online";

            if (status.Equals(UserStatus.DoNotDisturb))
            {
                theStatus = "dnd";
            }
            else if (status.Equals(UserStatus.Idle))
            {
                theStatus = "idle";
            }
            else if (status.Equals(UserStatus.Invisible))
            {
                theStatus = "invisible";
            }

            actualStatus = theStatus;

            string data = "{\"status\":\"" + theStatus + "\"}";
            var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/users/@me/settings");

            if (proxy != null)
            {
                request.Proxy = proxy.GetNewProxy();
            }
            else
            {
                request.Proxy = null;
            }

            request.UseDefaultCredentials = false;
            request.AllowAutoRedirect = false;

            var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

            request.Method = "PATCH";

            byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes(data);
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(requestBytes, 0, requestBytes.Length);
            requestStream.Close();

            var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
            {
                ["Host"] = "discord.com",
                ["Authorization"] = GetToken(),
                ["Accept-Encoding"] = "gzip, deflate, br",
                ["Content-Length"] = requestBytes.Length.ToString(),
                ["Content-Type"] = "application/json"
            });

            field.SetValue(request, headers);
            var response = request.GetResponse();
            response.Close();
            response.Dispose();
        }
        catch
        {

        }
    }

    public void SetNickname(string guildId, string nickname, DiscordProxy proxy)
    {
        try
        {
            string data = "{\"nick\":\"" + nickname + "\"}";
            var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/guilds/" + guildId + "/members/@me");

            if (proxy != null)
            {
                request.Proxy = proxy.GetNewProxy();
            }
            else
            {
                request.Proxy = null;
            }

            request.UseDefaultCredentials = false;
            request.AllowAutoRedirect = false;

            var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

            request.Method = "PATCH";

            byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes(data);
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(requestBytes, 0, requestBytes.Length);
            requestStream.Close();

            var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
            {
                ["Host"] = "discord.com",
                ["Authorization"] = GetToken(),
                ["Accept-Encoding"] = "gzip, deflate, br",
                ["Content-Length"] = requestBytes.Length.ToString(),
                ["Content-Type"] = "application/json"
            });

            field.SetValue(request, headers);

            var response = request.GetResponse();
            response.Close();
            response.Dispose();
        }
        catch
        {

        }
    }

    public void SetHypeSquad(HypeSquad house, DiscordProxy proxy)
    {
        try
        {
            int hypesquad = 1;

            if (house.Equals(HypeSquad.Balance))
            {
                hypesquad = 3;
            }
            else if (house.Equals(HypeSquad.Brilliance))
            {
                hypesquad = 2;
            }

            string data = "{\"house_id\":" + hypesquad.ToString() + "}";
            var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/hypesquad/online");

            if (proxy != null)
            {
                request.Proxy = proxy.GetNewProxy();
            }
            else
            {
                request.Proxy = null;
            }

            request.UseDefaultCredentials = false;
            request.AllowAutoRedirect = false;

            var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

            request.Method = "POST";

            byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes(data);
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(requestBytes, 0, requestBytes.Length);
            requestStream.Close();

            var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
            {
                ["Host"] = "discord.com",
                ["Authorization"] = GetToken(),
                ["Accept-Encoding"] = "gzip, deflate, br",
                ["Content-Length"] = requestBytes.Length.ToString(),
                ["Content-Type"] = "application/json"
            });

            field.SetValue(request, headers);
            var response = request.GetResponse();
            response.Close();
            response.Dispose();
        }
        catch
        {

        }
    }

    public void TypeInChannel(string channelId, DiscordProxy proxy)
    {
        try
        {
            var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/channels/" + channelId + "/typing");

            if (proxy != null)
            {
                request.Proxy = proxy.GetNewProxy();
            }
            else
            {
                request.Proxy = null;
            }

            request.UseDefaultCredentials = false;
            request.AllowAutoRedirect = false;

            var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

            request.Method = "POST";

            var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
            {
                ["Host"] = "discord.com",
                ["Authorization"] = GetToken(),
                ["Accept-Encoding"] = "gzip, deflate, br",
                ["Content-Length"] = "0"
            });

            field.SetValue(request, headers);
            var response = request.GetResponse();
            response.Close();
            response.Dispose();
        }
        catch
        {

        }
    }

    public void AddFriend(string friend, DiscordProxy proxy)
    {
        try
        {
            if (Utils.IsIDValid(friend))
            {
                AddFriendByID(friend, proxy);
            }
            else if (Utils.IsTagValid(friend))
            {
                AddFriendByTag(friend, proxy);
            }
        }
        catch
        {

        }
    }

    public void AddFriendByID(string userId, DiscordProxy proxy)
    {
        try
        {
            var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/users/@me/relationships/" + userId);

            if (proxy != null)
            {
                request.Proxy = proxy.GetNewProxy();
            }
            else
            {
                request.Proxy = null;
            }

            request.UseDefaultCredentials = false;
            request.AllowAutoRedirect = false;

            var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

            request.Method = "PUT";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write("{}");
                streamWriter.Close();
                streamWriter.Dispose();
            }

            var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
            {
                ["Host"] = "discord.com",
                ["Connection"] = "keep-alive",
                ["Content-Length"] = "2",
                ["X-Super-Properties"] = "eyJvcyI6IldpbmRvd3MiLCJicm93c2VyIjoiQ2hyb21lIiwiZGV2aWNlIjoiIiwic3lzdGVtX2xvY2FsZSI6Iml0LUlUIiwiYnJvd3Nlcl91c2VyX2FnZW50IjoiTW96aWxsYS81LjAgKFdpbmRvd3MgTlQgMTAuMDsgV2luNjQ7IHg2NCkgQXBwbGVXZWJLaXQvNTM3LjM2IChLSFRNTCwgbGlrZSBHZWNrbykgQ2hyb21lLzk2LjAuNDY2NC40NSBTYWZhcmkvNTM3LjM2IiwiYnJvd3Nlcl92ZXJzaW9uIjoiOTYuMC40NjY0LjQ1Iiwib3NfdmVyc2lvbiI6IjEwIiwicmVmZXJyZXIiOiIiLCJyZWZlcnJpbmdfZG9tYWluIjoiIiwicmVmZXJyZXJfY3VycmVudCI6IiIsInJlZmVycmluZ19kb21haW5fY3VycmVudCI6IiIsInJlbGVhc2VfY2hhbm5lbCI6InN0YWJsZSIsImNsaWVudF9idWlsZF9udW1iZXIiOjEwNzExNiwiY2xpZW50X2V2ZW50X3NvdXJjZSI6bnVsbH0=",
                ["X-Context-Properties"] = "eyJsb2NhdGlvbiI6IkNvbnRleHRNZW51In0=",
                ["X-Discord-Locale"] = GetLanguage(proxy),
                ["Authorization"] = GetToken(),
                ["Content-Type"] = "application/json",
                ["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/93.0.4577.82 Safari/537.36",
                ["Origin"] = "https://discord.com",
                ["Accept-Encoding"] = "gzip, deflate, br",
                ["Cookie"] = GetCookie(null)
            });

            field.SetValue(request, headers);
            var response = request.GetResponse();
            response.Close();
            response.Dispose();
        }
        catch
        {

        }
    }

    public void AddFriendByTag(string tag, DiscordProxy proxy)
    {
        try
        {
            string[] splitted = Strings.Split(tag, "#");
            string data = "{\"username\":\"" + splitted[0] + "\",\"discriminator\":" + splitted[1] + "}";
            var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/users/@me/relationships");

            if (proxy != null)
            {
                request.Proxy = proxy.GetNewProxy();
            }
            else
            {
                request.Proxy = null;
            }

            request.UseDefaultCredentials = false;
            request.AllowAutoRedirect = false;

            var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

            request.Method = "POST";

            byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes(data);
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(requestBytes, 0, requestBytes.Length);
            requestStream.Close();

            var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
            {
                ["Host"] = "discord.com",
                ["Connection"] = "keep-alive",
                ["Content-Length"] = requestBytes.Length.ToString(),
                ["X-Super-Properties"] = "eyJvcyI6IldpbmRvd3MiLCJicm93c2VyIjoiQ2hyb21lIiwiZGV2aWNlIjoiIiwic3lzdGVtX2xvY2FsZSI6Iml0LUlUIiwiYnJvd3Nlcl91c2VyX2FnZW50IjoiTW96aWxsYS81LjAgKFdpbmRvd3MgTlQgMTAuMDsgV2luNjQ7IHg2NCkgQXBwbGVXZWJLaXQvNTM3LjM2IChLSFRNTCwgbGlrZSBHZWNrbykgQ2hyb21lLzk2LjAuNDY2NC40NSBTYWZhcmkvNTM3LjM2IiwiYnJvd3Nlcl92ZXJzaW9uIjoiOTYuMC40NjY0LjQ1Iiwib3NfdmVyc2lvbiI6IjEwIiwicmVmZXJyZXIiOiIiLCJyZWZlcnJpbmdfZG9tYWluIjoiIiwicmVmZXJyZXJfY3VycmVudCI6IiIsInJlZmVycmluZ19kb21haW5fY3VycmVudCI6IiIsInJlbGVhc2VfY2hhbm5lbCI6InN0YWJsZSIsImNsaWVudF9idWlsZF9udW1iZXIiOjEwNzExNiwiY2xpZW50X2V2ZW50X3NvdXJjZSI6bnVsbH0=",
                ["X-Context-Properties"] = "eyJsb2NhdGlvbiI6IkFkZCBGcmllbmQifQ==",
                ["X-Discord-Locale"] = GetLanguage(proxy),
                ["Authorization"] = GetToken(),
                ["Content-Type"] = "application/json",
                ["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/93.0.4577.82 Safari/537.36",
                ["Origin"] = "https://discord.com",
                ["Accept-Encoding"] = "gzip, deflate, br",
                ["Cookie"] = GetCookie(null)
            });

            field.SetValue(request, headers);
            var response = request.GetResponse();
            response.Close();
            response.Dispose();
        }
        catch
        {

        }
    }

    public void AddFriendByTag(string username, string discriminator, DiscordProxy proxy)
    {
        try
        {
            string data = "{\"username\":\"" + username + "\",\"discriminator\":" + discriminator + "}";
            var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/users/@me/relationships");

            if (proxy != null)
            {
                request.Proxy = proxy.GetNewProxy();
            }
            else
            {
                request.Proxy = null;
            }

            request.UseDefaultCredentials = false;
            request.AllowAutoRedirect = false;

            var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

            request.Method = "POST";

            byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes(data);
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(requestBytes, 0, requestBytes.Length);
            requestStream.Close();

            var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
            {
                ["Host"] = "discord.com",
                ["Authorization"] = GetToken(),
                ["Accept-Encoding"] = "gzip, deflate, br",
                ["Content-Length"] = requestBytes.Length.ToString(),
                ["Content-Type"] = "application/json"
            });

            field.SetValue(request, headers);
            var response = request.GetResponse();
            response.Close();
            response.Dispose();
        }
        catch
        {

        }
    }

    public void RemoveFriend(string userId, DiscordProxy proxy)
    {
        try
        {
            var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/users/@me/relationships/" + userId);

            if (proxy != null)
            {
                request.Proxy = proxy.GetNewProxy();
            }
            else
            {
                request.Proxy = null;
            }

            request.UseDefaultCredentials = false;
            request.AllowAutoRedirect = false;

            var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

            request.Method = "DELETE";

            var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
            {
                ["Host"] = "discord.com",
                ["Authorization"] = GetToken(),
                ["Accept-Encoding"] = "gzip, deflate, br",
            });

            field.SetValue(request, headers);
            var response = request.GetResponse();
            response.Close();
            response.Dispose();
        }
        catch
        {

        }
    }

    public string FetchEmote(string channelId, string messageId, DiscordProxy proxy)
    {
        try
        {
            var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/channels/" + channelId + "/messages?limit=50");

            if (proxy != null)
            {
                request.Proxy = proxy.GetNewProxy();
            }
            else
            {
                request.Proxy = null;
            }

            request.UseDefaultCredentials = false;
            request.AllowAutoRedirect = false;

            var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

            request.Method = "GET";

            var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
            {
                ["Accept-Encoding"] = "gzip, deflate, br",
                ["Authorization"] = GetToken(),
                ["Host"] = "discord.com"
            });

            field.SetValue(request, headers);

            var response = request.GetResponse();
            dynamic dynJson = JsonConvert.DeserializeObject(Utils.DecompressResponse(Utils.ReadFully(response.GetResponseStream())));
            response.Close();
            response.Dispose();

            foreach (var item in dynJson)
            {
                try
                {
                    if ((string)item.id == messageId)
                    {
                        try
                        {
                            foreach (var item1 in item.reactions)
                            {
                                try
                                {
                                    string reaction = "", id = "";
                                    id = item1.emoji.id;
                                    reaction = item1.emoji.name;

                                    if (id != null && id != "")
                                    {
                                        reaction += ":" + id;
                                    }

                                    return reaction;
                                }
                                catch
                                {

                                }
                            }
                        }
                        catch
                        {

                        }
                    }
                }
                catch
                {

                }
            }

            return "";
        }
        catch
        {
            return "";
        }
    }

    public void RemoveReaction(string reaction, string channelId, string messageId, DiscordProxy proxy)
    {
        try
        {
            var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/channels/" + channelId + "/messages/" + messageId + "/reactions/" + reaction + "/@me");

            if (proxy != null)
            {
                request.Proxy = proxy.GetNewProxy();
            }
            else
            {
                request.Proxy = null;
            }

            request.UseDefaultCredentials = false;
            request.AllowAutoRedirect = false;

            var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

            request.Method = "DELETE";

            var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
            {
                ["Host"] = "discord.com",
                ["Authorization"] = GetToken(),
                ["Accept-Encoding"] = "gzip, deflate, br",
            });

            field.SetValue(request, headers);
            var response = request.GetResponse();
            response.Close();
            response.Dispose();
        }
        catch
        {

        }
    }

    public void JoinVoice(string guildId, string channelId, string userIdGoLive, bool microphoneMuted, bool headphonesMuted, bool videoEnabled, bool goLive, bool joinGoLive, bool speakInStage, DiscordProxy proxy)
    {
        try
        {
            if (!connectedToVoice)
            {
                connectedToVoice = true;
                this.data = new AutoReconnectData(guildId, channelId, userIdGoLive, microphoneMuted, headphonesMuted, videoEnabled, goLive, joinGoLive, speakInStage);

                SendToWS("{\"op\":4,\"d\":{\"guild_id\":\"" + guildId + "\",\"channel_id\":\"" + channelId + "\",\"self_mute\":" + microphoneMuted.ToString().ToLower() + ",\"self_deaf\":" + headphonesMuted.ToString().ToLower() + ",\"self_video\":" + videoEnabled.ToString().ToLower() + ",\"preferred_region\":null}}");

                if (speakInStage)
                {
                    SendSpeakRequestToStageChannel(guildId, channelId, proxy);
                }
                else
                {
                    if (goLive)
                    {
                        GoLive(guildId, channelId);
                    }

                    if (joinGoLive && Utils.IsIDValid(userIdGoLive))
                    {
                        JoinGoLive(guildId, channelId, userIdGoLive);
                    }
                }
            }
        }
        catch
        {
            connectedToVoice = false;
        }
    }

    public void LeaveVoice()
    {
        try
        {
            if (connectedToVoice)
            {
                connectedToVoice = false;
                SendToWS("{\"op\":4,\"d\":{\"guild_id\":null,\"channel_id\":null,\"self_mute\":false,\"self_deaf\":false,\"self_video\":false}}");
            }
        }
        catch
        {
            connectedToVoice = true;
        }
    }

    public void SendSpeakRequestToStageChannel(string guildId, string channelId, DiscordProxy proxy)
    {
        try
        {
            string timestamp = "";

            string year = "", month = "", day = "", hour = "", minute = "", second = "";

            year = DateTime.Now.Year.ToString();
            month = DateTime.Now.Month.ToString();

            if (month.Length == 1)
            {
                month = "0" + month;
            }

            day = DateTime.Now.Day.ToString();

            if (day.Length == 1)
            {
                day = "0" + day;
            }

            hour = DateTime.Now.Hour.ToString();

            if (hour.Length == 1)
            {
                hour = "0" + hour;
            }

            minute = DateTime.Now.Minute.ToString();

            if (minute.Length == 1)
            {
                minute = "0" + minute;
            }

            second = DateTime.Now.Minute.ToString();

            if (second.Length == 1)
            {
                second = "0" + second;
            }

            timestamp = year + "-" + month + "-" + day + "T" + hour + ":" + minute + ":" + second + "." + DateTime.Now.Millisecond.ToString() + "Z";

            string messageJson = "{\"request_to_speak_timestamp\":\"" + timestamp + "\",\"channel_id\":\"" + channelId + "\"}";
            var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/guilds/" + guildId + "/voice-states/@me");

            if (proxy != null)
            {
                request.Proxy = proxy.GetNewProxy();
            }
            else
            {
                request.Proxy = null;
            }

            request.UseDefaultCredentials = false;
            request.AllowAutoRedirect = false;

            var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

            request.Method = "PATCH";

            byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes(messageJson);
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(requestBytes, 0, requestBytes.Length);
            requestStream.Close();

            var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
            {
                ["Host"] = "discord.com",
                ["Authorization"] = GetToken(),
                ["Accept-Encoding"] = "gzip, deflate, br",
                ["Content-Length"] = requestBytes.Length.ToString(),
                ["Content-Type"] = "application/json"
            });

            field.SetValue(request, headers);
            var response = request.GetResponse();
            response.Close();
            response.Dispose();
        }
        catch
        {

        }
    }

    public void GoLive(string guildId, string channelId)
    {
        try
        {
            SendToWS("{\"op\":18,\"d\":{\"type\":\"guild\",\"guild_id\":\"" + guildId + "\",\"channel_id\":\"" + channelId + "\",\"preferred_region\":null}}");
            SendToWS("{\"op\":22,\"d\":{\"stream_key\":\"guild:" + guildId + ":" + channelId + ":" + GetUserId(null) + "\",\"paused\":false}}");
        }
        catch
        {

        }
    }

    public void StopGoLive(string guildId, string channelId)
    {
        try
        {
            SendToWS("{\"op\":19,\"d\":{\"stream_key\":\"guild:" + guildId + ":" + channelId + ":" + GetUserId(null) + "\"}}");
        }
        catch
        {

        }
    }

    public void JoinGoLive(string guildId, string channelId, string theUser)
    {
        try
        {
            SendToWS("{\"op\":20,\"d\":{\"stream_key\":\"guild:" + guildId + ":" + channelId + ":" + theUser + "\"}}");
        }
        catch
        {

        }
    }

    public void LeaveGoLive(string guildId, string channelId, string userId)
    {
        try
        {
            SendToWS("{\"op\":19,\"d\":{\"stream_key\":\"guild:" + guildId + ":" + channelId + ":" + userId + "\"}}");
        }
        catch
        {

        }
    }

    public void PhoneLock(DiscordProxy proxy)
    {
        try
        {
            var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/invites/otaku");

            if (proxy != null)
            {
                request.Proxy = proxy.GetNewProxy();
            }
            else
            {
                request.Proxy = null;
            }

            request.UseDefaultCredentials = false;
            request.AllowAutoRedirect = false;

            var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

            request.Method = "POST";

            var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
            {
                ["Host"] = "discord.com",
                ["Authorization"] = token
            });

            var response = request.GetResponse();
            response.Close();
            response.Dispose();
        }
        catch
        {

        }
    }

    public void DeleteMessage(string channelId, string messageId, DiscordProxy proxy)
    {
        try
        {
            var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/channels/" + channelId + "/messages/" + messageId);

            if (proxy != null)
            {
                request.Proxy = proxy.GetNewProxy();
            }
            else
            {
                request.Proxy = null;
            }

            request.UseDefaultCredentials = false;
            request.AllowAutoRedirect = false;

            var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

            request.Method = "DELETE";

            var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
            {
                ["Host"] = "discord.com",
                ["Authorization"] = GetToken(),
                ["Accept-Encoding"] = "gzip, deflate, br",
            });

            field.SetValue(request, headers);
            var response = request.GetResponse();
            response.Close();
            response.Dispose();
        }
        catch
        {

        }
    }

    public void SetAvatar(System.Drawing.Image image, DiscordProxy proxy)
    {
        try
        {
            var ms = new System.IO.MemoryStream();
            image.Save(ms, image.RawFormat);
            SetAvatar(Convert.ToBase64String(ms.ToArray()), proxy);
        }
        catch
        {

        }
    }

    public void SetAvatar(string base64, DiscordProxy proxy)
    {
        try
        {
            string content = "{\"avatar\":\"data:image/png;base64," + base64 + "\"}";
            var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/users/@me");

            if (proxy != null)
            {
                request.Proxy = proxy.GetNewProxy();
            }
            else
            {
                request.Proxy = null;
            }

            request.UseDefaultCredentials = false;
            request.AllowAutoRedirect = false;

            var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

            request.Method = "PATCH";

            byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes(content);
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(requestBytes, 0, requestBytes.Length);
            requestStream.Close();

            var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
            {
                ["Host"] = "discord.com",
                ["Connection"] = "keep-alive",
                ["Content-Length"] = requestBytes.Length.ToString(),
                ["X-Super-Properties"] = "eyJvcyI6IldpbmRvd3MiLCJicm93c2VyIjoiQ2hyb21lIiwiZGV2aWNlIjoiIiwic3lzdGVtX2xvY2FsZSI6Iml0LUlUIiwiYnJvd3Nlcl91c2VyX2FnZW50IjoiTW96aWxsYS81LjAgKFdpbmRvd3MgTlQgMTAuMDsgV2luNjQ7IHg2NCkgQXBwbGVXZWJLaXQvNTM3LjM2IChLSFRNTCwgbGlrZSBHZWNrbykgQ2hyb21lLzk2LjAuNDY2NC40NSBTYWZhcmkvNTM3LjM2IiwiYnJvd3Nlcl92ZXJzaW9uIjoiOTYuMC40NjY0LjQ1Iiwib3NfdmVyc2lvbiI6IjEwIiwicmVmZXJyZXIiOiIiLCJyZWZlcnJpbmdfZG9tYWluIjoiIiwicmVmZXJyZXJfY3VycmVudCI6IiIsInJlZmVycmluZ19kb21haW5fY3VycmVudCI6IiIsInJlbGVhc2VfY2hhbm5lbCI6InN0YWJsZSIsImNsaWVudF9idWlsZF9udW1iZXIiOjEwNzExNiwiY2xpZW50X2V2ZW50X3NvdXJjZSI6bnVsbH0=",
                ["X-Discord-Locale"] = GetLanguage(null),
                ["Authorization"] = GetToken(),
                ["Content-Type"] = "application/json",
                ["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.45 Safari/",
                ["Origin"] = "https://discord.com",
                ["Accept-Encoding"] = "gzip, deflate, br",
                ["Cookie"] = GetCookie(null)
            });

            field.SetValue(request, headers);
            var response = request.GetResponse();
            response.Close();
            response.Dispose();
        }
        catch
        {

        }
    }

    public void ResetAvatar(DiscordProxy proxy)
    {
        try
        {
            string content = "{\"avatar\":null}";
            var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/users/@me");

            if (proxy != null)
            {
                request.Proxy = proxy.GetNewProxy();
            }
            else
            {
                request.Proxy = null;
            }

            request.UseDefaultCredentials = false;
            request.AllowAutoRedirect = false;

            var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

            request.Method = "PATCH";

            byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes(content);
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(requestBytes, 0, requestBytes.Length);
            requestStream.Close();

            var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
            {
                ["Host"] = "discord.com",
                ["Connection"] = "keep-alive",
                ["Content-Length"] = requestBytes.Length.ToString(),
                ["X-Super-Properties"] = "eyJvcyI6IldpbmRvd3MiLCJicm93c2VyIjoiQ2hyb21lIiwiZGV2aWNlIjoiIiwic3lzdGVtX2xvY2FsZSI6Iml0LUlUIiwiYnJvd3Nlcl91c2VyX2FnZW50IjoiTW96aWxsYS81LjAgKFdpbmRvd3MgTlQgMTAuMDsgV2luNjQ7IHg2NCkgQXBwbGVXZWJLaXQvNTM3LjM2IChLSFRNTCwgbGlrZSBHZWNrbykgQ2hyb21lLzk2LjAuNDY2NC40NSBTYWZhcmkvNTM3LjM2IiwiYnJvd3Nlcl92ZXJzaW9uIjoiOTYuMC40NjY0LjQ1Iiwib3NfdmVyc2lvbiI6IjEwIiwicmVmZXJyZXIiOiIiLCJyZWZlcnJpbmdfZG9tYWluIjoiIiwicmVmZXJyZXJfY3VycmVudCI6IiIsInJlZmVycmluZ19kb21haW5fY3VycmVudCI6IiIsInJlbGVhc2VfY2hhbm5lbCI6InN0YWJsZSIsImNsaWVudF9idWlsZF9udW1iZXIiOjEwNzExNiwiY2xpZW50X2V2ZW50X3NvdXJjZSI6bnVsbH0=",
                ["X-Discord-Locale"] = GetLanguage(null),
                ["Authorization"] = GetToken(),
                ["Content-Type"] = "application/json",
                ["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.45 Safari/",
                ["Origin"] = "https://discord.com",
                ["Accept-Encoding"] = "gzip, deflate, br",
                ["Cookie"] = GetCookie(null)
            });

            field.SetValue(request, headers);
            var response = request.GetResponse();
            response.Close();
            response.Dispose();
        }
        catch
        {

        }
    }

    public void CreateThread(string channelId, string messageId, string threadName, DiscordProxy proxy)
    {
        try
        {
            string data = "{\"name\":\"" + threadName + "\",\"type\":11,\"auto_archive_duration\":1440,\"location\":\"Message\"}";
            var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/channels/" + channelId + "/messages/" + messageId + "/threads");

            if (proxy != null)
            {
                request.Proxy = proxy.GetNewProxy();
            }
            else
            {
                request.Proxy = null;
            }

            request.UseDefaultCredentials = false;
            request.AllowAutoRedirect = false;

            var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

            request.Method = "POST";

            byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes(data);
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(requestBytes, 0, requestBytes.Length);
            requestStream.Close();

            var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
            {
                ["Host"] = "discord.com",
                ["Authorization"] = GetToken(),
                ["Accept-Encoding"] = "gzip, deflate, br",
                ["Content-Length"] = requestBytes.Length.ToString(),
                ["Content-Type"] = "application/json"
            });

            field.SetValue(request, headers);
            var response = request.GetResponse();
            response.Close();
            response.Dispose();
        }
        catch
        {

        }
    }

    public List<string> GetChannelMessages(string channelId, DiscordProxy proxy)
    {
        try
        {
            List<string> messageIds = new List<string>();

            var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/channels/" + channelId + "/messages?limit=50");

            if (proxy != null)
            {
                request.Proxy = proxy.GetNewProxy();
            }
            else
            {
                request.Proxy = null;
            }

            request.UseDefaultCredentials = false;
            request.AllowAutoRedirect = false;

            var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

            request.Method = "GET";

            var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
            {
                ["Accept-Encoding"] = "gzip, deflate, br",
                ["Authorization"] = GetToken(),
                ["Host"] = "discord.com"
            });

            field.SetValue(request, headers);

            var response = request.GetResponse();
            dynamic dynJson = JsonConvert.DeserializeObject(Utils.DecompressResponse(Utils.ReadFully(response.GetResponseStream())));
            response.Close();
            response.Dispose();

            foreach (var item in dynJson)
            {
                try
                {
                    messageIds.Add((string)item.id);
                }
                catch
                {

                }
            }

            return messageIds;
        }
        catch
        {
            return new List<string>();
        }
    }

    public Tuple<string, string, string> GetButton(string channelId, string messageId, DiscordProxy proxy)
    {
        try
        {
            List<string> messageIds = new List<string>();

            var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/channels/" + channelId + "/messages?limit=50");

            if (proxy != null)
            {
                request.Proxy = proxy.GetNewProxy();
            }
            else
            {
                request.Proxy = null;
            }

            request.UseDefaultCredentials = false;
            request.AllowAutoRedirect = false;

            var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

            request.Method = "GET";

            var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
            {
                ["Accept-Encoding"] = "gzip, deflate, br",
                ["Authorization"] = GetToken(),
                ["Host"] = "discord.com"
            });

            field.SetValue(request, headers);

            var response = request.GetResponse();
            dynamic dynJson = JsonConvert.DeserializeObject(Utils.DecompressResponse(Utils.ReadFully(response.GetResponseStream())));
            response.Close();
            response.Dispose();

            string authorId = "", component_type = "", custom_id = "";

            foreach (var item in dynJson)
            {
                try
                {
                    if ((string)item.id == messageId)
                    {
                        try
                        {
                            authorId = (string)item.author.id;

                            try
                            {
                                foreach (var sesso1 in item.components)
                                {
                                    try
                                    {
                                        foreach (var sesso2 in sesso1)
                                        {
                                            try
                                            {
                                                foreach (var sesso3 in sesso2)
                                                {
                                                    try
                                                    {
                                                        foreach (var sesso4 in sesso3)
                                                        {
                                                            custom_id = (string)sesso4.custom_id;
                                                            component_type = (string)sesso4.type;
                                                            return new Tuple<string, string, string>(authorId, component_type, custom_id);
                                                        }
                                                    }
                                                    catch
                                                    {

                                                    }
                                                }
                                            }
                                            catch
                                            {

                                            }
                                        }
                                    }
                                    catch
                                    {

                                    }
                                }
                            }
                            catch
                            {

                            }
                        }
                        catch
                        {

                        }
                    }
                }
                catch
                {

                }
            }

            return new Tuple<string, string, string>(authorId, component_type, custom_id);
        }
        catch
        {
            return new Tuple<string, string, string>("", "", "");
        }
    }

    public void ClickButton(string applicationId, string channelId, string component_type, string custom_id, string guild_id, string message_id, DiscordProxy proxy)
    {
        try
        {
            string data = "{\"type\":3,\"nonce\":\"" + Utils.GetUniqueLong(18) + "\",\"guild_id\":\"" + guild_id + "\",\"channel_id\":\"" + channelId + "\",\"message_flags\":0,\"message_id\":\"" + message_id + "\",\"application_id\":\"" + applicationId + "\",\"data\":{\"component_type\":2,\"custom_id\":\"" + custom_id + "\"}}";
            var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/interactions");

            if (proxy != null)
            {
                request.Proxy = proxy.GetNewProxy();
            }
            else
            {
                request.Proxy = null;
            }

            request.UseDefaultCredentials = false;
            request.AllowAutoRedirect = false;

            var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

            request.Method = "POST";

            byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes(data);
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(requestBytes, 0, requestBytes.Length);
            requestStream.Close();

            var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
            {
                ["Host"] = "discord.com",
                ["Content-Length"] = requestBytes.Length.ToString(),
                ["Authorization"] = GetToken(),
                ["Content-Type"] = "application/json",
                ["Accept-Encoding"] = "gzip, deflate, br"
            });

            field.SetValue(request, headers);

            var response = request.GetResponse();

            response.Close();
            response.Dispose();
        }
        catch
        {

        }
    }

    public string GetLokiBotResult(string channelId, DiscordProxy proxy)
    {
        try
        {
            var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/channels/" + channelId + "/messages?limit=50");

            if (proxy != null)
            {
                request.Proxy = proxy.GetNewProxy();
            }
            else
            {
                request.Proxy = null;
            }

            request.UseDefaultCredentials = false;
            request.AllowAutoRedirect = false;

            var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

            request.Method = "GET";

            var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
            {
                ["Accept-Encoding"] = "gzip, deflate, br",
                ["Authorization"] = GetToken(),
                ["Host"] = "discord.com"
            });

            field.SetValue(request, headers);

            var response = request.GetResponse();
            string messages = Utils.DecompressResponse(Utils.ReadFully(response.GetResponseStream()));
            response.Close();
            response.Dispose();
            dynamic dynJson = JsonConvert.DeserializeObject(messages);

            foreach (var item in dynJson)
            {
                try
                {
                    foreach (var item1 in item.attachments)
                    {
                        try
                        {
                            return ((string)item1.filename).Replace("captcha_", "").Replace(".png", "");
                        }
                        catch
                        {

                        }
                    }
                }
                catch
                {

                }
            }
        }
        catch
        {

        }

        return "";
    }

    public void BypassLokiBot(string botId, DiscordProxy proxy)
    {
        try
        {
            string dmChannel = GetDMChannel(botId, proxy);
            string botResult = GetLokiBotResult(dmChannel, proxy);
            SendMessage(dmChannel, "!" + botResult, "", proxy);
        }
        catch
        {

        }
    }

    public void CreateInvite(string channelId, int durationIndex, int usesIndex, bool temporary, DiscordProxy proxy)
    {
        try
        {
            var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/channels/" + channelId + "/invites");
            string maxAge = "", maxUses = "";

            switch (durationIndex)
            {
                case 0:
                    maxAge = "1800";
                    break;
                case 1:
                    maxAge = "3600";
                    break;
                case 2:
                    maxAge = "21600";
                    break;
                case 3:
                    maxAge = "43200";
                    break;
                case 4:
                    maxAge = "86400";
                    break;
                case 5:
                    maxAge = "604800";
                    break;
                case 6:
                    maxAge = "0";
                    break;
            }

            switch (usesIndex)
            {
                case 0:
                    maxUses = "0";
                    break;
                case 1:
                    maxUses = "1";
                    break;
                case 2:
                    maxUses = "5";
                    break;
                case 3:
                    maxUses = "10";
                    break;
                case 4:
                    maxUses = "25";
                    break;
                case 5:
                    maxUses = "50";
                    break;
                case 6:
                    maxUses = "100";
                    break;
            }

            string data = "{\"max_age\":" + maxAge + ",\"max_uses\":" + maxUses + ",\"temporary\":" + temporary.ToString().ToLower() + "}";

            if (proxy != null)
            {
                request.Proxy = proxy.GetNewProxy();
            }
            else
            {
                request.Proxy = null;
            }

            request.UseDefaultCredentials = false;
            request.AllowAutoRedirect = false;

            var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

            request.Method = "POST";

            byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes(data);
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(requestBytes, 0, requestBytes.Length);
            requestStream.Close();

            var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
            {
                ["Host"] = "discord.com",
                ["Connection"] = "keep-alive",
                ["Content-Length"] = requestBytes.Length.ToString(),
                ["X-Super-Properties"] = "eyJvcyI6IldpbmRvd3MiLCJicm93c2VyIjoiQ2hyb21lIiwiZGV2aWNlIjoiIiwic3lzdGVtX2xvY2FsZSI6Iml0LUlUIiwiYnJvd3Nlcl91c2VyX2FnZW50IjoiTW96aWxsYS81LjAgKFdpbmRvd3MgTlQgMTAuMDsgV2luNjQ7IHg2NCkgQXBwbGVXZWJLaXQvNTM3LjM2IChLSFRNTCwgbGlrZSBHZWNrbykgQ2hyb21lLzk2LjAuNDY2NC40NSBTYWZhcmkvNTM3LjM2IiwiYnJvd3Nlcl92ZXJzaW9uIjoiOTYuMC40NjY0LjQ1Iiwib3NfdmVyc2lvbiI6IjEwIiwicmVmZXJyZXIiOiIiLCJyZWZlcnJpbmdfZG9tYWluIjoiIiwicmVmZXJyZXJfY3VycmVudCI6IiIsInJlZmVycmluZ19kb21haW5fY3VycmVudCI6IiIsInJlbGVhc2VfY2hhbm5lbCI6InN0YWJsZSIsImNsaWVudF9idWlsZF9udW1iZXIiOjEwNzExNiwiY2xpZW50X2V2ZW50X3NvdXJjZSI6bnVsbH0=",
                ["X-Context-Properties"] = "eyJsb2NhdGlvbiI6IkNvbnRleHQgTWVudSJ9",
                ["X-Discord-Locale"] = GetLanguage(proxy),
                ["Authorization"] = GetToken(),
                ["Content-Type"] = "application/json",
                ["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/93.0.4577.82 Safari/537.36",
                ["Origin"] = "https://discord.com",
                ["Accept-Encoding"] = "gzip, deflate, br",
                ["Cookie"] = GetCookie(null)
            });

            field.SetValue(request, headers);
            var response = request.GetResponse();
            response.Close();
            response.Dispose();
        }
        catch
        {

        }
    }

    public void SetGame(string game)
    {
        try
        {
            if (game == "")
            {
                SendToWS("{\"op\":3,\"d\":{\"since\":91879201,\"activities\":[],\"status\":\"" + actualStatus + "\",\"afk\":false}}");
            }
            else
            {
                SendToWS("{\"op\":3,\"d\":{\"since\":91879201,\"activities\":[{\"name\":\"" + game + "\",\"type\":0}],\"status\":\"" + actualStatus + "\",\"afk\":false}}");
            }
        }
        catch
        {

        }
    }

    public void SetAboutMe(string aboutMe, DiscordProxy proxy)
    {
        try
        {
            string data = "{\"bio\":\"" + aboutMe + "\"}";
            var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/users/@me");

            if (proxy != null)
            {
                request.Proxy = proxy.GetNewProxy();
            }
            else
            {
                request.Proxy = null;
            }

            request.UseDefaultCredentials = false;
            request.AllowAutoRedirect = false;

            var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

            request.Method = "PATCH";

            byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes(data);
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(requestBytes, 0, requestBytes.Length);
            requestStream.Close();

            var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
            {
                ["Host"] = "discord.com",
                ["Connection"] = "keep-alive",
                ["Content-Length"] = requestBytes.Length.ToString(),
                ["X-Super-Properties"] = "eyJvcyI6IldpbmRvd3MiLCJicm93c2VyIjoiQ2hyb21lIiwiZGV2aWNlIjoiIiwic3lzdGVtX2xvY2FsZSI6Iml0LUlUIiwiYnJvd3Nlcl91c2VyX2FnZW50IjoiTW96aWxsYS81LjAgKFdpbmRvd3MgTlQgMTAuMDsgV2luNjQ7IHg2NCkgQXBwbGVXZWJLaXQvNTM3LjM2IChLSFRNTCwgbGlrZSBHZWNrbykgQ2hyb21lLzk2LjAuNDY2NC40NSBTYWZhcmkvNTM3LjM2IiwiYnJvd3Nlcl92ZXJzaW9uIjoiOTYuMC40NjY0LjQ1Iiwib3NfdmVyc2lvbiI6IjEwIiwicmVmZXJyZXIiOiIiLCJyZWZlcnJpbmdfZG9tYWluIjoiIiwicmVmZXJyZXJfY3VycmVudCI6IiIsInJlZmVycmluZ19kb21haW5fY3VycmVudCI6IiIsInJlbGVhc2VfY2hhbm5lbCI6InN0YWJsZSIsImNsaWVudF9idWlsZF9udW1iZXIiOjEwNzExNiwiY2xpZW50X2V2ZW50X3NvdXJjZSI6bnVsbH0=",
                ["Authorization"] = GetToken(),
                ["Content-Type"] = "application/json",
                ["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.45 Safari/",
                ["Origin"] = "https://discord.com",
                ["Accept-Encoding"] = "gzip, deflate, br",
                ["Cookie"] = GetCookie(proxy)
            });

            field.SetValue(request, headers);
            var response = request.GetResponse();
            response.Close();
            response.Dispose();
        }
        catch
        {

        }
    }

    public void SetUsername(string username, string password, DiscordProxy proxy)
    {
        try
        {
            string data = "{\"username\":\"" + username + "\",\"password\":\"" + password + "\"}";
            var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/users/@me");

            if (proxy != null)
            {
                request.Proxy = proxy.GetNewProxy();
            }
            else
            {
                request.Proxy = null;
            }

            request.UseDefaultCredentials = false;
            request.AllowAutoRedirect = false;

            var field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

            request.Method = "PATCH";

            byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes(data);
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(requestBytes, 0, requestBytes.Length);
            requestStream.Close();

            var headers = new CustomWebHeaderCollection(new Dictionary<string, string>
            {
                ["Host"] = "discord.com",
                ["Connection"] = "keep-alive",
                ["Content-Length"] = requestBytes.Length.ToString(),
                ["X-Super-Properties"] = "eyJvcyI6IldpbmRvd3MiLCJicm93c2VyIjoiQ2hyb21lIiwiZGV2aWNlIjoiIiwic3lzdGVtX2xvY2FsZSI6Iml0LUlUIiwiYnJvd3Nlcl91c2VyX2FnZW50IjoiTW96aWxsYS81LjAgKFdpbmRvd3MgTlQgMTAuMDsgV2luNjQ7IHg2NCkgQXBwbGVXZWJLaXQvNTM3LjM2IChLSFRNTCwgbGlrZSBHZWNrbykgQ2hyb21lLzk2LjAuNDY2NC40NSBTYWZhcmkvNTM3LjM2IiwiYnJvd3Nlcl92ZXJzaW9uIjoiOTYuMC40NjY0LjQ1Iiwib3NfdmVyc2lvbiI6IjEwIiwicmVmZXJyZXIiOiIiLCJyZWZlcnJpbmdfZG9tYWluIjoiIiwicmVmZXJyZXJfY3VycmVudCI6IiIsInJlZmVycmluZ19kb21haW5fY3VycmVudCI6IiIsInJlbGVhc2VfY2hhbm5lbCI6InN0YWJsZSIsImNsaWVudF9idWlsZF9udW1iZXIiOjEwNzExNiwiY2xpZW50X2V2ZW50X3NvdXJjZSI6bnVsbH0=",
                ["Authorization"] = GetToken(),
                ["Content-Type"] = "application/json",
                ["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.45 Safari/",
                ["Origin"] = "https://discord.com",
                ["Accept-Encoding"] = "gzip, deflate, br",
                ["Cookie"] = GetCookie(proxy)
            });

            field.SetValue(request, headers);
            var response = request.GetResponse();
            response.Close();
            response.Dispose();
        }
        catch
        {

        }
    }
}