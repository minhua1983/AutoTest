using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTest.UI.Models
{
    public class MyTask
    {
        public string Title { get; set; } = "";
        public string Url { get; set; } = "";
        public List<MyAction> ActionList { get; set; } = new List<MyAction>();
        public int CurrentActionIndex { get; set; } = 0;
    }

    public class MyAction
    {
        public string Title { get; set; } = "";
        public List<MyStep> StepList { get; set; } = new List<MyStep>();
    }

    public class MyStep
    {
        public string Title { get; set; } = "";
        public string Id { get; set; } = "";
        public string Pattern { get; set; } = "";
        public string Value { get; set; } = "";
        public string Script { get; set; } = "";
        public string Invoke { get; set; } = "";
    }
}
