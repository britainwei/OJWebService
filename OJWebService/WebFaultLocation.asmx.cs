using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.IO;

namespace OJWebService
{
    /// <summary>
    /// WebFaultLocation 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class WebFaultLocation : System.Web.Services.WebService
    {
        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]
        public String Saying(String word)
        {
            return "britain says: " + word;
        }

        [WebMethod]
        public String FaultLocation(String[] inputs, String[] correctoutputs, String source)
        {
            TestSuiltContent suilt = new TestSuiltContent(inputs, correctoutputs, source);

            new FolderPrepare().createTestSuiltFolder(suilt);
            File.WriteAllText(@"F:\errorinwebservice.txt", "suilt prepare success");
            new MainFunction().run();

            return File.ReadAllText(@"\source\result.txt");
        }
    }
}
