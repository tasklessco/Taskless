using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Reflection.Metadata;
using Taskless.Libraries.Models;

namespace Taskless.Libraries.Components
{
    public partial class TaskTitle
    {
        #region Parameters

        [EditorRequired]
        [Parameter]
        public Group? Group { get; set; }

        #endregion

        #region Elements

        private ElementReference inputElement;

        #endregion

        #region Style
        private string GetSelectedCss() => Group is not null ? Group.Selected ? "task-item-selected" : "" : string.Empty;
        private string GetInputEditWidth() => Group is not null ? $"width: {((Group.Title.Length + 1) * 8) + 10}px" : string.Empty;
        private string GetEditCss() => Group is not null ? (Group.Edit && Group.Selected) ? "task-item-selected-edit" : string.Empty : string.Empty;

        #endregion

        #region Methods
        private void OnEnter(KeyboardEventArgs keyboardEventArgs)
        {
            if (keyboardEventArgs.Key.ToLowerInvariant() == "enter" && this.Group is not null)
            {
                this.Group.Edit = false;
            }
        }

        #endregion

        #region Overrides
        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (Group is not null && Group.Edit)
            {
                await inputElement.FocusAsync();
            }
        }

        #endregion
    }
}