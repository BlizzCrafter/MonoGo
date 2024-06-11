﻿using Microsoft.Xna.Framework;

namespace MonoGo.Engine.Particles.Profiles {
    public class PointProfile : Profile
    {
        public override void GetOffsetAndHeading(out Vector2 offset, out Axis heading) {
            offset = Vector2.Zero;

            FastRand.NextUnitVector(out heading);
        }
    }
}