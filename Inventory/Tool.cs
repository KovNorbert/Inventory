using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Inventory
{
    internal class Tool
    {
        public int ToolWorkID { get; set; }
        public string ToolSerialNumber { get; set; }
        public string ToolName { get; set; }
        public string ToolDescription { get; set; }
        public string ToolCategory { get; set; }
        public bool ToolIssued { get; set; }


        public Tool(string serialNumber, string name, string description, string category)
        {
            ToolSerialNumber = serialNumber;
            ToolName = name;
            ToolDescription = description;
            ToolCategory = category;
        }

        public Tool(string serialNumber, string name, string description, string category, bool issued)
        {
            ToolSerialNumber = serialNumber;
            ToolName = name;
            ToolDescription = description;
            ToolCategory = category;
            ToolIssued = issued;
        }

    }
}
