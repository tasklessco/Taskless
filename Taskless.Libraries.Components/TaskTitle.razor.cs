using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Taskless.Libraries.Models;

namespace Taskless.Libraries.Components
{
    public partial class TaskTitle
    {
        [EditorRequired]
        [Parameter]
        public Group? TaskGroup { get; set; }

        private string GetSelectedCss() => TaskGroup.Selected ? "task-item-selected" : "";
        public string GetInputEditWidth() => $"width: {(((this.TaskGroup.Title.Length + 1) * 8)) + 10}px";
        public string GetEditCss() => (TaskGroup.EditMode && TaskGroup.Selected) ? "task-item-selected-edit" : "";
        private ElementReference inputElement;
        public void OnEnter(KeyboardEventArgs keyboardEventArgs)
        {
            if (keyboardEventArgs.Key.ToLower() == "enter" || keyboardEventArgs.Key.ToLower() == "escape")
            {
                this.TaskGroup.EditMode = false;
            }
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (TaskGroup.EditMode)
            {
                await inputElement.FocusAsync();
            }
        }
    }
}