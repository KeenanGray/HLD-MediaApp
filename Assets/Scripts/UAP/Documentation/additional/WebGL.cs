/*!
 * \page WebGL WebGL
 * 
 * \section UAPWithWebGL WebGL Setup
 * This page lists the steps necessary to configure UAP for the WebGL platform.
 * 
 * Because WebGL doesn't support text-to-speech natively, you will need to provide a custom TTS solution.<br>
 * There are two options:
 * \li [Third-party TTS plugin](@ref CustomTTS) <br>
 * \li [Integrated Google Cloud TTS](@ref UseGoogleTTS) <br>
 * 
 * If you don't provide either, the plugin will not work once the project is built and deployed to the web.
 * <br><br>
 *
 * \section CustomTTS Third-party TTS plugin
 * UAP can easily be integrated with almost all third-party TTS plugins.<br>
 * If you already own another TTS plugins or wish to use one, please see the <a href="HowToGuides.html#ExchangeTheTTS"><b>Custom TTS</b></a> page for instructions on how to connect to a different TTS plugin.
 * <br><br>
 * 
 * \section UseGoogleTTS Google Cloud TTS
 * Google's Cloud API offers a Text-To-Speech interface which works great with WebGL.<br>
 * You can try it out here: <a href="https://cloud.google.com/text-to-speech/">Google Cloud Text To Speech</a>
 * <br><br>
 * UAP comes with an integrated Google Cloud TTS integration that is ready to be used on WebGL projects.<br>
 * Because Google Cloud is a paid service, you will need to provide your own API key.<br>
 * <br>
 * At the time of writing, there is a <b>free tier</b> available (up to 4 million characters/month).<br>
 * Aside from the free tier, Google also gives you a very gracious trial period, which you can use for development.<br>
 * See below on how to get a <b>free API key</b> and how to set up UAP to use it.<br>
 * <br>
 * You can test the implementation inside the Unity Editor - no need to deploy to the web during development.
 * 
 * \subsection GetAPIKey Step-By-Step: Get a Google Cloud API key
 * <h3>Step 1</h3>
 * Go to <a href="https://cloud.google.com/text-to-speech/">Google Cloud Text To Speech</a> and select <b>TRY IT FREE</b>.<br>
 * You will need to accept the Terms of Service in order to continue.
 * <table border="0"><tr><td><img src="images/GoogleTTS_Step01.jpg"></td></tr></table>
 * <h3>Step 2</h3>
 * Once you are in the dashboard, select the Google APIs button in the list.
 * <table border="0"><tr><td><img src="images/GoogleTTS_Step02.jpg"></td></tr></table>
 * <h3>Step 3</h3>
 * Search and find the <b>Cloud Text-to-Speech API</b> and select it.
 * <table border="0"><tr><td><img src="images/GoogleTTS_Step03.jpg"></td></tr></table>
 * <h3>Step 4</h3>
 * Enable this API - this will take a moment.
 * <table border="0"><tr><td><img src="images/GoogleTTS_Step04.jpg"></td></tr></table>
 * <h3>Step 5</h3>
 * Once it is enabled, you will need to create a new project to use the API.<br>
 * Select <b>Create</b> and give your project a name. No other settings are required.
 * <table border="0"><tr><td><img src="images/GoogleTTS_Step05.jpg"></td></tr></table>
 * <h3>Step 6</h3>
 * Almost done!<br>
 * After you've created your project, you can create an API key by adding credentials to the project.<br>
 * Select <b>API key</b> from the dropdown list.
 * <table border="0"><tr><td><img src="images/GoogleTTS_Step06.jpg"></td></tr></table>
 * <h3>Step 7</h3>
 * In the next page you will receive your API key.
 * <table border="0"><tr><td><img src="images/GoogleTTS_Step07.jpg"></td></tr></table>
 * <br>
 * <h3>Important Security Notice</h3>
 * By default, API keys are unrestricted. This means anyone can ready out your API from your
 * application and use your (free or paid) quota.<br>
 * To avoid this, never deploy with an unrestricted key.
 * <br><br>
 * You can set up your website HTTP referrer on the key to only allow requests from your own website 
 * to be accepted. Note that this will stop the TTS service from working inside the Unity Editor however.<br>
 * You could create a second, unrestricted API key for development only, or just work with 
 * Windows SAPI TTS while in Editor.
 * 
 * \subsection SetupAPIKey UAP Setup
 * Once you have your key, all you need to do is paste it into the plugin's settings.<br>
 * The Google TTS section can be found in the <b>Sounds</b> tab on the plugin (see image below).<br>
 * Don't forget to hit Apply on the UAP prefab to save your changes.
 * <table border="0"><tr><td><img src="images/GoogleTTS_UAPSetup.jpg"></td></tr><tr><td><i>Note: The string shown in the image is not a proper Google API key, but real keys have a similar format.</i></td></tr></table>
 */