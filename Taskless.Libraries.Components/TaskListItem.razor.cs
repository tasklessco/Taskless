using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Taskless.Libraries.Models;

namespace Taskless.Libraries.Components
{
    public partial class TaskListItem
    {
        [EditorRequired]
        [Parameter]
        public Item TaskItem { get; set; }

        private ElementReference inputElement;
        public string GetCheckedCss() => TaskItem.Checked ? "task-list-item-checked" : "";
        public string GetSelectedCss() => TaskItem.Selected ? "task-item-selected" : "";
        public string GetEditCss() => (TaskItem.EditMode && TaskItem.Selected) ? "task-item-selected-edit" : "";
        public string GetEmojiCss()
        {
            return TaskItem.Type switch
            {
                Item.TaskType.CHECKED => "task-list-item-emoji-checked",
                Item.TaskType.EXCLAMATION => "task-list-item-emoji-exclamation",
                Item.TaskType.QUESTION => "task-list-item-emoji-question",
                Item.TaskType.TASK => "task-list-item-emoji-task",
                Item.TaskType.BLOCKED => "task-list-item-emoji-blocked",
                _ => "",
            };
        }

        public void OnEnter(KeyboardEventArgs keyboardEventArgs)
        {
            if (keyboardEventArgs.Key.ToLower() == "enter" || keyboardEventArgs.Key.ToLower() == "escape")
            {
                this.TaskItem.EditMode = false;
            }
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (TaskItem.EditMode)
            {
                await inputElement.FocusAsync();
            }
        }
    }
}