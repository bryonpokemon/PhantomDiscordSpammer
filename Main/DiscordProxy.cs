using System.Net;
using System.Net.NetworkInformation;
using System.Net.Http;

public class DiscordProxy
{
    public string ip, port, username, password;

    public DiscordProxy(string ip, string port, string username = "", string password = "")
    {
        this.ip = ip;
        this.port = port;
        this.username = username;
        this.password = password;
    }

    public DiscordProxy(string proxy)
    {
        int colons = 0;

        foreach (char c in proxy)
        {
            if (c.Equals(':'))
            {
                colons++;
            }
        }

        string[] splitted = proxy.Split(':');

        if (colons == 1)
        {
            this.ip = splitted[0];
            this.port = splitted[1];
        }
        else
        {
            this.ip = splitted[0];
            this.port = splitted[1];
            this.username = splitted[2];
            this.password = splitted[3];
        }
    }

    public WebProxy GetNewProxy()
    {
        WebProxy proxy = null;

        if (Utils.lastProxyMode == 2)
        {
            proxy = new WebProxy(this.ip, int.Parse(this.port));

            if (this.username != "" && this.password != "")
            {
                proxy.UseDefaultCredentials = false;
                proxy.Credentials = new NetworkCredential(this.username, this.password);
            }
        }
        else if (Utils.lastProxyMode == 1)
        {
            proxy = new WebProxy($"https://{this.ip}:{this.port}");

            if (this.username != "" && this.password != "")
            {
                proxy.UseDefaultCredentials = false;
                proxy.Credentials = new NetworkCredential(this.username, this.password);
            }
        }
        else
        {
            proxy = new WebProxy($"http://{this.ip}:{this.port}");

            if (this.username != "" && this.password != "")
            {
                proxy.UseDefaultCredentials = false;
                proxy.Credentials = new NetworkCredential(this.username, this.password);
            }
        }

        return proxy;
    }
}