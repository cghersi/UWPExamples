using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Input;

namespace MousePointerCaption
{
    public interface IPointerEventManager
    {
        /// <summary>
        /// This is used to diagnose multiple Finger gestures.
        /// </summary>
        void OnPointerEntered(object sender, PointerRoutedEventArgs e);

        /// <summary>
        /// This is used to diagnose multiple Finger gestures.
        /// </summary>
        void OnPointerExited(object sender, PointerRoutedEventArgs e);

        /// <summary>
        /// Event raised when a pointer input (e.g mouse button) is clicked/tapped down.
        /// </summary>
        void OnPointerDown(object sender, PointerRoutedEventArgs e);

        /// <summary>
        /// Event raised when a pointer input (e.g mouse button) is released / or tap is over.
        /// </summary>
        void OnPointerUp(object sender, PointerRoutedEventArgs e);

        /// <summary>
        /// Event raised when a pointer input (e.g mouse button) is moved.
        /// </summary>
        void OnPointerMoved(object sender, PointerRoutedEventArgs e);

        /// <summary>
        /// Determines whether this pointer manager is currently actively managing a gesture
        /// (i.e. we are between OnPointerUp & OnPointerDown events).
        /// </summary>
        bool IsActive { get; }
    }
}
