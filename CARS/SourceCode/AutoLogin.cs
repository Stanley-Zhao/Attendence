using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO.IsolatedStorage;

namespace CARS.SourceCode
{
    public class LoginInfo
    {
        private string userName = string.Empty;

        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }
        private string password = string.Empty;

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public override string ToString()
        {
            string name = CryptographyStuff.AES_EncryptString(userName);
            string pw = CryptographyStuff.AES_EncryptString(password);

            return name + "||" + pw;
        }

        public LoginInfo() { }
        public LoginInfo(string value)
        {
            string[] values = value.Split(new string[] { "||" }, StringSplitOptions.None);

            if (!string.IsNullOrEmpty(values[0]))
                userName = CryptographyStuff.AES_DecryptString(values[0]);
            if (!String.IsNullOrEmpty(values[1]))
                password = CryptographyStuff.AES_DecryptString(values[1]);
        }
    }

    public class CarsConfig
    {
        private const string CONFIGFILE = "config.txt";
        private const string PWFILE = "carspw.txt";
        private static CarsConfig localConfig;

        private bool doSavePW = false;
        public bool DoSavePW
        {
            get
            {
                ReadConfig();
                return doSavePW;
            }
            set
            {
                doSavePW = value;
                SaveConfig();
            }
        }

        private bool showAllRecords = true;
        public bool ShowAllRecords
        {
            get
            {
                ReadConfig();
                return showAllRecords;
            }
            set
            {
                showAllRecords = value;
                SaveConfig();
            }
        }

        private bool doAutoLogin = false;
        public bool DoAutoLogin
        {
            get
            {
                ReadConfig();
                return doAutoLogin;
            }
            set
            {
                doAutoLogin = value;
                SaveConfig();
            }
        }

        public static CarsConfig Instance()
        {
            if (localConfig == null)
                localConfig = new CarsConfig();
            return localConfig;
        }

        private CarsConfig()
        {
            PrepareConfigFile();
            PrepareLoginInfoFile();
        }

        private void ReadConfig()
        {
            try
            {
                string value = Read(CONFIGFILE);
                string[] values = value.Split(new string[] { "||" }, StringSplitOptions.None);
                doSavePW = Convert.ToBoolean(values[0]);
                doAutoLogin = Convert.ToBoolean(values[1]);
                showAllRecords = Convert.ToBoolean(values[2]);
            }
            catch (Exception e)
            {
                doSavePW = false;
                doAutoLogin = false;
                Logger.Instance().Log(MessageType.Error, "ReadConfig()" + Environment.NewLine + e.ToString());
            }
        }

        private void SaveConfig()
        {
            try
            {
                string stringValue = doSavePW.ToString() + "||" + doAutoLogin.ToString() + "||" + showAllRecords.ToString();
                Write(CONFIGFILE, stringValue);
            }
            catch (Exception e)
            {
                Logger.Instance().Log(MessageType.Error, "AutoLogin - SaveConfig()" + Environment.NewLine + e.ToString());
            }
        }

        private void Write(string file, string value)
        {
            //Get　the　storage　file　for　the　application

            //Step1,　you　need　to　access　an　IsolatedStorageFile.
            //This　is　done　by　calling IsolatedStorageFile.GetUserStoreForApplication()　or　IsolatedStorageFile.GetUserStoreForSite().
            //There　are　some　differences　between　the　functions,　but　either　will　get　you　a　private　file　system　in　which　you　can　read　and　write　files.
            IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication(); //　Open&Create　the　file　for　writing

            //Step2,　you　need　create　or　open　your　file　almost　as　you　would　normally.
            //Instead　of　using　FileStream,　you　would　use　IsolatedStorageFileStream　(which　is　derived　from　FileStream).
            //This　means　that　once　you　have　your　stream,　you　can　use　it　anywhere　you　would　have　normally　used　a　normal　FileStream.
            IsolatedStorageFileStream stream = new IsolatedStorageFileStream(file, System.IO.FileMode.Truncate, System.IO.FileAccess.Write, isf); //　Use　the　stream　normally　in　a　TextWriter

            //Step3,　you　read　or　write　as　you　normally　would.
            System.IO.StreamWriter writer = new System.IO.StreamWriter(stream);
            writer.Write(value);
            writer.Flush();

            writer.Close();　//　Close　the　writer　so　data　is　flushed
            stream.Close();　//　Close　the　stream　too			
        }

        private void PrepareLoginInfoFile()
        {
            //Get　the　storage　file　for　the　application

            //Step1,　you　need　to　access　an　IsolatedStorageFile.
            //This　is　done　by　calling IsolatedStorageFile.GetUserStoreForApplication()　or　IsolatedStorageFile.GetUserStoreForSite().
            //There　are　some　differences　between　the　functions,　but　either　will　get　you　a　private　file　system　in　which　you　can　read　and　write　files.
            IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication(); //　Open&Create　the　file　for　writing

            //Step2,　you　need　create　or　open　your　file　almost　as　you　would　normally.
            //Instead　of　using　FileStream,　you　would　use　IsolatedStorageFileStream　(which　is　derived　from　FileStream).
            //This　means　that　once　you　have　your　stream,　you　can　use　it　anywhere　you　would　have　normally　used　a　normal　FileStream.
            IsolatedStorageFileStream stream = new IsolatedStorageFileStream(PWFILE, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write, isf); //　Use　the　stream　normally　in　a　TextWriter

            //Step3,　you　read　or　write　as　you　normally　would.
            System.IO.StreamWriter writer = new System.IO.StreamWriter(stream);
            writer.Write(string.Empty);
            writer.Flush();

            writer.Close();　//　Close　the　writer　so　data　is　flushed
            stream.Close();　//　Close　the　stream　too		
        }

        private void PrepareConfigFile()
        {
            //Get　the　storage　file　for　the　application

            //Step1,　you　need　to　access　an　IsolatedStorageFile.
            //This　is　done　by　calling IsolatedStorageFile.GetUserStoreForApplication()　or　IsolatedStorageFile.GetUserStoreForSite().
            //There　are　some　differences　between　the　functions,　but　either　will　get　you　a　private　file　system　in　which　you　can　read　and　write　files.
            IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication(); //　Open&Create　the　file　for　writing

            //Step2,　you　need　create　or　open　your　file　almost　as　you　would　normally.
            //Instead　of　using　FileStream,　you　would　use　IsolatedStorageFileStream　(which　is　derived　from　FileStream).
            //This　means　that　once　you　have　your　stream,　you　can　use　it　anywhere　you　would　have　normally　used　a　normal　FileStream.
            IsolatedStorageFileStream stream = new IsolatedStorageFileStream(CONFIGFILE, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write, isf); //　Use　the　stream　normally　in　a　TextWriter

            //Step3,　you　read　or　write　as　you　normally　would.
            System.IO.StreamWriter writer = new System.IO.StreamWriter(stream);
            writer.Write(string.Empty);
            writer.Flush();

            writer.Close();　//　Close　the　writer　so　data　is　flushed
            stream.Close();　//　Close　the　stream　too		
        }

        private string Read(string file)
        {
            //Get　the　storage　file　for　the　application

            //Step1,　you　need　to　access　an　IsolatedStorageFile.
            //This　is　done　by　calling IsolatedStorageFile.GetUserStoreForApplication()　or　IsolatedStorageFile.GetUserStoreForSite().
            //There　are　some　differences　between　the　functions,　but　either　will　get　you　a　private　file　system　in　which　you　can　read　and　write　files.
            IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication(); //　Open&Create　the　file　for　writing

            //Step2,　you　need　create　or　open　your　file　almost　as　you　would　normally.
            //Instead　of　using　FileStream,　you　would　use　IsolatedStorageFileStream　(which　is　derived　from　FileStream).
            //This　means　that　once　you　have　your　stream,　you　can　use　it　anywhere　you　would　have　normally　used　a　normal　FileStream.
            IsolatedStorageFileStream stream = new IsolatedStorageFileStream(file, System.IO.FileMode.Open, System.IO.FileAccess.Read, isf); //　Use　the　stream　normally　in　a　TextWriter

            //Step3,　you　read　or　write　as　you　normally　would.
            try
            {
                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                string result = reader.ReadToEnd();

                reader.Close();　//　Close　the　writer　so　data　is　flushed
                stream.Close();　//　Close　the　stream　too

                return result;
            }
            catch
            {
                return string.Empty;
            }
        }

        public void SaveLoginInfo(LoginInfo info)
        {
            try
            {
                Write(PWFILE, info.ToString());
            }
            catch (Exception e)
            {
                Logger.Instance().Log(MessageType.Error, "AutoLogin - SaveLoginInfo()" + Environment.NewLine + e.ToString());
            }
        }

        public LoginInfo LoadLoginInfo()
        {
            try
            {
                string value = Read(PWFILE);

                return new LoginInfo(value);
            }
            catch (Exception e)
            {
                Logger.Instance().Log(MessageType.Error, "AutoLogin - LoadLoginInfo()" + Environment.NewLine + e.ToString());
                return new LoginInfo();
            }
        }
    }
}
