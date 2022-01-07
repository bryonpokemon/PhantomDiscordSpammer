using System.Windows.Forms;
using System.Threading;
using System;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.InteropServices;
using MetroSuite;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

public partial class MainForm : MetroForm
{
    [DllImport("psapi.dll")]
    static extern int EmptyWorkingSet(IntPtr hwProc);

    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetProcessWorkingSetSize(IntPtr process, UIntPtr minimumWorkingSetSize, UIntPtr maximumWorkingSetSize);

    public List<DiscordClient> clients = new List<DiscordClient>();
    public List<string> invalidTokens = new List<string>();
    public int doneCheckingTokens = 0;

    public List<string> proxies = new List<string>();
    public List<string> invalidProxies = new List<string>();
    public int doneCheckingProxies = 0;
    public int proxyIndex = 0;

    public bool serverSpammer, dmSpammer, typingSpammer, webhookSpammer, massDmAdvertiser;
    public int multipleMessageIndex = 0, multipleDmMessageIndex = 0, multipleWebhookMessageIndex = 0, multipleDmAdvertiserMessageIndex = 0, tagAllIndex = 0, rolesTagAllIndex = 0, threadNameIndex = 0;
    public int completedUsers = 0;

    private string[] mediaFormats = new string[] { "jpg", "png", "bmp", "jpeg", "jfif", "jpe", "rle", "dib", "svg", "svgz" };
    public bool tokensChanged;
    private string[] games = new string[] { "Fortnite", "Apex Legends", "Soundpad", "World Of Warcraft", "KurtzPel", "Elsword", "League Of Legends", "Rainbow Six Siege", "Cuphead", "Dragon Ball FighterZ", "DRAGON BALL Z KAKAROT", "BIGFOOT", "The Forest", "Geometry Dash", "Trove", "AssaultCube", "BLOCKPOST", "Dying Light", "Heartstone", "osu!", "Outlast", "Outlast 2", "Tomb Raider", "Starbound", "Terraria", "Sid Meier's Civilization V", "Sid Meier's Civilization VI", "Paladins", "Overwatch", "Minecraft", "Little Inferno", "It Moves", "JUMP FORCE", "Closers" };

    public int guildManagerStatus = 0, serverSpammerStatus = 0, dmSpammerStatus = 0, reactionSpammerStatus = 0, friendSpammerStatus = 0, voiceSpammerStatus = 0, webhookSpammerStatus = 0, buttonSpammerStatus = 0, inviteSpammerStatus = 0;

    public List<DiscordClient> GetClients()
    {
        if (siticoneCheckBox37.Checked)
        {
            List<DiscordClient> theClients = new List<DiscordClient>();

            int limited = 1;
            string coso = gunaLineTextBox23.Text.Replace(" ", "").Replace('\t'.ToString(), "");

            if (coso.Length <= 6)
            {
                if (Microsoft.VisualBasic.Information.IsNumeric(coso))
                {
                    int temp = int.Parse(coso);

                    if (!(temp <= 0))
                    {
                        limited = temp;
                    }
                }
            }

            int i = 0;

            foreach (DiscordClient client in this.clients)
            {
                if (i == limited)
                {
                    break;
                }

                theClients.Add(clients[i]);
                i++;
            }

            return theClients;
        }

        return this.clients;
    }

