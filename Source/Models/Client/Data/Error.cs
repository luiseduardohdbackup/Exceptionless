﻿using System;
using Exceptionless.Models.Collections;

namespace Exceptionless.Models.Data {
    public class Error : InnerError {
        public Error() {
            Modules = new ModuleCollection();
        }

        /// <summary>
        /// Any modules that were loaded / referenced when the error occurred.
        /// </summary>
        public ModuleCollection Modules { get; set; }
    }
}