using CrewBoomMono;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public static class CharacterAnimationProvider
{
    public static int GetFreestyleAnimation(BrcCharacter character)
    {
        switch (character)
        {
            case BrcCharacter.Vinyl:
                if (UnityEngine.Random.value < 0.33f)
                {
                    return Animator.StringToHash("freestyle4");
                }
                if (UnityEngine.Random.value > 0.5f)
                {
                    return Animator.StringToHash("freestyle6");
                }
                return Animator.StringToHash("freestyle14");
            case BrcCharacter.Frank:
                if (UnityEngine.Random.value < 0.33f)
                {
                    return Animator.StringToHash("freestyle4");
                }
                if (UnityEngine.Random.value > 0.5f)
                {
                    return Animator.StringToHash("freestyle11");
                }
                return Animator.StringToHash("freestyle5");
            case BrcCharacter.Coil:
                if (UnityEngine.Random.value < 0.33f)
                {
                    return Animator.StringToHash("freestyle4");
                }
                if (UnityEngine.Random.value > 0.5f)
                {
                    return Animator.StringToHash("freestyle3");
                }
                return Animator.StringToHash("freestyle12");
            case BrcCharacter.Red:
            case BrcCharacter.FauxWithBoostPack:
            case BrcCharacter.FauxWithoutBoostPack:
                if (UnityEngine.Random.value < 0.33f)
                {
                    return Animator.StringToHash("freestyle2");
                }
                if (UnityEngine.Random.value > 0.5f)
                {
                    return Animator.StringToHash("freestyle19");
                }
                return Animator.StringToHash("freestyle14");
            case BrcCharacter.Tryce:
                if (UnityEngine.Random.value < 0.33f)
                {
                    return Animator.StringToHash("freestyle1");
                }
                if (UnityEngine.Random.value > 0.5f)
                {
                    return Animator.StringToHash("freestyle18");
                }
                return Animator.StringToHash("freestyle10");
            case BrcCharacter.Bel:
                if (UnityEngine.Random.value < 0.33f)
                {
                    return Animator.StringToHash("freestyle17");
                }
                if (UnityEngine.Random.value > 0.5f)
                {
                    return Animator.StringToHash("freestyle6");
                }
                return Animator.StringToHash("freestyle12");
            case BrcCharacter.Rave:
                if (UnityEngine.Random.value < 0.33f)
                {
                    return Animator.StringToHash("freestyle1");
                }
                if (UnityEngine.Random.value > 0.5f)
                {
                    return Animator.StringToHash("freestyle5");
                }
                return Animator.StringToHash("freestyle14");
            case BrcCharacter.DotExeMember:
            case BrcCharacter.DotExeBoss:
                if (UnityEngine.Random.value < 0.33f)
                {
                    return Animator.StringToHash("freestyle2");
                }
                if (UnityEngine.Random.value > 0.5f)
                {
                    return Animator.StringToHash("freestyle10");
                }
                return Animator.StringToHash("freestyle11");
            case BrcCharacter.Solace:
                if (UnityEngine.Random.value < 0.33f)
                {
                    return Animator.StringToHash("freestyle6");
                }
                if (UnityEngine.Random.value > 0.5f)
                {
                    return Animator.StringToHash("freestyle15");
                }
                return Animator.StringToHash("freestyle17");
            case BrcCharacter.DjCyber:
                if (UnityEngine.Random.value < 0.33f)
                {
                    return Animator.StringToHash("freestyle3");
                }
                if (UnityEngine.Random.value > 0.5f)
                {
                    return Animator.StringToHash("freestyle10");
                }
                return Animator.StringToHash("freestyle19");
            case BrcCharacter.EclipseMember:
                if (UnityEngine.Random.value < 0.33f)
                {
                    return Animator.StringToHash("freestyle7");
                }
                if (UnityEngine.Random.value > 0.5f)
                {
                    return Animator.StringToHash("freestyle10");
                }
                return Animator.StringToHash("freestyle17");
            case BrcCharacter.DevilTheoryMember:
                if (UnityEngine.Random.value < 0.33f)
                {
                    return Animator.StringToHash("freestyle1");
                }
                if (UnityEngine.Random.value > 0.5f)
                {
                    return Animator.StringToHash("freestyle6");
                }
                return Animator.StringToHash("freestyle8");
            case BrcCharacter.FleshPrince:
                if (UnityEngine.Random.value < 0.33f)
                {
                    return Animator.StringToHash("freestyle7");
                }
                if (UnityEngine.Random.value > 0.5f)
                {
                    return Animator.StringToHash("freestyle8");
                }
                return Animator.StringToHash("freestyle9");
            case BrcCharacter.Irene:
                if (UnityEngine.Random.value < 0.33f)
                {
                    return Animator.StringToHash("freestyle9");
                }
                if (UnityEngine.Random.value > 0.5f)
                {
                    return Animator.StringToHash("freestyle15");
                }
                return Animator.StringToHash("freestyle12");
            case BrcCharacter.Felix:
            case BrcCharacter.FelixWithCyberHead:
                if (UnityEngine.Random.value < 0.33f)
                {
                    return Animator.StringToHash("freestyle8");
                }
                if (UnityEngine.Random.value > 0.5f)
                {
                    return Animator.StringToHash("freestyle16");
                }
                return Animator.StringToHash("freestyle18");
            case BrcCharacter.OldHeadMember:
                if (UnityEngine.Random.value < 0.33f)
                {
                    return Animator.StringToHash("freestyle18");
                }
                if (UnityEngine.Random.value > 0.5f)
                {
                    return Animator.StringToHash("freestyle14");
                }
                return Animator.StringToHash("freestyle10");
            case BrcCharacter.Base:
                if (UnityEngine.Random.value < 0.33f)
                {
                    return Animator.StringToHash("freestyle18");
                }
                if (UnityEngine.Random.value > 0.5f)
                {
                    return Animator.StringToHash("freestyle9");
                }
                return Animator.StringToHash("freestyle16");
            case BrcCharacter.Jet:
                if (UnityEngine.Random.value < 0.33f)
                {
                    return Animator.StringToHash("freestyle13");
                }
                if (UnityEngine.Random.value > 0.5f)
                {
                    return Animator.StringToHash("freestyle17");
                }
                return Animator.StringToHash("freestyle15");
            case BrcCharacter.Mesh:
                if (UnityEngine.Random.value < 0.33f)
                {
                    return Animator.StringToHash("freestyle18");
                }
                if (UnityEngine.Random.value > 0.5f)
                {
                    return Animator.StringToHash("freestyle14");
                }
                return Animator.StringToHash("freestyle10");
            case BrcCharacter.FuturismMember:
                if (UnityEngine.Random.value < 0.33f)
                {
                    return Animator.StringToHash("freestyle2");
                }
                if (UnityEngine.Random.value > 0.5f)
                {
                    return Animator.StringToHash("freestyle13");
                }
                return Animator.StringToHash("freestyle15");
            case BrcCharacter.Rise:
                if (UnityEngine.Random.value < 0.33f)
                {
                    return Animator.StringToHash("freestyle3");
                }
                if (UnityEngine.Random.value > 0.5f)
                {
                    return Animator.StringToHash("freestyle11");
                }
                return Animator.StringToHash("freestyle19");
            case BrcCharacter.Shine:
                if (UnityEngine.Random.value < 0.33f)
                {
                    return Animator.StringToHash("freestyle7");
                }
                if (UnityEngine.Random.value > 0.5f)
                {
                    return Animator.StringToHash("freestyle12");
                }
                return Animator.StringToHash("freestyle14");
            default:
                return Animator.StringToHash("freestyle1");
        }
    }

    public static int GetSoftBounceAnimation(BrcCharacter character)
    {
        switch (character)
        {
            case BrcCharacter.Vinyl:
                return Animator.StringToHash("softBounce10");
            case BrcCharacter.Frank:
                return Animator.StringToHash("softBounce2");
            case BrcCharacter.Coil:
                return Animator.StringToHash("softBounce15");
            case BrcCharacter.Red:
                return Animator.StringToHash("bounce");
            case BrcCharacter.Tryce:
                return Animator.StringToHash("softBounce4");
            case BrcCharacter.Bel:
                return Animator.StringToHash("softBounce3");
            case BrcCharacter.Rave:
                return Animator.StringToHash("softBounce12");
            case BrcCharacter.DotExeMember:
                return Animator.StringToHash("softBounce11");
            case BrcCharacter.Solace:
                return Animator.StringToHash("softBounce14");
            case BrcCharacter.DjCyber:
                return Animator.StringToHash("softBounce6");
            case BrcCharacter.EclipseMember:
                return Animator.StringToHash("softBounce18");
            case BrcCharacter.DevilTheoryMember:
                return Animator.StringToHash("softBounce13");
            case BrcCharacter.FauxWithBoostPack:
                return Animator.StringToHash("softBounce1");
            case BrcCharacter.FleshPrince:
                return Animator.StringToHash("softBounce2");
            case BrcCharacter.Irene:
                return Animator.StringToHash("softBounce13");
            case BrcCharacter.Felix:
                return Animator.StringToHash("softBounce1");
            case BrcCharacter.OldHeadMember:
                return Animator.StringToHash("softBounce7");
            case BrcCharacter.Base:
                return Animator.StringToHash("softBounce1");
            case BrcCharacter.Jet:
                return Animator.StringToHash("softBounce1");
            case BrcCharacter.Mesh:
                return Animator.StringToHash("softBounce3");
            case BrcCharacter.FuturismMember:
                return Animator.StringToHash("softBounce5");
            case BrcCharacter.Rise:
                return Animator.StringToHash("softBounce8");
            case BrcCharacter.Shine:
                return Animator.StringToHash("softBounce9");
            case BrcCharacter.FauxWithoutBoostPack:
                return Animator.StringToHash("softBounce1");
            case BrcCharacter.DotExeBoss:
                return Animator.StringToHash("softBounce11");
            default:
                return Animator.StringToHash("bounce");
        }
    }
}