    public DiscordProxy GetProxy()
    {
        try
        {
            if (siticoneCheckBox23.Checked)
            {
                try
                {
                    if (proxyIndex >= proxies.Count)
                    {
                        proxyIndex = -1;
                    }

                    proxyIndex++;

                    if (proxyIndex >= proxies.Count)
                    {
                        proxyIndex = 0;
                    }

                    return new DiscordProxy(proxies[proxyIndex]);
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    public MainForm()
    {
        try
        {
            InitializeComponent();

            tokensChanged = true;

            try
            {
                if (!System.IO.File.Exists("tokens.txt"))
                {
                    System.IO.File.WriteAllText("tokens.txt", "");
                }
                else
                {
                    LoadTokens("tokens.txt");
                }
            }
            catch
            {

            }

            try
            {
                if (!System.IO.File.Exists("proxies.txt"))
                {
                    System.IO.File.WriteAllText("proxies.txt", "");
                }
                else
                {
                    LoadProxies("proxies.txt");
                }
            }
            catch
            {

            }

            CheckForIllegalCrossThreadCalls = false;

            siticoneComboBox2.SelectedIndex = 0;
            siticoneComboBox1.SelectedIndex = 0;

            Thread updateAllThread = new Thread(updateAll);
            updateAllThread.Priority = ThreadPriority.Highest;
            updateAllThread.Start();

            Thread updateAllThread1 = new Thread(updateAll1);
            updateAllThread1.Priority = ThreadPriority.Highest;
            updateAllThread1.Start();

            openFileDialog5.Filter = "All files (*.*)|*.*";

            foreach (string format in mediaFormats)
            {
                openFileDialog5.Filter += "|" + format.ToUpper() + " Image (*." + format + ")|*." + format;
            }

            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;

            System.Net.ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            System.Net.ServicePointManager.MaxServicePoints = int.MaxValue;

            siticoneComboBox4.SelectedIndex = 0;
            siticoneComboBox3.SelectedIndex = 6;
            siticoneComboBox5.SelectedIndex = 0;
            siticoneComboBox6.SelectedIndex = 0;
            siticoneComboBox7.SelectedIndex = 0;
            siticoneComboBox8.SelectedIndex = 0;
            siticoneComboBox9.SelectedIndex = 0;
            siticoneComboBox10.SelectedIndex = 0;
            siticoneComboBox11.SelectedIndex = 0;
        }
        catch
        {

        }
    }

    public void save()
    {
        try
        {
            string allProxies = "", allTokens = "";

            try
            {
                foreach (string proxy in proxies)
                {
                    try
                    {
                        if (allProxies == "")
                        {
                            allProxies = proxy;
                        }
                        else
                        {
                            allProxies += Environment.NewLine + proxy;
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

            try
            {
                System.IO.File.WriteAllText("proxies.txt", allProxies);
            }
            catch
            {

            }

            try
            {
                foreach (DiscordClient client in clients)
                {
                    try
                    {
                        if (allTokens == "")
                        {
                            allTokens = client.token;
                        }
                        else
                        {
                            allTokens += Environment.NewLine + client.token;
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

            try
            {
                System.IO.File.WriteAllText("tokens.txt", allTokens);
            }
            catch
            {

            }
        }
        catch
        {

        }
    }
    public void updateAll1()
    {
        while (true)
        {
            Thread.Sleep(10);

            metroLabel37.Text = $"Joined/left tokens: {guildManagerStatus}";
            metroLabel38.Text = $"Sent messages: {serverSpammerStatus}";
            metroLabel39.Text = $"Sent messages: {dmSpammerStatus}";
            metroLabel42.Text = $"Added/removed reactions: {reactionSpammerStatus}";
            metroLabel43.Text = $"Added/removed friends: {friendSpammerStatus}";
            metroLabel44.Text = $"Joined/left tokens: {voiceSpammerStatus}";
            metroLabel45.Text = $"Sent messages: {webhookSpammerStatus}";
            metroLabel46.Text = $"Clicked buttons: {buttonSpammerStatus}";
            metroLabel47.Text = $"Created invites: {inviteSpammerStatus}";
        }
    }

    public void updateAll()
    {
        while (true)
        {
            Thread.Sleep(500);

            if (metroLabel14.Text == "0")
            {
                invalidTokens.Clear();
                doneCheckingTokens = 0;
                siticoneButton2.Enabled = true;
                siticoneButton2.Text = "Remove dead tokens";
                metroLabel14.Text = clients.Count.ToString();
                siticoneButton1.Enabled = true;
            }

            if (clients.Count == 0)
            {
                metroLabel14.Text = "0";
            }

            if (metroLabel15.Text == "0")
            {
                invalidProxies.Clear();
                doneCheckingProxies = 0;
                siticoneButton3.Enabled = true;
                siticoneButton3.Text = "Remove dead proxies";
                metroLabel15.Text = proxies.Count.ToString();
                siticoneButton4.Enabled = true;
            }

            if (proxies.Count == 0)
            {
                metroLabel15.Text = "0";
            }

            if (Utils.users.Count == 0)
            {
                siticoneCheckBox7.Text = "Mass Mention";
            }
            else
            {
                siticoneCheckBox7.Text = "Mass Mention (" + Utils.users.Count.ToString() + ")";
            }

            if (gunaLineTextBox4.Text.Contains(","))
            {
                siticoneCheckBox8.Text = "Multiple Channels (" + Microsoft.VisualBasic.Strings.Split(gunaLineTextBox4.Text, ",").Length.ToString() + ")";
            }
            else
            {
                siticoneCheckBox8.Text = "Multiple Channels";
            }

            if (gunaTextBox1.Text.Contains("|"))
            {
                siticoneCheckBox9.Text = "Multiple Messages (" + Microsoft.VisualBasic.Strings.Split(gunaTextBox1.Text, "|").Length.ToString() + ")";
            }
            else
            {
                siticoneCheckBox9.Text = "Multiple Messages";
            }

            if (gunaLineTextBox6.Text.Contains(","))
            {
                siticoneCheckBox11.Text = "Multiple Users (" + Microsoft.VisualBasic.Strings.Split(gunaLineTextBox6.Text, ",").Length.ToString() + ")";
            }
            else
            {
                siticoneCheckBox11.Text = "Multiple Users";
            }

            if (gunaTextBox2.Text.Contains("|"))
            {
                siticoneCheckBox12.Text = "Multiple Messages (" + Microsoft.VisualBasic.Strings.Split(gunaTextBox2.Text, "|").Length.ToString() + ")";
            }
            else
            {
                siticoneCheckBox12.Text = "Multiple Messages";
            }

            if (gunaLineTextBox11.Text.Contains(","))
            {
                siticoneCheckBox5.Text = "Multiple Friends (" + Microsoft.VisualBasic.Strings.Split(gunaLineTextBox11.Text, ",").Length.ToString() + ")";
            }
            else
            {
                siticoneCheckBox5.Text = "Multiple Friends";
            }

            if (gunaLineTextBox12.Text.Contains(","))
            {
                siticoneCheckBox13.Text = "Multiple Channels (" + Microsoft.VisualBasic.Strings.Split(gunaLineTextBox12.Text, ",").Length.ToString() + ")";
            }
            else
            {
                siticoneCheckBox13.Text = "Multiple Channels";
            }

            if (gunaLineTextBox17.Text.Contains(","))
            {
                siticoneCheckBox22.Text = "Multiple Webhooks (" + Microsoft.VisualBasic.Strings.Split(gunaLineTextBox17.Text, ",").Length.ToString() + ")";
            }
            else
            {
                siticoneCheckBox22.Text = "Multiple Webhooks";
            }

            if (gunaTextBox3.Text.Contains("|"))
            {
                siticoneCheckBox27.Text = "Multiple Messages (" + Microsoft.VisualBasic.Strings.Split(gunaTextBox3.Text, "|").Length.ToString() + ")";
            }
            else
            {
                siticoneCheckBox27.Text = "Multiple Messages";
            }

            metroLabel8.Text = "Parsed users: " + Utils.users.Count.ToString() + ", completed users: " + completedUsers.ToString();

            if (gunaTextBox4.Text.Contains("|"))
            {
                siticoneCheckBox28.Text = "Multiple Messages (" + Microsoft.VisualBasic.Strings.Split(gunaTextBox4.Text, "|").Length.ToString() + ")";
            }
            else
            {
                siticoneCheckBox28.Text = "Multiple Messages";
            }

            if (gunaLineTextBox43.Text.Contains(","))
            {
                siticoneCheckBox39.Text = "Multiple Channels (" + Microsoft.VisualBasic.Strings.Split(gunaLineTextBox43.Text, ",").Length.ToString() + ")";
            }
            else
            {
                siticoneCheckBox39.Text = "Multiple Channels";
            }

            if (completedUsers == Utils.users.Count)
            {
                gunaButton20.Enabled = false;
                new Thread(reEnableDMAdvertiser).Start();
            }

            if (Utils.roles.Count == 0)
            {
                siticoneCheckBox29.Text = "Roles Mention";
            }
            else
            {
                siticoneCheckBox29.Text = "Roles Mention (" + Utils.roles.Count.ToString() + ")";
            }

            metroLabel32.Text = Utils.users.Count.ToString();
            metroLabel33.Text = Utils.roles.Count.ToString();

            if (pictureBox2.BackgroundImage == null)
            {
                siticoneButton22.Enabled = false;
            }
            else
            {
                siticoneButton22.Enabled = true;
            }

            if (tokensChanged)
            {
                tokensChanged = false;
                save();
            }
        }
    }

    public string getTagAllUser()
    {
        try
        {
            if (tagAllIndex >= Utils.users.Count)
            {
                tagAllIndex = -1;
            }

            tagAllIndex++;

            if (tagAllIndex >= Utils.users.Count)
            {
                tagAllIndex = 0;
            }

            return Utils.users[tagAllIndex];
        }
        catch
        {

        }

        return "";
    }

    public string getTagAllRole()
    {
        try
        {
            if (rolesTagAllIndex >= Utils.roles.Count)
            {
                rolesTagAllIndex = -1;
            }

            rolesTagAllIndex++;

            if (rolesTagAllIndex >= Utils.roles.Count)
            {
                rolesTagAllIndex = 0;
            }

            return Utils.roles[rolesTagAllIndex];
        }
        catch
        {

        }

        return "";
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        try
        {
            Process.GetCurrentProcess().Kill();
        }
        catch
        {

        }
    }

    private void bunifuHSlider1_Scroll(object sender, Utilities.BunifuSlider.BunifuHScrollBar.ScrollEventArgs e)
    {
        try
        {
            metroLabel17.Text = "Delay: " + bunifuHSlider1.Value.ToString() + "ms";
        }
        catch
        {

        }
    }

    private void bunifuHSlider2_Scroll(object sender, Utilities.BunifuSlider.BunifuHScrollBar.ScrollEventArgs e)
    {
        try
        {
            metroLabel18.Text = "Delay: " + bunifuHSlider2.Value.ToString() + "ms";
        }
        catch
        {

        }
    }

    private void bunifuHSlider3_Scroll(object sender, Utilities.BunifuSlider.BunifuHScrollBar.ScrollEventArgs e)
    {
        try
        {
            metroLabel19.Text = "Delay: " + bunifuHSlider3.Value.ToString() + "ms";
        }
        catch
        {

        }
    }

    private void bunifuHSlider4_Scroll(object sender, Utilities.BunifuSlider.BunifuHScrollBar.ScrollEventArgs e)
    {
        try
        {
            metroLabel20.Text = "Delay: " + bunifuHSlider4.Value.ToString() + "ms";
        }
        catch
        {

        }
    }

    private void bunifuHSlider5_Scroll(object sender, Utilities.BunifuSlider.BunifuHScrollBar.ScrollEventArgs e)
    {
        try
        {
            metroLabel21.Text = "Delay: " + bunifuHSlider5.Value.ToString() + "ms";
        }
        catch
        {

        }
    }

    private void bunifuHSlider6_Scroll(object sender, Utilities.BunifuSlider.BunifuHScrollBar.ScrollEventArgs e)
    {
        try
        {
            metroLabel22.Text = "Delay: " + bunifuHSlider6.Value.ToString() + "ms";
        }
        catch
        {

        }
    }

    private void bunifuHSlider9_Scroll(object sender, Utilities.BunifuSlider.BunifuHScrollBar.ScrollEventArgs e)
    {
        try
        {
            metroLabel25.Text = "Delay: " + bunifuHSlider9.Value.ToString() + "ms";
        }
        catch
        {

        }
    }

    private void bunifuHSlider10_Scroll(object sender, Utilities.BunifuSlider.BunifuHScrollBar.ScrollEventArgs e)
    {
        try
        {
            metroLabel26.Text = "Auto end: " + bunifuHSlider10.Value.ToString() + "ms";
        }
        catch
        {

        }
    }

    private void bunifuHSlider11_Scroll(object sender, Utilities.BunifuSlider.BunifuHScrollBar.ScrollEventArgs e)
    {
        try
        {
            metroLabel27.Text = "Delay: " + bunifuHSlider11.Value.ToString() + "ms";
        }
        catch
        {

        }
    }

    private void bunifuHSlider12_Scroll(object sender, Utilities.BunifuSlider.BunifuHScrollBar.ScrollEventArgs e)
    {
        try
        {
            metroLabel28.Text = "Delay: " + bunifuHSlider12.Value.ToString() + "ms";
        }
        catch
        {

        }
    }

    private void gunaLinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        try
        {
            Process.Start(gunaLinkLabel1.Text);
        }
        catch
        {

        }
    }

    private void siticoneButton1_Click(object sender, EventArgs e)
    {
        // Homepage - Reset Tokens
        try
        {
            clients.Clear();
            metroLabel14.Text = "0";

            invalidTokens.Clear();
            doneCheckingTokens = 0;
            siticoneButton2.Enabled = true;
            siticoneButton2.Text = "Remove dead tokens";
            tokensChanged = true;
        }
        catch
        {

        }
    }

    private void siticoneButton2_Click(object sender, EventArgs e)
    {
        // Homepage - Remove dead tokens
        try
        {
            invalidTokens.Clear();
            doneCheckingTokens = 0;
            siticoneButton2.Enabled = false;
            siticoneButton1.Enabled = false;
            siticoneButton2.Text = "Removing dead tokens...";
            Thread thread = new Thread(CheckTokens);
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }
        catch
        {

        }
    }

    public void CheckTokens()
    {
        try
        {
            try
            {
                foreach (DiscordClient client in clients)
                {
                    try
                    {
                        if (!siticoneButton2.Enabled)
                        {
                            Thread.Sleep(1);

                            Thread thread = new Thread(() => CheckToken(client));
                            thread.Priority = ThreadPriority.Highest;
                            thread.Start();
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

            try
            {
                while (doneCheckingTokens != clients.Count)
                {

                }
            }
            catch
            {

            }

            metroLabel14.Text = (clients.Count).ToString();

            foreach (string token in invalidTokens)
            {
                try
                {
                    foreach (DiscordClient client in clients)
                    {
                        try
                        {
                            if (client.token == token)
                            {
                                clients.Remove(client);
                                metroLabel14.Text = (clients.Count).ToString();

                                break;
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

            invalidTokens.Clear();
            doneCheckingTokens = 0;
            siticoneButton2.Enabled = true;
            siticoneButton2.Text = "Remove dead tokens";
            metroLabel14.Text = clients.Count.ToString();
            siticoneButton1.Enabled = true;
            tokensChanged = true;
        }
        catch
        {

        }
    }

    public void CheckToken(DiscordClient client)
    {
        try
        {
            if (!siticoneButton2.Enabled)
            {
                if (!Utils.IsTokenValid(client.token))
                {
                    invalidTokens.Add(client.token);
                    metroLabel14.Text = (clients.Count - invalidTokens.Count).ToString();
                }

                Interlocked.Increment(ref doneCheckingTokens);
            }
        }
        catch
        {
            Interlocked.Increment(ref doneCheckingTokens);
        }
    }

    private void siticoneButton5_Click(object sender, EventArgs e)
    {
        // Homepage - Load Tokens
        try
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    foreach (string fileName in openFileDialog1.FileNames)
                    {
                        try
                        {
                            LoadTokens(fileName);
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

    public void LoadTokens(string fileName)
    {
        foreach (string token in Utils.SplitToLines(System.IO.File.ReadAllText(fileName)))
        {
            try
            {
                string tok = Utils.GetCleanToken(token);
                bool inserted = false;

                if (!Utils.IsTokenFormatValid(tok))
                {
                    continue;
                }

                try
                {
                    foreach (DiscordClient client in clients)
                    {
                        try
                        {
                            if (client.token == tok)
                            {
                                inserted = true;
                                break;
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

                if (!inserted)
                {
                    clients.Add(new DiscordClient(tok));
                }
            }
            catch
            {

            }
        }

        metroLabel14.Text = clients.Count.ToString();
        tokensChanged = true;
    }

    public void LoadTokensFromClipboard()
    {
        try
        {
            foreach (string token in Utils.SplitToLines(Clipboard.GetText()))
            {
                try
                {
                    string tok = Utils.GetCleanToken(token);
                    bool inserted = false;

                    if (!Utils.IsTokenFormatValid(tok))
                    {
                        continue;
                    }

                    try
                    {
                        foreach (DiscordClient client in clients)
                        {
                            try
                            {
                                if (client.token == tok)
                                {
                                    inserted = true;
                                    break;
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

                    if (!inserted)
                    {
                        clients.Add(new DiscordClient(tok));
                    }
                }
                catch
                {

                }
            }

            metroLabel14.Text = clients.Count.ToString();
            tokensChanged = true;
        }
        catch
        {

        }
    }

    private void siticoneButton4_Click(object sender, EventArgs e)
    {
        // Homepage - Reset Proxies
        try
        {
            proxies.Clear();
            metroLabel15.Text = "0";

            invalidProxies.Clear();
            doneCheckingProxies = 0;
            siticoneButton3.Enabled = true;
            siticoneButton3.Text = "Remove dead proxies";
            tokensChanged = true;
        }
        catch
        {

        }
    }

    private void siticoneButton3_Click(object sender, EventArgs e)
    {
        // Homepage - Remove dead proxies
        try
        {
            invalidProxies.Clear();
            doneCheckingProxies = 0;
            siticoneButton3.Enabled = false;
            siticoneButton4.Enabled = false;
            siticoneButton3.Text = "Removing dead proxies...";
            Thread thread = new Thread(CheckProxies);
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }
        catch
        {

        }
    }

    public void CheckProxies()
    {
        try
        {
            foreach (string proxy in proxies)
            {
                try
                {
                    if (!siticoneButton3.Enabled)
                    {
                        Thread.Sleep(1);

                        Thread thread = new Thread(() => CheckProxy(proxy));
                        thread.Priority = ThreadPriority.Highest;
                        thread.Start();
                    }
                }
                catch
                {

                }
            }

            try
            {
                while (doneCheckingProxies != proxies.Count)
                {

                }
            }
            catch
            {

            }

            metroLabel15.Text = (proxies.Count).ToString();

            try
            {
                foreach (string proxy in invalidProxies)
                {
                    try
                    {
                        foreach (string anotherProxy in proxies)
                        {
                            try
                            {
                                if (proxy == anotherProxy)
                                {
                                    proxies.Remove(proxy);
                                    metroLabel15.Text = (proxies.Count).ToString();

                                    break;
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

            invalidProxies.Clear();
            doneCheckingProxies = 0;
            siticoneButton3.Enabled = true;
            siticoneButton3.Text = "Remove dead proxies";
            metroLabel15.Text = proxies.Count.ToString();
            siticoneButton4.Enabled = true;
            tokensChanged = true;
        }
        catch
        {

        }
    }

    public void CheckProxy(string proxy)
    {
        try
        {
            if (!siticoneButton3.Enabled)
            {
                if (!Utils.IsProxyValid(proxy))
                {
                    invalidProxies.Add(proxy);
                    metroLabel15.Text = (proxies.Count - invalidProxies.Count).ToString();
                }

                doneCheckingProxies++;
            }
        }
        catch
        {

        }
    }

    private void siticoneButton6_Click(object sender, EventArgs e)
    {
        // Homepage - Load Proxies
        try
        {
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    foreach (string fileName in openFileDialog2.FileNames)
                    {
                        try
                        {
                            LoadProxies(fileName);
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

        tokensChanged = true;
    }

    public void LoadProxies(string fileName)
    {
        try
        {
            foreach (string proxy in Utils.SplitToLines(System.IO.File.ReadAllText(fileName)))
            {
                try
                {
                    string prxy = Utils.GetCleanToken(proxy);
                    bool inserted = false;

                    if (!Utils.IsProxyFormatValid(prxy))
                    {
                        continue;
                    }

                    try
                    {
                        foreach (string theProxy in proxies)
                        {
                            try
                            {
                                if (theProxy == prxy)
                                {
                                    inserted = true;
                                    break;
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

                    if (!inserted)
                    {
                        proxies.Add(prxy);
                    }
                }
                catch
                {

                }
            }

            metroLabel15.Text = proxies.Count.ToString();
        }
        catch
        {

        }
    }

    public void LoadProxiesFromClipboard()
    {
        try
        {
            foreach (string proxy in Utils.SplitToLines(Clipboard.GetText()))
            {
                try
                {
                    string prxy = Utils.GetCleanToken(proxy);
                    bool inserted = false;

                    if (!Utils.IsProxyFormatValid(prxy))
                    {
                        continue;
                    }

                    try
                    {
                        foreach (string theProxy in proxies)
                        {
                            try
                            {
                                if (theProxy == prxy)
                                {
                                    inserted = true;
                                    break;
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

                    if (!inserted)
                    {
                        proxies.Add(prxy);
                    }
                }
                catch
                {

                }
            }

            metroLabel15.Text = proxies.Count.ToString();
        }
        catch
        {

        }
    }

    private void gunaButton1_Click(object sender, EventArgs e)
    {
        guildManagerStatus = 0;

        // Guild Manager - Join guild
        try
        {
            if (siticoneCheckBox3.Checked)
            {
                if (!Utils.IsCaptchaKeyValid(gunaLineTextBox20.Text))
                {
                    MessageBox.Show("The 2Captcha key is not valid! Go to insert a new valid one on the section 'Settings and Utils'.", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            Thread thread = new Thread(() => JoinGuild(Utils.GetInviteCodeByInviteLink(gunaLineTextBox1.Text), gunaLineTextBox2.Text, gunaLineTextBox3.Text, siticoneCheckBox1.Checked, siticoneCheckBox2.Checked, siticoneCheckBox3.Checked, siticoneCheckBox4.Checked));
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }
        catch
        {

        }
    }

    public void JoinGuild(string theInvite, string captchaBotID, string captchaBotChannelID, bool communityRules, bool reactionVerification, bool captchaBot, bool groupMode)
    {
        try
        {
            DiscordInvite invite = Utils.GetInviteInformations(theInvite, groupMode);

            if (!invite.valid)
            {
                MessageBox.Show("The inserted invite link / code is not valid! Check if the invite is valid and check if you got rate limited from the Discord API.", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (siticoneCheckBox33.Checked)
                {
                    gunaButton1.Enabled = false;
                    gunaButton2.Enabled = true;
                }

                string contextProperties = "";

                if (!groupMode)
                {
                    contextProperties = Utils.GetXCP(invite);
                }
                else
                {
                    contextProperties = Utils.GetGroupXCP(invite);
                }

                if (clients[0].IsJoinPatched(invite.inviteCode, contextProperties, null))
                {
                    MessageBox.Show("This feature is patched by Discord. We are very sorry about that.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                foreach (DiscordClient client in this.GetClients())
                {
                    try
                    {
                        Thread.Sleep(1);

                        if (siticoneOSToggleSwith1.Checked)
                        {
                            Thread.Sleep(bunifuHSlider1.Value);
                        }

                        Thread thread = new Thread(() => ClientJoin(client, invite, contextProperties, captchaBotID, captchaBotChannelID, communityRules, reactionVerification, captchaBot, groupMode, gunaLineTextBox20.Text));
                        thread.Priority = ThreadPriority.Highest;
                        thread.Start();
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

    public void ClientJoin(DiscordClient client, DiscordInvite invite, string contextProperties, string captchaBotID, string captchaBotChannelID, bool communityRules, bool reactionVerification, bool captchaBot, bool groupMode, string captchaKey)
    {
        if (siticoneCheckBox33.Checked)
        {
            Thread thread = new Thread(() => RaidClientJoin(client, invite, contextProperties, captchaBotID, captchaBotChannelID, communityRules, reactionVerification, captchaBot, groupMode, captchaKey, GetProxy()));
            thread.Priority = ThreadPriority.Highest;
            thread.Start();

            Thread thread1 = new Thread(() => RaidClientLeave(client, invite, groupMode, GetProxy()));
            thread1.Priority = ThreadPriority.Highest;
            thread1.Start();
        }
        else
        {
            client.JoinGuild(invite, contextProperties, captchaBotID, captchaBotChannelID, communityRules, reactionVerification, captchaBot, groupMode, captchaKey, GetProxy());
            Interlocked.Increment(ref guildManagerStatus);
        }
    }

    public void RaidClientJoin(DiscordClient client, DiscordInvite invite, string contextProperties, string captchaBotID, string captchaBotChannelID, bool communityRules, bool reactionVerification, bool captchaBot, bool groupMode, string captchaKey, DiscordProxy proxy)
    {
        while (siticoneCheckBox33.Checked && gunaButton2.Enabled)
        {
            Thread.Sleep(1);
            client.JoinGuild(invite, contextProperties, captchaBotID, captchaBotChannelID, communityRules, reactionVerification, captchaBot, groupMode, captchaKey, proxy);
            Interlocked.Increment(ref guildManagerStatus);
        }
    }

    public void RaidClientLeave(DiscordClient client, DiscordInvite invite, bool groupMode, DiscordProxy proxy)
    {
        while (siticoneCheckBox33.Checked && gunaButton2.Enabled)
        {
            Thread.Sleep(1);
            client.LeaveGuild(invite, groupMode, proxy);
            Interlocked.Increment(ref guildManagerStatus);
        }
    }


    private void gunaButton2_Click(object sender, EventArgs e)
    {
        guildManagerStatus = 0;
        // Guild Manager - Leave guild

        try
        {
            if (siticoneCheckBox33.Checked)
            {
                gunaButton2.Enabled = false;
                gunaButton1.Enabled = true;
                return;
            }

            Thread thread = new Thread(() => LeaveGuild(Utils.GetInviteCodeByInviteLink(gunaLineTextBox1.Text), siticoneCheckBox4.Checked));
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }
        catch
        {

        }
    }

    public void LeaveGuild(string guildInviteID, bool groupMode)
    {
        try
        {
            if (!Utils.IsIDValid(guildInviteID))
            {
                DiscordInvite invite = Utils.GetInviteInformations(guildInviteID, groupMode);

                if (!invite.valid)
                {
                    MessageBox.Show("The inserted invite link / code / ID is not valid! Check if the invite is valid and check if you got rate limited from the Discord API, try also to use the right guild ID.", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    foreach (DiscordClient client in this.GetClients())
                    {
                        try
                        {
                            Thread.Sleep(1);

                            if (siticoneOSToggleSwith1.Checked)
                            {
                                Thread.Sleep(bunifuHSlider1.Value);
                            }

                            Thread thread = new Thread(() =>
                            {
                                client.LeaveGuild(invite, groupMode, GetProxy());
                                Interlocked.Increment(ref guildManagerStatus);
                            });

                            thread.Priority = ThreadPriority.Highest;
                            thread.Start();
                        }
                        catch
                        {

                        }
                    }
                }
            }
            else
            {
                foreach (DiscordClient client in this.GetClients())
                {
                    try
                    {
                        Thread.Sleep(1);

                        if (siticoneOSToggleSwith1.Checked)
                        {
                            Thread.Sleep(bunifuHSlider1.Value);
                        }

                        Thread thread = new Thread(() =>
                        {
                            client.LeaveGuild(guildInviteID, groupMode, GetProxy());
                            Interlocked.Increment(ref guildManagerStatus);
                        });

                        thread.Priority = ThreadPriority.Highest;
                        thread.Start();
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

    private void gunaButton4_Click(object sender, EventArgs e)
    {
        serverSpammerStatus = 0;
        // Server Spammer - Start Spamming
        try
        {
            rolesTagAllIndex = 0;
            tagAllIndex = 0;
            gunaButton4.Enabled = false;
            gunaButton3.Enabled = true;

            if (siticoneCheckBox8.Checked)
            {
                if (!Utils.AreIDsValid(gunaLineTextBox4.Text))
                {
                    MessageBox.Show("The IDs of the channels are not valid!", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    gunaButton3.Enabled = false;
                    gunaButton4.Enabled = true;

                    return;
                }
            }
            else
            {
                if (!Utils.IsIDValid(gunaLineTextBox4.Text))
                {
                    MessageBox.Show("The ID of the channel is not valid!", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    gunaButton3.Enabled = false;
                    gunaButton4.Enabled = true;

                    return;
                }
            }

            Thread thread = new Thread(DoServerSpammer);
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }
        catch
        {

        }
    }

    private void gunaButton3_Click(object sender, EventArgs e)
    {
        // Server Spammer - Stop Spamming
        try
        {
            serverSpammer = false;
            new Thread(reEnableServerSpammer).Start();
        }
        catch
        {

        }
    }

    public void reEnableServerSpammer()
    {
        try
        {
            if (siticoneOSToggleSwith2.Checked)
            {
                Thread.Sleep(bunifuHSlider2.Value);
            }

            rolesTagAllIndex = 0;
            tagAllIndex = 0;
            gunaButton3.Enabled = false;
            gunaButton4.Enabled = true;
        }
        catch
        {

        }
    }

    public void DoServerSpammer()
    {
        try
        {
            serverSpammer = true;
            List<string> ids = new List<string>();

            if (siticoneCheckBox8.Checked)
            {
                ids = Utils.GetIDs(gunaLineTextBox4.Text);
            }

            for (int i = 0; i < siticoneComboBox6.SelectedIndex + 1; i++)
            {
                foreach (DiscordClient client in this.GetClients())
                {
                    Thread.Sleep(1);

                    try
                    {
                        try
                        {
                            if (siticoneCheckBox8.Checked)
                            {
                                if (siticoneComboBox5.SelectedIndex == 0)
                                {
                                    foreach (string id in ids)
                                    {
                                        Thread.Sleep(1);

                                        try
                                        {
                                            Thread.Sleep(1);

                                            if (siticoneOSToggleSwith2.Checked)
                                            {
                                                Thread.Sleep(bunifuHSlider2.Value);
                                            }

                                            Thread thread = new Thread(() => SpamServer(client, id));
                                            thread.Priority = ThreadPriority.Highest;
                                            thread.Start();
                                        }
                                        catch
                                        {

                                        }
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        Thread.Sleep(1);

                                        if (siticoneOSToggleSwith2.Checked)
                                        {
                                            Thread.Sleep(bunifuHSlider2.Value);
                                        }

                                        Thread thread = new Thread(() => SwitchSpamServer(client, ids));
                                        thread.Priority = ThreadPriority.Highest;
                                        thread.Start();
                                    }
                                    catch
                                    {

                                    }

                                }
                            }
                            else
                            {
                                try
                                {
                                    Thread.Sleep(1);

                                    if (siticoneOSToggleSwith2.Checked)
                                    {
                                        Thread.Sleep(bunifuHSlider2.Value);
                                    }

                                    Thread thread = new Thread(() => SpamServer(client, gunaLineTextBox4.Text));
                                    thread.Priority = ThreadPriority.Highest;
                                    thread.Start();
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
        }
        catch
        {

        }
    }

    public string getServerSpamMessage()
    {
        if (siticoneCheckBox41.Checked)
        {
            return Utils.GetBypass2000();
        }

        string msg = "";

        try
        {
            try
            {
                if (!siticoneCheckBox9.Checked)
                {
                    List<string> lines = new List<string>();

                    foreach (string line in Utils.SplitToLines(gunaTextBox1.Text))
                    {
                        lines.Add(line);
                    }

                    if (lines.Count != 1)
                    {
                        foreach (string line in lines)
                        {
                            msg = msg + " \\u000d" + line;
                        }
                    }
                    else
                    {
                        msg = gunaTextBox1.Text;
                    }
                }
                else
                {
                    if (multipleMessageIndex < 0)
                    {
                        multipleMessageIndex = 0;
                    }

                    int count = 0;

                    foreach (char c in gunaTextBox1.Text.ToCharArray())
                    {
                        if (c == '|')
                        {
                            count++;
                        }
                    }

                    if (multipleMessageIndex > count)
                    {
                        multipleMessageIndex = 0;
                    }

                    if (count == 0)
                    {
                        List<string> lines = new List<string>();

                        foreach (string line in Utils.SplitToLines(gunaTextBox1.Text))
                        {
                            lines.Add(line);
                        }

                        if (lines.Count != 1)
                        {
                            foreach (string line in lines)
                            {
                                msg = msg + " \\u000d" + line;
                            }
                        }
                        else
                        {
                            msg = gunaTextBox1.Text;
                        }

                        multipleMessageIndex++;
                    }
                    else if (count == 1 && Microsoft.VisualBasic.Strings.Split(gunaTextBox1.Text, "|")[1].Replace(" ", "").Replace('\t'.ToString(), "").Trim() == "")
                    {
                        string[] splitted = Microsoft.VisualBasic.Strings.Split(gunaTextBox1.Text, "|");
                        string definitive = splitted[0];
                        List<string> lines = new List<string>();

                        foreach (string line in Utils.SplitToLines(definitive))
                        {
                            lines.Add(line);
                        }

                        if (lines.Count != 1)
                        {
                            foreach (string line in lines)
                            {
                                msg = msg + " \\u000d" + line;
                            }
                        }
                        else
                        {
                            msg = definitive;
                        }

                        multipleMessageIndex++;
                    }
                    else
                    {
                        string[] splitted = Microsoft.VisualBasic.Strings.Split(gunaTextBox1.Text, "|");
                        string definitive = splitted[multipleMessageIndex];
                        List<string> lines = new List<string>();

                        foreach (string line in Utils.SplitToLines(definitive))
                        {
                            lines.Add(line);
                        }

                        if (lines.Count != 1)
                        {
                            foreach (string line in lines)
                            {
                                msg = msg + " \\u000d" + line;
                            }
                        }
                        else
                        {
                            msg = definitive;
                        }

                        if (multipleMessageIndex == count)
                        {
                            multipleMessageIndex = 0;
                        }
                        else
                        {
                            multipleMessageIndex++;
                        }
                    }
                }
            }
            catch
            {

            }

            if (Utils.users.Count == 0)
            {
                msg = msg.Replace("[mtag]", "");
                msg = msg.Replace("[all]", "");
            }

            if (Utils.roles.Count == 0)
            {
                msg = msg.Replace("[rtag]", "");
                msg = msg.Replace("[rall]", "");
            }

            if (siticoneCheckBox7.Checked)
            {
                try
                {
                    while (msg.Contains("[mtag]"))
                    {
                        try
                        {
                            string tag = getTagAllUser();

                            if (tag != "")
                            {
                                msg = Utils.ReplaceFirst(msg, "[mtag]", " <@" + tag + "> ");
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

                try
                {
                    string allUsers = "";

                    foreach (string user in Utils.users)
                    {
                        allUsers += "<@" + user + "> ";
                    }

                    allUsers = allUsers.Substring(0, allUsers.Length - 1);
                    msg = msg.Replace("[all]", allUsers);
                }
                catch
                {

                }
            }
            else
            {
                msg = msg.Replace(" [mtag] ", "");
                msg = msg.Replace(" [mtag]", "");
                msg = msg.Replace("[mtag]", "");

                msg = msg.Replace(" [all] ", "");
                msg = msg.Replace(" [all]", "");
                msg = msg.Replace("[all]", "");
            }

            if (siticoneCheckBox29.Checked)
            {
                try
                {
                    while (msg.Contains("[rtag]"))
                    {
                        try
                        {
                            string tag = getTagAllRole();

                            if (tag != "")
                            {
                                msg = Utils.ReplaceFirst(msg, "[rtag]", " <@&" + tag + "> ");
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

                try
                {
                    string allRoles = "";

                    foreach (string role in Utils.roles)
                    {
                        allRoles += "<@&" + role + "> ";
                    }

                    allRoles = allRoles.Substring(0, allRoles.Length - 1);
                    msg = msg.Replace("[rall]", allRoles);
                }
                catch
                {

                }
            }
            else
            {
                msg = msg.Replace(" [rtag] ", "");
                msg = msg.Replace(" [rtag]", "");
                msg = msg.Replace("[rtag]", "");

                msg = msg.Replace(" [rall] ", "");
                msg = msg.Replace(" [rall]", "");
                msg = msg.Replace("[rall]", "");
            }

            try
            {
                while (msg.Contains("[lag]"))
                {
                    try
                    {
                        msg = Utils.ReplaceFirst(msg, "[lag]", Utils.GetLagMessage());
                    }
                    catch
                    {

                    }
                }
            }
            catch
            {

            }

            try
            {
                while (msg.Contains("[rndnum]"))
                {
                    try
                    {
                        msg = Utils.ReplaceFirst(msg, "[rndnum]", Utils.GetUniqueInt(4).ToString());
                    }
                    catch
                    {

                    }
                }
            }
            catch
            {

            }

            try
            {
                while (msg.Contains("[rndstr]"))
                {
                    try
                    {
                        msg = Utils.ReplaceFirst(msg, "[rndstr]", Utils.GetUniqueKey(16));
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

        return msg;
    }

    public void SpamServer(DiscordClient client, string channelId)
    {
        Thread.Sleep(1);

        while (serverSpammer)
        {
            Thread.Sleep(1);

            try
            {
                if (siticoneOSToggleSwith2.Checked)
                {
                    Thread.Sleep(bunifuHSlider2.Value);
                }

                for (int i = 0; i < (siticoneComboBox10.SelectedIndex + 1); i++)
                {
                    if (client.SendMessage(channelId, getServerSpamMessage(), Utils.IsIDValid(gunaLineTextBox5.Text) ? gunaLineTextBox5.Text : "", GetProxy(), siticoneCheckBox34.Checked))
                    {
                        Interlocked.Increment(ref serverSpammerStatus);
                    }   
                }
            }
            catch
            {

            }
        }
    }

    public void SwitchSpamServer(DiscordClient client, List<string> ids)
    {
        Thread.Sleep(1);

        int channelIndex = 0, messages = 0;

        while (serverSpammer)
        {
            Thread.Sleep(1);

            try
            {
                if (siticoneOSToggleSwith2.Checked)
                {
                    Thread.Sleep(bunifuHSlider2.Value);
                }

                if (client.SendMessage(ids[channelIndex], getServerSpamMessage(), Utils.IsIDValid(gunaLineTextBox5.Text) ? gunaLineTextBox5.Text : "", GetProxy(), siticoneCheckBox34.Checked))
                {
                    Interlocked.Increment(ref serverSpammerStatus);
                }

                messages++;

                if (messages >= 3)
                {
                    if (channelIndex >= ids.Count)
                    {
                        channelIndex = -1;
                    }

                    channelIndex++;

                    if (channelIndex >= ids.Count)
                    {
                        channelIndex = 0;
                    }
                }
            }
            catch
            {

            }
        }
    }

    private void gunaButton5_Click(object sender, EventArgs e)
    {
        dmSpammerStatus = 0;
        // DM Spammer - Start Spamming
        try
        {
            gunaButton5.Enabled = false;
            gunaButton6.Enabled = true;

            if (siticoneCheckBox11.Checked)
            {
                if (!Utils.AreIDsValid(gunaLineTextBox6.Text))
                {
                    MessageBox.Show("The IDs of the users are not valid!", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    gunaButton6.Enabled = false;
                    gunaButton5.Enabled = true;

                    return;
                }
            }
            else
            {
                if (!Utils.IsIDValid(gunaLineTextBox6.Text))
                {
                    MessageBox.Show("The ID of the user is not valid!", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    gunaButton6.Enabled = false;
                    gunaButton5.Enabled = true;

                    return;
                }
            }

            Thread thread = new Thread(DoDMSpammer);
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }
        catch
        {

        }
    }

    private void gunaButton6_Click(object sender, EventArgs e)
    {
        // DM Spammer - Stop Spamming
        try
        {
            dmSpammer = false;
            new Thread(reEnableDMSpammer).Start();
        }
        catch
        {

        }
    }

    public void DoDMSpammer()
    {
        try
        {
            dmSpammer = true;

            foreach (DiscordClient client in this.GetClients())
            {
                Thread.Sleep(1);

                Thread thread = new Thread(() => ProcessClient(client));
                thread.Priority = ThreadPriority.Highest;
                thread.Start();
            }
        }
        catch
        {

        }
    }

    public void ProcessClient(DiscordClient client)
    {
        try
        {
            List<string> ids = new List<string>();

            try
            {
                if (siticoneCheckBox12.Checked)
                {
                    try
                    {
                        foreach (string id in Utils.GetIDs(gunaLineTextBox6.Text))
                        {
                            try
                            {
                                ids.Add(client.GetDMChannel(id, GetProxy()));
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
                else
                {
                    ids.Add(client.GetDMChannel(gunaLineTextBox6.Text, GetProxy()));
                }
            }
            catch
            {

            }

            try
            {
                foreach (string id in ids)
                {
                    try
                    {
                        if (!Utils.IsIDValid(id))
                        {
                            continue;
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

            for (int i = 0; i < siticoneComboBox7.SelectedIndex + 1; i++)
            {
                try
                {
                    foreach (string id in ids)
                    {
                        Thread.Sleep(1);

                        try
                        {
                            if (siticoneOSToggleSwith3.Checked)
                            {
                                Thread.Sleep(bunifuHSlider3.Value);
                            }

                            Thread thread = new Thread(() => SpamDM(client, id));
                            thread.Priority = ThreadPriority.Highest;
                            thread.Start();
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

    public string getDMSpamMessage()
    {
        if (siticoneCheckBox42.Checked)
        {
            return Utils.GetBypass2000();
        }

        string msg = "";

        try
        {
            try
            {
                if (!siticoneCheckBox12.Checked)
                {
                    List<string> lines = new List<string>();

                    foreach (string line in Utils.SplitToLines(gunaTextBox2.Text))
                    {
                        lines.Add(line);
                    }

                    if (lines.Count != 1)
                    {
                        foreach (string line in lines)
                        {
                            msg = msg + " \\u000d" + line;
                        }
                    }
                    else
                    {
                        msg = gunaTextBox2.Text;
                    }
                }
                else
                {
                    if (multipleDmMessageIndex < 0)
                    {
                        multipleDmMessageIndex = 0;
                    }

                    int count = 0;

                    foreach (char c in gunaTextBox2.Text.ToCharArray())
                    {
                        if (c == '|')
                        {
                            count++;
                        }
                    }

                    if (multipleDmMessageIndex > count)
                    {
                        multipleDmMessageIndex = 0;
                    }

                    if (count == 0)
                    {
                        List<string> lines = new List<string>();

                        foreach (string line in Utils.SplitToLines(gunaTextBox2.Text))
                        {
                            lines.Add(line);
                        }

                        if (lines.Count != 1)
                        {
                            foreach (string line in lines)
                            {
                                msg = msg + " \\u000d" + line;
                            }
                        }
                        else
                        {
                            msg = gunaTextBox2.Text;
                        }

                        multipleDmMessageIndex++;
                    }
                    else if (count == 1 && Microsoft.VisualBasic.Strings.Split(gunaTextBox2.Text, "|")[1].Replace(" ", "").Replace('\t'.ToString(), "").Trim() == "")
                    {
                        string[] splitted = Microsoft.VisualBasic.Strings.Split(gunaTextBox2.Text, "|");
                        string definitive = splitted[0];
                        List<string> lines = new List<string>();

                        foreach (string line in Utils.SplitToLines(definitive))
                        {
                            lines.Add(line);
                        }

                        if (lines.Count != 1)
                        {
                            foreach (string line in lines)
                            {
                                msg = msg + " \\u000d" + line;
                            }
                        }
                        else
                        {
                            msg = definitive;
                        }

                        multipleDmMessageIndex++;
                    }
                    else
                    {
                        string[] splitted = Microsoft.VisualBasic.Strings.Split(gunaTextBox2.Text, "|");
                        string definitive = splitted[multipleDmMessageIndex];
                        List<string> lines = new List<string>();

                        foreach (string line in Utils.SplitToLines(definitive))
                        {
                            lines.Add(line);
                        }

                        if (lines.Count != 1)
                        {
                            foreach (string line in lines)
                            {
                                msg = msg + " \\u000d" + line;
                            }
                        }
                        else
                        {
                            msg = definitive;
                        }

                        if (multipleDmMessageIndex == count)
                        {
                            multipleDmMessageIndex = 0;
                        }
                        else
                        {
                            multipleDmMessageIndex++;
                        }
                    }
                }
            }
            catch
            {

            }

            try
            {
                msg = msg.Replace(" [mtag] ", "");
                msg = msg.Replace(" [mtag]", "");
                msg = msg.Replace("[mtag]", "");

                msg = msg.Replace(" [all] ", "");
                msg = msg.Replace(" [all]", "");
                msg = msg.Replace("[all]", "");

                msg = msg.Replace(" [rtag] ", "");
                msg = msg.Replace(" [rtag]", "");
                msg = msg.Replace("[rtag]", "");

                msg = msg.Replace(" [rall] ", "");
                msg = msg.Replace(" [rall]", "");
                msg = msg.Replace("[rall]", "");
            }
            catch
            {

            }

            try
            {
                while (msg.Contains("[lag]"))
                {
                    try
                    {
                        msg = Utils.ReplaceFirst(msg, "[lag]", Utils.GetLagMessage());
                    }
                    catch
                    {

                    }
                }
            }
            catch
            {

            }

            try
            {
                while (msg.Contains("[rndnum]"))
                {
                    try
                    {
                        msg = Utils.ReplaceFirst(msg, "[rndnum]", Utils.GetUniqueInt(4).ToString());
                    }
                    catch
                    {

                    }
                }
            }
            catch
            {

            }

            try
            {
                while (msg.Contains("[rndstr]"))
                {
                    try
                    {
                        msg = Utils.ReplaceFirst(msg, "[rndstr]", Utils.GetUniqueKey(16));
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

        return msg;
    }

    public void reEnableDMSpammer()
    {
        try
        {
            if (siticoneOSToggleSwith3.Checked)
            {
                Thread.Sleep(bunifuHSlider3.Value);
            }

            gunaButton6.Enabled = false;
            gunaButton5.Enabled = true;
        }
        catch
        {

        }
    }

    public void SpamDM(DiscordClient client, string channelId)
    {
        Thread.Sleep(1);

        try
        {
            while (dmSpammer)
            {
                try
                {
                    Thread.Sleep(1);

                    if (siticoneOSToggleSwith3.Checked)
                    {
                        Thread.Sleep(bunifuHSlider3.Value);
                    }

                    for (int i = 0; i < (siticoneComboBox11.SelectedIndex + 1); i++)
                    {
                        if (client.SendMessage(channelId, getDMSpamMessage(), Utils.IsIDValid(gunaLineTextBox7.Text) ? gunaLineTextBox7.Text : "", GetProxy()))
                        {
                            Interlocked.Increment(ref dmSpammerStatus);
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

    private void gunaButton7_Click(object sender, EventArgs e)
    {
        reactionSpammerStatus = 0;
        // Reaction Spammer - Add reaction

        try
        {
            if (siticoneRadioButton1.Checked)
            {
                if (!Utils.IsEmojiValid(gunaLineTextBox8.Text))
                {
                    MessageBox.Show("The emoji is not valid! Please, insert a new valid one.", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                if (!Utils.IsEmoteValid(gunaLineTextBox8.Text))
                {
                    MessageBox.Show("The emote is not valid! Please, insert a new valid one.", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (!Utils.IsIDValid(gunaLineTextBox9.Text))
            {
                MessageBox.Show("The ID of the channel is not valid! Please, insert a new valid one.", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Utils.IsIDValid(gunaLineTextBox10.Text))
            {
                MessageBox.Show("The ID of the message is not valid! Please, insert a new valid one.", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Thread thread = new Thread(() => DoReactionAdder(gunaLineTextBox8.Text, gunaLineTextBox9.Text, gunaLineTextBox10.Text));
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }
        catch
        {

        }
    }

    private void gunaButton8_Click(object sender, EventArgs e)
    {
        reactionSpammerStatus = 0;
        // Reaction Spammer - Remove reaction

        try
        {
            if (siticoneRadioButton1.Checked)
            {
                if (!Utils.IsEmojiValid(gunaLineTextBox8.Text))
                {
                    MessageBox.Show("The emoji is not valid! Please, insert a new valid one.", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                if (!Utils.IsEmoteValid(gunaLineTextBox8.Text))
                {
                    MessageBox.Show("The emote is not valid! Please, insert a new valid one.", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (!Utils.IsIDValid(gunaLineTextBox9.Text))
            {
                MessageBox.Show("The ID of the channel is not valid! Please, insert a new valid one.", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Utils.IsIDValid(gunaLineTextBox10.Text))
            {
                MessageBox.Show("The ID of the message is not valid! Please, insert a new valid one.", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Thread thread = new Thread(() => DoReactionRemover(gunaLineTextBox8.Text, gunaLineTextBox9.Text, gunaLineTextBox10.Text));
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }
        catch
        {

        }
    }

    public void DoReactionAdder(string reaction, string channelId, string messageId)
    {
        try
        {
            foreach (DiscordClient client in this.GetClients())
            {
                try
                {
                    Thread.Sleep(1);

                    if (siticoneOSToggleSwith4.Checked)
                    {
                        Thread.Sleep(bunifuHSlider4.Value);
                    }

                    Thread thread = new Thread(() => AddReaction(client, reaction, channelId, messageId));
                    thread.Priority = ThreadPriority.Highest;
                    thread.Start();
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

    public void DoReactionRemover(string reaction, string channelId, string messageId)
    {
        try
        {
            foreach (DiscordClient client in this.GetClients())
            {
                try
                {
                    Thread.Sleep(1);

                    if (siticoneOSToggleSwith4.Checked)
                    {
                        Thread.Sleep(bunifuHSlider4.Value);
                    }

                    Thread thread = new Thread(() => RemoveReaction(client, reaction, channelId, messageId));
                    thread.Priority = ThreadPriority.Highest;
                    thread.Start();
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

    public void AddReaction(DiscordClient client, string reaction, string channelId, string messageId)
    {
        try
        {
            client.AddReaction(reaction, channelId, messageId, GetProxy());
            Interlocked.Increment(ref reactionSpammerStatus);
        }
        catch
        {

        }
    }

    public void RemoveReaction(DiscordClient client, string reaction, string channelId, string messageId)
    {
        try
        {
            client.RemoveReaction(reaction, channelId, messageId, GetProxy());
            Interlocked.Increment(ref reactionSpammerStatus);
        }
        catch
        {

        }
    }

    private void gunaButton10_Click(object sender, EventArgs e)
    {
        friendSpammerStatus = 0;
        // Friend Spammer - Add friend

        try
        {
            if (siticoneCheckBox35.Checked)
            {
                if (siticoneCheckBox5.Checked)
                {
                    if (!Utils.AreIDsValid(gunaLineTextBox11.Text))
                    {
                        MessageBox.Show("The IDs of the users are not valid! Please, insert new valids.", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    if (!Utils.IsIDValid(gunaLineTextBox11.Text))
                    {
                        MessageBox.Show("The ID of the user is not valid! Please, insert a new valid one.", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                gunaButton10.Enabled = false;
                gunaButton9.Enabled = true;
            }
            else
            {
                if (siticoneCheckBox5.Checked)
                {
                    if (!Utils.AreFriendsValid(gunaLineTextBox11.Text))
                    {
                        MessageBox.Show("The IDs and / or tags of the users are not valid! Please, insert new valids.", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    if (!Utils.IsFriendValid(gunaLineTextBox11.Text))
                    {
                        MessageBox.Show("The ID / tag of the user is not valid! Please, insert a new valid one.", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }

            Thread thread = new Thread(() => DoFriendAdder(gunaLineTextBox11.Text, siticoneCheckBox5.Checked));
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }
        catch
        {

        }
    }

    private void gunaButton9_Click(object sender, EventArgs e)
    {
        friendSpammerStatus = 0;
        // Friend Spammer - Remove friend

        try
        {
            if (siticoneCheckBox5.Checked)
            {
                if (!Utils.AreIDsValid(gunaLineTextBox11.Text))
                {
                    MessageBox.Show("The IDs of the users are not valid! Please, insert new valids.", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                if (!Utils.IsIDValid(gunaLineTextBox11.Text))
                {
                    MessageBox.Show("The ID of the user is not valid! Please, insert a new valid one.", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (siticoneCheckBox35.Checked)
            {
                gunaButton9.Enabled = false;
                gunaButton10.Enabled = true;
            }

            Thread thread = new Thread(() => DoFriendRemover(gunaLineTextBox11.Text, siticoneCheckBox5.Checked));

            thread.Start();
        }
        catch
        {

        }
    }

    public void DoFriendAdder(string str, bool multiple)
    {
        try
        {
            foreach (DiscordClient client in this.GetClients())
            {
                Thread.Sleep(1);

                try
                {
                    if (multiple)
                    {
                        try
                        {
                            foreach (string user in Utils.GetFriends(str))
                            {
                                try
                                {
                                    Thread.Sleep(1);

                                    if (siticoneOSToggleSwith5.Checked)
                                    {
                                        Thread.Sleep(bunifuHSlider5.Value);
                                    }

                                    Thread thread = new Thread(() => AddFriend(client, user));
                                    thread.Priority = ThreadPriority.Highest;
                                    thread.Start();
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
                    else
                    {
                        try
                        {
                            if (siticoneOSToggleSwith5.Checked)
                            {
                                Thread.Sleep(bunifuHSlider5.Value);
                            }

                            Thread thread = new Thread(() => AddFriend(client, str));
                            thread.Priority = ThreadPriority.Highest;
                            thread.Start();
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

    public void DoFriendRemover(string str, bool multiple)
    {
        try
        {
            foreach (DiscordClient client in this.GetClients())
            {
                Thread.Sleep(1);

                try
                {
                    if (multiple)
                    {
                        try
                        {
                            foreach (string user in Utils.GetIDs(str))
                            {
                                Thread.Sleep(1);

                                if (siticoneOSToggleSwith5.Checked)
                                {
                                    Thread.Sleep(bunifuHSlider5.Value);
                                }

                                try
                                {
                                    Thread thread = new Thread(() => RemoveFriend(client, user));
                                    thread.Priority = ThreadPriority.Highest;
                                    thread.Start();
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
                    else
                    {
                        try
                        {
                            if (siticoneOSToggleSwith5.Checked)
                            {
                                Thread.Sleep(bunifuHSlider5.Value);
                            }

                            Thread thread = new Thread(() => RemoveFriend(client, str));
                            thread.Priority = ThreadPriority.Highest;
                            thread.Start();
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

    public void AddFriend(DiscordClient client, string friend)
    {
        try
        {
            client.AddFriend(friend, GetProxy());
            Interlocked.Increment(ref friendSpammerStatus);

            if (siticoneCheckBox35.Checked && gunaButton9.Enabled)
            {
                if (siticoneOSToggleSwith5.Checked)
                {
                    Thread.Sleep(bunifuHSlider5.Value);
                }

                RemoveFriend(client, friend);
            }
        }
        catch
        {

        }
    }

    public void RemoveFriend(DiscordClient client, string userId)
    {
        try
        {
            client.RemoveFriend(userId, GetProxy());
            Interlocked.Increment(ref friendSpammerStatus);

            if (siticoneCheckBox35.Checked && gunaButton9.Enabled)
            {
                if (siticoneOSToggleSwith5.Checked)
                {
                    Thread.Sleep(bunifuHSlider5.Value);
                }

                AddFriend(client, userId);
            }
        }
        catch
        {

        }
    }

    private void gunaButton11_Click(object sender, EventArgs e)
    {
        // Typing Spammer - Start Spamming
        try
        {
            gunaButton11.Enabled = false;
            gunaButton12.Enabled = true;

            if (siticoneCheckBox13.Checked)
            {
                if (!Utils.AreIDsValid(gunaLineTextBox12.Text))
                {
                    MessageBox.Show("The IDs of the channels are not valid!", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    gunaButton12.Enabled = false;
                    gunaButton11.Enabled = true;

                    return;
                }
            }
            else
            {
                if (!Utils.IsIDValid(gunaLineTextBox12.Text))
                {
                    MessageBox.Show("The ID of the channel is not valid!", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    gunaButton12.Enabled = false;
                    gunaButton11.Enabled = true;

                    return;
                }
            }

            typingSpammer = true;
            Thread thread = new Thread(DoTypingSpammer);
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }
        catch
        {

        }
    }

    private void gunaButton12_Click(object sender, EventArgs e)
    {
        // Typing Spammer - Stop Spamming
        try
        {
            typingSpammer = false;
            gunaButton12.Enabled = false;
            gunaButton11.Enabled = true;
        }
        catch
        {

        }
    }

    public void DoTypingSpammer()
    {
        try
        {
            foreach (DiscordClient client in this.GetClients())
            {
                try
                {
                    Thread.Sleep(1);

                    if (siticoneOSToggleSwith6.Checked)
                    {
                        Thread.Sleep(bunifuHSlider6.Value);
                    }

                    if (siticoneCheckBox13.Checked)
                    {
                        try
                        {
                            foreach (string id in Utils.GetIDs(gunaLineTextBox12.Text))
                            {
                                try
                                {
                                    if (siticoneOSToggleSwith6.Checked)
                                    {
                                        Thread.Sleep(bunifuHSlider6.Value);
                                    }

                                    Thread thread = new Thread(() => TypingSpam(client, id));
                                    thread.Priority = ThreadPriority.Highest;
                                    thread.Start();
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
                    else
                    {
                        try
                        {
                            Thread thread = new Thread(() => TypingSpam(client, gunaLineTextBox12.Text));
                            thread.Priority = ThreadPriority.Highest;
                            thread.Start();
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

    public void TypingSpam(DiscordClient client, string channelId)
    {
        try
        {
            while (typingSpammer)
            {
                try
                {
                    Thread.Sleep(1);

                    client.TypeInChannel(channelId, GetProxy());
                    Thread.Sleep(8000);
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

    private void gunaButton15_Click(object sender, EventArgs e)
    {
        voiceSpammerStatus = 0;
        // Voice Spammer - Join voice
        try
        {
            if (!Utils.IsIDValid(gunaLineTextBox14.Text))
            {
                MessageBox.Show("The ID of the guild is not valid! Please, insert a new valid one.", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Utils.IsIDValid(gunaLineTextBox15.Text))
            {
                MessageBox.Show("The ID of the channel is not valid! Please, insert a new valid one.", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Thread thread = new Thread(DoJoinVoice);
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }
        catch
        {

        }
    }

    private void gunaButton16_Click(object sender, EventArgs e)
    {
        voiceSpammerStatus = 0;
        // Voice Spammer - Leave voice
        try
        {
            Thread thread = new Thread(DoLeaveVoice);
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }
        catch
        {

        }
    }

    public void DoJoinVoice()
    {
        try
        {
            foreach (DiscordClient client in this.GetClients())
            {
                try
                {
                    Thread.Sleep(1);

                    if (siticoneOSToggleSwith9.Checked)
                    {
                        Thread.Sleep(bunifuHSlider9.Value);
                    }

                    Thread thread = new Thread(() => JoinVoice(client));
                    thread.Priority = ThreadPriority.Highest;
                    thread.Start();
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

    public void DoLeaveVoice()
    {
        try
        {
            foreach (DiscordClient client in this.GetClients())
            {
                try
                {
                    Thread.Sleep(1);

                    if (siticoneOSToggleSwith9.Checked)
                    {
                        Thread.Sleep(bunifuHSlider9.Value);
                    }

                    Thread thread = new Thread(() => LeaveVoice(client));
                    thread.Priority = ThreadPriority.Highest;
                    thread.Start();
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

    public void JoinVoice(DiscordClient client)
    {
        // Join voice there.

        try
        {
            client.JoinVoice(gunaLineTextBox14.Text, gunaLineTextBox15.Text, gunaLineTextBox16.Text, siticoneCheckBox14.Checked, siticoneCheckBox15.Checked, siticoneCheckBox16.Checked, siticoneCheckBox17.Checked, siticoneCheckBox18.Checked, siticoneCheckBox20.Checked, GetProxy());
            Interlocked.Increment(ref voiceSpammerStatus);

            if (siticoneCheckBox21.Checked)
            {
                if (siticoneOSToggleSwith10.Checked)
                {
                    Thread.Sleep(bunifuHSlider10.Value);
                }

                LeaveVoice(client);
            }
        }
        catch
        {

        }
    }

    public void LeaveVoice(DiscordClient client)
    {
        // Leave voice there.

        try
        {
            client.LeaveVoice();
            Interlocked.Increment(ref voiceSpammerStatus);
        }
        catch
        {

        }
    }

    private void gunaButton17_Click(object sender, EventArgs e)
    {
        webhookSpammerStatus = 0;
        // Webhook Spammer - Start Spamming
        try
        {
            gunaButton17.Enabled = false;
            webhookSpammer = true;
            Thread thread = new Thread(DoWebhookSpammer);
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }
        catch
        {

        }
    }

    public void DoWebhookSpammer()
    {
        try
        {
            if (siticoneCheckBox22.Checked)
            {
                try
                {
                    foreach (string webhook in Utils.GetIDs(gunaLineTextBox17.Text))
                    {
                        Thread.Sleep(1);

                        try
                        {
                            if (!Utils.IsWebhookValid(webhook))
                            {
                                MessageBox.Show("The webhooks are not valid ('" + webhook + "'). Please, remove the invalid webhooks and try again.", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                gunaButton17.Enabled = true;
                                return;
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

                gunaButton18.Enabled = true;

                try
                {
                    try
                    {
                        Thread.Sleep(1);

                        for (int i = 0; i < siticoneComboBox1.SelectedIndex + 1; i++)
                        {
                            Thread.Sleep(1);

                            foreach (string webhook in Utils.GetIDs(gunaLineTextBox17.Text))
                            {
                                Thread.Sleep(1);

                                try
                                {
                                    Thread thread = new Thread(() => SpamWebhook(webhook));
                                    thread.Priority = ThreadPriority.Highest;
                                    thread.Start();
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
                catch
                {

                }
            }
            else
            {
                try
                {
                    if (!Utils.IsWebhookValid(gunaLineTextBox17.Text))
                    {
                        MessageBox.Show("The webhook is not valid! Please, insert a new valid one.", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        gunaButton17.Enabled = true;
                        return;
                    }

                    gunaButton18.Enabled = true;

                    try
                    {
                        for (int i = 0; i < siticoneComboBox8.SelectedIndex + 1; i++)
                        {
                            try
                            {
                                Thread thread = new Thread(() => SpamWebhook(gunaLineTextBox17.Text));
                                thread.Priority = ThreadPriority.Highest;
                                thread.Start();
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

    private void gunaButton18_Click(object sender, EventArgs e)
    {
        // Webhook Spammer - Stop Spamming
        try
        {
            webhookSpammer = false;
            gunaButton18.Enabled = false;
            gunaButton17.Enabled = true;
        }
        catch
        {

        }
    }

    public string GetWebhookSpammerMessage()
    {
        if (siticoneCheckBox44.Checked)
        {
            return Utils.GetBypass2000();
        }

        string msg = "";

        try
        {
            try
            {
                if (!siticoneCheckBox27.Checked)
                {
                    List<string> lines = new List<string>();

                    foreach (string line in Utils.SplitToLines(gunaTextBox3.Text))
                    {
                        lines.Add(line);
                    }

                    if (lines.Count != 1)
                    {
                        foreach (string line in lines)
                        {
                            msg = msg + " \\u000d" + line;
                        }
                    }
                    else
                    {
                        msg = gunaTextBox3.Text;
                    }
                }
                else
                {
                    if (multipleWebhookMessageIndex < 0)
                    {
                        multipleWebhookMessageIndex = 0;
                    }

                    int count = 0;

                    foreach (char c in gunaTextBox3.Text.ToCharArray())
                    {
                        if (c == '|')
                        {
                            count++;
                        }
                    }

                    if (multipleWebhookMessageIndex > count)
                    {
                        multipleWebhookMessageIndex = 0;
                    }

                    if (count == 0)
                    {
                        List<string> lines = new List<string>();

                        foreach (string line in Utils.SplitToLines(gunaTextBox3.Text))
                        {
                            lines.Add(line);
                        }

                        if (lines.Count != 1)
                        {
                            foreach (string line in lines)
                            {
                                msg = msg + " \\u000d" + line;
                            }
                        }
                        else
                        {
                            msg = gunaTextBox3.Text;
                        }

                        multipleWebhookMessageIndex++;
                    }
                    else if (count == 1 && Microsoft.VisualBasic.Strings.Split(gunaTextBox3.Text, "|")[1].Replace(" ", "").Replace('\t'.ToString(), "").Trim() == "")
                    {
                        string[] splitted = Microsoft.VisualBasic.Strings.Split(gunaTextBox3.Text, "|");
                        string definitive = splitted[0];
                        List<string> lines = new List<string>();

                        foreach (string line in Utils.SplitToLines(definitive))
                        {
                            lines.Add(line);
                        }

                        if (lines.Count != 1)
                        {
                            foreach (string line in lines)
                            {
                                msg = msg + " \\u000d" + line;
                            }
                        }
                        else
                        {
                            msg = definitive;
                        }

                        multipleWebhookMessageIndex++;
                    }
                    else
                    {
                        string[] splitted = Microsoft.VisualBasic.Strings.Split(gunaTextBox3.Text, "|");
                        string definitive = splitted[multipleWebhookMessageIndex];
                        List<string> lines = new List<string>();

                        foreach (string line in Utils.SplitToLines(definitive))
                        {
                            lines.Add(line);
                        }

                        if (lines.Count != 1)
                        {
                            foreach (string line in lines)
                            {
                                msg = msg + " \\u000d" + line;
                            }
                        }
                        else
                        {
                            msg = definitive;
                        }

                        if (multipleWebhookMessageIndex == count)
                        {
                            multipleWebhookMessageIndex = 0;
                        }
                        else
                        {
                            multipleWebhookMessageIndex++;
                        }
                    }
                }
            }
            catch
            {

            }

            try
            {
                msg = msg.Replace(" [mtag] ", "");
                msg = msg.Replace(" [mtag]", "");
                msg = msg.Replace("[mtag]", "");

                msg = msg.Replace(" [all] ", "");
                msg = msg.Replace(" [all]", "");
                msg = msg.Replace("[all]", "");

                msg = msg.Replace(" [rtag] ", "");
                msg = msg.Replace(" [rtag]", "");
                msg = msg.Replace("[rtag]", "");

                msg = msg.Replace(" [rall] ", "");
                msg = msg.Replace(" [rall]", "");
                msg = msg.Replace("[rall]", "");
            }
            catch
            {

            }

            try
            {
                while (msg.Contains("[lag]"))
                {
                    try
                    {
                        msg = Utils.ReplaceFirst(msg, "[lag]", Utils.GetLagMessage());
                    }
                    catch
                    {

                    }
                }
            }
            catch
            {

            }

            try
            {
                while (msg.Contains("[rndnum]"))
                {
                    try
                    {
                        msg = Utils.ReplaceFirst(msg, "[rndnum]", Utils.GetUniqueInt(4).ToString());
                    }
                    catch
                    {

                    }
                }
            }
            catch
            {

            }

            try
            {
                while (msg.Contains("[rndstr]"))
                {
                    try
                    {
                        msg = Utils.ReplaceFirst(msg, "[rndstr]", Utils.GetUniqueKey(16));
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

        return msg;
    }

    public void SpamWebhook(string url)
    {
        try
        {
            while (webhookSpammer)
            {
                try
                {
                    if (siticoneOSToggleSwith11.Checked)
                    {
                        Thread.Sleep(bunifuHSlider11.Value);
                    }

                    Utils.SendMessageToWebhook(url, gunaLineTextBox18.Text, gunaLineTextBox19.Text, GetWebhookSpammerMessage());
                    Interlocked.Increment(ref webhookSpammerStatus);
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

    public int actualIndex = 0;
    public List<string> blacklisted = new List<string>();

    private void gunaButton19_Click(object sender, EventArgs e)
    {
        // Mass DM Advertiser - Start Advertising
        try
        {
            if (Utils.users.Count == 0)
            {
                MessageBox.Show("There are no parsed users! Please, join in a guild and let the Spammer parse the users.", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (Utils.GetCleanToken(gunaTextBox4.Text) == "")
            {
                MessageBox.Show("Please, insert a valid message for advertising!", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            actualIndex = 0;
            blacklisted.Clear();

            try
            {
                if (System.IO.File.Exists("blacklisted.txt"))
                {
                    blacklisted.AddRange(System.IO.File.ReadAllLines("blacklisted.txt"));
                }
            }
            catch
            {

            }

            gunaButton19.Enabled = false;
            gunaButton23.Enabled = false;
            gunaButton20.Enabled = true;
            completedUsers = 0;
            massDmAdvertiser = true;

            Thread thread = new Thread(DoMassDMAdvertiser);
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }
        catch
        {

        }
    }

    private void gunaButton20_Click(object sender, EventArgs e)
    {
        // Mass DM Advertiser - Stop Advertising
        try
        {
            massDmAdvertiser = false;
            new Thread(reEnableDMAdvertiser).Start();
        }
        catch
        {

        }
    }

    public void reEnableDMAdvertiser()
    {
        try
        {
            if (siticoneOSToggleSwith12.Checked)
            {
                Thread.Sleep(bunifuHSlider12.Value);
            }

            try
            {
                if (siticoneCheckBox45.Checked)
                {
                    string actualContent = "";

                    if (System.IO.File.Exists("blacklisted.txt"))
                    {
                        actualContent = System.IO.File.ReadAllText("blacklisted.txt");
                    }

                    string newBlacklisted = "";

                    foreach (string id in blacklisted)
                    {
                        if (!actualContent.Contains(id))
                        {
                            if (newBlacklisted == "")
                            {
                                newBlacklisted = id;
                            }
                            else
                            {
                                newBlacklisted += "\r\n" + id;
                            }
                        }
                    }

                    foreach (string id in Utils.users)
                    {
                        if (!actualContent.Contains(id))
                        {
                            if (newBlacklisted == "")
                            {
                                newBlacklisted = id;
                            }
                            else
                            {
                                newBlacklisted += "\r\n" + id;
                            }
                        }
                    }

                    System.IO.File.WriteAllText("blacklisted.txt", actualContent + "\r\n" + newBlacklisted);
                }
            }
            catch
            {

            }

            blacklisted.Clear();
            actualIndex = 0;

            gunaButton20.Enabled = false;
            gunaButton19.Enabled = true;
            gunaButton23.Enabled = true;
        }
        catch
        {

        }
    }

    public void DoMassDMAdvertiser()
    {
        try
        {

            foreach (DiscordClient client in this.GetClients())
            {
                if (siticoneOSToggleSwith12.Checked)
                {
                    Thread.Sleep(bunifuHSlider12.Value);
                }

                Thread.Sleep(1);

                Thread thread = new Thread(() => ProcessOtherClient(client));
                thread.Priority = ThreadPriority.Highest;
                thread.Start();
            }
        }
        catch
        {

        }
    }

    public void ProcessOtherClient(DiscordClient client)
    {
        try
        {
            if (massDmAdvertiser)
            {
                if (siticoneOSToggleSwith12.Checked)
                {
                    Thread.Sleep(bunifuHSlider12.Value);
                }

                try
                {
                    List<string> preparedUsers = new List<string>();
                    int dms = 5;

                    if (siticoneComboBox9.SelectedIndex == 1)
                    {
                        dms = 20;
                    }
                    else if (siticoneComboBox9.SelectedIndex == 2)
                    {
                        dms = 200;
                    }

                    try
                    {
                        if (client.IsPhoneVerified(GetProxy()))
                        {
                            dms = 18;

                            if (siticoneComboBox9.SelectedIndex == 1)
                            {
                                dms = 40;
                            }
                            else if (siticoneComboBox9.SelectedIndex == 2)
                            {
                                dms = 400;
                            }
                        }
                    }
                    catch
                    {

                    }

                    try
                    {
                        for (int i = actualIndex; i < Utils.users.Count; i++)
                        {
                            try
                            {
                                if (dms > 0)
                                {
                                    if (siticoneCheckBox43.Checked)
                                    {
                                        if (blacklisted.Contains(Utils.users[i]))
                                        {
                                            Interlocked.Increment(ref actualIndex);
                                            Interlocked.Increment(ref completedUsers);
                                            continue;
                                        }
                                    }

                                    dms--;
                                    Interlocked.Increment(ref actualIndex);
                                    preparedUsers.Add(Utils.users[i]);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            catch
                            {
                                break;
                            }
                        }
                    }
                    catch
                    {

                    }

                    try
                    {
                        if (siticoneOSToggleSwith12.Checked)
                        {
                            Thread.Sleep(bunifuHSlider12.Value);
                        }

                        Thread thread = new Thread(() => Advertise(client, preparedUsers));
                        thread.Priority = ThreadPriority.Highest;
                        thread.Start();
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

    public void Advertise(DiscordClient client, List<string> users)
    {
        try
        {
            if (massDmAdvertiser)
            {
                try
                {
                    foreach (string user in users)
                    {
                        Thread.Sleep(1);

                        try
                        {
                            if (massDmAdvertiser)
                            {
                                try
                                {
                                    if (siticoneOSToggleSwith12.Checked)
                                    {
                                        Thread.Sleep(bunifuHSlider12.Value);
                                    }

                                    if (massDmAdvertiser)
                                    {
                                        Thread thread = new Thread(() =>
                                        {
                                            client.SendMessage(client.GetDMChannel(user, GetProxy()), GetMassDMAdvertiserMessage(), "", GetProxy());
                                            Interlocked.Increment(ref completedUsers);
                                        });
                                        thread.Priority = ThreadPriority.Highest;
                                        thread.Start();
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

    public string GetMassDMAdvertiserMessage()
    {
        string msg = "";

        try
        {
            try
            {
                if (!siticoneCheckBox28.Checked)
                {
                    List<string> lines = new List<string>();

                    foreach (string line in Utils.SplitToLines(gunaTextBox4.Text))
                    {
                        lines.Add(line);
                    }

                    if (lines.Count != 1)
                    {
                        foreach (string line in lines)
                        {
                            msg = msg + " \\u000d" + line;
                        }
                    }
                    else
                    {
                        msg = gunaTextBox4.Text;
                    }
                }
                else
                {
                    if (multipleDmAdvertiserMessageIndex < 0)
                    {
                        multipleDmAdvertiserMessageIndex = 0;
                    }

                    int count = 0;

                    foreach (char c in gunaTextBox4.Text.ToCharArray())
                    {
                        if (c == '|')
                        {
                            count++;
                        }
                    }

                    if (multipleDmAdvertiserMessageIndex > count)
                    {
                        multipleDmAdvertiserMessageIndex = 0;
                    }

                    if (count == 0)
                    {
                        List<string> lines = new List<string>();

                        foreach (string line in Utils.SplitToLines(gunaTextBox4.Text))
                        {
                            lines.Add(line);
                        }

                        if (lines.Count != 1)
                        {
                            foreach (string line in lines)
                            {
                                msg = msg + " \\u000d" + line;
                            }
                        }
                        else
                        {
                            msg = gunaTextBox4.Text;
                        }

                        multipleDmAdvertiserMessageIndex++;
                    }
                    else if (count == 1 && Microsoft.VisualBasic.Strings.Split(gunaTextBox4.Text, "|")[1].Replace(" ", "").Replace('\t'.ToString(), "").Trim() == "")
                    {
                        string[] splitted = Microsoft.VisualBasic.Strings.Split(gunaTextBox4.Text, "|");
                        string definitive = splitted[0];
                        List<string> lines = new List<string>();

                        foreach (string line in Utils.SplitToLines(definitive))
                        {
                            lines.Add(line);
                        }

                        if (lines.Count != 1)
                        {
                            foreach (string line in lines)
                            {
                                msg = msg + " \\u000d" + line;
                            }
                        }
                        else
                        {
                            msg = definitive;
                        }

                        multipleDmAdvertiserMessageIndex++;
                    }
                    else
                    {
                        string[] splitted = Microsoft.VisualBasic.Strings.Split(gunaTextBox4.Text, "|");
                        string definitive = splitted[multipleDmAdvertiserMessageIndex];
                        List<string> lines = new List<string>();

                        foreach (string line in Utils.SplitToLines(definitive))
                        {
                            lines.Add(line);
                        }

                        if (lines.Count != 1)
                        {
                            foreach (string line in lines)
                            {
                                msg = msg + " \\u000d" + line;
                            }
                        }
                        else
                        {
                            msg = definitive;
                        }

                        if (multipleDmAdvertiserMessageIndex == count)
                        {
                            multipleDmAdvertiserMessageIndex = 0;
                        }
                        else
                        {
                            multipleDmAdvertiserMessageIndex++;
                        }
                    }
                }
            }
            catch
            {

            }

            try
            {
                msg = msg.Replace(" [mtag] ", "");
                msg = msg.Replace(" [mtag]", "");
                msg = msg.Replace("[mtag]", "");

                msg = msg.Replace(" [all] ", "");
                msg = msg.Replace(" [all]", "");
                msg = msg.Replace("[all]", "");

                msg = msg.Replace(" [rtag] ", "");
                msg = msg.Replace(" [rtag]", "");
                msg = msg.Replace("[rtag]", "");

                msg = msg.Replace(" [rall] ", "");
                msg = msg.Replace(" [rall]", "");
                msg = msg.Replace("[rall]", "");
            }
            catch
            {

            }

            try
            {
                while (msg.Contains("[lag]"))
                {
                    try
                    {
                        msg = Utils.ReplaceFirst(msg, "[lag]", Utils.GetLagMessage());
                    }
                    catch
                    {

                    }
                }
            }
            catch
            {

            }

            try
            {
                while (msg.Contains("[rndnum]"))
                {
                    try
                    {
                        msg = Utils.ReplaceFirst(msg, "[rndnum]", Utils.GetUniqueInt(4).ToString());
                    }
                    catch
                    {

                    }
                }
            }
            catch
            {

            }

            try
            {
                while (msg.Contains("[rndstr]"))
                {
                    try
                    {
                        msg = Utils.ReplaceFirst(msg, "[rndstr]", Utils.GetUniqueKey(16));
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

        return msg;
    }

    private void siticoneButton8_Click(object sender, EventArgs e)
    {
        // Settings and Utils - Set new nickname for all tokens
        try
        {
            if (!Utils.IsIDValid(gunaLineTextBox21.Text))
            {
                MessageBox.Show("The ID of the guild is not valid! Please, insert a new valid one.", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Thread thread = new Thread(DoSetNickName);
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }
        catch
        {

        }
    }

    public void DoSetNickName()
    {
        try
        {
            foreach (DiscordClient client in this.GetClients())
            {
                try
                {
                    Thread thread = new Thread(() => SetNickname(client, gunaLineTextBox21.Text, gunaLineTextBox22.Text));
                    thread.Priority = ThreadPriority.Highest;
                    thread.Start();
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

    public void SetNickname(DiscordClient client, string guildId, string nickname)
    {
        try
        {
            client.SetNickname(guildId, nickname, GetProxy());
        }
        catch
        {

        }
    }

    private void siticoneButton11_Click(object sender, EventArgs e)
    {
        // Settings and Utils - Set new online status for all tokens
        try
        {
            Thread thread = new Thread(DoSetStatus);
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }
        catch
        {

        }
    }

    public void DoSetStatus()
    {
        try
        {
            UserStatus status = UserStatus.Online;

            if (siticoneComboBox2.SelectedIndex == 1)
            {
                status = UserStatus.Idle;
            }
            else if (siticoneComboBox2.SelectedIndex == 2)
            {
                status = UserStatus.DoNotDisturb;
            }
            else if (siticoneComboBox2.SelectedIndex == 3)
            {
                status = UserStatus.Invisible;
            }

            foreach (DiscordClient client in this.GetClients())
            {
                try
                {
                    Thread thread = new Thread(() => SetStatus(client, status));
                    thread.Priority = ThreadPriority.Highest;
                    thread.Start();
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

    public void SetStatus(DiscordClient client, UserStatus status)
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

            try
            {
                client.ConnectToWebSocket();
            }
            catch
            {

            }

            try
            {
                client.SetStatus(status, GetProxy());
            }
            catch
            {

            }

            try
            {
                client.SendToWS("{\"op\":3,\"d\":{\"status\":\"" + theStatus + "\",\"since\":0,\"activities\":[],\"afk\":false}}");
            }
            catch
            {

            }
        }
        catch
        {

        }
    }

    private void siticoneButton7_Click(object sender, EventArgs e)
    {
        // Reaction Spammer - Fetch from message
        try
        {
            if (!Utils.IsIDValid(gunaLineTextBox9.Text))
            {
                MessageBox.Show("The ID of the channel is not valid! Please, insert a new valid one.", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Utils.IsIDValid(gunaLineTextBox10.Text))
            {
                MessageBox.Show("The ID of the message is not valid! Please, insert a new valid one.", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            siticoneButton7.Enabled = false;
            siticoneButton7.Text = "Fetching";
            Thread thread = new Thread(FetchEmote);
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }
        catch
        {

        }
    }

    public void FetchEmote()
    {
        try
        {
            try
            {
                gunaLineTextBox8.Text = clients[0].FetchEmote(gunaLineTextBox9.Text, gunaLineTextBox10.Text, GetProxy());
            }
            catch
            {

            }

            siticoneButton7.Enabled = true;
            siticoneButton7.Text = "Fetch from message";
        }
        catch
        {

        }
    }

    private void siticoneButton12_Click(object sender, EventArgs e)
    {
        // Settings and Utils - Generate Text
        try
        {
            Thread thread = new Thread(GenerateText);
            thread.SetApartmentState(ApartmentState.STA);

            thread.Start();
        }
        catch
        {

        }
    }

    private void siticoneButton10_Click(object sender, EventArgs e)
    {
        Thread thread = new Thread(() => ParseUsers(gunaLineTextBox25.Text, gunaLineTextBox24.Text));
        thread.Priority = ThreadPriority.Highest;
        thread.Start();
    }

    public void EnableParse()
    {
        siticoneButton10.Enabled = true;
        siticoneButton10.Text = "Parse Users for this guild";
    }

    public void ParseUsers(string guildId, string channelId)
    {
        try
        {
            siticoneButton10.Enabled = false;
            siticoneButton10.Text = "Parsing Users...";

            if (!Utils.IsIDValid(guildId))
            {
                MessageBox.Show("The ID of the guild that you inserted is not valid!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                EnableParse();
                return;
            }

            if (!Utils.IsIDValid(channelId))
            {
                MessageBox.Show("The ID of the channel that you inserted is not valid! Please, ensure to insert the ID of the channel of this guild that most of the users are visible to you.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                EnableParse();
                return;
            }

            if (!Utils.IsTokenValid(clients[0].token))
            {
                MessageBox.Show("Failed to parse users! Please, ensure that your tokens are all valid or if your tokens list is empty!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                EnableParse();
                return;
            }

            if (!System.IO.Directory.Exists("UserParser"))
            {
                MessageBox.Show("Cannot find Parse Users plugin!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                EnableParse();
                return;
            }

            if (!System.IO.File.Exists("UserParser\\ParseUsers.exe"))
            {
                MessageBox.Show("Cannot find Parse Users plugin!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                EnableParse();
                return;
            }

            if (System.IO.File.Exists("users.txt"))
            {
                System.IO.File.Delete("users.txt");
            }

            Utils.users.Clear();
            Process process = new Process(); ;
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "UserParser\\ParseUsers.exe";
            startInfo.Arguments = clients[0].token + " " + guildId + " " + channelId;
            process.StartInfo = startInfo;
            // Process.Start("UserParser\\ParseUsers.exe", clients[0].token + " " + guildId + " " + channelId)
            process.Start();

            while (!System.IO.File.Exists("users.txt"))
            {
                Thread.Sleep(1000);
            }

            process.Kill();
            Utils.users.AddRange(System.IO.File.ReadAllLines("users.txt"));
            System.IO.File.Delete("users.txt");
            EnableParse();
        }
        catch
        {
            MessageBox.Show("Failed to parse users! Please, ensure that your tokens are all valid or if your tokens list is empty!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            EnableParse();
        }
    }

    private void siticoneButton13_Click(object sender, EventArgs e)
    {
        if (!(Utils.users.Count > 0))
        {
            return;
        }

        if (saveFileDialog1.ShowDialog().Equals(DialogResult.OK))
        {
            string preparedList = "";

            foreach (string user in Utils.users)
            {
                if (preparedList == "")
                {
                    preparedList = user;
                }
                else
                {
                    preparedList += Environment.NewLine + user;
                }
            }

            System.IO.File.WriteAllText(saveFileDialog1.FileName, preparedList);
        }
    }

    private void siticoneButton14_Click(object sender, EventArgs e)
    {
        if (openFileDialog3.ShowDialog().Equals(DialogResult.OK))
        {
            try
            {
                string realLine = "";

                foreach (string line in Utils.SplitToLines(System.IO.File.ReadAllText(openFileDialog3.FileName)))
                {
                    realLine = line.Replace(" ", "").Trim().Replace('\t'.ToString(), "");

                    if (Utils.IsIDValid(realLine))
                    {
                        Utils.users.Add(realLine);
                    }
                }
            }
            catch
            {

            }
        }
    }

    private void siticoneButton18_Click(object sender, EventArgs e)
    {
        // Miscellaneous - Phone Lock all loaded tokens
        Thread thread = new Thread(PhoneLockAll);
        thread.Priority = ThreadPriority.Highest;
        thread.Start();
    }

    public void PhoneLockAll()
    {
        foreach (DiscordClient client in clients)
        {
            Thread.Sleep(1);
            Thread thread = new Thread(() => PhoneLock(client));
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }
    }

    public void PhoneLock(DiscordClient client)
    {
        client.PhoneLock(GetProxy());
    }

    private void siticoneButton19_Click(object sender, EventArgs e)
    {

    }

    private void siticoneButton20_Click(object sender, EventArgs e)
    {

    }

    private void siticoneButton17_Click(object sender, EventArgs e)
    {
        Thread thread = new Thread(() => ParseRoles(gunaLineTextBox27.Text));
        thread.Priority = ThreadPriority.Highest;
        thread.Start();
    }

    public void ParseRoles(string guildId)
    {
        try
        {
            if (!Utils.IsIDValid(guildId))
            {
                MessageBox.Show("The ID of the guild that you have inserted is not valid!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Utils.IsTokenValid(clients[0].token))
            {
                MessageBox.Show("Failed to parse users! Please, ensure that your tokens are all valid or if your tokens list is empty!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Utils.roles = clients[0].GetGuildRoles(guildId, GetProxy());
        }
        catch
        {
            MessageBox.Show("Failed to parse roles! Please, ensure that your tokens are all valid or if your tokens list is empty!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void siticoneButton16_Click(object sender, EventArgs e)
    {
        if (!(Utils.roles.Count > 0))
        {
            return;
        }

        if (saveFileDialog2.ShowDialog().Equals(DialogResult.OK))
        {
            string preparedList = "";

            foreach (string role in Utils.roles)
            {
                if (preparedList == "")
                {
                    preparedList = role;
                }
                else
                {
                    preparedList += Environment.NewLine + role;
                }
            }

            System.IO.File.WriteAllText(saveFileDialog2.FileName, preparedList);
        }
    }

    private void siticoneButton15_Click(object sender, EventArgs e)
    {
        if (openFileDialog4.ShowDialog().Equals(DialogResult.OK))
        {
            try
            {
                string realLine = "";

                foreach (string line in Utils.SplitToLines(System.IO.File.ReadAllText(openFileDialog4.FileName)))
                {
                    realLine = line.Replace(" ", "").Trim().Replace('\t'.ToString(), "");

                    if (Utils.IsIDValid(realLine))
                    {
                        Utils.roles.Add(realLine);
                    }
                }
            }
            catch
            {

            }
        }
    }

    private void siticoneButton21_Click(object sender, EventArgs e)
    {
        if (openFileDialog5.ShowDialog().Equals(DialogResult.OK))
        {
            pictureBox2.BackgroundImage = System.Drawing.Image.FromFile(openFileDialog5.FileName);
        }
    }

    private void siticoneButton22_Click(object sender, EventArgs e)
    {
        Thread thread = new Thread(SetAvatarAll);
        thread.Priority = ThreadPriority.Highest;
        thread.Start();
    }

    public void SetAvatarAll()
    {
        foreach (DiscordClient client in clients)
        {
            try
            {
                Thread.Sleep(1);
                Thread thread = new Thread(() => SetAvatar(client));
                thread.Priority = ThreadPriority.Highest;
                thread.Start();
            }
            catch
            {

            }
        }
    }

    public void SetAvatar(DiscordClient client)
    {
        try
        {
            client.SetAvatar(pictureBox2.BackgroundImage, GetProxy());
        }
        catch
        {

        }
    }

    private void siticoneCheckBox35_CheckedChanged(object sender, EventArgs e)
    {
        if (siticoneCheckBox35.Checked)
        {
            gunaButton10.Enabled = true;
            gunaButton9.Enabled = false;
            gunaButton10.Text = "Start Spamming";
            gunaButton9.Text = "Stop Spamming";
        }
        else
        {
            gunaButton10.Enabled = true;
            gunaButton9.Enabled = true;
            gunaButton10.Text = "Add friend";
            gunaButton9.Text = "Remove friend";
        }
    }

    private void siticoneCheckBox33_CheckedChanged(object sender, EventArgs e)
    {
        if (siticoneCheckBox33.Checked)
        {
            gunaButton1.Text = "Start Spamming";
            gunaButton2.Text = "Stop Spamming";
            gunaButton1.Enabled = true;
            gunaButton2.Enabled = false;
        }
        else
        {
            gunaButton1.Text = "Join guild";
            gunaButton2.Text = "Leave guild";
            gunaButton1.Enabled = true;
            gunaButton2.Enabled = true;
        }
    }

    private void siticoneButton23_Click(object sender, EventArgs e)
    {
        Thread thread = new Thread(ResetAvatarAll);
        thread.Priority = ThreadPriority.Highest;
        thread.Start();
    }

    public void ResetAvatarAll()
    {
        foreach (DiscordClient client in clients)
        {
            Thread.Sleep(1);
            Thread thread = new Thread(() => ResetAvatar(client));
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }
    }

    public void ResetAvatar(DiscordClient client)
    {
        client.ResetAvatar(GetProxy());
    }

    private void gunaButton13_Click(object sender, EventArgs e)
    {
        buttonSpammerStatus = 0;
        // Button Spammer - Spam Button
        if (!Utils.IsIDValid(gunaLineTextBox26.Text))
        {
            MessageBox.Show("The ID of the guild is not valid!", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        if (!Utils.IsIDValid(gunaLineTextBox30.Text))
        {
            MessageBox.Show("The ID of the channel is not valid!", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        if (!Utils.IsIDValid(gunaLineTextBox31.Text))
        {
            MessageBox.Show("The ID of the message is not valid!", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        Thread thread = new Thread(DoButtonSpammer);
        thread.Priority = ThreadPriority.Highest;
        thread.Start();
    }

    public void DoButtonSpammer()
    {
        try
        {
            Tuple<string, string, string> informations = clients[0].GetButton(gunaLineTextBox30.Text, gunaLineTextBox31.Text, GetProxy());

            foreach (DiscordClient client in GetClients())
            {
                Thread.Sleep(1);

                Thread thread = new Thread(() =>
                {
                    client.ClickButton(informations.Item1, gunaLineTextBox30.Text, informations.Item2, informations.Item3, gunaLineTextBox26.Text, gunaLineTextBox31.Text, GetProxy());
                    Interlocked.Increment(ref buttonSpammerStatus);
                });

                thread.Priority = ThreadPriority.Highest;
                thread.Start();
            }
        }
        catch
        {

        }
    }

    private void bunifuHSlider7_Scroll(object sender, Utilities.BunifuSlider.BunifuHScrollBar.ScrollEventArgs e)
    {
        metroLabel35.Text = "Delay: " + bunifuHSlider7.Value.ToString() + "ms";
    }

    private void siticoneButton19_Click_1(object sender, EventArgs e)
    {
        Thread thread = new Thread(SetGameAll);
        thread.Priority = ThreadPriority.Highest;
        thread.Start();
    }

    public void SetGameAll()
    {
        foreach (DiscordClient client in GetClients())
        {
            Thread.Sleep(1);
            Thread thread = new Thread(() => SetGame(client, gunaLineTextBox28.Text));
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }
    }

    public void SetGame(DiscordClient client, string game)
    {
        client.SetGame(game);
    }

    private void siticoneButton33_Click(object sender, EventArgs e)
    {

    }

    private void siticoneButton25_Click(object sender, EventArgs e)
    {
        if (folderBrowserDialog1.ShowDialog().Equals(DialogResult.OK))
        {
            gunaLineTextBox33.Text = folderBrowserDialog1.SelectedPath;
        }
    }

    private void siticoneButton24_Click(object sender, EventArgs e)
    {
        if (!System.IO.Directory.Exists(gunaLineTextBox33.Text))
        {
            MessageBox.Show("The directory that you have specified does not exist!", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        List<string> files = new List<string>();

        foreach (string file in System.IO.Directory.GetFiles(gunaLineTextBox33.Text))
        {
            if (!System.IO.Path.GetExtension(file).ToLower().Equals(".png") && !System.IO.Path.GetExtension(file).ToLower().Equals(".jpg") && !System.IO.Path.GetExtension(file).ToLower().Equals(".jpeg"))
            {
                continue;
            }

            files.Add(file);
        }

        if (files.Count <= 0)
        {
            MessageBox.Show("There are no valid avatars in the folder that you specified!", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        Thread thread = new Thread(() => SetRandomAvatarsAll(files));
        thread.Priority = ThreadPriority.Highest;
        thread.Start();
    }

    public void SetRandomAvatarsAll(List<string> usernames)
    {
        int username = 0;

        foreach (DiscordClient client in GetClients())
        {
            Thread.Sleep(1);
            System.Drawing.Image image = System.Drawing.Image.FromFile(usernames[username]);
            Thread thread = new Thread(() => client.SetAvatar(image, GetProxy()));
            thread.Priority = ThreadPriority.Highest;
            thread.Start();

            if (username >= usernames.Count)
            {
                username = -1;
            }

            username++;

            if (username >= usernames.Count)
            {
                username = 0;
            }
        }
    }

    private void siticoneButton26_Click(object sender, EventArgs e)
    {
        openFileDialog6.FileName = "";

        if (openFileDialog6.ShowDialog().Equals(DialogResult.OK))
        {
            gunaLineTextBox34.Text = openFileDialog6.FileName;
        }
    }

    private void siticoneButton27_Click(object sender, EventArgs e)
    {
        string path = gunaLineTextBox34.Text;
        string password = gunaLineTextBox35.Text;

        if (!System.IO.File.Exists(path))
        {
            MessageBox.Show("The file that you specified does not exist!", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        if (!System.IO.Path.GetExtension(path).ToLower().Equals(".txt"))
        {
            MessageBox.Show("The file that you specified is not a text file (*.txt)!", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        if (password.Replace(" ", "").Replace('\t'.ToString(), "") == "" || password.Length > 100)
        {
            MessageBox.Show("Please, insert a valid password!");
            return;
        }

        List<string> usernames = new List<string>();

        foreach (string username in System.IO.File.ReadAllLines(path))
        {
            if (username.Replace(" ", "").Replace('\t'.ToString(), "") == "")
            {
                continue;
            }

            if (username.Length > 100)
            {
                continue;
            }

            usernames.Add(username);
        }

        if (usernames.Count <= 0)
        {
            MessageBox.Show("Please, insert some usernames in the file!", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        Thread thread = new Thread(() => SetRandomUsernamesAll(usernames));
        thread.Priority = ThreadPriority.Highest;
        thread.Start();
    }

    public void SetRandomUsernamesAll(List<string> usernames)
    {
        int username = 0;

        foreach (DiscordClient client in GetClients())
        {
            Thread.Sleep(1);
            Thread thread = new Thread(() => client.SetUsername(usernames[username], gunaLineTextBox35.Text, GetProxy()));
            thread.Priority = ThreadPriority.Highest;
            thread.Start();

            if (username >= usernames.Count)
            {
                username = -1;
            }

            username++;

            if (username >= usernames.Count)
            {
                username = 0;
            }
        }
    }

    private void siticoneButton29_Click(object sender, EventArgs e)
    {
        Thread thread = new Thread(SetUsernameAll);
        thread.Priority = ThreadPriority.Highest;
        thread.Start();
    }

    public void SetUsernameAll()
    {
        foreach (DiscordClient client in GetClients())
        {
            Thread.Sleep(1);
            Thread thread = new Thread(() => client.SetUsername(gunaLineTextBox37.Text, gunaLineTextBox36.Text, GetProxy()));
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }

    }

    private void siticoneButton28_Click(object sender, EventArgs e)
    {
        Thread thread = new Thread(RandomActivitiesAll);
        thread.Priority = ThreadPriority.Highest;
        thread.Start();
    }

    public void RandomActivitiesAll()
    {
        int game = 0;

        foreach (DiscordClient client in GetClients())
        {
            Thread.Sleep(1);
            Thread thread = new Thread(() => SetGame(client, games[game]));
            thread.Priority = ThreadPriority.Highest;
            thread.Start();

            if (game >= games.Length)
            {
                game = -1;
            }

            game++;

            if (game >= games.Length)
            {
                game = 0;
            }
        }
    }

    private void siticoneButton30_Click(object sender, EventArgs e)
    {
        Thread thread = new Thread(SetAboutMeAll);
        thread.Priority = ThreadPriority.Highest;
        thread.Start();
    }

    public void SetAboutMeAll()
    {
        foreach (DiscordClient client in GetClients())
        {
            Thread.Sleep(1);
            Thread thread = new Thread(() => client.SetAboutMe(gunaLineTextBox38.Text, GetProxy()));
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }
    }

    private void siticoneButton31_Click(object sender, EventArgs e)
    {
        openFileDialog7.FileName = "";

        if (openFileDialog7.ShowDialog().Equals(DialogResult.OK))
        {
            gunaLineTextBox39.Text = openFileDialog7.FileName;
        }
    }

    private void siticoneButton34_Click(object sender, EventArgs e)
    {
        Thread thread = new Thread(DisconnectFromWS);
        thread.Priority = ThreadPriority.Highest;
        thread.Start();
    }

    public void DisconnectFromWS()
    {
        foreach (DiscordClient client in GetClients())
        {
            Thread.Sleep(1);

            Thread thread = new Thread(() => client.DisconnectFromWebSocket());
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }
    }

    private void siticoneButton35_Click(object sender, EventArgs e)
    {
        if (!Utils.IsIDValid(gunaLineTextBox32.Text))
        {
            MessageBox.Show("The ID of the Bot that you have inserted is not valid!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        Thread thread = new Thread(BypassLokiBot);
        thread.Priority = ThreadPriority.Highest;
        thread.Start();
    }

    public void BypassLokiBot()
    {
        foreach (DiscordClient client in GetClients())
        {
            Thread.Sleep(1);

            Thread thread = new Thread(() => client.BypassLokiBot(gunaLineTextBox32.Text, GetProxy()));
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }
    }

    private void siticoneButton36_Click(object sender, EventArgs e)
    {
        LoadTokensFromClipboard();
    }

    private void siticoneButton37_Click(object sender, EventArgs e)
    {
        LoadProxiesFromClipboard();
    }

    private void siticoneButton33_Click_1(object sender, EventArgs e)
    {

    }

    private void bunifuHSlider13_Scroll(object sender, Utilities.BunifuSlider.BunifuHScrollBar.ScrollEventArgs e)
    {
        metroLabel40.Text = "Delay: " + bunifuHSlider13.Value.ToString() + "ms";
    }

    private void gunaButton14_Click(object sender, EventArgs e)
    {
        inviteSpammerStatus = 0;

        if (siticoneCheckBox39.Checked)
        {
            if (!Utils.AreIDsValid(gunaLineTextBox43.Text))
            {
                MessageBox.Show("The IDs of the channels that you have inserted are not valid!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        else
        {
            if (!Utils.IsIDValid(gunaLineTextBox43.Text))
            {
                MessageBox.Show("The ID of the channel that you have inserted is not valid!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        Thread thread = new Thread(DoInviteSpammer);
        thread.Priority = ThreadPriority.Highest;
        thread.Start();
    }

    public void DoInviteSpammer()
    {
        try
        {
            List<string> channelIds = new List<string>();

            if (siticoneCheckBox39.Checked)
            {
                channelIds.AddRange(gunaLineTextBox43.Text.Split(','));
            }
            else
            {
                channelIds.Add(gunaLineTextBox43.Text);
            }

            foreach (string channelId in channelIds)
            {
                try
                {
                    Thread.Sleep(1);

                    if (siticoneOSToggleSwith13.Checked)
                    {
                        Thread.Sleep(bunifuHSlider13.Value);
                    }

                    Thread thread = new Thread(() => ProcessInvite(channelId));
                    thread.Priority = ThreadPriority.Highest;
                    thread.Start();
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

    public void ProcessInvite(string channelId)
    {
        try
        {
            if (siticoneOSToggleSwith13.Checked)
            {
                Thread.Sleep(bunifuHSlider13.Value);
            }

            List<string> messageIds = clients[0].GetChannelMessages(channelId, GetProxy());
            int messageIndex = 0, max = messageIds.Count - 1;

            foreach (DiscordClient client in GetClients())
            {
                Thread thread = new Thread(() =>
                {
                    client.CreateInvite(channelId, siticoneComboBox3.SelectedIndex, siticoneComboBox4.SelectedIndex, siticoneCheckBox36.Checked, GetProxy());
                    Interlocked.Increment(ref inviteSpammerStatus);
                });
                thread.Priority = ThreadPriority.Highest;
                thread.Start();
            }
        }
        catch
        {

        }
    }

    private void siticoneButton33_Click_2(object sender, EventArgs e)
    {
        Thread thread = new Thread(ResetGameAll);
        thread.Priority = ThreadPriority.Highest;
        thread.Start();
    }

    public void ResetGameAll()
    {
        foreach (DiscordClient client in GetClients())
        {
            Thread.Sleep(1);
            Thread thread = new Thread(() => SetGame(client, ""));
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }
    }

    private void siticoneButton20_Click_1(object sender, EventArgs e)
    {
        Thread thread = new Thread(ConnectWSAll);
        thread.Priority = ThreadPriority.Highest;
        thread.Start();
    }

    public void ConnectWSAll()
    {
        foreach (DiscordClient client in GetClients())
        {
            Thread.Sleep(1);
            Thread thread = new Thread(() => client.ConnectToWebSocket());
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }
    }

    private void siticoneCheckBox41_CheckedChanged(object sender, EventArgs e)
    {
        gunaTextBox1.Enabled = !siticoneCheckBox41.Checked;
    }

    private void siticoneCheckBox42_CheckedChanged(object sender, EventArgs e)
    {
        gunaTextBox2.Enabled = !siticoneCheckBox42.Checked;
    }

    private void siticoneCheckBox44_CheckedChanged(object sender, EventArgs e)
    {
        gunaTextBox3.Enabled = !siticoneCheckBox44.Checked;
    }

    private void siticoneButton38_Click(object sender, EventArgs e)
    {
        if (!Utils.IsIDValid(gunaLineTextBox29.Text))
        {
            MessageBox.Show("Please, insert a valid guild ID!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        Thread thread = new Thread(ParseChannels);
        thread.Priority = ThreadPriority.Highest;
        thread.Start();
    }

    public void ParseChannels()
    {
        try
        {
            List<string> channels = clients[0].GetGuildChannels(gunaLineTextBox29.Text, null);
            string ids = "";

            foreach (string channelId in channels)
            {
                if (ids == "")
                {
                    ids = channelId;
                }
                else
                {
                    ids += ", " + channelId;
                }
            }

            gunaTextBox6.Text = ids;
        }
        catch
        {
            MessageBox.Show("Please, insert a valid guild ID!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void gunaButton21_Click(object sender, EventArgs e)
    {
        Thread thread = new Thread(DeleteWebhooks);
        thread.Priority = ThreadPriority.Highest;
        thread.Start();
    }

    public void DeleteWebhooks()
    {
        try
        {
            foreach (string webhook in Utils.GetIDs(gunaLineTextBox17.Text))
            {
                Thread.Sleep(1);

                try
                {
                    if (!Utils.IsWebhookValid(webhook))
                    {
                        MessageBox.Show("The webhooks are not valid ('" + webhook + "'). Please, remove the invalid webhooks and try again.", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
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

        try
        {
            try
            {
                Thread.Sleep(1);

                foreach (string webhook in Utils.GetIDs(gunaLineTextBox17.Text))
                {
                    Thread.Sleep(1);

                    try
                    {
                        Thread thread = new Thread(() => DeleteWebhook(webhook));
                        thread.Priority = ThreadPriority.Highest;
                        thread.Start();
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

    public void DeleteWebhook(string webhook)
    {
        try
        {
            Utils.DeleteWebhook(webhook);
        }
        catch
        {

        }
    }

    private void siticoneButton39_Click(object sender, EventArgs e)
    {
        Utils.users.Clear();
        metroLabel32.Text = "0";
    }

    private void siticoneButton40_Click(object sender, EventArgs e)
    {
        Utils.roles.Clear();
        metroLabel33.Text = "0";
    }

    private void gunaButton23_Click(object sender, EventArgs e)
    {
        System.IO.File.WriteAllText("blacklisted.txt", "");
    }

    private void siticoneButton41_Click(object sender, EventArgs e)
    {
        try
        {
            string allTokens = "";

            foreach (DiscordClient client in clients)
            {
                if (allTokens == "")
                {
                    allTokens = client.GetToken();
                }
                else
                {
                    allTokens += Environment.NewLine + client.GetToken();
                }
            }

            Clipboard.SetText(allTokens);
        }
        catch
        {

        }
    }

    private void siticoneButton42_Click(object sender, EventArgs e)
    {
        try
        {
            if (saveFileDialog3.ShowDialog().Equals(DialogResult.OK))
            {
                string allTokens = "";

                foreach (DiscordClient client in clients)
                {
                    if (allTokens == "")
                    {
                        allTokens = client.GetToken();
                    }
                    else
                    {
                        allTokens += Environment.NewLine + client.GetToken();
                    }
                }

                System.IO.File.WriteAllText(saveFileDialog3.FileName, allTokens);
            }
        }
        catch
        {

        }
    }

    private void siticoneButton44_Click(object sender, EventArgs e)
    {
        try
        {
            string allProxies = "";

            foreach (string proxy in proxies)
            {
                if (allProxies == "")
                {
                    allProxies = proxy;
                }
                else
                {
                    allProxies += Environment.NewLine + proxy;
                }
            }

            Clipboard.SetText(allProxies);
        }
        catch
        {

        }
    }

    private void siticoneButton43_Click(object sender, EventArgs e)
    {
        try
        {
            if (saveFileDialog4.ShowDialog().Equals(DialogResult.OK))
            {
                string allProxies = "";

                foreach (string proxy in proxies)
                {
                    if (allProxies == "")
                    {
                        allProxies = proxy;
                    }
                    else
                    {
                        allProxies += Environment.NewLine + proxy;
                    }
                }

                System.IO.File.WriteAllText(saveFileDialog4.FileName, allProxies);
            }
        }
        catch
        {

        }
    }

    private void siticoneRadioButton4_CheckedChanged(object sender, EventArgs e)
    {
        Utils.lastProxyMode = 0;
    }

    private void siticoneRadioButton3_CheckedChanged(object sender, EventArgs e)
    {
        Utils.lastProxyMode = 1;
    }

    private void siticoneRadioButton5_CheckedChanged(object sender, EventArgs e)
    {
        Utils.lastProxyMode = 2;
    }

    private void siticoneButton32_Click(object sender, EventArgs e)
    {
        string path = gunaLineTextBox39.Text;

        if (!System.IO.File.Exists(path))
        {
            MessageBox.Show("The file that you specified does not exist!", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        if (!System.IO.Path.GetExtension(path).ToLower().Equals(".txt"))
        {
            MessageBox.Show("The file that you specified is not a text file (*.txt)!", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        List<string> usernames = new List<string>();

        foreach (string username in System.IO.File.ReadAllLines(path))
        {
            if (username.Replace(" ", "").Replace('\t'.ToString(), "") == "")
            {
                continue;
            }

            if (username.Length > 190)
            {
                continue;
            }

            usernames.Add(username);
        }

        if (usernames.Count <= 0)
        {
            MessageBox.Show("Please, insert some about me(s) in the file!", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        Thread thread = new Thread(() => SetRandomAboutMesAll(usernames));
        thread.Priority = ThreadPriority.Highest;
        thread.Start();
    }

    public void SetRandomAboutMesAll(List<string> usernames)
    {
        int username = 0;

        foreach (DiscordClient client in GetClients())
        {
            Thread.Sleep(1);
            Thread thread = new Thread(() => client.SetAboutMe(usernames[username], GetProxy()));
            thread.Priority = ThreadPriority.Highest;
            thread.Start();

            if (username >= usernames.Count)
            {
                username = -1;
            }

            username++;

            if (username >= usernames.Count)
            {
                username = 0;
            }
        }
    }

    public void GenerateText()
    {
        try
        {
            gunaTextBox5.Text = "";
            int placeholders = 0;

            if (gunaLineTextBox13.Text.Length > 8 || !Microsoft.VisualBasic.Information.IsNumeric(gunaLineTextBox13.Text))
            {
                MessageBox.Show("The number of placeholders is not valid! Please, insert a valid number of placeholders to generate.", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            placeholders = int.Parse(gunaLineTextBox13.Text);

            if (placeholders <= 0)
            {
                MessageBox.Show("The number of placeholders is not valid! Please, insert a valid number of placeholders to generate.", "PhantomDiscordSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string result = "";

            for (int i = 0; i < placeholders; i++)
            {
                if (siticoneCheckBox6.Checked)
                {
                    result += "[rndnum] ";
                }

                if (siticoneCheckBox10.Checked)
                {
                    result += "[rndstr] ";
                }

                if (siticoneCheckBox24.Checked)
                {
                    result += "[mtag] ";
                }

                if (siticoneCheckBox25.Checked)
                {
                    result += "[lag] ";
                }

                if (siticoneCheckBox30.Checked)
                {
                    result += "[all] ";
                }

                if (siticoneCheckBox31.Checked)
                {
                    result += "[rtag] ";
                }

                if (siticoneCheckBox32.Checked)
                {
                    result += "[rall] ";
                }
            }

            gunaTextBox5.Text = result.Substring(0, result.Length - 1);

            try
            {
                if (siticoneCheckBox26.Checked)
                {
                    Clipboard.SetText(gunaTextBox5.Text);
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

    private void siticoneCheckBox19_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            Utils.globalAutoReconnect = siticoneCheckBox19.Checked;
        }
        catch
        {

        }
    }

    private void siticoneButton9_Click(object sender, EventArgs e)
    {
        // Settings and Utils - Set new HypeSquad for all tokens
        try
        {
            Thread thread = new Thread(DoHypeSquadSetter);
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }
        catch
        {

        }
    }

    public void DoHypeSquadSetter()
    {
        try
        {
            HypeSquad hypeSquad = HypeSquad.Balance;

            if (siticoneComboBox1.SelectedIndex == 1)
            {
                hypeSquad = HypeSquad.Bravery;
            }
            else if (siticoneComboBox1.SelectedIndex == 2)
            {
                hypeSquad = HypeSquad.Brilliance;
            }

            try
            {
                foreach (DiscordClient client in this.GetClients())
                {
                    Thread.Sleep(1);

                    try
                    {
                        Thread thread = new Thread(() => SetHypeSquad(client, hypeSquad));
                        thread.Priority = ThreadPriority.Highest;
                        thread.Start();
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

    public void SetHypeSquad(DiscordClient client, HypeSquad hypeSquad)
    {
        try
        {
            client.SetHypeSquad(hypeSquad, GetProxy());
        }
        catch
        {

        }
    }

    private void siticoneCheckBox1_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            if (siticoneCheckBox4.Checked)
            {
                siticoneCheckBox1.Checked = false;
            }
        }
        catch
        {

        }
    }

    private void siticoneCheckBox2_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            if (siticoneCheckBox4.Checked)
            {
                siticoneCheckBox2.Checked = false;
            }
        }
        catch
        {

        }
    }

    private void siticoneCheckBox3_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            if (siticoneCheckBox4.Checked)
            {
                siticoneCheckBox3.Checked = false;
            }
        }
        catch
        {

        }
    }
}