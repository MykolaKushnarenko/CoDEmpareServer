using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using CoDEmpare.Algorithm;
using CoDEmpare.Tokkin;


namespace TextGUIModule
{
    public class Analysis
    {
        private Normalization _normalize;
        public List<string> CompliteCodeMain { get; private set; } = new List<string>();
        public List<string> CompliteCodeChild { get; } = new List<string>();
        private TokkinFactory _factory = null;
        private ATokkining _aTokkining = null;


        public void RunAnalysis(string language, byte[] code)
        {
            _normalize = new Normalization(code);
            CompliteCodeMain = _normalize.Normal();
            _factory = new TokkinFactory(language);
            _aTokkining = _factory.Create();
            _aTokkining?.Tokening(CompliteCodeMain);
        }

        public void SetCodeMainFromDB(string code)
        {
            CompliteCodeMain.Clear();
            for (int i = 0; i < code.Length; i++)
            {
                CompliteCodeMain.Add(code[i].ToString());
            }
        }
        public void SetCodeChildFromDB(string code)
        {
            CompliteCodeChild.Clear();
            for (int i = 0; i < code.Length; i++)
            {
                CompliteCodeChild.Add(code[i].ToString());
            }
        }
        public List<string> InserToDB()
        {
            List<string> grams = new List<string>();
            for (int i = 0; i < CompliteCodeMain.Count - 2; i++)
            {
                string threeGram = CompliteCodeMain[i] + CompliteCodeMain[i + 1] + CompliteCodeMain[i + 2];
                grams.Add(threeGram);
                
            }

            return grams;
        }

        public string FileName(string path)
        {
            string result;
            result = Path.GetFileName(path);
            return result;
        }
        //public string GetVersion(string path)
        //{
        //    FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo();
        //    return myFileVersionInfo.FileVersion;
        //}
        public string GetHash(string path)
        {
            using (FileStream stream = File.OpenRead(path))
            {
                SHA256Managed sha = new SHA256Managed();
                byte[] hash = sha.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", String.Empty);
            }
        }
        public string GetNormalizeCode()
        {
            string normalJoin = string.Join("", CompliteCodeMain.ToArray());
            return normalJoin;
        }

        public double ResultAlgorithm(int numberOfAlg)
        {
            AlgorithmFactory fuctory = new AlgorithmFactory(numberOfAlg, CompliteCodeMain, CompliteCodeChild);
            IAlgorithm algorithm = fuctory.Create();
            algorithm?.CompareRes();
            return algorithm.Result;
        }
    }
}
