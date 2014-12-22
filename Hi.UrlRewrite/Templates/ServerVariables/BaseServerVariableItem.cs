using System;
using Sitecore.Data.Items;
using System.Collections.Generic;
using Sitecore.Data.Fields;
using Sitecore.Web.UI.WebControls;

namespace Hi.UrlRewrite.Templates.ServerVariables
{
    public partial class BaseServerVariableItem : CustomItem
    {

        public static readonly string TemplateId = "{ED218F0C-2AF5-4D0A-AA45-E089F9862E0C}";

        #region Inherited Base Templates

        private readonly BaseUrlRewriteItem _BaseUrlRewriteItem;
        public BaseUrlRewriteItem BaseUrlRewrite { get { return _BaseUrlRewriteItem; } }

        #endregion

        #region Boilerplate CustomItem Code

        public BaseServerVariableItem(Item innerItem)
            : base(innerItem)
        {
            _BaseUrlRewriteItem = new BaseUrlRewriteItem(innerItem);
        }

        public static implicit operator BaseServerVariableItem(Item innerItem)
        {
            return innerItem != null ? new BaseServerVariableItem(innerItem) : null;
        }

        public static implicit operator Item(BaseServerVariableItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code


        #region Field Instance Methods


        public TextField VariableName
        {
            get
            {
                return new TextField(InnerItem.Fields["Variable Name"]);
            }
        }

        public TextField Value
        {
            get
            {
                return new TextField(InnerItem.Fields["Value"]);
            }
        }

        public CheckboxField ReplaceExistingValue
        {
            get
            {
                return new CheckboxField(InnerItem.Fields["Replace existing value"]);
            }
        }

        #endregion //Field Instance Methods
    }
}