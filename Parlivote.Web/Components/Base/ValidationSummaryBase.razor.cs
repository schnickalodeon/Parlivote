using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Parlivote.Web.Models;

namespace Parlivote.Web.Components.Base;

public partial class ValidationSummaryBase : ComponentBase
{
    [Parameter]
    public IDictionary ValidationErrors { get; set; }

    private Dictionary<string, string> validationDictionary;
    private ComponentState state;

    protected override void OnInitialized()
    {
        this.state = ComponentState.Loading;
    }

    protected override void OnParametersSet()
    {
        SetValidationDictionary();
    }

    private void SetValidationDictionary()
    {
        this.validationDictionary = new Dictionary<string, string>();
        foreach(DictionaryEntry validationError in this.ValidationErrors)
        {
            string parameter = validationError.Key.ToString();
            var value = validationError.Value;
            string[] messages = value as string[];

            if (messages != null && messages.Length > 0)
            {
                string joinedMessages = string.Join(",", messages);
                this.validationDictionary.Add(parameter, joinedMessages);
            }
            else
            {
                this.validationDictionary.Add(parameter,"");
            }
        }
        this.state = ComponentState.Content;
    }
}