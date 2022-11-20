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
        public bool Edit { get; set; }
        public bool SubTaskSelected => Items.Any(x => x.Selected);
        public List<Item> Items { get; set; }


        public Group() { }

        public Group(string title, int order, bool selected, bool editMode, List<Item> taskItems)
        {
            Id = Guid.NewGuid();
            Title = title;
            Items = taskItems;
            Order = order;
            Selected = selected;
            Edit = editMode;
        }
    }
}
