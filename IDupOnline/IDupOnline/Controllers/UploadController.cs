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

namespace IDupOnline.Controllers
{
    public class UploadController : Controller
    {

        public UploadController()
        {

        }

        public ActionResult UploadDocument()
        {
            return View();
        }

        [HttpPost("FileUpload")]
        public async Task<IActionResult> Index(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);
            var filePaths = new List<string>();

            foreach(var formFile in files)
            {
                if(formFile.Length > 0)
                {
                    var filePath = Path.GetTempFileName();
                    filePaths.Add(filePath);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            return Ok(new { count = files.Count, size, filePaths });
        }
    }
}
