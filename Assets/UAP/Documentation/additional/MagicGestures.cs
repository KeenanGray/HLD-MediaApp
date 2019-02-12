/*!
 * \page MagicGestures Magic Gestures
 * 
 * <h1>What is a Magic Gesture?</h1>
 * 
 * Swipes and double taps are not the only way that blind users navigate an app. There are a few special gestures 
 * that will trigger certain actions, such as "Back" or "Pause", or "Read the screen from the Top".<br>
 * If you are interested in reading more, here is a reference of gestures and keyboard commands that are supported by Apple's VoiceOver screen reader: <a href="http://axslab.com/articles/ios-voiceover-gestures-and-keyboard-commands.php"><b>VoiceOver Gestures</b></a>
 * 
 * These gestures are invaluable to a visually impaired user, as they cannot find the back or pause button 
 * the same way a seeing user can, or the soft back button that Android has on the bottom of the screen. <br> 
 * Your app or game can react to these gestures by registering for OnPause or OnBack callback events.
 * 
 * <em>Magic Gestures will be detected even when the Accessibility Manager is paused, but not when it is disabled.</em><br>
 * This allows the game to suspend the accessibility if the main gameplay requires it, but still allow blind players
 * to pause the game.
 * 
 * DEBUGGING: In Editor, holding the ALT key down while tapping the left mouse button will emulate a second finger. This allows debugging and testing of gestures requiring more than one finger during mobile development. By default, only mouse taps/clicks are registered. If developing for mobile, you can turn on "Mouse Swiping" in the Settings to enable swipe simulation with the mouse as well.
 * 
 * \section SupportedGestures Supported Magic Gestures
 * 
 * Here is a list of magic gestures that are supported.<br>
 * Register with the Accessibility Manager to get a callback when the user performs these gestures, 
 * so that your app can react to it. 
 * 
 * \subsection PauseCallback Pause
 * \li iOS: Two Finger Double Tap
 * \li Android: Two Finger Double Tap
 * \li Windows: Escape
 * 
 * Register for callbacks: UAP_AccessibilityManager::RegisterOnPauseToggledCallback()
 * 
 * \subsection EnableDisable Enable / Disable Accessibility
 * \li Android & iOS: Three Finger Triple Tap
 * 
 * Register for callbacks: UAP_AccessibilityManager::RegisterAccessibilityModeChangeCallback()
 * 
 * \subsection ReadFromTop Read From Top
 * \li Android & iOS: Two Finger Swipe Up
 * 
 * Override this functionality: UAP_AccessibilityManager::SetTwoFingerSwipeUpHandler()
 * 
 * \subsection ReadFromTop Read From Current Element
 * \li Android & iOS: Two Finger Swipe Down
 * 
 * Override this functionality: UAP_AccessibilityManager::SetTwoFingerSwipeDownHandler()
 * 
 * \subsection Other Other
 * The plugin detects other swipes and taps and can make callbacks to your app.<br>
 * You can subscribe to these to offer additional accessibility features. Read here why you should do that: <a href="BestPractices.html"><b>Best Practices</b></a><br>
 * These are only triggered when the plugin is enabled, so you won't have to worry about irritating your sighted users.
 * - Two Finger Single Tap - UAP_AccessibilityManager::RegisterOnTwoFingerSingleTapCallback()
 * - Three Finger Single Tap - UAP_AccessibilityManager::RegisterOnThreeFingerSingleTapCallback()
 * - Three Finger Dobule Tap - UAP_AccessibilityManager::RegisterOnThreeFingerDoubleTapCallback()
*/