using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using EN.Models;
using System.Web.Configuration;

namespace EN.Controllers
{
    public class HomeController : Controller
    {
 
        // GET: Home
        public ActionResult LockSystem()
        {
            return View();
        }
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(string password)
        {
            string msg = string.Empty;
            string InitVector = @"@dg5fi#$"; //16byte 1chr = 2byte(unicode)  
            //string baseUrl = "D://";
            string baseUrl ="~/File/" ;
            ReturnData result = new ReturnData();
            result.success = false;
            try
            {
                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase file = Request.Files[0];
                    string filename = Path.GetFileName(file.FileName);
                    string outputFile = Path.Combine(Server.MapPath(baseUrl),Path.GetFileName("encry_" + filename));
                    UnicodeEncoding UE = new UnicodeEncoding();
                    byte[] key = UE.GetBytes(password);
                    byte[] IV = UE.GetBytes(InitVector);
                    FileStream fsCrypt = new FileStream(outputFile, FileMode.Create);
                    RijndaelManaged RMCrypto = new RijndaelManaged();
                    ICryptoTransform encryptor = RMCrypto.CreateEncryptor(key, IV);
                    CryptoStream cs = new CryptoStream(fsCrypt, encryptor, CryptoStreamMode.Write);
                    int data;
                    while ((data = file.InputStream.ReadByte()) != -1)
                    {
                        cs.WriteByte((byte)data);
                    }
                    file.InputStream.Close();
                    cs.Close();
                    fsCrypt.Close();
                    result.success = true;
                    result.url = "encry_" + filename;
                    result.name = "encry_" + filename;

                    return RedirectToAction("GetFile", new { name = "encry_" + filename });
                  //  GetFile("encry_" + filename);
                    //msg = "File successfully encrypted and saved to " + baseUrl + result.url;
                }
            }
            catch (Exception e)
            {
                result.success = false;
                result.url = string.Empty;
                result.name = string.Empty;
                result.error = e.Message;
                msg = "Failed to encrypt the file";
            }

           
            return View(msg);
            //return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetFile(string name)
        {
            var files =new DirectoryInfo(Server.MapPath("~/File/"));
            System.IO.FileInfo[] item = files.GetFiles(name);
            List<string> items = new List<string>();
            foreach (var f in item)
            {
                items.Add(f.Name);
            }
            return View(items);
        }
        public ActionResult DecryptFile()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DecryptFile(string password)
        {

            string msg = string.Empty;
            string InitVector = @"@dg5fi#$";
            //string baseUrl = "D://";
            string baseUrl = "~/File/";
            ReturnData result = new ReturnData();
            try
            {
                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase file = Request.Files[0];
                    string filename = Path.GetFileName(file.FileName);
                    string outputFile = Path.Combine(Server.MapPath(baseUrl), "decry_" + filename);
                    UnicodeEncoding UE = new UnicodeEncoding();
                    byte[] key = UE.GetBytes(password);
                    byte[] IV = UE.GetBytes(InitVector);
                    RijndaelManaged RMCrypto = new RijndaelManaged();
                    ICryptoTransform decryptor = RMCrypto.CreateDecryptor(key, IV);
                    CryptoStream cs = new CryptoStream(file.InputStream, decryptor, CryptoStreamMode.Read);
                    FileStream fsOut = new FileStream(outputFile, FileMode.Create);
                    int data;
                    while ((data = cs.ReadByte()) != -1)
                    {
                        fsOut.WriteByte((byte)data);
                    }
                    file.InputStream.Close();
                    fsOut.Close();
                    cs.Close();
                    result.name = "decry_" + filename;
                    result.url = "decry_" + filename;
                    result.success = true;

                    return RedirectToAction("GetFile", new { name = "decry_" + filename });
                }
            }
            catch (Exception e) {
                msg = "Failed to deencrypt the file";
            }

            return View(msg);
            //return new JsonResult
            //{
            //    Data = result
            //};
        }

        public FileResult Download(string filename)
        {
            //var item = new DirectoryInfo(Server.MapPath("~/File/"));
            //System.IO.FileInfo[] fileInfo = item.GetFiles(filename);
            //List<string> items = new List<string>();
            //foreach (var f in fileInfo)
            //{
            //    items.Add(f.Name);
            //}
            var files = Path.Combine(Server.MapPath("~/File/"), filename);
           // var data= files,"application/force- download", Path.GetFileName(filename);
            return File(files, "application/force- download", Path.GetFileName(filename));
           
        }
        public string Delete(string filename)
        {
          //Download(filename);
            var files = Path.Combine(Server.MapPath("~/File/"), filename);
            FileInfo fi = new FileInfo(files);
            if(fi!=null)
            {
                System.IO.File.Delete(files);
                fi.Delete();

            }
            return files;
        }
    }
}