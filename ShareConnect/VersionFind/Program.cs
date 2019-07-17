 using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security;
namespace VersionFind
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine();

            string url = Directory.GetCurrentDirectory();
            string[] text = System.IO.File.ReadAllLines(Directory.GetCurrentDirectory()+"\\SiteList.txt");

            foreach(string siteUrl in text)
            {
                Library lib = new Library();
              //  List<Vclass> valueList = new List<Vclass>();
                string siteCollectionUrl =siteUrl;
                string userName = ConfigurationManager.AppSettings["username"].ToString();
                string password = ConfigurationManager.AppSettings["password"].ToString();

                Console.WriteLine("Signing in ..");
                ClientContext ctx = new ClientContext(siteCollectionUrl);
                SecureString secureString = new SecureString();
                password.ToList().ForEach(secureString.AppendChar);

                // Namespace: Microsoft.SharePoint.Client  
                ctx.Credentials = new SharePointOnlineCredentials(userName, secureString);
                foreach (var l in lib.getAllLib(ctx))
                {
                    Console.WriteLine("Handling Items from :" + l.Title);
                    lib.getItemsFromLibrary(ctx, Guid.Parse(l.Id));
                }
                Console.WriteLine("finished !!");
            }


            Console.ReadKey();
                
           
        }
        
    }
}
