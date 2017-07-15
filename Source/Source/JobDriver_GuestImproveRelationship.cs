using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace Hospitality
{
    public class JobDriver_GuestImproveRelationship : JobDriver_GuestBase
    {
        protected override IEnumerable<Toil> MakeNewToils()
        {
            //this.FailOn(FailCondition);
            var gotoGuest = GotoGuest(pawn, Talkee); // Jump target
            yield return gotoGuest;
            yield return Toils_Interpersonal.WaitToBeAbleToInteract(pawn);
            yield return Toils_Reserve.ReserveQueue(TargetIndex.A);

            yield return Interact(Talkee, InteractionDefOf.BuildRapport, GuestUtility.InteractIntervalAbsoluteMin).JumpIf(() => IsBusy(pawn) || IsBusy(Talkee), gotoGuest); 
            yield return TryImproveRelationship(pawn, Talkee);
            yield return Toils_Interpersonal.SetLastInteractTime(TargetIndex.A);
        }

        public Toil TryImproveRelationship(Pawn recruiter, Pawn guest)
        {
            var toil = new Toil
            {
                initAction = () => {
                    if (!recruiter.CanTalkTo(guest)) return;
                    InteractionDef intDef = DefDatabase<InteractionDef>.GetNamed("GuestDiplomacy"); 
                    recruiter.interactions.TryInteractWith(guest, intDef);
                },
                socialMode = RandomSocialMode.Off,
                defaultCompleteMode = ToilCompleteMode.Delay,
                defaultDuration = 350
            };
            //toil.AddFailCondition(FailCondition);
            return toil;
        }

        protected override bool FailCondition()
        {
            return base.FailCondition() || !Talkee.ImproveRelationship();
        }
    }
}