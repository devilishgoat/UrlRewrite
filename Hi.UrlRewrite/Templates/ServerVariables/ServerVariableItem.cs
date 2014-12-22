using System;
using Hi.UrlRewrite.Templates.ServerVariables;
using Sitecore.Data.Items;
using System.Collections.Generic;
using Sitecore.Data.Fields;
using Sitecore.Web.UI.WebControls;

namespace Hi.UrlRewrite.Templates.Conditions
{
    public partial class ServerVariableItem : CustomItem
    {

        public static readonly string TemplateId = "{E073CD4C-2747-433D-B6C0-3C52BA953C97}";

        #region Inherited Base Templates

        private readonly BaseServerVariableItem _BaseServerVariableItem;
        public BaseServerVariableItem BaseServerVariableItem { get { return _BaseServerVariableItem; } }

        #endregion

        #region Boilerplate CustomItem Code

        public ServerVariableItem(Item innerItem)
            : base(innerItem)
        {
            _BaseServerVariableItem = new BaseServerVariableItem(innerItem);
        }

        public static implicit operator ServerVariableItem(Item innerItem)
        {
            return innerItem != null ? new ServerVariableItem(innerItem) : null;
        }

        public static implicit operator Item(ServerVariableItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code

        #region Field Instance Methods

        #endregion //Field Instance Methods
    }
}