﻿using System.Collections.Generic;
using SWLOR.Game.Server.Core.NWScript.Enum;
using SWLOR.Game.Server.Service.AbilityService;
using SWLOR.Game.Server.Service.PerkService;

namespace SWLOR.Game.Server.Feature.AbilityDefinition.FirstAid
{
    public class InfusionAbilityDefinition: FirstAidBaseAbilityDefinition
    {
        private const string Tier1Tag = "ABILITY_INFUSION_1";
        private const string Tier2Tag = "ABILITY_INFUSION_2";

        public override Dictionary<FeatType, AbilityDetail> BuildAbilities()
        {
            Infusion1();
            Infusion2();

            return Builder.Build();
        }

        private string Validation(uint activator, uint target, int level)
        {
            if (!IsWithinRange(activator, target))
            {
                return "Your target is too far away.";
            }

            if (HasMorePowerfulEffect(target, level,
                    new(Tier1Tag, 1),
                    new(Tier2Tag, 2)))
            {
                return "Your target is already enhanced by a more powerful effect.";
            }

            return string.Empty;
        }

        private void Impact(uint activator, uint target, int amount, string effectTag)
        {
            const float BaseDuration = 24f;
            var willMod = GetAbilityModifier(AbilityType.Willpower, activator);
            var duration = BaseDuration + willMod * 2.5f;

            RemoveEffectByTag(target, Tier1Tag, Tier2Tag);

            var effect = EffectRegenerate(amount, 6f);
            effect = TagEffect(effect, effectTag);
            ApplyEffectToObject(DurationType.Temporary, effect, target, duration);
        }

        private void Infusion1()
        {
            Builder.Create(FeatType.Infusion1, PerkType.Infusion)
                .Name("Infusion I")
                .Level(1)
                .HasRecastDelay(RecastGroup.Infusion, 60f)
                .HasActivationDelay(2f)
                .HasMaxRange(30.0f)
                .RequirementStamina(6)
                .UsesAnimation(Animation.LoopingGetMid)
                .IsCastedAbility()
                .UnaffectedByHeavyArmor()
                .HasCustomValidation((activator, target, level, location) =>
                {
                    return Validation(activator, target, 1);
                })
                .HasImpactAction((activator, target, _, _) =>
                {
                    Impact(activator, target, 20, Tier1Tag);
                });
        }

        private void Infusion2()
        {
            Builder.Create(FeatType.Infusion2, PerkType.Infusion)
                .Name("Infusion II")
                .Level(2)
                .HasRecastDelay(RecastGroup.Infusion, 60f)
                .HasActivationDelay(2f)
                .HasMaxRange(30.0f)
                .RequirementStamina(8)
                .UsesAnimation(Animation.LoopingGetMid)
                .IsCastedAbility()
                .UnaffectedByHeavyArmor()
                .HasCustomValidation((activator, target, level, location) =>
                {
                    return Validation(activator, target, 2);
                })
                .HasImpactAction((activator, target, _, _) =>
                {
                    Impact(activator, target, 40, Tier2Tag);
                });
        }
    }
}
