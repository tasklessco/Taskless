using Microsoft.AspNetCore.Components;
using Taskless.Libraries.Models;

namespace Taskless.Libraries.Components
{
    public partial class TaskList
    {
        [Parameter]
        [EditorRequired]
        public List<Item>? Items { get; set; }
    }
}