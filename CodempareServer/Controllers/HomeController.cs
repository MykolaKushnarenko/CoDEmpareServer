using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using TextGUIModule;

namespace COURCEClientServer2.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        private List<string> _result = new List<string>();
        private DataBaseLite _db = new DataBaseLite();
        public ActionResult Index()
        {
            
            return View("MainPage");
        }
        [HttpPost]
        public async Task<string> GetNumer(string sName)
        {
            string s = await Task.Run(()=> JsonConvert.DeserializeObject<string>(sName));
            return await Task.Run(() => JsonConvert.DeserializeObject<string>($"Hello {sName}"));
        }

        [HttpPost]
        public async Task<string> GetComipeType(string lang)
        {
            string result = await Task.Run(() =>
            {
                _result = _db.GetCompile(lang);
                string s = JsonConvert.SerializeObject(_result);
                return s;
            });
            return result;
        }

        [HttpPost]
        public string TwoParams(string a, string b)
        {
            
            int first = JsonConvert.DeserializeObject<int>(a);
            int two = JsonConvert.DeserializeObject<int>(b);
            string result = JsonConvert.SerializeObject(a + b);
            return result;
        }

        [HttpPost]
        public async Task<string> AddCode(AddingCodeObject param)
        {
            bool isOver = await _db.AddingSubmit(param.Name, param.Description, param.CompileType, param.Code, param.IsSearch, param.FileMane);

            _result.Clear();
            _result.Add(JsonConvert.SerializeObject(isOver));
            if (param.IsSearch)
            {
                _result.Add(_db.GetOrignCodeFromId(_db.IdMainFileForHist));
                _result.Add(_db.GetOrignCodeFromId(_db.IdiDenticalFie));
                _db.SetCodeMain(_db.IdMainFileForHist);
                _db.SetCodeChild(_db.IdiDenticalFie);
                _result.Add(String.Format("Levenshtein Distance : {0:0.##}", _db.Code.ResultAlgorithm(1)));
                _result.Add(String.Format("WShiling : {0:0.##}", _db.Code.ResultAlgorithm(2)));
                _result.Add(String.Format("Haskel : {0:0.##}", _db.Code.ResultAlgorithm(0)));
            }
            return JsonConvert.SerializeObject(_result);
        }
    }
}