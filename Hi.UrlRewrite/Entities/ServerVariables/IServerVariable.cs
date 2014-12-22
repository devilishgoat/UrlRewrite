using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hi.UrlRewrite.Entities.ServerVariables
{
    public interface IServerVariable
    {
        string Name { get; set; }
        string VariableName { get; set; }
        string Value { get; set; }
        bool ReplaceExistingValue { get; set; }
    }

}