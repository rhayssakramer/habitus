using System;
using System.Collections.Generic;

namespace Habitus.Domain.Enums
{
    public enum TaskPriority
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Urgent = 4
    }

    public enum TaskStatus
    {
        Todo = 1,
        InProgress = 2,
        Completed = 3,
        Cancelled = 4
    }

    public enum RecurrenceType
    {
        None = 0,
        Daily = 1,
        Weekly = 2,
        Monthly = 3,
        Yearly = 4,
        Custom = 5
    }
}
