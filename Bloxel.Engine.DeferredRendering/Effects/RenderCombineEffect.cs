using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Bloxel.Engine.DeferredRendering.Effects
{
    public class RenderCombineEffect : Effect
    {
        public RenderCombineEffect(Effect cloneSource)
            : base(cloneSource)
        { }
    }
}