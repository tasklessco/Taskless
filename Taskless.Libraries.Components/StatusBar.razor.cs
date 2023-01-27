using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Reflection.Metadata;
using Taskless.Libraries.Models;

namespace Taskless.Libraries.Components
{
    public partial class StatusBar
    {
        [Parameter]
        public bool Saved { get; set; } = false;

        public string GetCss => Saved ? "saved" : "saving";
        public string GetText => Saved ? "✓" : "·";
    }
}