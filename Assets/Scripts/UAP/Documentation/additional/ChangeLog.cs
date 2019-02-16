/*!
 * \page Changelogs Change Logs
 * 
 * \section Version8 Version 1.0.6
 * Added Localization support (English & German included, more languages can be added)
 * Added 32Bit support for NVDA and Windows SAPI (x86 build target)
 * Bug Fix: MacOS TTS no longer occasionally throws 'Process has already finished' error
 * Bug Fix: Loading new scene will no longer cause issues with Windows or MacOS TTS
 * UAP Icon is now also drawn next to GameObjects with UI Group Root component
 *
 * \section Version7 Version 1.0.5
- Added Google API TTS support for WebGL
- Added generic support for third-party TTS systems
- Added function to query whether Magic Gestures are enabled
- Bug Fix: All localized UI elements now properly use the Localize() function
- Bug Fix: Fixed debug log function not respecting checkbox
- Delayed Windows TTS initialization until it is being used to increase compatibility with some Windows versions
- Fixed compile warning about GetInstanceID in manager class
 * 
 * \section Version6 Version 1.0.4
- Improved/Fixed TalkBack detection on Android 8.x
- Added Experimental WebGL support
- Optimization of Selection Group component
- New Option: Plugin on/off can be linked directly to VoiceOver (optional)
- Bug Fix: Scroll View jittering when auto-scrolling
- uGUI: Better handling of old/incorrect/broken scroll view setups
- Using the ALT key to emulate two finger taps with the mouse in Editor
- New Option: Use mouse swipes on PC to emulate swipes for mobile development
- Added SaySkippable() function to make an announcement that can be interrupted by player input
- Some performance optimizations
 * 
 * 
 * \section Version5 Version 1.0.3
- Added MacOS voice support
- Added prefix option to combine your text and label content (<a href="UIElements.html#Prefix"><b>Documentation</b></a>)
- Scrolling: Added option to disable auto-scroll
- Manual Traversal Order will work even when no parent is set
- Bug Fix: Read from top/current now triggers reliably
- Read from top/current can now be triggered from code (UAP_AccessibilityManager.ReadFromTop())
- New Option: Plugin saves enabled state can be turned on/off
 * 
 * 
 * \section Version4 Version 1.0.2
- Added button to Inspector to manually toggle Accessibility mode during Editor play
- Added helper function to format larger numbers in a VoiceOver/TalkBack friendly manner
- uGUI: The element frame will be automatically recreated if it is deleted
- uGUI: ScrollViews that have no ViewPort set will now be handled by attempting to locate the viewport automatically
- Bug Fix: Starting Play in an accessible scene without the Accessibility Manager prefab will no longer cause a crash
- Bug Fix: uGUI - switching scenes no longer throws an error about a missing RectTransform
 * 
 * 
 * \section Version3 Version 1.0.1
- Update to Unity 2017.1.1p3
- References to UI elements that were removed while UAP was paused/blocked are now handled correctly
- Support for 3D UIs added for uGUI (support for Canvas Render Modes "Screen Space - Camera" and "World Space")
- Added World Space UI Example
- Calling BlockInput() can now optionally stop and clear all queued speech
- Bug Fix: Function SelectNothing() now correctly hides the item highlight frame
- UI Groups added in background while popup is active will be suspended until popup is closed
- Bug Fix: Deleting the very last item on the screen while it is selected no longer causes nothing to be selected
 * 
 * 
 * \section Version2 Version 1.0.0
- Updated the graphics in the scrollable 2D map example
- Accessibility prefab can now comfortably be added to scene via the menu
- All Update() calls removed for UAP_BaseElement to improve performance
- Android Emulator now works with UAP
- Updated Logo for the plugin
- Added Quick Start Guide as PDF
 * 
 * 
 * \section Version1 Version 0.9.5 (BETA)
- UAP Manager Settings no longer displays a false warning when VoiceOver is enabled
- Path to Plugin folder is stored in a variable, to make moving the plugin easy
- NGUI support can now be enabled/disabled in the menu
- Added NGUI example navigation scene
- NGUI support for popup lists (dropdown lists) added
- Accessibility will no longer turn itself on after being disabled manually
- Proper Version Check, with displayed change log and automatic version recheck
 * 
 * 
 * \section Version0b Version 0.9.3 (BETA)
- Added option to disable VoiceOver
- Added menu option to check for updates for UAP plugin
- Switched to use native swipe recognition on iOS
 * 
 * 
 * \section Version0a Version 0.9.0 (BETA)
- NGUI Support added
- Added About window for UAP plugin
 *  

*/