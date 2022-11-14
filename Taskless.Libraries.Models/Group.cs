using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taskless.Libraries.Models
{
    public class Group
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }
        public bool Selected { get; set; }
        public bool EditMode { get; set; }
        public bool SubTaskSelected => TaskItems.Any(x => x.Selected);
        public List<Item> TaskItems { get; set; }

        public Group(string title, int order, bool selected, bool editMode, List<Item> taskItems)
        {
            Id = Guid.NewGuid();
            Title = title;
            TaskItems = taskItems;
            Order = order;
            Selected = selected;
            EditMode = editMode;
        }
    }
}
