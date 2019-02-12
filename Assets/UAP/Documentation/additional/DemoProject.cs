/*!
 * \page DemoProject Examples
 * 
 * UAP ships with a Navigation Example scene (for uGUI and NGUI) and a complete playable accessible Match 3 sample game.
 * <table border="0"><tr><td><img src="images/DemoProjectView.jpg"></td></tr></table>
 * <b>Note:</b> If you want to release your app or have no more need for the examples, it is safe to remove the entire <i>Examples</i> folder without affecting the plugin's functionality.
 * 
 * \section Match3 Match 3 Game
 * The Examples folder contains a complete accessible Match 3 game which demonstrates the use of the plugin.<br>
 * The UI used in the Match 3 game example was created by Vasili Tkach. It is available for free here: <a href="https://dribbble.com/shots/2261532--Funtique-game-UI-kit-free-PSD"><b>Funtique UI by Vasili Tkach</b></a><Br>
 * <table border="0"><tr><td><img src="images/Match3_01.jpg"></td><td><img src="images/Match3_02.jpg"></td><td><img src="images/Match3_03.jpg"></td><td><img src="images/Match3_04.jpg"></td></tr></table>
 * 
 \subsection ExtraUsability Extra Feedback for Blind Players
 * To make the gameplay more streamlined for blind players, who cannot see the entire board, the remaining moves and the goal listing, the 
 * demo project adds a few helpful usability features.
 * <ul>
 * <li>The game reads out the level goals at the beginning of the game</li>
 * <li>When gems are combined that are needed to complete the level goals, the app will read out how many gems of the same color are still needed.</li>
 * <li>When the player taps the screen once with two fingers, the levels goals and the remaining moves are read out.</li>
 * <li>When the player performs the Magic Tap, the game is paused or unpaused</li>
 * <li>The game repeats these special gestures at the beginning of each game <i>after the level goals</i> (so that they can be skipped)</li>
 * </ul>
 * 
 \subsection Navigation Navigation on the Play Grid
 * Usually, all screen elements are navigated with left and right swipes, one by one.<br>
 * This one-dimensional navigation doesn't make sense for a game that is based on a two dimensional grid.<br>
 * In a Match 3 game, gems can be swapped  with their direct neighbors in all directions. So it makes sense to let blind players explore the play area in two dimensions, too.<br>
 * 
 * <h3>2D Navigation</h3>
 * The navigation type can be set individually for each UI Group. <br>
 * To make a group use 2D navigation instead of the regular left and right grid, enable the checkbox <i>2D Navigation</i> in the Navigation section in the group's properties.<br>
 * When this is enabled, swiping up and down will not jump to the next group, but to the UI element below the current one. On Windows, the navigation is done with the left, right, up and down arrows.
 * <table border="0"><tr><td><img src="images/Navigation2D.jpg"></td></tr></table>
 * 
 * <h3>Constrain Navigation</h3>
 * When the bottom of the play grid is reached, and the user swipe right or down again, the plugin would jump to the next UI Group.<Br>
 * This could be irritating because a blind player doesn't know when the border is reach and will accidentally leave the play grid.<Br>
 * Instead you can constrain the navigation to stick to the group in each direction. When this is enabled, a sound effect will play when the user has reached
 * the borders of the grid and navigation will stop.<br>
 * Note that with this the only way for the player to leave the UI group is by using Touch Explore to find the other elements on the screen.<br>
 * If you constrain the navigation make sure to offer other options for the users to leave your game again, such as a Magic Gesture.
 * 
 \subsection BlockInput Blocking Input During Animations
 * 
 * Whenever gems are destroyed, the game plays an animation and updates the level goals.<Br>
 * To avoid clashing with input from the player during this time, the game asks the plugin to temporarily 
 * suspend accepting input. Once all animations have finished, the suspension is lifted and the game can continue.<Br>
 * You can block input temporarily using UAP_AccessibilityManager::BlockInput().<br>
 * This function will suspend input, but will not disable accessibility altogether, meaning features like Text-To-Speech will still work.
 * 
 * 
*/