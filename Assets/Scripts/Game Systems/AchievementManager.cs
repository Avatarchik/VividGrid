using UnityEngine;
using UnityEngine.SocialPlatforms;

public class AchievementManager : MonoBehaviour {

	void Start () {

		// Authenticate user on start
		Social.localUser.Authenticate(ProcessAuthentication);
	}

    void ProcessAuthentication (bool success) {
        if (success) {
            Debug.Log ("Authenticated, checking achievements");

            // Request loaded achievements, and register a callback for processing them
            Social.LoadAchievements (ProcessLoadedAchievements);
        } else {
            Debug.Log ("Failed to authenticate user");
        }
    }

    void ProcessLoadedAchievements (IAchievement[] achievements) {

        if (achievements.Length == 0) {
            Debug.Log ("Error: no achievements found");
        } else {
            Debug.Log ("Got " + achievements.Length + " achievements");
        }
     
        // This function should probably check against the saved info
        // about achievements and if any achievements are farther along
        // than is represented here, it should update them to reflect
        // any offline progress.

/*
        // You can also call into the functions like this
        Social.ReportProgress ("Achievement01", 100.0, result => {
            if (result)
                Debug.Log ("Successfully reported achievement progress");
            else
                Debug.Log ("Failed to report achievement");
        });
*/
    }
}
