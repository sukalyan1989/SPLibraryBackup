using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VersionFind
{
public class Library
    {
        public string Title { get; set; }
        public string Id { get; set; }


        //function to fetch all the libraries from a site
        public List<Library> getAllLib(ClientContext ctx)
        {
            List<Library> libList = new List<Library>();
            var x = ctx.Web.Lists;
            ctx.Load(x);
            ctx.ExecuteQuery();
            foreach(var l in x)
            {
                if(l.BaseTemplate== 101)
                {
                    var z = new Library();
                    z.Id = l.Id.ToString();
                    z.Title = l.Title;
                     libList.Add(z);
                }
            }
            return libList;
        }

        //function to get all items from a library
        public void getItemsFromLibrary(ClientContext ctx ,Guid id)
        {
           var x= ctx.Web.Lists.GetById(id);
            ctx.Load(x);
            ctx.ExecuteQuery();
            ctx.Load(x.RootFolder);
            ctx.ExecuteQuery();
            ctx.Load(x.RootFolder.Folders);
            ctx.ExecuteQuery();

            processFolderClientobj(x.RootFolder.ServerRelativeUrl,ctx);
            foreach (Folder folder in x.RootFolder.Folders)
            {
                Console.WriteLine("Handling Folder : "+folder.Name);
                processFolderClientobj(folder.ServerRelativeUrl,ctx);
            }

            Console.WriteLine("Done");
            
        }


        // process each directory /Library 
        public static void processFolderClientobj(string folderURL , ClientContext site)
        {
            string Destination = @"c:\\temp";
          
            var web = site.Web;
            site.Load(web);
            site.ExecuteQuery();
            Folder folder = web.GetFolderByServerRelativeUrl(folderURL);
            //site.Load(folder);
            //site.ExecuteQuery();
            //site.Load(folder.Files);
            //site.ExecuteQuery(); 
            //site.Load(folder.Folders);
            //site.ExecuteQuery();

            GetFolders(folder, site, Destination);
            //ProcessFolder(folder, Destination, site);
            //if (CheckIfContainsFolder(site, folder).Count > 0) // A and B
            //{

            //    foreach(Folder f in CheckIfContainsFolder(site, folder))
            //    {
            //        ProcessFolder(f, Destination, site);
            //        if (CheckIfContainsFolder(site, f).Count > 0)
            //        {

            //        }
            //    }

            //}








          
     

        }


        //function to read files
        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }


        //get files from folder
        public static void ProcessFolder(Folder folder,String Destination,ClientContext site)
        {
            site.Load(folder);
            site.ExecuteQuery();
            site.Load(folder.Files);
            site.ExecuteQuery();
            foreach (Microsoft.SharePoint.Client.File file in folder.Files)
            {
                Console.WriteLine("Handling File Name : " + file.Name);
                string destinationfolder = Destination + "/" + folder.ServerRelativeUrl;
                try
                {

                    Stream fs = Microsoft.SharePoint.Client.File.OpenBinaryDirect(site, file.ServerRelativeUrl).Stream;
                    byte[] binary = ReadFully(fs);
                    if (!Directory.Exists(destinationfolder))
                    {
                        Directory.CreateDirectory(destinationfolder);
                    }
                    FileStream stream = new FileStream(destinationfolder + "/" + file.Name, FileMode.Create);
                    BinaryWriter writer = new BinaryWriter(stream);
                    writer.Write(binary);
                    writer.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
            }

        }


        // get folder contents of a folder
        public static FolderCollection CheckIfContainsFolder(ClientContext site,Folder folder)
        {

            site.Load(folder);
            site.ExecuteQuery();
            site.Load(folder.Folders);
            site.ExecuteQuery();
            return folder.Folders;

        }


        public static int GetFolders(Folder folder , ClientContext site , string Destination)
        {
            ProcessFolder(folder, Destination, site);
            FolderCollection f = CheckIfContainsFolder(site, folder);
            if (f.Count == 0){

                return 0;

            }
            else
            {
                foreach(Folder fol in f)
                {
                    ProcessFolder(fol, Destination, site);
                    GetFolders(fol,site,Destination); 
                    
                }
            }
            return 0;
        }





    }
}
