using NLog;
using System;
using System.Diagnostics;

namespace Dargon.LeagueOfLegends.Session.Phases
{
   public class LeaguePhaseQuit : LeaguePhaseBase
   {
      private static readonly Logger logger = LogManager.GetCurrentClassLogger();

      public LeaguePhaseQuit(LeaguePhaseContext context) : base(context, LeagueSessionPhase.Quit) {
      }

      public override void BeginPhase(BeginPhaseArgs args)
      {
         base.BeginPhase(args);
         logger.Info("Reached Terminal (Quit) League Phase");
      }

      public override void HandleRadsUserKernelLaunched(IProcess process) { throw new InvalidOperationException(); }
      public override void HandleLauncherLaunched(IProcess process) { throw new InvalidOperationException(); }
      public override void HandlePatcherLaunched(IProcess process) { throw new InvalidOperationException(); }
      public override void HandleLauncherQuit(IProcess process) { throw new InvalidOperationException(); }
      public override void HandleClientLaunched(IProcess process) { throw new InvalidOperationException(); }
      public override void HandleClientQuit(IProcess process) { throw new InvalidOperationException(); }
      public override void HandleGameLaunched(IProcess process) { throw new InvalidOperationException(); }
      public override void HandleGameQuit(IProcess process) { throw new InvalidOperationException(); }
   }
}