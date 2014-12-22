using System;
using System.Collections.Generic;
using System.Web;
using Hi.UrlRewrite.Entities.Actions;
using Hi.UrlRewrite.Entities.ServerVariables;

namespace Hi.UrlRewrite.Processing.Results
{

    public class RuleResult
    {
        public bool StopProcessing { get; set; }
        public bool StoppedProcessing { get; set; }

        public Uri OriginalUri { get; set; }
        public Uri RewrittenUri { get; set; }

        public Guid ItemId { get; set; }
        public bool RuleMatched { get; set; }

        public IBaseAction ResultAction { get; set; }
        public ConditionMatchResult ConditionMatchResult { get; set; }

        public List<IServerVariable> ServerVariables { get; set; }

        public RuleResult()
        {
            ServerVariables = new List<IServerVariable>();
        }
    }
}
