using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OJWebService
{
    public class TestSuiltContent
    {
        public String[] inputs { get; set; }
        public String[] correctoutputs { get; set; }
        public String source { get; set; }

        public TestSuiltContent(String[] inputs, String[] correctoutputs, String source)
        {
            this.inputs = inputs;
            this.correctoutputs = correctoutputs;
            this.source = source;
        }
    }
}