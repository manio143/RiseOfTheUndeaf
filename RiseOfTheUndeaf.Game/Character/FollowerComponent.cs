using Stride.Core;
using Stride.Engine;
using Stride.Engine.Design;

namespace RiseOfTheUndeaf.Character
{
    [DataContract("FollowerComponent")]
    [Display("Follow")]
    [DefaultEntityComponentProcessor(typeof(FollowerProcessor), ExecutionMode = ExecutionMode.Runtime)]
    public class FollowerComponent : EntityComponent
    {
        /// <summary>
        /// Entity to be followed.
        /// </summary>
        public Entity Target { get; set; }

        /// <summary>
        /// Should the character be following the entity?
        /// </summary>
        public bool Enabled { get; set; } = true;
    }
}
