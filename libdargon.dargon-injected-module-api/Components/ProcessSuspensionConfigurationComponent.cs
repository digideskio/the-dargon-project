﻿using System.Collections.Generic;
using ItzWarty;

namespace Dargon.InjectedModule.Components
{
   public class ProcessSuspensionConfigurationComponent : IConfigurationComponent
   {
      private const string kEnableProcessSuspensionFlagName = "--enable-process-suspension";
      private const string kSuspendedProcessesPropertyName = "launchsuspended";

      private readonly ISet<string> processNames;

      public ProcessSuspensionConfigurationComponent(ISet<string> processNames) { this.processNames = processNames; }

      public void AmendBootstrapConfiguration(BootstrapConfigurationBuilder builder)
      {
         builder.SetFlag(kEnableProcessSuspensionFlagName);
         builder.SetProperty(kSuspendedProcessesPropertyName, processNames.Join(","));
      }
   }
}
