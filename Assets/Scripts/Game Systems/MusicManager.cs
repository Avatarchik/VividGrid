using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Song {

	private int currentSongID;

	// Audio Clips
	private AudioClip menuVersion;
	private Dictionary<int, AudioClip> inGameSegments = new Dictionary<int, AudioClip>();

	public void UnloadSong () {

		Resources.UnloadAsset(menuVersion);
		foreach ( var item in inGameSegments ) {
		    Resources.UnloadAsset(item.Value);
		}

		menuVersion = null;
		inGameSegments.Clear();
	}

	public void LoadSong ( string name, int numberOfClips ) {

		currentSongID = 1;

		// create a starting filepath for the song
		var filepath = "Music/" + name + "/" + name;

		// load all the in game clips
		for ( int i = 1; i <= numberOfClips; i++ ) {
			var fullPath = filepath + "_" + i.ToString("00");
			var clip = Resources.Load<AudioClip>(fullPath);
			inGameSegments.Add(i, clip);

			Debug.Log("Loaded clip from " + fullPath);
		}

		// load the menu version
		menuVersion = Resources.Load<AudioClip>(filepath + "_m");
		Debug.Log("Loaded clip from " + filepath + "_m");
	}

	public AudioClip GetCurrentInGameClip () {

		return inGameSegments[currentSongID];
	}

	public AudioClip GetCurrentMenuClip () {

		return menuVersion;
	}

	public void SetCurrentID ( int newLevelID ) {

	}
}

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour {

	// ************ Singleton Logic ***************
   public static MusicManager Instance;
	void Awake() {
		if (Instance) {
			DestroyImmediate(gameObject);
		} else {
			DontDestroyOnLoad(gameObject);
			Instance = this;
			Initialize();
		}
	}
	// ********************************************

	public string defaultSong;
	public float crossfadeDuration;

	[SerializeField] private AudioSource audioPlayer;
	[SerializeField] private AudioSource crossfader;

	private bool isCrossfading;
	private bool isTransitioning;

	private Song currentSong = new Song();
	private Song nextSong = new Song();

	// Use this for initialization
	void Initialize () {

		audioPlayer.volume = 1.0f;
		crossfader.volume = 0.0f;

		audioPlayer.loop = true;
		crossfader.loop = true;
	}

	// Use this for new scene set up
	void OnLevelWasLoaded ( int level ) {

	}


	// ******** TESTING FUNCTIONS ********

	public void EnterLevelTest () {
		EnterLevel(2);
	}

	public void EnterZoneTest () {
		StartCoroutine(EnterZone("song01", 5));
	}

	public void SetInitialClipTest () {
		SetInitialClip(Resources.Load<AudioClip>("Music/song01/song01_01"));
	}

	// ***********************************


	/*
		SetInitialClip ( AudioClip clip )
		Should be called ASAP to the game starting, with some type of
		starter audio clip to be ambient.
	*/
	public void SetInitialClip ( AudioClip clip ) {

		audioPlayer.clip = clip;
		audioPlayer.Play();
	}
	
	/*
		EnterZone( string zoneSongName )
		When the player enters a zone, load all the new songs, then
		crossfade from the old menu music to the new menu music. Finally,
		unload the old song and make the current one.
	*/
	public IEnumerator EnterZone ( string zoneSongName, int numClips ) {

		if ( currentSong != null ) { // if switching zones
			nextSong.LoadSong(zoneSongName, numClips);
			var nextClip = nextSong.GetCurrentMenuClip();

			yield return StartCoroutine(transition(nextClip));

			var oldCurrentSong = currentSong;
			currentSong = nextSong;
			nextSong = oldCurrentSong;

			// shutdown old song
			oldCurrentSong.UnloadSong();

		} else { // if first zone
			currentSong.LoadSong(zoneSongName, numClips);
			var nextClip = currentSong.GetCurrentMenuClip();
			StartCoroutine(transition(nextClip));
		}
	}

	/*
		EnterLevel( int levelID )
		When the player enters a song, crossfade from the menu song to the
		correct song segment for that levelID.
	*/
	public void EnterLevel ( int levelID ) {

		if ( currentSong != null ) {
			currentSong.SetCurrentID(levelID);
			var nextClip = currentSong.GetCurrentInGameClip();
			StartCoroutine(transition(nextClip));
		} else {
			Debug.Log("Tried to load level AudioClip with no Song loaded!");
		}
	}

	/*
		ReturnToMenu ()
		When the player leaves a song and goes back to the menu, crossfade
		from the in game song to the menu song.
	*/
	public void ReturnToMenu () {

		if ( currentSong != null ) {
			var nextClip = currentSong.GetCurrentMenuClip();
			StartCoroutine(transition(nextClip));
		} else {
			Debug.Log("Tried to load level AudioClip with no Song loaded!");
		}
	}


	// private functions
	private IEnumerator transition ( AudioClip nextClip ) {

		if (isCrossfading)
			Debug.Log("Is Crossfading");
		else
			Debug.Log("Is Not Crossfading");

			Debug.Log("Begin Transition");
		
		if (isCrossfading) {

			// move the players along, and let the crossfade keep riding it out
			var time = audioPlayer.time;
			audioPlayer.Stop();
			crossfader.Stop();

			audioPlayer.clip = crossfader.clip;
			crossfader.clip = nextClip;

			audioPlayer.volume = crossfader.volume;
			crossfader.volume = 1.0f - audioPlayer.volume;

			audioPlayer.time = time;
			crossfader.time = time;

			audioPlayer.Play();
			crossfader.Play();

		} else {
			// start a fresh crossfade
			isTransitioning = true;

			// set up the crossfader
			crossfader.volume = 0.0f;
			crossfader.clip = nextClip;
			crossfader.time = audioPlayer.time;
			crossfader.Play();

			// execute the fade
			yield return StartCoroutine(crossfade());
		}
	}

	private IEnumerator crossfade () {

		Debug.Log("Begin Crossfade");
		isCrossfading = true;

		while( audioPlayer.volume > 0.05f )
		{
			var newVolume = Mathf.Lerp( audioPlayer.volume, 0.0f, crossfadeDuration * Time.deltaTime );
			audioPlayer.volume = newVolume;
			crossfader.volume = 1.0f - newVolume;
			yield return null;
		}

		// Close enough, turn it off!
		audioPlayer.volume = 0.0f;
		crossfader.volume = 1.0f;

		isCrossfading = false;
		Debug.Log("End Crossfade");

		endTransition();
	}

	private void endTransition() {

		// swap the two references
		var oldAudioPlayer = audioPlayer;
		audioPlayer = crossfader;
		crossfader = oldAudioPlayer;

		// shut down the crossfader
		crossfader.Stop();
		crossfader.clip = null;

		isTransitioning = false;
		Debug.Log("End Transition");
	}
}
