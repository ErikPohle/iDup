using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IDupOnline.Models;
using System.Web;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Collections;

namespace IDupOnline.Controllers
{

    // UploadController inherits the properties of Controller
    public class UploadController : Controller
    {

        // default constructor
        public UploadController()
        {

        }

        // on file upload, we process the images here
        [HttpPost("FileUpload")]
        public async Task<IActionResult> Index(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);
            var filePaths = new List<string>();
            ArrayList bytes = new ArrayList();
            foreach (IFormFile formFile in files)
            {

                // only accept file types of jpeg, png, or jpg
                if (formFile.ContentType.Equals("image/jpeg") || formFile.ContentType.Equals("image/png") || formFile.ContentType.Equals("image/jpg")) {

                    // file has no data
                    if (formFile.Length > 0)
                    {

                        // save the filepath of each file to a list
                        var filePath = Path.GetTempFileName();
                        filePaths.Add(filePath);

                        // save each byte of each file to an arraylist
                        bytes.Add(imageToBytes(formFile));

                        // stream the file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                        }
                    }
                }

                // file is not an image that has the corrrect extensions
                else
                {
                    return BadRequest(new { error = "Only images with file extensions equal to .jpg, .png, or .jpeg are accepted." });
                }
            }

            // are the images equal when comparing their bytes?
            bool isEqual = compareBytes(bytes);

            // if isEqual is false then response = "The Images Are Not Equal" and vice-versa
            string response = isEqual == false ? "The Images Are Not Equal" : "The Images Are Equal";

            // save the response for if the images are duplicates into a ViewBag so we can access the variables on the View Page
            ViewBag.isDup = response;

            // return a new view (just takes us to a new page)
            return View("ValidationPage");
        }

        public string imageToBytes(IFormFile file)
        {
            string bytes = "";
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var fileBytes = ms.ToArray();
                bytes = Convert.ToBase64String(fileBytes);
            }

            return bytes;
        }

        public bool compareBytes(ArrayList bytes)
        {
            string compareTo = "";

            if (bytes.Count <= 1)
                return false;

            foreach(string curByte in bytes)
            {
                if(compareTo.Equals("") && compareTo.Length == 0)
                {
                    compareTo = curByte;
                    continue;
                }
                if(!curByte.Equals(compareTo))
                {
                    return false;
                }
            }

            return true;
        }

        public string formatBytesArrayToString(ArrayList bytes)
        {
            if (bytes == null || bytes.Count == 0)
                return "";

            string returnValue = "";
            bool firstVal = true;
            foreach(string curByte in bytes)
            {
                if (firstVal == true)
                    returnValue = curByte;
                else
                {
                    returnValue += " /n ";
                    returnValue += curByte;
                }
            }
            return returnValue;
        }

    }
}
