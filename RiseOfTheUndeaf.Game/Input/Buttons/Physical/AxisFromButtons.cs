using Stride.Core;
using Stride.Input;

namespace RiseOfTheUndeaf.Input.Buttons
{
    /// <summary>
    /// A description of an axis from two physical buttons.
    /// </summary>
    [DataContract]
    public class AxisFromButtons : IPhysicalButton
    {
        /// <summary>
        /// Button representing negative axis value.
        /// </summary>
        public IPhysicalButton NegativeButton { get; set; }

        /// <summary>
        /// Button representing positive axis value.
        /// </summary>
        public IPhysicalButton PositiveButton { get; set; }

        /// <inheritdoc/>
        public IVirtualButton ToVirtual(InputSettings settings)
        {
            return new VirtualButtonTwoWay(NegativeButton.ToVirtual(settings), PositiveButton.ToVirtual(settings));
        }
    }
}
