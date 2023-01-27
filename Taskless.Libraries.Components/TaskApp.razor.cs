using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Taskless.Libraries.Models;
using Toolbelt.Blazor.HotKeys2;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Taskless.Libraries.Components
{
    public partial class TaskApp
    {
        #region Properties

        private List<Group>? Groups;
        private HotKeysContext? HotkeyContext;

        #endregion

        #region Injects

        [Inject]
        public HotKeys? HotKeys { get; set; }

        [Inject]
        public IJSRuntime? JSRuntime { get; set; }

        [Inject]
        public ILocalStorageService LocalStorage { get; set; }


        #endregion

        #region Overrides
        protected async override Task OnInitializedAsync()
        {
            // Groups data
            this.Groups = await GetData();

            // Asign stuff to keys
            if (this.HotKeys is not null)
            {
                this.HotkeyContext = this.HotKeys.CreateContext();
                this.HotkeyContext
                    .Add(ModCode.Shift, Code.ArrowUp, async () => { await MoveUp(); })
                    .Add(ModCode.Shift, Code.ArrowDown, async () => { await MoveDown(); })
                    .Add(ModCode.Shift, Code.PageUp, async () => { await MoveGroupUp(); })
                    .Add(ModCode.Shift, Code.PageDown, async () => { await MoveGroupDown(); })
                    .Add(ModCode.Shift, Code.X, async () => { await ToggleType(); })
                    .Add(ModCode.Shift, Code.C, async () => { await ToggleChecked(); })
                    .Add(ModCode.Shift, Code.E, async () => { await ToggleEdit(); })
                    .Add(ModCode.Shift, Code.R, async () => { await RemoveItem(); })
                    .Add(ModCode.Shift, Code.N, async () => { await NewItem(); })
                    .Add(ModCode.Shift, Code.G, async () => { await NewGroup(); })
                    .Add(ModCode.Shift, Code.T, async () => { await ToggleTheme(); })
                    .Add(ModCode.Shift, Code.H, async () => { await ShowHelp(); })
                    .Add(ModCode.Shift, Code.F, async () => { await RestartSelection(); })
                    .Add(ModCode.Shift, Code.Home, async () => { await GoToFirstInGroup(); })
                    .Add(ModCode.Shift, Code.End, async () => { await GoToLastInGroup(); })
                    .Add(ModCode.Shift, Code.S, async () => { await SaveDataAsync(); });
            }
        }

        #endregion

        #region Data

        private Group GetInitialData()
        {
            var group = new Group("Welcome to Task(keyboard)less", 0, true, false, new());

            group.Items.Add(new("Shift + Up to move up", false, false, 0, false, Item.TaskType.NONE));
            group.Items.Add(new("Shift + Down to move up", false, false, 0, false, Item.TaskType.NONE));
            group.Items.Add(new("Shift + G to create a new group", false, false, 0, false, Item.TaskType.NONE));
            group.Items.Add(new("Shift + N to create a new task", false, false, 0, false, Item.TaskType.NONE));
            group.Items.Add(new("Shift + R to remove item or entire group", false, false, 0, false, Item.TaskType.NONE));
            group.Items.Add(new("Shift + E to edit", false, false, 0, false, Item.TaskType.NONE));
            group.Items.Add(new("Shift + X to change type", false, false, 0, false, Item.TaskType.NONE));
            group.Items.Add(new("Shift + C to check an item or entire group", false, false, 0, false, Item.TaskType.EXCLAMATION));
            group.Items.Add(new("Shift + T to change color theme", true, false, 0, false, Item.TaskType.NONE));
            group.Items.Add(new("Shift + H to show help", true, false, 0, false, Item.TaskType.NONE));
            group.Items.Add(new("Shift + S to save", true, false, 0, false, Item.TaskType.NONE));

            return group;
        }

        private async Task<List<Group>> GetData()
        {
            if (JSRuntime is not null)
            {
                var data = await LocalStorage.GetItemAsync<List<Group>?>("data");

                if (data is not null)
                {
                    return data;
                }
                else
                {
                    List<Group> res = new();
                    res.Add(GetInitialData());

                    return res;
                }
            }

            return new();
        }

        private async Task SaveDataAsync()
        {
            if (JSRuntime is not null)
            {
                await LocalStorage.SetItemAsync("data", this.Groups);
            }
        }

        #endregion

        #region Management

        private async Task ToggleType()
        {
            if (Groups is not null)
            {
                foreach (var taskGroup in Groups)
                {
                    if (taskGroup.SubTaskSelected)
                    {
                        foreach (var item in taskGroup.Items)
                        {
                            if (item.Selected)
                            {
                                var nextType = item.Type + 1;
                                if ((int)nextType == 6)
                                {
                                    item.Type = 0;
                                }
                                else
                                {
                                    item.Type++;
                                }

                                break;
                            }
                        }
                    }
                }
            }

            StateHasChanged();

            await SaveDataAsync();
        }

        private async Task ToggleChecked()
        {
            if (Groups is not null)
            {
                foreach (var taskGroup in Groups)
                {
                    if (taskGroup.Selected || taskGroup.SubTaskSelected)
                    {
                        if (taskGroup.Selected)
                        {
                            foreach (var item in taskGroup.Items)
                            {
                                item.Checked = true;
                            }
                        }
                        else if (taskGroup.SubTaskSelected)
                        {
                            foreach (var taskItem in taskGroup.Items)
                            {
                                if (taskItem.Selected)
                                {
                                    taskItem.Checked = !taskItem.Checked;
                                }
                            }
                        }

                        break;
                    }
                }
            }

            StateHasChanged();

            await SaveDataAsync();
        }

        private async Task ToggleEdit()
        {
            if (Groups is not null)
            {
                foreach (var taskGroup in Groups)
                {
                    if (taskGroup.Selected || taskGroup.SubTaskSelected)
                    {
                        if (taskGroup.Selected)
                        {
                            taskGroup.Edit = !taskGroup.Edit;
                        }
                        else if (taskGroup.SubTaskSelected)
                        {
                            foreach (var taskItem in taskGroup.Items)
                            {
                                if (taskItem.Selected)
                                {
                                    taskItem.Edit = !taskItem.Edit;
                                }
                            }
                        }

                        break;
                    }
                }
            }

            StateHasChanged();
        }

        private async Task RemoveItem()
        {
            if (Groups is not null)
            {
                var hasToRemoveTaskGroup = false;
                var taskGroupIdToRemove = new Guid();
                foreach (var taskGroup in Groups)
                {
                    if (taskGroup.Selected || taskGroup.SubTaskSelected)
                    {
                        if (taskGroup.Selected)
                        {
                            hasToRemoveTaskGroup = true;
                            taskGroupIdToRemove = taskGroup.Id;
                        }
                        else if (taskGroup.SubTaskSelected)
                        {
                            // Find item to remove
                            var itemToRemove = taskGroup.Items.Single(x => x.Selected);

                            var id = itemToRemove?.Id;
                            if (itemToRemove is not null)
                            {
                                // Move up first
                                MoveUp();
                                // Delete prior item
                                taskGroup.Items.RemoveAll(x => x.Id == id);
                                break;
                            }
                        }

                        break;
                    }
                }

                if (hasToRemoveTaskGroup)
                {
                    // Fetch item
                    var item = Groups.SingleOrDefault(x => x.Id == taskGroupIdToRemove);
                    if (item is not null)
                    {
                        // Remove taskgroup
                        Groups.RemoveAll(x => x.Id == taskGroupIdToRemove);
                        // Split list
                        var firstPart = Groups.Skip(0).Take(item.Order + 1).ToList();
                        var secondPart = Groups.Skip(item.Order + 1).ToList();
                        // Check if there's something before our item
                        if (firstPart.Count > 0)
                        {
                            firstPart.Last().Selected = true;
                        }
                        else if (secondPart.Count > 0)
                        {
                            firstPart.First().Selected = true;
                        }
                    }
                }
            }

            StateHasChanged();

            await SaveDataAsync();
        }

        private async Task NewItem()
        {
            if (Groups is not null)
            {
                for (int j = 0; j < Groups.Count; j++)
                {
                    var taskGroup = Groups[j];
                    var breakFor = false;
                    if (taskGroup.SubTaskSelected || taskGroup.Selected)
                    {
                        // Create new one
                        var newItem = new Item("", false, true, -1, true, Item.TaskType.NONE);
                        // If this is selected it means it's on the top level
                        // either create a task in the first place
                        if (taskGroup.Selected)
                        {
                            taskGroup.Selected = false;
                            // Split list
                            var firstPart = new List<Item>();
                            var secondPart = taskGroup.Items.ToList();
                            // Add to first part
                            firstPart.Add(newItem);
                            // Join lists
                            var endList = firstPart.Concat(secondPart);
                            // Reassign
                            taskGroup.Items = endList.ToList();
                            breakFor = true;
                        }
                        else if (taskGroup.SubTaskSelected)
                        {
                            // This should go the next iteration in the taskItems
                            for (int i = 0; i < taskGroup?.Items.Count; i++)
                            {
                                var item = taskGroup.Items[i];
                                if (item.Selected)
                                {
                                    // Remove selection
                                    item.Selected = false;
                                    item.Edit = false;
                                    taskGroup.Selected = false;
                                    // Split list
                                    var firstPart = taskGroup.Items.Skip(0).Take(item.Order + 1).ToList();
                                    var secondPart = taskGroup.Items.Skip(item.Order + 1).ToList();
                                    // Add to first part
                                    firstPart.Add(newItem);
                                    // Join lists
                                    var endList = firstPart.Concat(secondPart).ToList();
                                    // Reassign
                                    taskGroup.Items = endList.ToList();
                                    breakFor = true;
                                    break;
                                }
                            }
                        }

                        if (breakFor)
                        {
                            // Update order
                            for (int c = 0; c < taskGroup?.Items.Count; c++)
                            {
                                Item? taskItem = taskGroup.Items[c];
                                taskItem.Order = c;
                            }

                            break;
                        }
                    }
                }
            }

            StateHasChanged();

            await SaveDataAsync();
        }

        private async Task ToggleTheme()
        {
            if (JSRuntime is not null)
            {
                await JSRuntime.InvokeVoidAsync("toggleTheme");
            }
        }

        private async Task ShowHelp()
        {

        }

        private async Task NewGroup()
        {
            if (Groups is not null)
            {
                var newTaskGroups = new List<Group>();
                if (Groups.Count > 0)
                {
                    for (int j = 0; j < Groups.Count; j++)
                    {
                        var taskGroup = Groups[j];
                        var breakFor = false;
                        if (taskGroup.SubTaskSelected || taskGroup.Selected)
                        {
                            if (taskGroup.Selected)
                            {
                                taskGroup.Selected = false;
                            }
                            else
                            {
                                foreach (var item in taskGroup.Items)
                                {
                                    if (item.Selected)
                                    {
                                        item.Selected = false;
                                        break;
                                    }
                                }
                            }

                            // Create new one
                            var newItem = new Models.Group("", -1, true, true, new());
                            // If this is selected it means it's on the top level
                            // either create a task in the first place
                            taskGroup.Selected = false;
                            // Split list
                            var firstPart = Groups.Skip(0).Take(taskGroup.Order + 1).ToList();
                            var secondPart = Groups.Skip(taskGroup.Order + 1).ToList();
                            // Add to first part
                            firstPart.Add(newItem);
                            // Join lists
                            var endList = firstPart.Concat(secondPart);
                            // Reassign
                            newTaskGroups = endList.ToList();
                            breakFor = true;
                        }

                        if (breakFor)
                        {
                            Groups = newTaskGroups;
                            break;
                        }
                    }
                }
                else
                {
                    // Create new one
                    var newItem = new Models.Group("", -1, true, true, new());
                    // Add it
                    Groups.Add(newItem);
                }

                // Update order
                for (int c = 0; c < Groups.Count; c++)
                {
                    Models.Group? taskGroup = Groups[c];
                    taskGroup.Order = c;
                }
            }

            StateHasChanged();

            await SaveDataAsync();
        }

        private async Task MoveDown()
        {
            if (Groups is not null)
            {
                for (int j = 0; j < Groups.Count; j++)
                {
                    var taskGroup = Groups[j];
                    var breakFor = false;
                    if (taskGroup.SubTaskSelected || taskGroup.Selected)
                    {
                        // If this is selected it means it's on the top level
                        // either goes to subtask or next taskgroup
                        if (taskGroup.Selected)
                        {
                            // Set first as selected
                            if (taskGroup?.Items.Count > 0)
                            {
                                var item = taskGroup.Items[0];
                                item.Selected = true;
                                item.Edit = false;
                                taskGroup.Selected = false;
                            }
                            // Go to next Group
                            else
                            {
                                if (Groups.Count < j + 1)
                                {
                                    Groups[j + 1].Selected = true;
                                }
                            }
                        }
                        else if (taskGroup.SubTaskSelected)
                        {
                            // This should go the next iteration in the taskItems
                            for (int i = 0; i < taskGroup?.Items.Count; i++)
                            {
                                var item = taskGroup.Items[i];
                                if (item.Selected)
                                {
                                    // If its inside taks items
                                    if (taskGroup.Items.Count > i + 1)
                                    {
                                        item.Selected = false;
                                        item.Edit = false;
                                        taskGroup.Items[i + 1].Selected = true;
                                    }
                                    // This should go to next task group
                                    else
                                    {
                                        if (Groups.Count > j + 1)
                                        {
                                            item.Selected = false;
                                            Groups[j + 1].Selected = true;
                                            breakFor = true;
                                        }
                                    }

                                    break;
                                }
                            }
                        }

                        if (breakFor)
                        {
                            break;
                        }
                    }
                }
            }

            StateHasChanged();

            await SaveDataAsync();
        }

        private async Task MoveUp()
        {
            if (Groups is not null)
            {
                for (int j = 0; j < Groups.Count; j++)
                {
                    var taskGroup = Groups[j];
                    var breakFor = false;
                    if (taskGroup.SubTaskSelected || taskGroup.Selected)
                    {
                        // If this is selected it means it's on the top level
                        // either goes to subtask or next taskgroup
                        if (taskGroup.Selected)
                        {
                            if (taskGroup.Order > 0)
                            {
                                taskGroup.Selected = false;
                                taskGroup.Edit = false;
                                if (Groups.Count > j - 1)
                                {
                                    // If it has items go to last one
                                    if (Groups[j - 1]?.Items.Count > 0)
                                    {
                                        var count = Groups[j - 1].Items.Count - 1;
                                        var item = Groups[j - 1].Items[count];
                                        item.Selected = true;
                                        taskGroup.Selected = false;
                                        breakFor = true;
                                    }
                                    else
                                    {
                                        // Otherwise just select group
                                        Groups[j - 1].Selected = true;
                                        breakFor = true;
                                    }
                                }
                            }

                            breakFor = true;
                        }
                        else if (taskGroup.SubTaskSelected)
                        {
                            // This should go the previous iteration in the taskItems
                            for (int i = 0; i < taskGroup?.Items.Count; i++)
                            {
                                var item = taskGroup.Items[i];
                                if (item.Selected)
                                {
                                    // If its inside taks items
                                    if (taskGroup.Items.Count > i - 1 && i - 1 >= 0)
                                    {
                                        item.Selected = false;
                                        item.Edit = false;
                                        taskGroup.Items[i - 1].Selected = true;
                                        breakFor = true;
                                    }
                                    // This should go to next task group
                                    else
                                    {
                                        item.Selected = false;
                                        item.Edit = false;
                                        Groups[j].Selected = true;
                                        breakFor = true;
                                    }

                                    break;
                                }
                            }
                        }

                        if (breakFor)
                        {
                            break;
                        }
                    }
                }
            }

            StateHasChanged();

            await SaveDataAsync();
        }

        private async Task MoveGroupUp()
        {
            if (Groups is not null)
            {
                for (int j = 0; j < Groups.Count; j++)
                {
                    var taskGroup = Groups[j];
                    var breakFor = false;
                    if (taskGroup.SubTaskSelected || taskGroup.Selected)
                    {
                        if (j != 0)
                        {
                            // Unselect everything
                            foreach (var item in Groups)
                            {
                                item.Selected = false;

                                foreach (var subItems in item.Items)
                                {
                                    subItems.Selected = false;
                                }
                            }


                            // Select next group
                            if (Groups.Count > j - 1)
                            {
                                Groups[j - 1].Selected = true;
                                breakFor = true;
                            }

                            if (breakFor)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            StateHasChanged();

            await SaveDataAsync();
        }

        private async Task MoveGroupDown()
        {
            if (Groups is not null)
            {
                for (int j = 0; j < Groups.Count; j++)
                {
                    var taskGroup = Groups[j];
                    var breakFor = false;
                    if (taskGroup.SubTaskSelected || taskGroup.Selected)
                    {
                        if (j + 1 != Groups.Count)
                        {
                            // Unselect everything
                            foreach (var item in Groups)
                            {
                                item.Selected = false;

                                foreach (var subItems in item.Items)
                                {
                                    subItems.Selected = false;
                                }
                            }


                            // Select next group
                            if (Groups.Count >= j + 1)
                            {
                                Groups[j + 1].Selected = true;
                                breakFor = true;
                            }

                            if (breakFor)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            StateHasChanged();

            await SaveDataAsync();
        }

        private async Task GoToFirstInGroup()
        {
            if (Groups is not null)
            {
                for (int j = 0; j < Groups.Count; j++)
                {
                    var taskGroup = Groups[j];
                    var breakFor = false;
                    if (taskGroup.SubTaskSelected || taskGroup.Selected)
                    {
                        // Unselect everything
                        foreach (var item in Groups)
                        {
                            item.Selected = false;

                            foreach (var subItems in item.Items)
                            {
                                subItems.Selected = false;
                            }
                        }


                        // Select first of group
                        if (taskGroup.Items.Count >= 0)
                        {
                            taskGroup.Items[0].Selected = true;
                            breakFor = true;
                        }

                        if (breakFor)
                        {
                            break;
                        }
                    }
                }
            }

            StateHasChanged();

            await SaveDataAsync();
        }

        private async Task GoToLastInGroup()
        {
            if (Groups is not null)
            {
                for (int j = 0; j < Groups.Count; j++)
                {
                    var taskGroup = Groups[j];
                    var breakFor = false;
                    if (taskGroup.SubTaskSelected || taskGroup.Selected)
                    {
                        // Unselect everything
                        foreach (var item in Groups)
                        {
                            item.Selected = false;

                            foreach (var subItems in item.Items)
                            {
                                subItems.Selected = false;
                            }
                        }

                        // Select first of group
                        for (int i = 0; i < taskGroup.Items.Count; i++)
                        {
                            if (i + 1 == taskGroup.Items.Count)
                            {
                                taskGroup.Items[i].Selected = true;
                                breakFor = true;
                            }
                        }

                        if (breakFor)
                        {
                            break;
                        }
                    }
                }
            }

            StateHasChanged();

            await SaveDataAsync();
        }

        private async Task RestartSelection()
        {
            if (Groups is not null)
            {
                for (int j = 0; j < Groups.Count; j++)
                {
                    var breakFor = false;
                    // Unselect everything
                    foreach (var item in Groups)
                    {
                        item.Selected = false;

                        foreach (var subItems in item.Items)
                        {
                            subItems.Selected = false;
                        }
                    }


                    // Select next group
                    if (Groups.Count > 0)
                    {
                        Groups[0].Selected = true;
                        breakFor = true;
                    }

                    if (breakFor)
                    {
                        break;
                    }
                }
            }

            StateHasChanged();

            await SaveDataAsync();
        }

        #endregion

        #region Misc

        public void Dispose()
        {
        }

        #endregion
    }
}