using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;


namespace UI
{
    class Config
    {
        //dwg文件名,dwg文件中的生成的mark(id)
        public Dictionary<string, string> DwgFiles { get; set; }

        public string ProjectName { get; set; }

        public void Write(string path)
        {
            string str = JsonConvert.SerializeObject(this);
            string name = string.Format("{0}\\{1}.proj",path,Global.CurrentProjectName);
            Fs.WriteStr(str, name);
        }

        public static Config Read(string name)
        {
            FileStream aFile = new FileStream(name, FileMode.Open);
            StreamReader sr = new StreamReader(aFile);
            string strLine;
            strLine = sr.ReadLine();

            JsonSerializer serializer = new JsonSerializer();
            StringReader sr1 = new StringReader(strLine);
            object o = serializer.Deserialize(new JsonTextReader(sr1), typeof(Config));
            Config config = o as Config;
            return config;
        }

        public void DwgAppend(string name)
        {
            string id = Project.Instance.DwgFilesId[name];

            DwgFiles.Add(name, id);
        }

        public void DwgRemoved(string name)
        {
            DwgFiles.Remove(name);
        }

    }
}
