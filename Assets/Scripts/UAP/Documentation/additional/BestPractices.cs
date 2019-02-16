/*!
 * \page BestPractices Best Practices
 * With very little effort, you can make your app even more accessible for visually impaired users. Here are a few design guidelines and best practices you might consider: 
 * 
 * \li <b>Keep It Short</b><br>
 * In case you manually label buttons and other UI elements, or write custom hints, keep them <b>short and to the point</b>. Most importantly, put the <b>information at the beginning</b> of the text. <br>
 * It's not about writing pretty dialog. Blind users want to navigate <b>quickly</b>. They will listen only to as much of a description as necessary and then move on. Don't make their lives harder.<br>
 * In a nutshell: <i>"You have 15 moves left."</i> is bad. <i>"15 Moves left"</i> is better.<br>
 * 
 * \li <b>Label Elements Manually/Dynamically</b><br>
 * Manually labeling accessibility components provides context for blind users. Instead of just the number "15", write "15 moves left" into the component directly. Sighted users will never see this, but it creates a smoother experience for blind users.<br>
 * This is especially important for dialog. Blind users can't see the speaker, so you need to add his or her name at the beginning of the text - or risk confusion.<br>
 * You can write text directly from script by referencing the Accessible UI Element base class ::UAP_BaseElement and writing into m_Text. Make sure there is no reference name label set up in the inspector.<Br>
 * For a code example, take a look at the Pause Menu in the include Match 3 Game example. The script pausemenu.cs writes text directly into the accessibility component of one of the buttons.
 * 
 * \li <b>Add Additional Feedback</b><br>
 * Sighted users can grasp a screen within a second. They can see their low energy bar, the health bar of their enemy, the cards in their hand etc.<Br>
 * Use the provided TTS functionality to provide additional information for your blind users. See UAP_AccessibilityManager::Say() and UAP_AccessibilityManager::SaySkippable() to check out how to use it.<br>
 * There is a blog post with more tips here: <a href="https://icodelikeagirl.com/2017/03/17/unity-accessibility-plugin-update-13-how-fast-can-you-make-an-existing-app-accessible/"><b>Making an App Accessible</b></a><br>
 * A few good examples:<br>
 * - Blindie Match - announces how many moves the player has left after taking a turn, and how many gems of the current color still need to be destroyed, and the Pause menu is opened with the Magic Tap.
 * - Crafting Kingdom - announces when the storage has run full, plays a jingle when a treasure chest appears, and reads out the remaining product count after selling in the city.
 * - Lost Cities - announces what cards of the same color are already on the table when the player selects a card in his hand, reads out emoticons posted by opponent, announces opponent's turn actions.<Br>
 * 
 * \li <b>Don't Force Users To Type</b><br>
 * Typing with an on-screen keyboard is <b>VERY</b> cumbersome when blind.<br>
 * If you prefill edit boxes with sensible data, you can take that frustration away.<br>
 * If your players can choose a name of their hero, offer a name generator button, and prefill the edit box with a randomly generated name. Blind users can just accept the generated name, or decide to spend the time to change it.
 * 
 * \li <b>Offer Information Shortcuts</b><br>
 * Consider building in ways that blind users can request for additional output to speed up their experience. Example: Read out the current level goal progress upon a two finger tap, read out the player's health upon a two finger swipe down etc. 
 * Sighted users only need to glance at the top of the screen to see their health, and it's awesome if you give blind players a similar option. Otherwise they would have to navigate the entire screen to find the correct labels, and then back.
 * These multi-finger tap and swipes are already commonly used in accessible apps. There are a number of useful finger gestures detected in the Accessibility plugin, and you app only needs to register itself as a listener to these events.<br>
 * At the very least, support the Magic Tap to Pause/Unpause the game.<br>
 * A few good examples:<br>
 * - Blindie Match - announces the player's current progress and move count when tapping the screen once with two fingers.
 * - Blindfold Solitaire - various multi-finger swipes and taps read out the card stacks, the tableau and draw a new card on command, almost entirely eliminating the need to swipe through the screen
 * 
  \li <b>Avoid Duplication</b><br>
 * Don't duplicate any information in label, value and hint - people want to use it to skip quickly. Make the label short, put only relevant information in the value, do not explain that this is a button in the hint, only explain what using this element does<br>
 * Example: "Music On Off Toggle Button" is a horrible name for a Music On/Off Toggle button. Just call if "Music", and name the toggle states "On" and "Off". Since this is a toggle, the plugin will automatically read it out in a way that makes sense.
 * 
  \li <b>Group Your UI</b><br>
 * Make use of UI Groups to sensibly group your UI elements together, if that makes sense for your app or menu. It can vastly improve the speed at which your users can navigate your game/app and make using it a pleasant experience.<br>
 * A good practice is for popup or dialog windows to have the close/OK button in it's own group. That way blind users can swipe down once and immediately close the window.<br>
 * If you haven't already, read up on UI Groups here: <a href="MakeYourUIAccessible.html#UIRoot"><b>UI Roots</b></a>
 * 
 * \li <b>Order Your UI</b><br>
 * The order in which a blind user can jump through your UI makes a huge difference for their playing speed. All those extra swipes add up.<Br>
 * The more often a button needs to be clicked or a label read, the easier it should be to reach.<br>
 * You don't have to change your UI for this, you can simply set a manual traversal order for the individual accessible elements.<br>
 * Example: In the game Crafting Kingdom, the "Sell All" button in the market is set up to come before the label with the sale price, even though that label is visually the next item in the row.<br>
 * This was changed based on player feedback. After checking the price once, players wanted a quick way to sell their products.
 * 
 * \li <b>Offer Instructions</b><br>
 * Make your app's instructions available via the menu. Different apps and games have different controls, so it is important that the blind users are able to find instructions whenever they need them. The pause menu is another good place to offer this.<Br>
 * Best is to put an extra menu button in the main menu that will read out the navigation instructions - this is where blind users will look for it.<br>
 * You can make this button only appear if accessibility is on, as to not confuse seeing players. Use the ::UAP_GameObjectEnabler to automatically hide/unhide the button.<br>
 * Check out the game "Crafting Kingdom". It has an additional "Quit" button in the main menu that only appears when the accessibility mode is on.
 * 
  \li <b>Page Headings</b><br>
 * Put a label at the top of each screen/container with the heading for this screen/popup/dialog window. This will serve as a heading for the blind users. Use it to write the menu or screen name. The label doesn't have to contain actual visible text, or even have a text component at all. It's sufficient to only have the Accessible Label component. The screen reader will read it out for blind users. 
 * 
  \li <b>Page Changes</b><br>
 * Consider playing a subtle sound when the menu screen of your game opens up a completely new view or page. That way the blind users know that something has happened.
 * 
 * \li <b>Don't Talk Too Slow</b><br>
 * If you go the extra mile and use voice actors (or yourself) to record your own audio instead of using text to speech, make SURE they don't speak too slowly.<br>
 * Blind and low-vision gamers listen to text-to-speech all day long. They learned to listen fast. They can hear 490 words per minute and still understand. 
 * Just watch or listen to a few accessible gaming reviews and notice how fast the screen readers of the reviewers are (many actually slow them down for the review). 
 * No need to speed run through the text, just don't let your voice actors get too comfortable on each syllable or you will bore your audience to pieces.
 * 
 * \li <b>Label Images</b><br>
 * Attach an Accessibility Label to images. Instead of the image, the screen reader will read out the text set inside the label, which could for example be an audio description.
 * <table border="0">
 * <tr><td><img src="images/ImageBeforeLabel.png"></td></tr>
 * </table>
 * 
 * \li <b>Use Prefixes for Context</b><br>
 * For lives, virtual currencies, boosts and statistics, use the comfortable prefix function to add context to your displayed numbers without touching your code.<br>
 * See <a href="UIElements.html#Prefix"><b>Name Prefixes</b></a> for more information.
 * <table border="0">
 * <tr><td><img src="images/CompositeLabel_Example.png"></td></tr>
 * </table>
 * 
  \li <b>Split Up Long Texts</b><br>
 * Whenever you're presenting a long text, split it up into multiple labels. That way, if the player missed something, they don't have to start all the way from the beginning, but can just go back a paragraph.<Br>
 * To see an example of this, check out the Instructions in the Match 3 sample game in the Examples folder of the plugin.
 * If at all possible, keep menu screens to one page long, scrolling makes it more difficult to use Touch Explore. <br>
 * 
 * \li <b>Combine Labels</b><br>
 * If possible, combine multiple labels into one, so that blind players don't have to any swipe more than necessary, as that is cumbersome.<br>
 * You have two options to combine information into one label:
 * - Use <a href="UIElements.html#CombineLabels"><b>Combination Labels</b></a> (formerly called <a href="UIElements.html#Prefix"><b>Name Prefixes</b></a>) to combine multiple UI labels into one accessible label. This is great to automatically combine text from multiple labels, and works with localization.
 * - Fill in the text for the accessible label from code.
 * 
 * <h3>Notes</h3>
 * \li Mention that your game is accessible in the <b>store page description</b>. It doesn't even need to be in the top few lines. Blind users are more likely to read/listen to the entire description than seeing ones (for that very reason). Usually the only way for a blind user to find out whether a game is accessible is to install and test it. 
 * Also - mention what type of app/game/gameplay your app features in the description - for many games this is surprisingly hard to tell without pictures. "Slay the dragon and collect all eggs" could be the description of a Match 3 game just as well as a text based adventure.
 * \li Sometimes it is unavoidable to do some special scripting for sight impaired users. Use the UAP_AccessibilityManager::IsEnabled() function to check whether your code should branch, and suspend the automatic UI navigation temporarily if needed using the UAP_AccessibilityManager::BlockInput() function.
*/