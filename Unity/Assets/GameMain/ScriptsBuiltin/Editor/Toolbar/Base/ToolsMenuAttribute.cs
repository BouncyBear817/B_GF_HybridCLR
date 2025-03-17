using System;

namespace GameMain.Editor
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ToolsMenuAttribute : Attribute
    {
        public string ItemName;

        public int Priority;

        public int SubPriority;

        public string OwnerType;

        protected ToolsMenuAttribute(string itemName, string ownerType, int priority, int subPriority)
        {
            ItemName = itemName;
            Priority = priority;
            OwnerType = ownerType;
            SubPriority = subPriority;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ToolsMenuClassAttribute : ToolsMenuAttribute
    {
        public bool IsUtility;

        public ToolsMenuClassAttribute(string itemName, string ownerType, int priority, int subPriority = 0, bool isUtility = true) : base(itemName, ownerType, priority, subPriority)
        {
            IsUtility = isUtility;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ToolsMenuMethodAttribute : ToolsMenuAttribute
    {
        public ToolsMenuMethodAttribute(string itemName, string ownerType, int priority, int subPriority) : base(itemName, ownerType, priority, subPriority)
        {
        }
    }
}