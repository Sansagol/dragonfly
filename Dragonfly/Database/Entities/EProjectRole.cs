﻿using Dragonfly.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Database.Entities
{
    /// <summary>Class represent the role for the project.</summary>
    public class EProjectRole
    {
        public decimal ID { get; set; }
        public string Name { get; set; }
        /// <summary>Describe the access rules to a project for this role.</summary>
        public ProjectAccessFunction AccessToProject { get; set; }
        public bool IsAdmin { get; set; }

        public bool IsAccessFunction(ProjectAccessFunction checkFunction)
        {
            return (checkFunction & AccessToProject) == checkFunction;
        }

        /// <summary>Create the text which describe the access function to the project.</summary>
        /// <returns>String with description of the access.</returns>
        public string GetFunctionsDescriptions()
        {
            List<String> functions = new List<string>();
            foreach (ProjectAccessFunction function in Enum.GetValues(typeof(ProjectAccessFunction)))
            {
                if (IsAccessFunction(function))
                    functions.Add(function.ToString());
            }
            if (functions.Count > 1)
                functions.RemoveAt(0);
            return string.Join("\n", functions);
        }
    }
}