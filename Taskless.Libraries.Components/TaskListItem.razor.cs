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
        #region Parameters

        [EditorRequired]
        [Parameter]
        public Item? Item { get; set; }

        #endregion

        #region Elements

        private ElementReference inputElement;

        #endregion

        #region Style

        private string GetCheckedCss() => Item is not null ? Item.Checked ? "task-list-item-checked" : string.Empty : string.Empty;
        private string GetSelectedCss() => Item is not null ? Item.Selected ? "task-item-selected" : string.Empty : string.Empty;
        private string GetEditCss() => Item is not null ? (Item.Edit && Item.Selected) ? "task-item-selected-edit" : string.Empty : string.Empty;
        private string GetEmojiCss()
        {
            if (Item is not null)
            {
                return Item.Type switch
                {
                    Item.TaskType.CHECKED => "task-list-item-emoji-checked",
                    Item.TaskType.EXCLAMATION => "task-list-item-emoji-exclamation",
                    Item.TaskType.QUESTION => "task-list-item-emoji-question",
                    Item.TaskType.TASK => "task-list-item-emoji-task",
                    Item.TaskType.BLOCKED => "task-list-item-emoji-blocked",
                    Item.TaskType.NONE => string.Empty,
                    _ => string.Empty,
                };
            }

            return string.Empty;
        }

        #endregion

        #region Methods

        public void OnEnter(KeyboardEventArgs keyboardEventArgs)
        {
            if (keyboardEventArgs.Key.ToLower() == "enter" && Item is not null)
            {
                Item.Edit = false;
            }
        }

        #endregion

        #region Overrides

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (Item is not null && Item.Edit)
            {
                await inputElement.FocusAsync();
            }
        }

        #endregion
    }
}