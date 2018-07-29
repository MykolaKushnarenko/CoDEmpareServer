using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using COURCEClientServer2.ObjectDataSender;
using Newtonsoft.Json;
using TextGUIModule;

namespace COURCEClientServer2.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        private List<string> _result = new List<string>();
        private DataBaseAPI _db = new DataBaseAPI();
        public ActionResult Index()
        {
            
            return View("MainPage");
        }

        private void GetResultList()
        {
            _result.Add(_db.GetOrignCodeFromId(_db.IdMainFileForHist));
            _result.Add(_db.GetOrignCodeFromId(_db.IdiDenticalFie));
            _db.SetCodeMain(_db.IdMainFileForHist);
            _db.SetCodeChild(_db.IdiDenticalFie);
            double resVarnFish = _db.Code.ResultAlgorithm(1);
            double resVShiling = _db.Code.ResultAlgorithm(2);
            double resHeskel = _db.Code.ResultAlgorithm(0);
            _result.Add(String.Format("Levenshtein Distance : {0:0.##}", resVarnFish));
            _result.Add(String.Format("WShiling : {0:0.##}", resVShiling));
            _result.Add(String.Format("Haskel : {0:0.##}", resHeskel));
            _db.AddingHistiry(resVarnFish, resVShiling, resHeskel);
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
        public async Task<string> AddCode(AddingCodeObject param)
        {
            bool isOver = await _db.AddingSubmit(param.Name, param.Description, param.CompileType, param.Code, param.IsSearch, param.FileMane);
            _result.Clear();
            _result.Add(JsonConvert.SerializeObject(isOver));
            if (param.IsSearch)
            {
                GetResultList();
            }
            return JsonConvert.SerializeObject(_result);
        }

        [HttpGet]
        public string GetListSubmit()
        {
            List<string> listAllSubmit = _db.DescSubm();
            string result = JsonConvert.SerializeObject(listAllSubmit);
            return result;
        }

        [HttpPost]
        public string SearchFromListSubmit(string tagForSearch)
        {
            _db.SearchIn(tagForSearch);
            _result.Clear();
            _result.Add(JsonConvert.SerializeObject(false));
            GetResultList();
            return JsonConvert.SerializeObject(_result);
        }

        [HttpGet]
        public string GetListHistory()
        {
            List<string> listAllHistory = _db.GetListHistory();
            string result = JsonConvert.SerializeObject(listAllHistory);
            return result;
        }

        [HttpPost]
        public string Registration(RegistrationObject regInfo)
        {
            _db.RegistsAccount(regInfo.Name, regInfo.EMail, regInfo.Password);
            return JsonConvert.SerializeObject(true);
        }

        [HttpPost]
        public string Autification(string login, string password)
        {
            List<object> resultAfterAutif = new List<object>()
            {
                _db.Autification(login, password),
                _db.GetNameFromLogin(login)
        };
            return JsonConvert.SerializeObject(resultAfterAutif);
        }
    }
}