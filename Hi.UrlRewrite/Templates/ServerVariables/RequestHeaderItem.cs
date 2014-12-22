using System;
using Hi.UrlRewrite.Templates.ServerVariables;
using Sitecore.Data.Items;
using System.Collections.Generic;
using Sitecore.Data.Fields;
using Sitecore.Web.UI.WebControls;

namespace Hi.UrlRewrite.Templates.Conditions
{
    public partial class RequestHeaderItem : CustomItem
    {

        public static readonly string TemplateId = "{698FFCC4-0D22-4B97-ACFE-3D04994B4B65}";

        #region Inherited Base Templates

        private readonly BaseServerVariableItem _BaseServerVariableItem;
        public BaseServerVariableItem BaseServerVariableItem { get { return _BaseServerVariableItem; } }

        #endregion

        #region Boilerplate CustomItem Code

        public RequestHeaderItem(Item innerItem)
            : base(innerItem)
        {
            _BaseServerVariableItem = new BaseServerVariableItem(innerItem);
        }

        public static implicit operator RequestHeaderItem(Item innerItem)
        {
            return innerItem != null ? new RequestHeaderItem(innerItem) : null;
        }

        public static implicit operator Item(RequestHeaderItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code

        #region Field Instance Methods

        #endregion //Field Instance Methods
    }
}