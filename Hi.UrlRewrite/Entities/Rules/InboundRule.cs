﻿using System;
using System.Collections.Generic;
using Hi.UrlRewrite.Entities.Actions;
using Hi.UrlRewrite.Entities.Conditions;
using Hi.UrlRewrite.Entities.ServerVariables;

namespace Hi.UrlRewrite.Entities.Rules
{
    [Serializable]
    public class InboundRule
    {
        public Guid ItemId { get; set; }
        public string Name { get; set; }
        public RequestedUrl? RequestedUrl { get; set; }
        public string Pattern { get; set; }
        public bool IgnoreCase { get; set; }
        public IBaseAction Action { get; set; }
        public LogicalGrouping? ConditionLogicalGrouping { get; set; }
        public Using? Using { get; set; }
        public List<Condition> Conditions { get; set; }
        public List<IServerVariable> ServerVariables { get; set; }
        public bool Enabled { get; set; }
        public string SiteNameRestriction { get; set; }

        public InboundRule()
        {
            Conditions = new List<Condition>();
            ServerVariables = new List<IServerVariable>();
        }
    }
}
