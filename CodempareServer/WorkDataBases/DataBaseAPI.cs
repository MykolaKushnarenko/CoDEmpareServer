using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace TextGUIModule
{
    public class DataBaseAPI
    {
        private string _dataBasesConfig = WebConfigurationManager.ConnectionStrings["DatabaseConnectionString1"].ConnectionString;
        private SqlConnection conn;
        private SqlCommand command;
        private int countLine = 0;
        private int idMainFileForHist = -1;
        private int idiDenticalFie = -1;
        public Analysis Code { get; private set; }
        public int IdMainFileForHist
        {
            get
            {
                return idMainFileForHist;
            }
        }
        public int IdiDenticalFie
        {
            get
            {
                return idiDenticalFie;
            }
        }
        private void CreateTables ()
        {
            string createLanguage = "CREATE TABLE [dbo].[language]("
                                    + "[id][int] IDENTITY(1, 1) NOT NULL,"
                                    + "[denomination] [varchar] (max) NOT NULL,"
                                    + "CONSTRAINT[PK_language] PRIMARY KEY CLUSTERED"
                                    + "( [id] ASC )WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]"
                                    + " ) ON[PRIMARY] TEXTIMAGE_ON[PRIMARY]";
            string createCompile = "CREATE TABLE [dbo].[compiler]("
                                   + "[id][int] IDENTITY(1, 1) NOT NULL,"
                                   + "[fullname] [varchar] (max) NOT NULL,"
                                   + "[compilertype] [varchar] (max) NOT NULL,"
                                   + "[id_language] [int] NOT NULL,"
                                   + "CONSTRAINT[PK_compiler] PRIMARY KEY CLUSTERED ("
                                   + "[id] ASC )WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]"
                                   + ") ON[PRIMARY] TEXTIMAGE_ON[PRIMARY]"
                                   + "ALTER TABLE[dbo].[compiler] WITH CHECK ADD CONSTRAINT[FK_compiler_language] FOREIGN KEY([id_language])"
                                   + "REFERENCES[dbo].[language] ([id])";
            string createCode = "CREATE TABLE [dbo].[code]("
                                + "[id][int] IDENTITY(1, 1) NOT NULL,"
                                + "[origincode] [varchar] (max) NOT NULL,"
                                + "[normalizecode] [varchar] (max) NOT NULL,"
                                + "[lengthorigincode] [int]NOT NULL,"
                                + "[countgram] [int] NOT NULL,"
                                + "[tag] [varchar] (max) NOT NULL,"
                                + "CONSTRAINT[PK_code] PRIMARY KEY CLUSTERED"
                                + "("
                                + "[id] ASC"
                                + ")WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]"
                                + ") ON[PRIMARY] TEXTIMAGE_ON[PRIMARY]";
            string createGram = "CREATE TABLE [dbo].[gram]("
                                + "[id][int] IDENTITY(1, 1) NOT NULL,"
                                + "[gram] [varchar] (max) NOT NULL,"
                                + "CONSTRAINT[PK_gram] PRIMARY KEY CLUSTERED ("
                                + "[id] ASC )WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]"
                                + ") ON[PRIMARY] TEXTIMAGE_ON[PRIMARY]";
            string createKGrams = "CREATE TABLE [dbo].[kgrmams]("
                                  + "[id][int] IDENTITY(1, 1) NOT NULL,"
                                  + "[id_gram] [int] NOT NULL,"
                                  + "[id_code] [int] NULL,"
                                  + "CONSTRAINT[PK_kgrmams] PRIMARY KEY CLUSTERED"
                                  + "([id] ASC )WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]"
                                  + ") ON[PRIMARY]"
                                  + "ALTER TABLE [dbo].[kgrmams]  WITH CHECK ADD  CONSTRAINT [FK_kgrmams_code] FOREIGN KEY([id_code])"
                                  + "REFERENCES[dbo].[code]([id])"
                                  + "ALTER TABLE [dbo].[kgrmams]  WITH CHECK ADD  CONSTRAINT [FK_kgrmams_gram] FOREIGN KEY([id_gram])"
                                  + "REFERENCES[dbo].[gram]([id])";
            string createAccount = "CREATE TABLE [dbo].[account]("
                                + "[id][int] IDENTITY(1, 1) NOT NULL,"
                                + "[password] [varchar] (max) NOT NULL,"
                                + "[email] [varchar] (max) NOT NULL,"
                                + "[image]"
                                + "[varbinary]"
                                + "(max) NULL,"
                                + "CONSTRAINT[PK_account] PRIMARY KEY CLUSTERED"
                                + "("
                                + "[id] ASC"
                                + ")WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]"
                                + ") ON[PRIMARY] TEXTIMAGE_ON[PRIMARY]";
            string createUser = "CREATE TABLE[dbo].[user]("
                                + "[id][int] IDENTITY(1, 1) NOT NULL,"
                                + " [name] [varchar] (max) NOT NULL,"
                                + " [id_account] [int] NULL,"
                                + "CONSTRAINT[PK_user] PRIMARY KEY CLUSTERED"
                                + "("
                                + "[id] ASC"
                                + ")WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]"
                                + ") ON[PRIMARY] TEXTIMAGE_ON[PRIMARY]"
                                + "ALTER TABLE [dbo].[user]  WITH CHECK ADD  CONSTRAINT [FK_user_account] FOREIGN KEY([id_account])"
                                + "REFERENCES[dbo].[account]([id])";
            string createSubmit = "CREATE TABLE [dbo].[submit]("
                                  + "[id][int] IDENTITY(1, 1) NOT NULL,"
                                  + "[description] [varchar](max) NULL,"
                                  + "[datetimesend] [varchar] (max) NOT NULL,"
                                  + "[id_compiler] [int] NOT NULL,"
                                  + "[id_user] [int] NOT NULL,"
                                  + "[tag] [varchar] (max) NOT NULL,"
                                  + "CONSTRAINT[PK_submit] PRIMARY KEY CLUSTERED"
                                  + "( [id] ASC )WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]"
                                  + ") ON[PRIMARY] TEXTIMAGE_ON[PRIMARY]"
                                  + "ALTER TABLE [dbo].[submit]  WITH CHECK ADD  CONSTRAINT [FK_submit_compiler] FOREIGN KEY([id_compiler])"
                                  + "REFERENCES[dbo].[compiler]([id])"
                                  + "ALTER TABLE [dbo].[submit]  WITH CHECK ADD  CONSTRAINT [FK_submit_user] FOREIGN KEY([id_user])"
                                  + "REFERENCES[dbo].[user]([id])";
            string createHistory = "CREATE TABLE [dbo].[history]("
                                   + "[id][int] IDENTITY(1, 1) NOT NULL,"
                                   + "[date] [varchar] (max) NOT NULL,"
                                   + "[state_vshingl] [float] NOT NULL,"
                                   + "[state_distenceliv] [float] NOT NULL,"
                                   + "[state_heskela] [float] NOT NULL,"
                                   + "[id_sublit1] [int] NOT NULL,"
                                   + "[id_submit2] [int] NOT NULL,"
                                   + "CONSTRAINT[PK_history] PRIMARY KEY CLUSTERED"
                                   + "( [id] ASC )WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]"
                                   + ") ON[PRIMARY] TEXTIMAGE_ON[PRIMARY]"
                                   + "ALTER TABLE [dbo].[history]  WITH CHECK ADD  CONSTRAINT [FK_history_submit] FOREIGN KEY([id_sublit1])"
                                   + "REFERENCES[dbo].[submit]([id])"
                                   + "ALTER TABLE [dbo].[history]  WITH CHECK ADD  CONSTRAINT [FK_history_submit1] FOREIGN KEY([id_submit2])"
                                   + "REFERENCES[dbo].[submit]([id])";
            string createFile = "CREATE TABLE [dbo].[file]("
                                + "[id][int] IDENTITY(1, 1) NOT NULL,"
                                + "[name] [varchar] (max) NOT NULL,"
                                + "[ver] [bigint] NULL,"
                                + "[hash] [bigint] NOT NULL,"
                                + "[id_submit] [int] NOT NULL,"
                                + "[id_code] [int] NOT NULL,"
                                + "CONSTRAINT[PK_file] PRIMARY KEY CLUSTERED("
                                + "[id] ASC"
                                + ")WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]"
                                + ")ON[PRIMARY] TEXTIMAGE_ON[PRIMARY]"
                                + "ALTER TABLE [dbo].[file]  WITH CHECK ADD  CONSTRAINT [FK_file_code] FOREIGN KEY([id_code])"
                                + "REFERENCES[dbo].[code]([id])"
                                + "ALTER TABLE [dbo].[file]  WITH CHECK ADD  CONSTRAINT [FK_file_submit] FOREIGN KEY([id_submit])"
                                + "REFERENCES[dbo].[submit]([id])";

            InitTable(createLanguage);
            InitTable(createCompile);
            InitTable(createCode);
            InitTable(createGram);
            InitTable(createKGrams);
            InitTable(createAccount);
            InitTable(createUser);
            InitTable(createSubmit);
            InitTable(createHistory);
            InitTable(createFile);
            DefoultValue();
        }
        public DataBaseAPI()
        {
            conn = new SqlConnection(_dataBasesConfig);
            IsNull();
            command = new SqlCommand();
            Code = new Analysis();
        }

        public List<string> GetListHistory()
        {
            List<string> listHist = new List<string>();
            int countHist = 0;
            string histDesc = "";
            conn.Open();
            using (command = new SqlCommand("select count([history].id) from [history]", conn))
            {
                using (IDataReader r = command.ExecuteReader())
                {
                    if (r.Read())
                    {
                        countHist = r.GetInt32(0);
                    }
                }
            }

            for (int i = 0; i < countHist; i++)
            {
                using (command =
                    new SqlCommand(
                        "select [user].name from [user] join submit on [submit].id_user = [user].id join [history] on [history].id_sublit1 = [submit].id where [history].id = @id",
                        conn))
                {
                    command.Parameters.Add(new SqlParameter("@id", i + 1));
                    using (IDataReader r = command.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            histDesc += "Hist id: " + (i + 1);
                            histDesc += " | ";
                            histDesc += r.GetString(0);
                            histDesc += " | ";
                        }

                    }
                }
                using (command =
                    new SqlCommand(
                        "select [user].name from [user] join [submit] on [submit].id_user = [user].id join [history] on [history].id_submit2 = [submit].id where [history].id = @id",
                        conn))
                {
                    command.Parameters.Add(new SqlParameter("@id", i + 1));
                    using (IDataReader r = command.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            histDesc += r.GetString(0);
                            histDesc += " | ";
                        }
                    }
                }

                using (command =
                    new SqlCommand(
                        "select [history].state_distenceLiv, [history].state_heskela, [history].state_vshingl from [history] where [history].id = @id",
                        conn))
                {
                    command.Parameters.Add(new SqlParameter("@id", i + 1));
                    using (IDataReader r = command.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            histDesc += r.GetDouble(0);
                            histDesc += " | ";
                            histDesc += r.GetDouble(1); 
                            histDesc += " | ";
                            histDesc += r.GetDouble(2);

                        }
                    }
                }

                if (histDesc != "")
                {
                    listHist.Add(histDesc);
                }

                histDesc = "";
            }
            conn.Close();
            return listHist;
        }
        //public string GetInfoSubm(bool isMain)
        //{
        //    string info = "";
        //    conn.Open();
        //    using (command = new SqlCommand("select User.Name, Submit.description from User join Submit on Submit.id_User = User.id join File on File.id_Submit = Submit.id where File.id = @id", conn))
        //    {
        //        if (isMain)
        //        {
        //            command.Parameters.Add(new SqlParameter("@id", idMainFileForHist));
        //        }
        //        else
        //            command.Parameters.Add(new SqlParameter("@id", idiDenticalFie));

        //        using (IDataReader r = command.ExecuteReader())
        //        {
        //            if (r.Read())
        //            {
        //                info = "Имя: " + r.GetString(0);
        //                info += "\r";
        //                info +="Описание: "+ r.GetString(1);

        //            }

        //        }
        //    }

        //    conn.Close();
        //    return info;
        //}
        private string TextWrite(byte[] code)
        {
            Stream stream = new MemoryStream(code);
            string s = "";
            using (StreamReader file = new StreamReader(stream))
            {
                while (!file.EndOfStream)
                {
                    s += file.ReadLine();
                    s += "\r\n";
                    countLine++;
                }
            }

            return s;
        }

        public void AddingHistiry(double resVarnFis, double resVShiling, double resHeskel)
        {
            conn.Open();
            int idMainSub = -1;
            int idChaildSub = -1;
            using (command = new SqlCommand("select [submit].id from [submit] join [file] on [submit].id = [file].id_submit where [file].id = @id", conn))
            {
                command.Parameters.Add(new SqlParameter("@id", idMainFileForHist));
                using (IDataReader r = command.ExecuteReader())
                {
                    if (r.Read())
                    {
                        idMainSub = r.GetInt32(0);

                    }

                }
            }
            using (command = new SqlCommand("select [submit].id from [submit] join [file] on [submit].id = [file].id_submit where [file].id = @id", conn))
            {
                command.Parameters.Add(new SqlParameter("@id", idiDenticalFie));
                using (IDataReader r = command.ExecuteReader())
                {
                    if (r.Read())
                    {
                        idChaildSub = r.GetInt32(0);

                    }

                }
            }
            using (command = new SqlCommand(
                "insert into history (date,state_vshingl,state_distenceliv,state_heskela, id_sublit1, id_submit2 ) values(@Date,@State_VShingl,@State_DistenceLiv,@State_Heskela, @id_Sublit1, @id_Submit2 )", conn))
            {
                command.Parameters.Add(new SqlParameter("@Date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                command.Parameters.Add(new SqlParameter("@State_VShingl", resVShiling));
                command.Parameters.Add(new SqlParameter("@State_DistenceLiv", resVarnFis));
                command.Parameters.Add(new SqlParameter("@State_Heskela", resHeskel));
                command.Parameters.Add(new SqlParameter("@id_Sublit1", idMainSub));
                command.Parameters.Add(new SqlParameter("@id_Submit2", idChaildSub));
                command.ExecuteNonQuery();
            }
            conn.Close();
        }
        public void SearchIn(string tagAddingSub)
        {
            conn.Open();
            List<string> gram = new List<string>();
            Dictionary<int,string> nameFile = new Dictionary<int, string>();
            Dictionary<int, double > resCode = new Dictionary<int, double>();
            Analysis search = new Analysis();
            bool isDel = false;
            string mainCode = "";
            string childCode = "";
            double maxRes = Double.MinValue; 
            using (command = new SqlCommand("select [file].id from [file] join [submit] on [file].id_submit = [submit].id where [submit].tag = @Tag", conn))
            {
                command.Parameters.Add(new SqlParameter("@Tag", tagAddingSub));
                using (IDataReader r = command.ExecuteReader())
                {
                    if (r.Read())
                    {
                        idMainFileForHist = r.GetInt32(0);
                        
                    }

                }
            }

            using (command = new SqlCommand(
                "select [gram].gram from [gram] join [kgrmams] on [kgrmams].id_gram = [gram].id join [code] on [kgrmams].id_code = [code].id join [file] on [file].id_code = [code].id where [file].id = @id", conn))
            {
                command.Parameters.Add(new SqlParameter("@id", idMainFileForHist));
                using (IDataReader r = command.ExecuteReader())
                {
                    while (r.Read())
                    {
                        gram.Add(r.GetString(0));
                    }

                }
            }


            for (int i = 0; i < gram.Count/2; i++)
            {
                using (command = new SqlCommand(
                    "select DISTINCT [file].name, [file].id from [file] join [code] on [file].id_code = [code].id join [kgrmams] on [kgrmams].id_code = [code].id " +
                    "join [gram] on [kgrmams].id_gram = [gram].id where [gram].gram = @gram and [file].id != @idFile", conn))
                {

                    command.Parameters.Add(new SqlParameter("@gram", gram[i]));
                    command.Parameters.Add(new SqlParameter("@idFile", idMainFileForHist));
                    using (IDataReader r = command.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            if (!nameFile.ContainsKey(r.GetInt32(1)) && isDel && nameFile.Count > 1)
                            {
                                nameFile.Remove(r.GetInt32(1));
                            }
                            else if (!isDel)
                            {
                                nameFile.Add(r.GetInt32(1),r.GetString(0));
                            }
                            
                        }
                           
                    }
                }
                isDel = true;
            }
            using (command = new SqlCommand(
                "select [code].normalizecode from [code] join [file] on [file].id_code = [code].id where [file].id = @id", conn))
            {
                command.Parameters.Add(new SqlParameter("@id", idMainFileForHist));
                using (IDataReader r = command.ExecuteReader())
                {
                    if (r.Read())
                        mainCode += r.GetString(0);
                }
            }
            search.SetCodeMainFromDB(mainCode);
            foreach (int key in nameFile.Keys)
            {
                using (command = new SqlCommand(
                    "select [code].normalizecode from [code] join [file] on [file].id_code = [code].id where [file].id = @id", conn))
                {
                    command.Parameters.Add(new SqlParameter("@id", key));
                    using (IDataReader r = command.ExecuteReader())
                    {
                        if (r.Read())
                            childCode += r.GetString(0);
                    }
                }
                search.SetCodeChildFromDB(childCode);
                resCode.Add(key,((search.ResultAlgorithm(0)+ search.ResultAlgorithm(1) + search.ResultAlgorithm(2)) /3));
            }

            foreach (int Key in resCode.Keys)
            {
                if (resCode[Key] > maxRes)
                {
                    maxRes = resCode[Key];
                    idiDenticalFie = Key;
                }
                    
                    
            }
            conn.Close();
        }

        //public bool IsNotEnpty()
        //{
        //    conn.Open();
        //    int res = 0;
        //    using (command = new SqlCommand(
        //        "select count(id) from File; ", conn))
        //    {

        //        using (IDataReader r = command.ExecuteReader())
        //        {
        //            while (r.Read())
        //            {
        //                res = r.GetInt32(0);
        //            }

        //        }
        //    }
        //    conn.Close();
        //    if (res > 5)
        //    {
        //        return true;
        //    }
        //    else return false;
        //}
        public List<string> DescSubm()
        {
            List<string> allDesc = new List<string>();
            conn.Open();
            string s = "";
            using (command = new SqlCommand(
                "SELECT [file].name, [user].name, [submit].tag from [user] join [submit] on [submit].id_user = [user].id join [file] on [file].id_submit = [submit].id;", conn))
            {
                
                using (IDataReader r = command.ExecuteReader())
                {
                    while (r.Read())
                    {
                        s += r.GetString(0);
                        s += "|";
                        s += r.GetString(1);
                        s += "|";
                        s += r.GetString(2);
                        allDesc.Add(s);
                        s = "";
                    }
                        
                }
            }
            conn.Close();
            return allDesc;
        }
        private void SetCodeMain(string tag)
        {
            string tokins = GetTokingCode(tag);
            Code.SetCodeMainFromDB(tokins);

        }
        public void SetCodeMain(int id)
        {
            string tagSubmit = "";
            conn.Open();
            using (command = new SqlCommand(
                "select [submit].tag from [submit] join [file] on [file].id_submit = [submit].id where [file].id = @id", conn))
            {
                command.Parameters.Add(new SqlParameter("@id", idMainFileForHist));
                using (IDataReader r = command.ExecuteReader())
                {
                    if (r.Read())
                        tagSubmit += r.GetString(0);
                }
            }

            conn.Close();
            SetCodeMain(tagSubmit);
        }
        public void SetCodeChild(int id)
        {
            string tagSubmit = "";
            conn.Open();
            using (command = new SqlCommand(
                "select [submit].tag from [submit] join [file] on [file].id_submit = [submit].id where [file].id = @id", conn))
            {
                command.Parameters.Add(new SqlParameter("@id", idiDenticalFie));
                using (IDataReader r = command.ExecuteReader())
                {
                    if (r.Read())
                        tagSubmit += r.GetString(0);
                }
            }

            conn.Close();
            SetCodeChild(tagSubmit);
        }
        private void SetCodeChild(string tag)
        {
            string tokins = GetTokingCode(tag);
            Code.SetCodeChildFromDB(tokins);

        }
        private string GetTokingCode(string tag)
        {
            string originCode = "";
            conn.Open();
            using (command = new SqlCommand(
                "SELECT [code].normalizecode from [code] join [file] on [file].id_code = [code].id join [submit] on [file].id_submit = [submit].id where [submit].tag like @Tag;", conn))
            {
                command.Parameters.Add(new SqlParameter("@Tag", tag));
                using (IDataReader r = command.ExecuteReader())
                {
                    if (r.Read())
                        originCode += r.GetString(0);
                }
            }
            conn.Close();
            return originCode;
        }

        public string GetOrignCodeFromId(int id)
        {
            string tagSubmit = "";
            conn.Open();
            using (command = new SqlCommand(
                "select [submit].tag from [submit] join [file] on [file].id_submit = [submit].id where [file].id = @id", conn))
            {
                command.Parameters.Add(new SqlParameter("@id", id));
                using (IDataReader r = command.ExecuteReader())
                {
                    if (r.Read())
                        tagSubmit += r.GetString(0);
                }
            }
            conn.Close();
            return GetOrignCode(tagSubmit);
        }
        private string GetOrignCode(string tag)
        {
            string originCode = "";
            conn.Open();
            using (command = new SqlCommand(
                "SELECT [code].origincode from [code] join [file] on [file].id_code = [code].id join submit on [file].id_submit = [submit].id where [submit].tag like @Tag;", conn))
            {
                command.Parameters.Add(new SqlParameter("@Tag", tag));
                using (IDataReader r = command.ExecuteReader())
                {
                    if (r.Read())
                        originCode += r.GetString(0);
                }
            }
            conn.Close();
            return originCode;
        }
        /*------------------Code---------------------*/
        private void AddingCode(byte[] code, string lang, string tagOnSumb, bool serachNow, string nameFile)
        {
            
            Code.RunAnalysis(lang, code);
            string normaList = Code.GetNormalizeCode();

            string origncode = TextWrite(code);
            List<string> gramsList = Code.InserToDB();
            int countGram = gramsList.Count;
            string tag = DateTime.Now.ToString() + countGram.ToString();
            int indexCode = -1;
            int indexSubm = -1;
            using (command = new SqlCommand(
                "insert into [code] (originCode,normalizeCode,lengthorigincode,countGram, tag ) values(@OriginCode,@NormalizeCode,@LengthOriginCode,@CountGram,@Tag)", conn))
            {
                command.Parameters.Add(new SqlParameter("@OriginCode", origncode));
                command.Parameters.Add(new SqlParameter("@NormalizeCode", normaList));
                command.Parameters.Add(new SqlParameter("@LengthOriginCode", countLine));
                command.Parameters.Add(new SqlParameter("@CountGram", countGram));
                command.Parameters.Add(new SqlParameter("@Tag", tag));
                command.ExecuteNonQuery();
            }
            using (command = new SqlCommand(
                "SELECT id from [code] where tag like @tag", conn))
            {
                command.Parameters.Add(new SqlParameter("@tag", tag));
                using (IDataReader r = command.ExecuteReader())
                {
                    if (r.Read())
                        indexCode = r.GetInt32(0);
                }
            }
            int gramIsTables = 0;
            int idgram = -1;
            for (int i = 0; i < gramsList.Count-1; i++)
            {
                using (command = new SqlCommand(
                    "SELECT COUNT(id) from [gram] where gram like @gram", conn))
                {
                    command.Parameters.Add(new SqlParameter("@gram", gramsList[i]));
                    using (IDataReader r = command.ExecuteReader())
                    {
                        if (r.Read())
                            gramIsTables = r.GetInt32(0);
                    }
                }

                if (gramIsTables == 0)
                {
                    using (command = new SqlCommand(
                        "insert into [gram] (gram) values(@Gram)", conn))
                    {
                        command.Parameters.Add(new SqlParameter("@Gram", gramsList[i]));
                        command.ExecuteNonQuery();
                    }
                }
                using (command = new SqlCommand(
                    "SELECT id from [gram] where gram like @gram", conn))
                {
                    command.Parameters.Add(new SqlParameter("@gram", gramsList[i]));
                    using (IDataReader r = command.ExecuteReader())
                    {
                        if (r.Read())
                            idgram = r.GetInt32(0);
                    }
                }
                using (command = new SqlCommand(
                    "insert into [kgrmams] (id_gram,id_code) values(@id_Gram,@id_Code)", conn))
                {
                    command.Parameters.Add(new SqlParameter("@id_Gram", idgram));
                    command.Parameters.Add(new SqlParameter("@id_Code", indexCode));
                    command.ExecuteNonQuery();
                }
            }
            using (command = new SqlCommand(
                "SELECT id from [submit] where tag like @Tag", conn))
            {
                command.Parameters.Add(new SqlParameter("@Tag", tagOnSumb));
                using (IDataReader r = command.ExecuteReader())
                {
                    if (r.Read())
                        indexSubm = r.GetInt32(0);
                }
            }

            using (command = new SqlCommand(
                "insert into [file] (name,ver,hash,id_submit, id_code ) values(@Name,@Ver,@Hash,@id_Submit, @id_Code )", conn))
            {
                command.Parameters.Add(new SqlParameter("@Name", nameFile));
                command.Parameters.Add(new SqlParameter("@Ver", 1));
                command.Parameters.Add(new SqlParameter("@Hash", 10));
                command.Parameters.Add(new SqlParameter("@id_Submit", indexSubm));
                command.Parameters.Add(new SqlParameter("@id_Code", indexCode));
                command.ExecuteNonQuery();
            }
            conn.Close();
            if (serachNow)
            {
                SearchIn(tagOnSumb);
                
            }
        }
        /*------------------Code---------------------*/
        /*------------------Submit---------------------*/
        public Task<bool> AddingSubmit(string name, string desc,string compolType, byte[] code, bool serachNow, string nameFile)
        {
            return Task.Run(() =>
            {
                conn.Open();
                string language = "";
                int amount = 0;
                int indexUser = 0;
                int indexCompil = 0;
                string tahSubmit = "";
                try
                {
                    using (command = new SqlCommand(
                          "SELECT COUNT(id) from [user] where name like @name", conn))
                    {
                        command.Parameters.Add(new SqlParameter("@name", name));
                        using (IDataReader r = command.ExecuteReader())
                        {
                            if (r.Read())
                                amount = r.GetInt32(0);
                        }
                    }

                    if (amount == 0)
                    {
                        using (command = new SqlCommand(
                               "insert into [user] (name) values(@Name)", conn))
                        {
                            command.Parameters.Add(new SqlParameter("@Name", name));
                            command.ExecuteNonQuery();
                        }
                    }

                    using (command = new SqlCommand(
                        "SELECT id from [user] where name like @name", conn))
                    {
                        command.Parameters.Add(new SqlParameter("@name", name));
                        using (IDataReader r = command.ExecuteReader())
                        {
                            if (r.Read())
                                indexUser = r.GetInt32(0);
                        }
                    }
                    using (command = new SqlCommand(
                        "SELECT id from [compiler] where compilerType like @copmType", conn))
                    {
                        command.Parameters.Add(new SqlParameter("@copmType", compolType));
                        using (IDataReader r = command.ExecuteReader())
                        {
                            if (r.Read())
                                indexCompil = r.GetInt32(0);
                        }
                    }
                    using (command = new SqlCommand(
                        "insert into [submit] (tag, description, datetimesend, id_compiler, id_user) values(@Tag,@dect,@Data,@indComp,@indUse)", conn))
                    {
                        string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        tahSubmit = name + time;
                        command.Parameters.Add(new SqlParameter("@dect", desc));
                        command.Parameters.Add(new SqlParameter("@Data", time));
                        command.Parameters.Add(new SqlParameter("@indComp", indexCompil));
                        command.Parameters.Add(new SqlParameter("@indUse", indexUser));
                        command.Parameters.Add(new SqlParameter("@Tag", tahSubmit));
                        command.ExecuteNonQuery();
                    }
                    using (command = new SqlCommand(
                        "SELECT denomination from [language] join [compiler] on compiler.id_language = language.id where compilerType like @copmType", conn))
                    {
                        command.Parameters.Add(new SqlParameter("@copmType", compolType));
                        using (IDataReader r = command.ExecuteReader())
                        {
                            if (r.Read())
                                language = r.GetString(0);
                        }
                    }

                }
                catch
                {
                    return false;
                }

                AddingCode(code, language, tahSubmit, serachNow, nameFile);
                conn.Close();
                return true;
            });
            
        }

        public List<string> GetCompile(string lang)
        {
            int idlang = -1;
            conn.Open();
            List<string> compList = new List<string>();
            using (command = new SqlCommand(
                "SELECT id from language denomination where denomination like @lang", conn))
            {
                command.Parameters.Add(new SqlParameter("@lang", lang));
                using (IDataReader r = command.ExecuteReader())
                {
                    if (r.Read())
                        idlang = r.GetInt32(0);
                }
            }
            using (command = new SqlCommand(
                "SELECT compilertype from compiler where id_language = @id_lg", conn))
            {
                command.Parameters.Add(new SqlParameter("@id_lg", idlang));
                using (IDataReader r = command.ExecuteReader())
                {
                    if (r.Read())
                        compList.Add(r.GetString(0));
                }
            }
            conn.Close();
            return compList;
        }
        /*------------------Submit---------------------*/
        /*------------Account---------------*/
        public void RegistsAccount(string name, string email, string password)
        {
            int idAccount = 0;
            conn.Open();
            using (command = new SqlCommand("Insert into account(password,email)" +
                                               "Values(@pass, @email);", conn))
            {
                command.Parameters.Add(new SqlParameter("@pass", CryptographyPassword(password)));
                command.Parameters.Add(new SqlParameter("@email", email));
                command.ExecuteNonQuery();
            }

            using (command = new SqlCommand("select id from [account] where [account].email = @email;", conn))
            {
                command.Parameters.Add(new SqlParameter("@email", email));
                using (IDataReader r = command.ExecuteReader())
                {
                    if (r.Read())
                    {
                        idAccount = r.GetInt32(0);
                    }
                    
                }
            }

            using (command = new SqlCommand("insert into [user](Name,id_account)" +
                                               "Values(@Name,@id_account);", conn))
            {
                command.Parameters.Add(new SqlParameter("@Name", name));
                command.Parameters.Add(new SqlParameter("@id_account", idAccount));
                command.ExecuteNonQuery();
            }
            conn.Close();
        }
        /*------------Account---------------*/
        /*------------Autification---------------*/

        public bool Autification(string email, string password)
        {
            string pass = "";
            conn.Open();
            using (command =
                new SqlCommand("select [account].password from [account] where [account].email = @email", conn))
            {
                command.Parameters.Add(new SqlParameter("@email", email));
                using (IDataReader r = command.ExecuteReader())
                {
                    if (r.Read())
                    {
                        pass = r.GetString(0);
                    }
                }
            }
            conn.Close();
            if (CryptographyPassword(password) == pass)
            {
                return true;
            }
            else
                return false;
        }

        private string CryptographyPassword(string password)
        {
            byte[] data = Encoding.ASCII.GetBytes(password);
            byte[] passwordInBype;
            SHA512 shaM = new SHA512Managed();
            passwordInBype = shaM.ComputeHash(data);
            string hashPassword = Encoding.ASCII.GetString(passwordInBype);
            return hashPassword;
        }


        private bool Connections()
        {
            conn = new SqlConnection(_dataBasesConfig);
            try
            {
                conn.Open();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void DefoultValue()
        {
            string defLang = "insert into language (denomination)"
                             + "values('C#'),"
                             + "('Java'), "
                             + "('C'), "
                             + "('C++'); ";
            InitTable(defLang);
            string defCompil = "insert into compiler (FullName,CompilerType,id_language)"
                               + "values('Comol for C#. Version 2.1', 'dotnet',1),"
                               + "('Comol for Java. Version 2.1', 'javavm',2), "
                               + "('Comol for C. Version 2.0', 'custom',3), "
                               + "('Comol for C+. Version 2.1', 'native',4); ";
            InitTable(defCompil);

        }

        private void InitTable(string commands)
        {   
            conn.Open();
            command = new SqlCommand(commands, conn);
            command.ExecuteNonQuery();
            conn.Close();
        }
        private bool IsNull()
        {
            conn.Open();
            DataTable allTableInDB = conn.GetSchema("Tables");
            conn.Close();
            if (allTableInDB.Rows.Count > 0)
            {
                return false;

            }
            else
            {
                CreateTables();
                return true;
            }
            
        }

        public string GetNameFromLogin(string login)
        {
            string name = "";
            conn.Open();
            using (command = new SqlCommand("select name from [user] join [account] on [user].id_account = [account].id where [account].email = @email", conn))
            {
                command.Parameters.Add(new SqlParameter("@email", login));
                using (IDataReader r = command.ExecuteReader())
                {
                    if (r.Read())
                    {
                        name = r.GetString(0);
                    }

                }
            }
            conn.Close();
            return name;
        }

        public void UpdateImage(string nameUser, byte[] image)
        {
            conn.Open();
            using (command = new SqlCommand("UPDATE [account] SET [account].image = @image FROM [account] join [user] on [user].id_account = [account].id where [user].name = @name", conn))
            {
                command.Parameters.Add(new SqlParameter("@image", image));
                command.Parameters.Add(new SqlParameter("@name", nameUser));
                command.ExecuteNonQuery();
            }
            conn.Close();
        }

        public byte[] GetImageUser(string email)
        {
            conn.Open();
            byte[] getingImage = new byte[] { };
            using (command = new SqlCommand("select [account].image from [account] where [account].email = @email", conn))
            {
                command.Parameters.Add(new SqlParameter("@email", email));
                SqlDataReader myDataReader;
                myDataReader = command.ExecuteReader();

                while (myDataReader.Read())
                {
                    getingImage = new byte[((byte[])myDataReader["image"]).Length];
                    getingImage = (byte[])myDataReader["image"];
                    break;
                }
            }
            conn.Close();
            return getingImage;
        }
    }
}
