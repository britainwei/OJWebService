﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;

namespace OJWebService
{
    /// <summary>
    /// 用于准备跑数集据的时候准备必备路径
    /// </summary>
    public class FolderPrepare
    {
        public void createTestSuiltFolder(TestSuiltContent suilt)
        {
            init();
            this.createFolder( @"\source\");
            File.WriteAllText(@"\source\sourceFile.c", suilt.source);
            this.createFolder(@"\source\inputs\");
            this.makeFiles(suilt.inputs, @"\source\inputs\t");
            this.createFolder(@"\source\correctoutputs\");
            this.makeFiles(suilt.correctoutputs, @"\source\correctoutputs\t");
        }
        /// <summary>
        /// 删掉上次运行产生的input和correctoutput文件，防止干扰
        /// </summary>
        public void init()
        {
            try { 
                Directory.Delete(@"\source\", true);
            }
            catch 
            {
            }
        }

        public void createFolder(String path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public void makeFiles(String[] content, String fileName)
        {
            int i = 0; 
            foreach (String tmp in content)
            {
                File.WriteAllText(fileName+i, tmp);
                i++;
            }
        }
    }
}