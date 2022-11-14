using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taskless.Libraries.Models
{
    public class Item
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public bool Checked { get; set; }
        public int Order { get; set; }
        public bool Selected { get; set; }
        public TaskType Type { get; set; }

        public bool Edit { get; set; }

        public Item(string text, bool _checked, bool edit, int order, bool selected, TaskType type)
        {
            Id = Guid.NewGuid();
            Text = text;
            Checked = _checked;
            Type = type;
            Edit = edit;
            Order = order;
            Selected = selected;
        }

        public enum TaskType
        {
            TASK,
            CHECKED,
            BLOCKED,
            QUESTION,
            EXCLAMATION,
            NONE
        }
    }
}
