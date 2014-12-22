﻿using Hi.UrlRewrite.Entities;
using Hi.UrlRewrite.Entities.Actions;
using Hi.UrlRewrite.Entities.Conditions;
using Hi.UrlRewrite.Entities.Rules;
using Hi.UrlRewrite.Processing;
using Hi.UrlRewrite.Templates;
using Hi.UrlRewrite.Templates.Action;
using Hi.UrlRewrite.Templates.Action.Base;
using Hi.UrlRewrite.Templates.Conditions;
using Hi.UrlRewrite.Templates.ServerVariables;
using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Sitecore.Data;
using Hi.UrlRewrite.Entities.ServerVariables;

namespace Hi.UrlRewrite
{
    public static class ItemExtensions
    {
        public static InboundRule ToInboundRule(this InboundRuleItem inboundRuleItem, IEnumerable<BaseConditionItem> conditionItems, IEnumerable<BaseServerVariableItem> serverVariablesItems, string siteNameRestriction)
        {

            if (inboundRuleItem == null)
            {
                return null;
            }

            var inboundRule = new InboundRule
            {
                ItemId = inboundRuleItem.ID.Guid,
                Name = inboundRuleItem.Name,
                IgnoreCase = inboundRuleItem.IgnoreCase.Checked,
                Pattern = inboundRuleItem.Pattern.Value,
                Enabled = inboundRuleItem.Enabled.Checked
            };

            var requestedUrlItem = inboundRuleItem.RequestedUrl.TargetItem;
            RequestedUrl? requestedUrlType = null;
            if (requestedUrlItem != null)
            {
                var requestUrlItemId = requestedUrlItem.ID.ToString();
                switch (requestUrlItemId)
                {
                    case Constants.RequestedUrlType_MatchesThePattern_ItemId:
                        requestedUrlType = RequestedUrl.MatchesThePattern;
                        break;
                    case Constants.RequestedUrlType_DoesNotMatchThePattern_ItemId:
                        requestedUrlType = RequestedUrl.DoesNotMatchThePattern;
                        break;
                    default:
                        break;
                }
            }
            inboundRule.RequestedUrl = requestedUrlType;

            var logicalGroupingItem = inboundRuleItem.LogicalGrouping.TargetItem;
            LogicalGrouping? logicalGroupingType = null;
            if (logicalGroupingItem != null)
            {
                var logicalGroupingItemId = logicalGroupingItem.ID.ToString();
                switch (logicalGroupingItemId)
                {
                    case Constants.LogicalGroupingType_MatchAll_ItemId:
                        logicalGroupingType = LogicalGrouping.MatchAll;
                        break;
                    case Constants.LogicalGroupingType_MatchAny_ItemId:
                        logicalGroupingType = LogicalGrouping.MatchAny;
                        break;
                    default:
                        break;
                }
            }
            inboundRule.ConditionLogicalGrouping = logicalGroupingType;

            var usingItem = inboundRuleItem.Using.TargetItem;
            Using? usingType = null;
            if (usingItem != null)
            {
                var usingItemId = usingItem.ID.ToString();
                switch (usingItemId)
                {
                    case Constants.UsingType_RegularExpressions_ItemId:
                        usingType = Using.RegularExpressions;
                        break;
                    case Constants.UsingType_Wildcards_ItemId:
                        usingType = Using.Wildcards;
                        break;
                    case Constants.UsingType_ExactMatch_ItemId:
                        usingType = Using.ExactMatch;
                        break;
                    default:
                        break;
                }
            }
            inboundRule.Using = usingType;

            if (inboundRuleItem.Action == null)
            {
                Log.Warn(string.Format("UrlRewrite - No action set on rule with ItemID: {0}", inboundRuleItem.ID), typeof(ItemExtensions));

                return null;
            }

            var baseActionItem = inboundRuleItem.Action.TargetItem;
            IBaseAction baseAction = null;
            if (baseActionItem != null)
            {
                var baseActionItemTemplateId = baseActionItem.TemplateID.ToString();

                if (baseActionItemTemplateId.Equals(RedirectItem.TemplateId, StringComparison.InvariantCultureIgnoreCase))
                {
                    baseAction = new RedirectItem(baseActionItem).ToRedirectAction();
                }
                else if (baseActionItemTemplateId.Equals(AbortRequestItem.TemplateId,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    baseAction = new AbortRequestItem(baseActionItem).ToAbortRequestAction();
                }
                else if (baseActionItemTemplateId.Equals(CustomResponseItem.TemplateId,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    baseAction = new CustomResponseItem(baseActionItem).ToCustomResponseAction();
                }
                else if (baseActionItemTemplateId.Equals(ItemQueryRedirectItem.TemplateId))
                {
                    baseAction = new ItemQueryRedirectItem(baseActionItem).ToItemQueryRedirectAction();
                }
            }
            inboundRule.Action = baseAction;

            if (conditionItems != null)
            {
                inboundRule.Conditions = conditionItems
                    .Select(e => e.ToCondition())
                    .Where(e => e != null)
                    .ToList();
            }

            if (serverVariablesItems != null)
            {
                inboundRule.ServerVariables = serverVariablesItems
                    .Select(e => e.ToServerVariable())
                    .Where(e => e != null)
                    .ToList();
            }

            inboundRule.SiteNameRestriction = siteNameRestriction;

            return inboundRule;
        }

        public static RedirectAction ToRedirectAction(this RedirectItem redirectItem)
        {

            if (redirectItem == null)
            {
                return null;
            }

            var redirectTo = redirectItem.BaseRedirectAction.RewriteUrl;
            string actionRewriteUrl;
            Guid? redirectItemId;
            string redirectItemAnchor;

            RulesEngine.GetRedirectUrlOrItemId(redirectTo, out actionRewriteUrl, out redirectItemId, out redirectItemAnchor);

            var redirectAction = new RedirectAction
            {
                Name = redirectItem.Name,
                AppendQueryString = redirectItem.BaseRedirectAction.AppendQueryString.Checked,
                RewriteUrl = actionRewriteUrl,
                RewriteItemId = redirectItemId,
                RewriteItemAnchor = redirectItemAnchor
            };

            var baseRewriteItem = redirectItem.BaseRedirectAction.BaseRewriteItem;
            GetBaseRewriteItem(baseRewriteItem, redirectAction);

            return redirectAction;
        }

        private static void GetStopProcessing(BaseStopProcessingActionItem redirectItem, IBaseStopProcessingAction redirectAction)
        {
            redirectAction.StopProcessingOfSubsequentRules =
                redirectItem.StopProcessingOfSubsequentRules.Checked;
        }

        private static void GetCacheability(BaseCacheItem httpCacheabilityTypeItem, IBaseCache redirectAction)
        {
            var httpCacheabilityTypeTargetItem = httpCacheabilityTypeItem.HttpCacheability.TargetItem;
            HttpCacheability? httpCacheability = null;
            if (httpCacheabilityTypeTargetItem != null)
            {
                switch (httpCacheabilityTypeTargetItem.ID.ToString())
                {
                    case Constants.HttpCacheabilityType_NoCache_ItemId:
                        httpCacheability = HttpCacheability.NoCache;
                        break;
                    case Constants.HttpCacheabilityType_Private_ItemId:
                        httpCacheability = HttpCacheability.Private;
                        break;
                    case Constants.HttpCacheabilityType_Server_ItemId:
                        httpCacheability = HttpCacheability.Server;
                        break;
                    case Constants.HttpCacheabilityType_ServerAndNoCache_ItemId:
                        httpCacheability = HttpCacheability.ServerAndNoCache;
                        break;
                    case Constants.HttpCacheabilityType_Public_ItemId:
                        httpCacheability = HttpCacheability.Public;
                        break;
                    case Constants.HttpCacheabilityType_ServerAndPrivate_ItemId:
                        httpCacheability = HttpCacheability.ServerAndPrivate;
                        break;
                    default:
                        break;
                }
            }
            redirectAction.HttpCacheability = httpCacheability;
        }

        private static void GetStatusCode(BaseRedirectTypeItem redirectTypeItem, IBaseStatusCode redirectAction)
        {
            var redirectTypeTargetItem = redirectTypeItem.RedirectType.TargetItem;
            RedirectActionStatusCode? redirectType = null;
            if (redirectTypeTargetItem != null)
            {
                switch (redirectTypeTargetItem.ID.ToString())
                {
                    case Constants.RedirectType_Permanent_ItemId:
                        redirectType = RedirectActionStatusCode.Permanent;
                        break;
                    case Constants.RedirectType_Found_ItemId:
                        redirectType = RedirectActionStatusCode.Found;
                        break;
                    case Constants.RedirectType_SeeOther_ItemId:
                        redirectType = RedirectActionStatusCode.SeeOther;
                        break;
                    case Constants.RedirectType_Temporary_ItemId:
                        redirectType = RedirectActionStatusCode.Temporary;
                        break;
                    default:
                        break;
                }
            }
            redirectAction.StatusCode = redirectType;
        }

        public static AbortRequestAction ToAbortRequestAction(this AbortRequestItem abortRequestItem)
        {
            if (abortRequestItem == null)
            {
                return null;
            }

            var abortRequestAction = new AbortRequestAction()
            {
                Name = abortRequestItem.Name
            };

            return abortRequestAction;
        }

        public static CustomResponseAction ToCustomResponseAction(this CustomResponseItem customResponseItem)
        {
            if (customResponseItem == null)
            {
                return null;
            }

            var customResponseAction = new CustomResponseAction()
            {
                Name = customResponseItem.Name,
            };

            var statusCode = 0;

            if (!int.TryParse(customResponseItem.StatusCode.Value, out statusCode))
            {
                return null;
            }

            customResponseAction.StatusCode = statusCode;

            if (customResponseItem.SubstatusCode.Value != null)
            {
                int outSubStatusCode = 0;
                if (int.TryParse(customResponseItem.SubstatusCode.Value, out outSubStatusCode))
                {
                    customResponseAction.SubStatusCode = outSubStatusCode;
                }
            }

            customResponseAction.ErrorDescription = customResponseItem.ErrorDescription.Value;
            customResponseAction.Reason = customResponseItem.Reason.Value;

            return customResponseAction;
        }

        public static ItemQueryRedirectAction ToItemQueryRedirectAction(this ItemQueryRedirectItem itemQueryRedirectItem)
        {
            if (itemQueryRedirectItem == null)
            {
                return null;
            }

            var itemQueryRedirectAction = new ItemQueryRedirectAction()
            {
                Name = itemQueryRedirectItem.Name,
                ItemQuery = itemQueryRedirectItem.ItemQuery.Value
            };

            var baseRewriteItem = itemQueryRedirectItem.BaseRewriteItem;
            GetBaseRewriteItem(baseRewriteItem, itemQueryRedirectAction);

            return itemQueryRedirectAction;
        }

        private static void GetBaseRewriteItem(BaseRewriteItem baseRewriteItem, IBaseRewrite redirect)
        {
            var stopProcessingItem = baseRewriteItem.BaseStopProcessingAction;
            GetStopProcessing(stopProcessingItem, redirect);

            var redirectTypeItem = baseRewriteItem.BaseRedirectTypeItem;
            GetStatusCode(redirectTypeItem, redirect);

            var httpCacheabilityTypeItem = baseRewriteItem.BaseCacheItem;
            GetCacheability(httpCacheabilityTypeItem, redirect);
        }

        public static Condition ToCondition(this BaseConditionItem baseConditionItem)
        {
            var condition = GetBaseConditionInfo(baseConditionItem);

            if (condition == null) return null;

            var baseConditionItemTemplateId = baseConditionItem.InnerItem.TemplateID;

            if (baseConditionItemTemplateId.Equals(new ID(ConditionItem.TemplateId)))
            {
                var conditionInputItem = new ConditionItem(baseConditionItem).ConditionInput.TargetItem;
                Tokens? conditionInputType = null;

                if (conditionInputItem != null)
                {
                    switch (conditionInputItem.ID.ToString())
                    {
                        case Constants.ConditionInputType_QueryString_ItemId:
                            conditionInputType = Tokens.QUERY_STRING;
                            break;
                        case Constants.ConditionInputType_HttpHost_ItemId:
                            conditionInputType = Tokens.HTTP_HOST;
                            break;
                        case Constants.ConditionInputType_Https_ItemId:
                            conditionInputType = Tokens.HTTPS;
                            break;
                        default:
                            break;
                    }
                }

                condition.InputString = string.Format("{{{0}}}", conditionInputType);
            }
            else if (baseConditionItemTemplateId.Equals(new ID(ConditionAdvancedItem.TemplateId)))
            {
                condition.InputString = new ConditionAdvancedItem(baseConditionItem).ConditionInputString.Value;
            }

            return condition;
        }

        private static Condition GetBaseConditionInfo(BaseConditionItem conditionItem)
        {
            if (conditionItem == null)
            {
                return null;
            }

            var condition = new Condition
            {
                Name = conditionItem.Name,
                Pattern = conditionItem.Pattern.Value,
                IgnoreCase = conditionItem.IgnoreCase.Checked
            };

            var checkIfInputStringItem = conditionItem.CheckIfInputString.TargetItem;
            CheckIfInputString? checkIfInputStringType = null;

            if (checkIfInputStringItem != null)
            {
                switch (checkIfInputStringItem.ID.ToString())
                {
                    case Constants.CheckIfInputStringType_IsAFile_ItemId:
                        checkIfInputStringType = CheckIfInputString.IsAFile;
                        break;
                    case Constants.CheckIfInputStringType_IsNotAFile_ItemId:
                        checkIfInputStringType = CheckIfInputString.IsNotAFile;
                        break;
                    case Constants.CheckIfInputStringType_IsADirectory_ItemId:
                        checkIfInputStringType = CheckIfInputString.IsADirectory;
                        break;
                    case Constants.CheckIfInputStringType_IsNotADirectory_ItemId:
                        checkIfInputStringType = CheckIfInputString.IsNotADirctory;
                        break;
                    case Constants.CheckIfInputStringType_MatchesThePattern_ItemId:
                        checkIfInputStringType = CheckIfInputString.MatchesThePattern;
                        break;
                    case Constants.CheckIfInputStringType_DoesNotMatchThePattern_ItemId:
                        checkIfInputStringType = CheckIfInputString.DoesNotMatchThePattern;
                        break;
                    default:
                        break;
                }
            }
            condition.CheckIfInputString = checkIfInputStringType;

            return condition;
        }

        public static IServerVariable ToServerVariable(this BaseServerVariableItem baseServerVariableItem)
        {
            if (baseServerVariableItem == null)
            {
                return null;
            }

            IServerVariable serverVariable;

            if (baseServerVariableItem.ID.Equals(new ID(ServerVariableItem.TemplateId)))
            {
                serverVariable = new ServerVariable
                {
                    Name = baseServerVariableItem.Name,
                    ReplaceExistingValue = baseServerVariableItem.ReplaceExistingValue.Checked,
                    VariableName = baseServerVariableItem.VariableName.Value,
                    Value = baseServerVariableItem.Value.Value
                };

            }
            else
            {
                serverVariable = new RequestHeader
                {
                    Name = baseServerVariableItem.Name,
                    ReplaceExistingValue = baseServerVariableItem.ReplaceExistingValue.Checked,
                    VariableName = baseServerVariableItem.VariableName.Value,
                    Value = baseServerVariableItem.Value.Value
                };
            }

            return serverVariable;
        }

    }
}
