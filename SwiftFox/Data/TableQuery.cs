﻿using System.ComponentModel;

namespace SwiftFox.Data
{
    public class TableQuery
    {
        public string TableSchema { get; set; } = null!;
        public string TableName { get; set; } = null!;
        public List<string> Columns { get; set; } = new();
        public Filter Filter { get; set; } = new();
        public Dictionary<string, SortDirection> OrderBy { get; set; } = new();
        public int Skip { get; set; }
        [DefaultValue(100)]
        public int Take { get; set; } = 100;
        public bool Verbose { get; set; }
    }
}