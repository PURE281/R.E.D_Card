using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using DG.Tweening.Plugins.Core.PathCore;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;

namespace CSVToolKit
{
    public class CSVParser : MonoBehaviour
    {
        private CSVParser() { }
        private static CSVParser instance = null;
        public static CSVParser Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CSVParser();
                }
                return instance;
            }
        }

        private char lineSeperater = '\n';
        private char fieldSeperator = ',';

        void SetLineSeparator(char seperator)
        {
            lineSeperater = seperator;
        }

        void SetFieldSeperator(char seperator)
        {
            fieldSeperator = seperator;
        }

        public List<List<string>> ReadData(string path, string filename)
        {
            List<List<string>> result = new List<List<string>>();
            try
            {
                this.copy(filename);
                var source = new StreamReader((GetPath()+path) + "/" + filename);
                var fileContents = source.ReadToEnd();
                source.Close();
                var records = fileContents.Split(lineSeperater);


                // TextAsset csvFile = Resources.Load<TextAsset>(filename);
                // string[] records = csvFile.text.Split (lineSeperater);
                foreach (string record in records)
                {
                    List<string> row = new List<string>();
                    string[] fields = record.Split(fieldSeperator);
                    foreach (string field in fields)
                        row.Add(field);

                    result.Add(row);
                }

            }
            catch (Exception ex)
            {
                Debug.LogError($"Something went wrong while reading content{ex.Message}");
            }
            return result;
        }
        public List<List<string>> ReadPlayerCardInfos(string path, string filename)
        {
            List<List<string>> result = new List<List<string>>();
            string _temPath = (GetPath() + path) + "/" + filename;
            if (!File.Exists(_temPath))
            {
                //初始化
                FileStream fileStream = File.Create(_temPath);
                fileStream.Close();
                try
                {
                    string data = "";
                    foreach (string value in new List<string>() { "CID", "Proficiency" })
                        data += value + fieldSeperator;

                    File.AppendAllText(_temPath, data);

#if UNITY_EDITOR
                    UnityEditor.AssetDatabase.Refresh();
#endif
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Something went wrong while writing content{ex.Message}");
                }
            }
            try
            {
                var source = new StreamReader((GetPath() + path) + "/" + filename);
                var fileContents = source.ReadToEnd();
                source.Close();
                var records = fileContents.Split(lineSeperater);
                foreach (string record in records)
                {
                    List<string> row = new List<string>();
                    string[] fields = record.Split(fieldSeperator);
                    foreach (string field in fields)
                        row.Add(field);

                    result.Add(row);
                }

            }
            catch (Exception ex)
            {
                Debug.LogError($"Something went wrong while reading content{ex.Message}");
            }
            return result;
        }

        public List<List<string>> ReadBattleCardInfos(string path, string filename)
        {
            List<List<string>> result = new List<List<string>>();
            string _temPath = (GetPath() + path) + "/" + filename;
            if (!File.Exists(_temPath))
            {
                //初始化
                FileStream fileStream = File.Create(_temPath);
                fileStream.Close();
                try
                {
                    string data = "";
                    foreach (string value in new List<string>() { "CID", "Proficiency" })
                        data += value + fieldSeperator;

                    File.AppendAllText(_temPath, data);

#if UNITY_EDITOR
                    UnityEditor.AssetDatabase.Refresh();
#endif
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Something went wrong while writing content{ex.Message}");
                }
            }
            try
            {
                var source = new StreamReader((GetPath() + path) + "/" + filename);
                var fileContents = source.ReadToEnd();
                source.Close();
                var records = fileContents.Split(lineSeperater);
                foreach (string record in records)
                {
                    List<string> row = new List<string>();
                    string[] fields = record.Split(fieldSeperator);
                    foreach (string field in fields)
                        row.Add(field);

                    result.Add(row);
                }

            }
            catch (Exception ex)
            {
                Debug.LogError($"Something went wrong while reading content{ex.Message}");
            }
            return result;
        }
        public void AddData(string path, string filename, List<string> values)
        {
            try
            {
                this.copy(filename);
                string data = lineSeperater.ToString();
                foreach (string value in values)
                    data += value + fieldSeperator;

                File.AppendAllText(GetPath()+path + "/" + filename, data);

#if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
#endif
            }
            catch (Exception ex)
            {
                Debug.LogError("Something went wrong while writing content");
            }
        }
        public void AddPlayerCardData(string path, List<int> values)
        {
            try
            {
                string data = lineSeperater.ToString();
                foreach (int value in values)
                    data += value + fieldSeperator;

                File.AppendAllText(GetPath() + path, data);

#if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
#endif
            }
            catch (Exception ex)
            {
                Debug.LogError("Something went wrong while writing content");
            }
        }
        public void AddBattleCardData(string path, List<int> values)
        {
            try
            {
                string data = lineSeperater.ToString();
                foreach (int value in values)
                    data += value + fieldSeperator;

                File.AppendAllText(GetPath() + path, data);

#if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
#endif
            }
            catch (Exception ex)
            {
                Debug.LogError("Something went wrong while writing content");
            }
        }
        private static string GetPath()
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {

            }
#if UNITY_EDITOR
            return Application.persistentDataPath;
#elif UNITY_ANDROID
			return Application.persistentDataPath;
#elif UNITY_IPHONE
			return GetiPhoneDocumentsPath();
#else
			return Application.dataPath;
#endif
        }

        private static string GetiPhoneDocumentsPath()
        {
            string path = Application.dataPath.Substring(0, Application.dataPath.Length - 5);
            path = path.Substring(0, path.LastIndexOf('/'));
            return path + "/Documents";
        }

        public void copy(string fileName = "test.txt")
        {
            //文件地址
            string url;
            //Mac，Windows或Linux平台
#if UNITY_EDITOR || UNITY_STANDALONE
            url = $"{Application.dataPath}/StreamingAssets/{fileName}";
            //ios平台路径
#elif UNITY_IPHONE
            url = $"file://{Application.dataPath}/Raw/{fileName}";
    //安卓路径
#elif UNITY_ANDROID
            url = $"jar:file://{Application.dataPath}!/assets/{fileName}";
 
#endif
            if (!Directory.Exists($"{Application.persistentDataPath}/StreamingAssets"))
            {
                Directory.CreateDirectory($"{Application.persistentDataPath}/StreamingAssets");
            }
            string persistentUrl = $"{Application.persistentDataPath}/StreamingAssets/{fileName}";

            if (!File.Exists(persistentUrl))
            {
                Debug.Log($"{persistentUrl} 文件不存在,从StreamingAssets中Copy!");
                WWW www = new WWW(url);
                while (true)
                {
                    if (www.isDone)
                    {
                        if (www.error == null)
                        {
                            //本次读的文本 
                            File.WriteAllText(persistentUrl, www.text);
                            Debug.Log($"持久化目录: {persistentUrl}");
                            break;
                        }
                        else
                        {
                            Debug.LogWarning($"没得到StreamingAssets的文件 : {fileName}");
                        }
                    }
                }
            }
            else
            {
                Debug.Log($"{persistentUrl} 文件已存在!");
                //删除,重新拉,以此保证每次都是最新的
                File.Delete(persistentUrl);
                this.copy(fileName);
            }
        }
    }
    
}