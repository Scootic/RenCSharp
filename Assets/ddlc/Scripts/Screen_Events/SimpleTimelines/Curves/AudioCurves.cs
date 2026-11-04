using System;
using UITK_SimpleTimeline;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RenCSharp.Sequences
{
    public class PlaySFXCurve : TypedTimelineCurve<PlaySFX>
    {
        protected override int ExcludedKeyframeTangentModes() => 15; //exclude all but cliff
        public override string ShorthandCurveName() => "Play SFX Curve";
        public override string SpawnKeyframeName() => "Audio Asset Reference Keyframe";
        public override string ToAffectName() => "";

        public override void Evaluate(float time)
        {
            try
            {
                PlaySFX b = AtTime(time).Value;
                if (b.Position == Vector3.zero) {
                    _ = Audio_Manager.AM.Play2DSFX(b.AudioAsset, b.MinPitch, b.MaxPitch, b.Volume, b.Environmental, b.Loop);
                }
                else
                {
                    Vector3 pos = root != null ? root.transform.position + b.Position : b.Position;
                    _ = Audio_Manager.AM.Play3DSFX(b.AudioAsset, pos, b.MinPitch, b.MaxPitch, b.Volume, b.Environmental, b.Loop);
                }
            }
            catch
            {
                return;
            }
        }

        public override PlaySFX EvaluateValue(float time)
        {
            try
            {
                TimelineKeyframe<PlaySFX> atTime = AtTime(time);
                return atTime.Value;
            }
            catch
            {
                return new PlaySFX().Default();
            }
        }

        public override string EvaluateMessage(float time)
        {
            try
            {
                PlaySFX b = AtTime(time).Value;
                return $"Playing SFX: {b.AudioAsset.AssetGUID}";
            }
            catch
            {
                return "Not playing SFX!";
            }
        }


        public override string ToString()
        {
            return "Audio/Play SFX Curve";
        }
    }

    [Serializable]
    public struct PlaySFX : IDefaultableNotNull<PlaySFX>
    {
        public AssetReference AudioAsset;
        public Vector3 Position;
        public float Volume, MinPitch, MaxPitch;
        public bool Loop, OnlyOne, Environmental;
        public PlaySFX Default()
        {
            return new PlaySFX()
            {
                Volume = 1,
                MinPitch = 1,
                MaxPitch = 1,
                OnlyOne = false,
                Loop = false,
                Environmental = false
            };
        }
    }
}
