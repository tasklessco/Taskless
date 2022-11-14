using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Taskless.Libraries.Models;

namespace Taskless.Libraries.Components
{
    public partial class TaskGroup
    {
        [Parameter]
        [EditorRequired]
        public Group Group { get; set; }
    }
}