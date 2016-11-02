using System;
using System.Configuration;
using System.Net;
using System.IO;
using System.Text;
using TallComponents.com;
using System.Threading.Tasks;

class WebDavTest
{
    private static async Task DoTests()
    {
        try
        {
            var webdav = new WebDav(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);
            var url = ConfigurationManager.AppSettings["url"];

            string szURL1 = $@"{url}/foobar1.txt";
            string szURL2 = $@"{url}/foobar2.txt";
            string szURL3 = $@"{url}/foobar3";
            string szURL4 = $@"{url}/";

            await webdav.GetFolder(szURL4);

            string szContent = String.Format(
                @"Date/Time: {0} {1}",
                DateTime.Now.ToShortDateString(),
                DateTime.Now.ToLongTimeString());

            await webdav.PutFile(szURL1, szContent, true);
            await webdav.CopyFile(szURL1, szURL2, true);
            await webdav.MoveFile(szURL2, szURL1, true);

            await webdav.GetFile(szURL1);
            await webdav.DeleteFile(szURL1);

            await webdav.CreateFolder(szURL3);
            await webdav.DeleteFolder(szURL3);

            await webdav.GetFolder(szURL4);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }


    [STAThread]
    static void Main(string[] args)
    {
        DoTests();
        Console.ReadLine();
    }

}