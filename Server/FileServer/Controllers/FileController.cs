using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FileServer.Controllers
{
    public class FileController : Controller
    {
        private string _folderName = "VersionFiles";
        private static string _launcherName = Directory.GetCurrentDirectory() + "\\LauncherFiles";
        [Route("api/GetFileList")]
        [HttpGet]
       public IActionResult GetFileList(string versionName)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory() + "\\" + _folderName, versionName + "\\FilesList.json");
            if (!Directory.Exists(_folderName))
            {
                Directory.CreateDirectory(_folderName);
            }
            if (System.IO.File.Exists(filePath))
            {
                var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                return File(stream, "application/octet-stream", "FilesList.json");
            }
            else
            {
                return NotFound();
            }
        }
        [Route("api/GetFile")]
        [HttpGet]
        public IActionResult GetFile(string versionName, string fileName)
        {
            string filePath = Path.Combine(_folderName, versionName + "/" + fileName);
            if (!Directory.Exists(_folderName))
            {
                Directory.CreateDirectory(_folderName);
            }
            if (System.IO.File.Exists(filePath))
            {
                var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                return File(stream, "application/octet-stream", fileName);
            }
            else
            {
                return NotFound();
            }
        }

        [Route("api/GetLauncher")]
        [HttpGet]
        public IActionResult GetLauncher()
        {
            string filePath = Path.Combine(_launcherName, "Launcher");
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            if (System.IO.File.Exists(filePath + "\\LauncherRelease.exe"))
            {
                var stream = new FileStream(filePath + "\\LauncherRelease.exe", FileMode.Open, FileAccess.Read);
                return File(stream, "application/octet-stream", "LauncherRelease.exe");
            }
            else
            {
                return NotFound();
            }
        }


        [Route("api/GetUpdater")]
        [HttpGet]
        public IActionResult GetUpdater()
        {
            string filePath = Path.Combine(_launcherName, "Launcher");
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            if (System.IO.File.Exists(filePath + "\\Updater.exe"))
            {
                var stream = new FileStream(filePath + "\\Updater.exe", FileMode.Open, FileAccess.Read);
                return File(stream, "application/octet-stream", "Updater.exe");
            }
            else
            {
                return NotFound();
            }
        }

        [Route("api/GetLauncherVersion")]
        [HttpGet]
        public IActionResult GetLauncherVersion()
        {
            string filePath = _launcherName + "\\LauncherVersion.ini";
            if (!Directory.Exists(_launcherName))
            {
                Directory.CreateDirectory(_launcherName);
            }
            if (System.IO.File.Exists(filePath))
            {
                 var stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Read);
                var readStream = new StreamReader(stream);
                return Ok(readStream.ReadLine());
            }
            else
            {
                return NotFound();
            }
        }

        [Route("api/GetDirectories")]
        [HttpGet]
        public IActionResult GetDirectories()
        {
            string pathString = Directory.GetCurrentDirectory() + "\\" + _folderName;
            var dirs = Directory.GetDirectories(pathString);
            for(int i = 0; i < dirs.Length; i++)
            {
                dirs[i] = dirs[i].Remove(0, pathString.Length + 1);
            }
            var versions = new Version();
            versions.Versions = new List<string>(dirs);
            var jsonRet = JsonConvert.SerializeObject(versions);
            return Ok(versions);
        }
    }
}
