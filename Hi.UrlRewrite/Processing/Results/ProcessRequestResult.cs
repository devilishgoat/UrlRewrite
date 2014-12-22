using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using Hi.UrlRewrite.Entities.Actions;
using Hi.UrlRewrite.Entities.ServerVariables;

namespace Hi.UrlRewrite.Processing.Results
{
    public class ProcessRequestResult
    {

        public ProcessRequestResult(Uri originalUri, RuleResult finalRuleResult, bool matchedAtLeastOneRule, List<RuleResult> processedResults)
        {
            OriginalUri = originalUri;
            MatchedAtLeastOneRule = matchedAtLeastOneRule;
            ProcessedResults = processedResults;

            if (finalRuleResult != null)
            {
                RewrittenUri = finalRuleResult.RewrittenUri;
                FinalAction = finalRuleResult.ResultAction;
            }

            ServerVariables = processedResults
                .Where(pr => pr.ServerVariables.Count > 0)
                .SelectMany(pr => pr.ServerVariables)
                .ToList();
        }

        public Uri OriginalUri { get; set; }
        public Uri RewrittenUri { get; set; }

        public int? StatusCode 
        { 
            get 
            {
                if (FinalAction is RedirectAction)
                {
                    var redirectAction = FinalAction as RedirectAction;
                    if (redirectAction.StatusCode.HasValue)
                    {
                        return (int) (redirectAction.StatusCode.Value);
                    }
                }
                else if (FinalAction is CustomResponseAction)
                {
                    var customResponse = FinalAction as CustomResponseAction;
                    return customResponse.StatusCode;
                }

                return null;
            }
        }

        public IBaseAction FinalAction { get; set; }

        public bool MatchedAtLeastOneRule { get; set; }
        public List<RuleResult> ProcessedResults { get; set; }
        public List<IServerVariable> ServerVariables { get; set; }

    }
}
